using QuanLyKhachSan.Data;
using System;
using System.Data;
using System.Data.SqlClient;

namespace QuanLyKhachSan.Services
{
    public class DichVuService
    {
        // 1. Thêm dịch vụ phát sinh cho phòng (dùng cho FrmDichVu)
        public static void ThemDichVuChoPhong(string soPhong, string maDV, int soLuong)
        {
            using (SqlConnection conn = new SqlConnection(Db.ConnectionString))
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                string query = "INSERT INTO ChiTietSuDungDichVu (SoPhong, MaDV, SoLuong) VALUES (@SoPhong, @MaDV, @SoLuong)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@SoPhong", soPhong);
                    cmd.Parameters.AddWithValue("@MaDV", maDV);
                    cmd.Parameters.AddWithValue("@SoLuong", soLuong);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        // 2. Lấy toàn bộ danh sách dịch vụ (dùng cho FrmDanhMucDichVu)
        public static DataTable LayDanhSachDichVu()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(Db.ConnectionString))
            {
                conn.Open();
                string query = "SELECT * FROM DichVu";
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

        // 3. Thêm mới dịch vụ vào danh mục
        public static void ThemMoiDichVu(string maDV, string tenDV, decimal donGia)
        {
            using (SqlConnection conn = new SqlConnection(Db.ConnectionString))
            {
                conn.Open();
                string query = "INSERT INTO DichVu (MaDV, TenDV, DonGia) VALUES (@MaDV, @TenDV, @DonGia)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@MaDV", maDV);
                    cmd.Parameters.AddWithValue("@TenDV", tenDV);
                    cmd.Parameters.AddWithValue("@DonGia", donGia);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // 4. Xóa dịch vụ khỏi danh mục
        public static void XoaDichVu(string maDV)
        {
            using (SqlConnection conn = new SqlConnection(Db.ConnectionString))
            {
                conn.Open();
                string query = "DELETE FROM DichVu WHERE MaDV = @MaDV";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@MaDV", maDV);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}