using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace QuanLyKhachSan
{
    public class DatabaseHelper
    {
        // Lấy chuỗi kết nối từ file App.config
        
        private static string connectionString = ConfigurationManager.ConnectionStrings["QuanLyKhachSanDB"].ConnectionString;
        // 1. Hàm dùng để lấy dữ liệu (SELECT) -> Trả về DataTable để đổ vào DataGridView, ComboBox...
        public static DataTable ExecuteQuery(string query)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt);
                }
                catch (Exception ex)
                {
                    throw new Exception("Lỗi truy vấn dữ liệu: " + ex.Message);
                }
            }
            return dt;
        }

        // 2. Hàm dùng để Thêm, Sửa, Xóa dữ liệu (INSERT, UPDATE, DELETE) -> Trả về số dòng thay đổi
        public static int ExecuteNonQuery(string query)
        {
            int rowsAffected = 0;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    rowsAffected = cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new Exception("Lỗi thực thi lệnh: " + ex.Message);
                }
            }
            return rowsAffected;
        }
    }
}