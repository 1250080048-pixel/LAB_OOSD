using System.Data;
using System.Data.SqlClient;
using QuanLyKhachSan.Data;

namespace QuanLyKhachSan.Services
{
    public class PhongService
    {
        public static DataTable LayDanhSachPhong()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(Db.ConnectionString))
            {
                conn.Open();
                string query = "SELECT p.SoPhong, lp.TenLoai, p.TrangThai FROM Phong p JOIN LoaiPhong lp ON p.MaLoai = lp.MaLoai";
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