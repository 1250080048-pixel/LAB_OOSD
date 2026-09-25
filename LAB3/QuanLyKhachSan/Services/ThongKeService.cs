using QuanLyKhachSan.Data;
using System.Data;
using System.Data.SqlClient;

namespace QuanLyKhachSan.Services
{
    public class ThongKeService
    {
        public static DataTable LayDoanhThu()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(Db.ConnectionString))
            {
                conn.Open();
                // Câu lệnh truy vấn doanh thu các phiếu đã thanh toán
                string query = "SELECT SoPhieu, SoPhong, NgayTraThucTe, TongTien FROM PhieuDatPhong WHERE TrangThai = N'Đã thanh toán'";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }
            return dt;
        }
    }
}