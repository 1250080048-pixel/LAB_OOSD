using System;
using System.Data;
using System.Data.SqlClient;
using QuanLyThuVien.Data;

namespace QuanLyThuVien.Services
{
    public class TraSachService
    {
        // Phương thức xử lý trả sách và phát sinh phiếu phạt (nếu có)
        public KetQuaXuLy TraSach(string maChiTiet, string maNhanVien, DateTime ngayTra, string tinhTrang, decimal phiPhat)
        {
            if (string.IsNullOrWhiteSpace(maChiTiet))
                return KetQuaXuLy.Loi("Vui lòng chọn sách cần trả.");
            if (string.IsNullOrWhiteSpace(maNhanVien))
                return KetQuaXuLy.Loi("Vui lòng chọn nhân viên nhận trả.");
            if (string.IsNullOrWhiteSpace(tinhTrang))
                return KetQuaXuLy.Loi("Vui lòng chọn tình trạng sách.");

            using (SqlConnection cn = Db.OpenConnection())
            using (SqlTransaction tx = cn.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    string maDauSach = "";
                    DateTime ngayMuon = DateTime.MinValue;
                    DateTime ngayHenTra = DateTime.MinValue;

                    // 1. Kiểm tra thông tin chi tiết phiếu mượn
                    using (SqlCommand cmd = new SqlCommand(@"SELECT ct.MaDauSach, ct.NgayTraThucTe, pm.NgayMuon, pm.NgayHenTra 
                                                           FROM ChiTietPhieuMuon ct 
                                                           JOIN PhieuMuon pm ON pm.MaPhieuMuon = ct.MaPhieuMuon 
                                                           WHERE ct.MaChiTiet = @MaCT", cn, tx))
                    {
                        cmd.Parameters.AddWithValue("@MaCT", maChiTiet);
                        using (SqlDataReader rd = cmd.ExecuteReader())
                        {
                            if (!rd.Read())
                                return KetQuaXuLy.Loi("Không tìm thấy chi tiết mượn.");
                            if (!rd.IsDBNull(1))
                                return KetQuaXuLy.Loi("Sách này đã được trả trước đó.");

                            maDauSach = rd.GetString(0);
                            ngayMuon = rd.GetDateTime(2);
                            ngayHenTra = rd.GetDateTime(3);
                        }
                    }

                    if (ngayTra.Date < ngayMuon.Date)
                        return KetQuaXuLy.Loi("Ngày trả không thể trước ngày mượn.");

                    // 2. Phân tích tình trạng sách và quy định phạt
                    bool quaHan = ngayTra.Date > ngayHenTra.Date;
                    string tt = tinhTrang.Trim();
                    bool mat = tt.Equals("Mất", StringComparison.OrdinalIgnoreCase);
                    bool huHong = tt.IndexOf("Rách", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                  tt.IndexOf("Hư", StringComparison.OrdinalIgnoreCase) >= 0;

                    bool canPhat = quaHan || mat || huHong;
                    if (canPhat && phiPhat <= 0)
                        return KetQuaXuLy.Loi("Trường hợp trả trễ/mất/hư hỏng phải nhập phí phạt cụ thể lớn hơn 0.");

                    // 3. Cập nhật trạng thái trả sách vào ChiTietPhieuMuon
                    using (SqlCommand up = new SqlCommand(@"UPDATE ChiTietPhieuMuon 
                                                          SET NgayTraThucTe = @NgayTra, TinhTrangTra = @TinhTrang 
                                                          WHERE MaChiTiet = @MaCT AND NgayTraThucTe IS NULL", cn, tx))
                    {
                        up.Parameters.AddWithValue("@NgayTra", ngayTra.Date);
                        up.Parameters.AddWithValue("@TinhTrang", tt);
                        up.Parameters.AddWithValue("@MaCT", maChiTiet);
                        if (up.ExecuteNonQuery() != 1)
                            throw new InvalidOperationException("Không cập nhật được trạng thái trả sách.");
                    }

                    // 4. Xử lý tồn kho: Sách mất hoặc hư hỏng nặng thì không đưa lại vào kho cho mượn
                    if (!mat && !huHong)
                    {
                        using (SqlCommand updateSl = new SqlCommand("UPDATE DauSach SET SoLuongHienCo = SoLuongHienCo + 1 WHERE MaDauSach = @Ma", cn, tx))
                        {
                            updateSl.Parameters.AddWithValue("@Ma", maDauSach);
                            updateSl.ExecuteNonQuery();
                        }
                    }

                    // 5. Nếu có phạt thì tạo phiếu phạt
                    if (canPhat)
                    {
                        string maPhieuPhat = "PP" + DateTime.Now.ToString("yyyyMMddHHmmssfff");
                        string lyDo = "";
                        if (quaHan) lyDo += "Trả trễ hạn. ";
                        if (mat) lyDo += "Mất sách. ";
                        if (huHong) lyDo += "Sách hư hỏng/rách. ";

                        using (SqlCommand insPhat = new SqlCommand(@"INSERT INTO PhieuPhat (MaPhieuPhat, MaChiTiet, MaNhanVien, NgayPhat, LyDo, PhiPhat) 
                                                                     VALUES (@MaPP, @MaCT, @MaNV, @NgayPhat, @LyDo, @PhiPhat)", cn, tx))
                        {
                            insPhat.Parameters.AddWithValue("@MaPP", maPhieuPhat);
                            insPhat.Parameters.AddWithValue("@MaCT", maChiTiet);
                            insPhat.Parameters.AddWithValue("@MaNV", maNhanVien);
                            insPhat.Parameters.AddWithValue("@NgayPhat", ngayTra.Date);
                            insPhat.Parameters.AddWithValue("@LyDo", lyDo.Trim());
                            insPhat.Parameters.AddWithValue("@PhiPhat", phiPhat);
                            insPhat.ExecuteNonQuery();
                        }
                    }

                    tx.Commit();
                    return KetQuaXuLy.Ok("Trả sách thành công!");
                }
                catch (Exception ex)
                {
                    tx.Rollback();
                    return KetQuaXuLy.Loi("Lỗi hệ thống khi trả sách: " + ex.Message);
                }
            }
        }
    }
}