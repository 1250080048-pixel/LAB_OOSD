using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using QuanLyThuVien.Data;

namespace QuanLyThuVien.Services
{
    public class MuonSachService
    {
        // Kiểm tra điều kiện mượn sách trước khi lập phiếu
        public KetQuaXuLy KiemTraDieuKienMuon(string maDocGia, int soSachMuonMoi)
        {
            if (string.IsNullOrWhiteSpace(maDocGia))
                return KetQuaXuLy.Loi("Vui lòng chọn độc giả.");
            if (soSachMuonMoi < 1)
                return KetQuaXuLy.Loi("Phải chọn ít nhất 1 đầu sách.");
            if (soSachMuonMoi > 3)
                return KetQuaXuLy.Loi("Một lần lập phiếu chỉ được chọn tối đa 3 đầu sách khác nhau.");

            DataTable the = Db.Query(@"SELECT TOP 1 MaThe, HanSuDung, DaDongLePhi, TrangThai 
                                     FROM TheDocGia WHERE MaDocGia=@Ma AND TrangThai=1 
                                     ORDER BY HanSuDung DESC", new SqlParameter("@Ma", maDocGia));

            if (the.Rows.Count == 0)
                return KetQuaXuLy.Loi("Độc giả chưa có thẻ thư viện đang hoạt động.");

            DateTime han = Convert.ToDateTime(the.Rows[0]["HanSuDung"]);
            bool lePhi = Convert.ToBoolean(the.Rows[0]["DaDongLePhi"]);

            if (han.Date < DateTime.Today)
                return KetQuaXuLy.Loi("Thẻ thư viện đã hết hạn.");
            if (!lePhi)
                return KetQuaXuLy.Loi("Độc giả chưa đóng lệ phí năm nên thẻ chưa có giá trị.");

            // Kiểm tra sách quá hạn (giả định dùng phương thức thực thi scalar hoặc query tương đương)
            // Lưu ý: Đảm bảo lớp Db của bạn có hỗ trợ phương thức Scalar hoặc dùng Query thay thế nếu cần.
            int quaHan = Convert.ToInt32(Db.Scalar(@"SELECT COUNT(*) 
                                                   FROM PhieuMuon pm 
                                                   JOIN ChiTietPhieuMuon ct ON ct.MaPhieuMuon = pm.MaPhieuMuon 
                                                   WHERE pm.MaDocGia = @Ma AND ct.NgayTraThucTe IS NULL 
                                                   AND pm.NgayHenTr < CAST(GETDATE() AS date)",
                                                   new SqlParameter("@Ma", maDocGia)));
            if (quaHan > 0)
                return KetQuaXuLy.Loi("Độc giả còn sách quá hạn chưa trả nên không được mượn thêm.");

            int dangMuon = Convert.ToInt32(Db.Scalar(@"SELECT COUNT(*) 
                                                     FROM PhieuMuon pm 
                                                     JOIN ChiTietPhieuMuon ct ON ct.MaPhieuMuon = pm.MaPhieuMuon 
                                                     WHERE pm.MaDocGia = @Ma AND ct.NgayTraThucTe IS NULL",
                                                     new SqlParameter("@Ma", maDocGia)));
            if (dangMuon + soSachMuonMoi > 3)
                return KetQuaXuLy.Loi("Tổng số sách đang mượn và sắp mượn không được vượt quá 3 cuốn.");

            return KetQuaXuLy.Ok("Độc giả đủ điều kiện mượn sách.");
        }

        // Lập phiếu mượn trong Transaction để đảm bảo tính an toàn dữ liệu
        public KetQuaXuLy LapPhieuMuon(string maDocGia, string maNhanVien, IList<string> maDauSach, DateTime ngayMuon, DateTime ngayHenTra)
        {
            if (maDauSach == null || maDauSach.Count == 0)
                return KetQuaXuLy.Loi("Danh sách sách mượn không hợp lệ.");

            HashSet<string> unique = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (string ma in maDauSach)
            {
                if (!string.IsNullOrWhiteSpace(ma)) unique.Add(ma.Trim());
            }

            KetQuaXuLy kq = KiemTraDieuKienMuon(maDocGia, unique.Count);
            if (!kq.ThanhCong) return kq;

            if (string.IsNullOrWhiteSpace(maNhanVien))
                return KetQuaXuLy.Loi("Vui lòng chọn nhân viên lập phiếu.");
            if (ngayHenTra.Date < ngayMuon.Date)
                return KetQuaXuLy.Loi("Ngày hẹn trả không được trước ngày mượn.");

            using (SqlConnection cn = Db.OpenConnection())
            using (SqlTransaction tx = cn.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    // 1. Kiểm tra tồn kho và khóa dòng dữ liệu
                    foreach (string maSach in unique)
                    {
                        using (SqlCommand check = new SqlCommand("SELECT SoLuongHienCo FROM DauSach WITH (UPDLOCK,HOLDLOCK) WHERE MaDauSach=@Ma", cn, tx))
                        {
                            check.Parameters.AddWithValue("@Ma", maSach);
                            object o = check.ExecuteScalar();
                            if (o == null)
                            {
                                tx.Rollback();
                                return KetQuaXuLy.Loi("Không tìm thấy đầu sách " + maSach + ".");
                            }
                            if (Convert.ToInt32(o) <= 0)
                            {
                                tx.Rollback();
                                return KetQuaXuLy.Loi("Đầu sách " + maSach + " đã hết trong kho.");
                            }
                        }
                    }

                    // 2. Tạo mã phiếu mượn tự động
                    string maPM = "PM" + DateTime.Now.ToString("yyyyMMddHHmmssfff");

                    // 3. Insert vào bảng PhieuMuon
                    using (SqlCommand insPm = new SqlCommand(@"INSERT INTO PhieuMuon (MaPhieuMuon, MaDocGia, MaNhanVien, NgayMuon, NgayHenTra)
                                                               VALUES (@MaPM, @MaDG, @MaNV, @NgayMuon, @HentRa)", cn, tx))
                    {
                        insPm.Parameters.AddWithValue("@MaPM", maPM);
                        insPm.Parameters.AddWithValue("@MaDG", maDocGia);
                        insPm.Parameters.AddWithValue("@MaNV", maNhanVien);
                        insPm.Parameters.AddWithValue("@NgayMuon", ngayMuon.Date);
                        insPm.Parameters.AddWithValue("@HentRa", ngayHenTra.Date);
                        insPm.ExecuteNonQuery();
                    }

                    // 4. Insert chi tiết phiếu mượn và cập nhật số lượng tồn kho
                    int i = 1;
                    foreach (string maSach in unique)
                    {
                        string maCT = maPM + "_" + i.ToString("00");
                        using (SqlCommand insCt = new SqlCommand(@"INSERT INTO ChiTietPhieuMuon (MaChiTiet, MaPhieuMuon, MaDauSach)
                                                                 VALUES (@MaCT, @MaPM, @MaSach)", cn, tx))
                        {
                            insCt.Parameters.AddWithValue("@MaCT", maCT);
                            insCt.Parameters.AddWithValue("@MaPM", maPM);
                            insCt.Parameters.AddWithValue("@MaSach", maSach);
                            insCt.ExecuteNonQuery();
                        }

                        // Giảm số lượng hiện có của đầu sách đi 1
                        using (SqlCommand updateSl = new SqlCommand("UPDATE DauSach SET SoLuongHienCo = SoLuongHienCo - 1 WHERE MaDauSach = @Ma", cn, tx))
                        {
                            updateSl.Parameters.AddWithValue("@Ma", maSach);
                            updateSl.ExecuteNonQuery();
                        }
                        i++;
                    }

                    tx.Commit();
                    return KetQuaXuLy.Ok("Lập phiếu mượn thành công!");
                }
                catch (Exception ex)
                {
                    tx.Rollback();
                    return KetQuaXuLy.Loi("Lỗi hệ thống khi lập phiếu mượn: " + ex.Message);
                }
            }
        }
    }
}