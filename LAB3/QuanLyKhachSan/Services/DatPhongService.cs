using System;
using System.Data;
using System.Data.SqlClient;
using QuanLyKhachSan.Data;

namespace QuanLyKhachSan.Services
{
    public static class DatPhongService
    {
        // Kiểm tra phòng có bị trùng lịch trong khoảng thời gian [ngayNhan, ngayTra] hay không
        public static bool KiemTraTrungLich(string soPhong, DateTime ngayNhan, DateTime ngayTra, string excludeSoPhieuDat = null)
        {
            string query = @"
                SELECT COUNT(*) 
                FROM ChiTietPhieuDat CT
                JOIN PhieuDatPhong P ON CT.SoPhieuDat = P.SoPhieuDat
                WHERE CT.SoPhong = @SoPhong 
                  AND P.TrangThai IN (N'Đã đặt', N'Đang ở')
                  AND (@Exclude IS NULL OR P.SoPhieuDat <> @Exclude)
                  AND (P.NgayNhan < @NgayTra AND P.NgayTraDuKien > @NgayNhan)";

            var dt = Db.Query(query,
                new SqlParameter("@SoPhong", soPhong),
                new SqlParameter("@NgayNhan", ngayNhan.Date),
                new SqlParameter("@NgayTra", ngayTra.Date),
                new SqlParameter("@Exclude", (object)excludeSoPhieuDat ?? DBNull.Value)
            );

            int count = Convert.ToInt32(dt.Rows[0][0]);
            return count == 0; // Trả về true nếu phòng trống (không trùng lịch)
        }

        // Kiểm tra số người ở có vượt quá sức chứa tối đa của phòng không
        public static bool KiemTraSucChua(string soPhong, int soNguoiO)
        {
            var dt = Db.Query("SELECT SoNguoiToiDa FROM Phong WHERE SoPhong = @SoPhong",
                new SqlParameter("@SoPhong", soPhong));
            if (dt.Rows.Count > 0)
            {
                int max = Convert.ToInt32(dt.Rows[0]["SoNguoiToiDa"]);
                return soNguoiO <= max;
            }
            return false;
        }

        // Lập phiếu đặt phòng mới (có transaction đảm bảo toàn vẹn dữ liệu)
        public static void TaoPhieuDat(string soPhieu, string maKhach, string maNV, DateTime ngayNhan, DateTime ngayTra, decimal tienCoc, string kenhDat, string soPhong, int soNguoi)
        {
            if (!KiemTraSucChua(soPhong, soNguoi))
                throw new Exception("Số lượng người ở vượt quá sức chứa tối đa của phòng!");

            if (!KiemTraTrungLich(soPhong, ngayNhan, ngayTra))
                throw new Exception("Phòng đã có lịch đặt hoặc đang có khách lưu trú trong khoảng thời gian này!");

            using (var cn = Db.OpenConnection())
            using (var tran = cn.BeginTransaction())
            {
                try
                {
                    string sqlPhieu = @"INSERT INTO PhieuDatPhong(SoPhieuDat, MaKhach, MaNVLeTan, NgayLap, NgayNhan, NgayTraDuKien, TienCoc, KenhDat, TrangThai)
                                        VALUES(@SoPhieu, @MaKhach, @MaNV, GETDATE(), @NgayNhan, @NgayTra, @TienCoc, @KenhDat, N'Đã đặt')";
                    using (var cmd = new SqlCommand(sqlPhieu, cn, tran))
                    {
                        cmd.Parameters.AddWithValue("@SoPhieu", soPhieu);
                        cmd.Parameters.AddWithValue("@MaKhach", maKhach);
                        cmd.Parameters.AddWithValue("@MaNV", maNV);
                        cmd.Parameters.AddWithValue("@NgayNhan", ngayNhan);
                        cmd.Parameters.AddWithValue("@NgayTra", ngayTra);
                        cmd.Parameters.AddWithValue("@TienCoc", tienCoc);
                        cmd.Parameters.AddWithValue("@KenhDat", kenhDat);
                        cmd.ExecuteNonQuery();
                    }

                    string sqlCT = @"INSERT INTO ChiTietPhieuDat(SoPhieuDat, SoPhong, SoNguoi) VALUES(@SoPhieu, @SoPhong, @SoNguoi)";
                    using (var cmd = new SqlCommand(sqlCT, cn, tran))
                    {
                        cmd.Parameters.AddWithValue("@SoPhieu", soPhieu);
                        cmd.Parameters.AddWithValue("@SoPhong", soPhong);
                        cmd.Parameters.AddWithValue("@SoNguoi", soNguoi);
                        cmd.ExecuteNonQuery();
                    }

                    string sqlUpd = "UPDATE Phong SET TrangThai = N'Đã đặt' WHERE SoPhong = @SoPhong";
                    using (var cmd = new SqlCommand(sqlUpd, cn, tran))
                    {
                        cmd.Parameters.AddWithValue("@SoPhong", soPhong);
                        cmd.ExecuteNonQuery();
                    }

                    tran.Commit();
                }
                catch
                {
                    tran.Rollback();
                    throw;
                }
            }
        }
    }
}