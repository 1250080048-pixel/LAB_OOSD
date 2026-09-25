using QuanLyKhachSan.Data;
using System;
using System.Data;
using System.Data.SqlClient;

namespace QuanLyKhachSan.Services
{
    public class TraPhongService
    {
        // Tính tổng tiền dịch vụ của phòng
        public static decimal TinhTongTienDichVu(string soPhong)
        {
            decimal tongTienDV = 0;
            using (SqlConnection conn = new SqlConnection(Db.ConnectionString))
            {
                if (conn.State == ConnectionState.Closed) conn.Open();

                // Giả sử bảng ChiTietSuDungDichVu liên kết với bảng DichVu để lấy đơn giá
                string query = @"SELECT SUM(ct.SoLuong * dv.DonGia) 
                                 FROM ChiTietSuDungDichVu ct 
                                 JOIN DichVu dv ON ct.MaDV = dv.MaDV 
                                 WHERE ct.SoPhong = @SoPhong";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@SoPhong", soPhong);
                    object result = cmd.ExecuteScalar();
                    if (result != DBNull.Value && result != null)
                    {
                        tongTienDV = Convert.ToDecimal(result);
                    }
                }
            }
            return tongTienDV;
        }

        // Thực hiện thanh toán và trả phòng
        public static void ThanhToanVaTraPhong(string soPhieu, string soPhong, decimal tongTienThanhToan)
        {
            using (SqlConnection conn = new SqlConnection(Db.ConnectionString))
            {
                if (conn.State == ConnectionState.Closed) conn.Open();

                // Cập nhật trạng thái phiếu đặt phòng thành đã thanh toán / hoàn thành
                string query = "UPDATE PhieuDatPhong SET TrangThai = N'Đã thanh toán', NgayTraThucTe = GETDATE() WHERE SoPhieu = @SoPhieu";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@SoPhieu", soPhieu);
                    cmd.ExecuteNonQuery();
                }

                // Cập nhật trạng thái phòng về Trống
                string updatePhong = "UPDATE Phong SET TrangThai = N'Trống' WHERE SoPhong = @SoPhong";
                using (SqlCommand cmdPhong = new SqlCommand(updatePhong, conn))
                {
                    cmdPhong.Parameters.AddWithValue("@SoPhong", soPhong);
                    cmdPhong.ExecuteNonQuery();
                }
            }
        }
    }
}