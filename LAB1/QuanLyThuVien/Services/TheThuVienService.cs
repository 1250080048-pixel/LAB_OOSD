using System;
using System.Data;
using System.Data.SqlClient;
using QuanLyThuVien.Data;

namespace QuanLyThuVien.Services
{
    public class TheThuVienService
    {
        public KetQuaXuLy CapThe(string maDocGia, DateTime ngayCap, DateTime hanSuDung, bool daDongLePhi)
        {
            if (string.IsNullOrWhiteSpace(maDocGia))
                return KetQuaXuLy.Loi("Vui lòng nhập mã độc giả.");
            if (hanSuDung.Date < ngayCap.Date)
                return KetQuaXuLy.Loi("Hạn sử dụng phải từ ngày cấp trở đi.");

            // Kiểm tra xem độc giả có tồn tại hay không
            object tonTai = Db.Scalar("SELECT COUNT(*) FROM DocGia WHERE MaDocGia=@Ma", new SqlParameter("@Ma", maDocGia));
            if (Convert.ToInt32(tonTai) == 0)
                return KetQuaXuLy.Loi("Không tìm thấy độc giả.");

            using (SqlConnection cn = Db.OpenConnection())
            using (SqlTransaction tx = cn.BeginTransaction())
            {
                try
                {
                    // 1. Kiểm tra xem độc giả có đang sở hữu thẻ nào còn hạn hay không
                    using (SqlCommand check = new SqlCommand(@"SELECT COUNT(*) FROM TheDocGia 
                                                             WHERE MaDocGia=@Ma AND TrangThai=1 AND HanSuDung>=@NgayCap", cn, tx))
                    {
                        check.Parameters.AddWithValue("@Ma", maDocGia);
                        check.Parameters.AddWithValue("@NgayCap", ngayCap.Date);
                        if (Convert.ToInt32(check.ExecuteScalar()) > 0)
                        {
                            tx.Rollback();
                            return KetQuaXuLy.Loi("Độc giả đang có một thẻ còn giá trị sử dụng.");
                        }
                    }

                    // 2. Vô hiệu hóa thẻ cũ (chuyển TrangThai về 0)
                    using (SqlCommand off = new SqlCommand("UPDATE TheDocGia SET TrangThai=0 WHERE MaDocGia=@Ma AND TrangThai=1", cn, tx))
                    {
                        off.Parameters.AddWithValue("@Ma", maDocGia);
                        off.ExecuteNonQuery();
                    }

                    // 3. Tạo mã thẻ mới tự động và Insert vào bảng TheDocGia
                    string maThe = "THE_" + maDocGia + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff");
                    using (SqlCommand ins = new SqlCommand(@"INSERT INTO TheDocGia (MaThe, MaDocGia, NgayCap, HanSuDung, DaDongLePhi, TrangThai)
                                                             VALUES (@MaThe, @MaDG, @NgayCap, @Han, @LePhi, 1)", cn, tx))
                    {
                        ins.Parameters.AddWithValue("@MaThe", maThe);
                        ins.Parameters.AddWithValue("@MaDG", maDocGia);
                        ins.Parameters.AddWithValue("@NgayCap", ngayCap.Date);
                        ins.Parameters.AddWithValue("@Han", hanSuDung.Date);
                        ins.Parameters.AddWithValue("@LePhi", daDongLePhi);
                        ins.ExecuteNonQuery();
                    }

                    tx.Commit();
                    return KetQuaXuLy.Ok("Cấp thẻ thư viện thành công. Mã thẻ: " + maThe);
                }
                catch (Exception ex)
                {
                    tx.Rollback();
                    return KetQuaXuLy.Loi("Lỗi hệ thống khi cấp thẻ: " + ex.Message);
                }
            }
        }
    }
}