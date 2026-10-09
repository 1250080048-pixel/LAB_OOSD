using System.Configuration;
using System.Data.SqlClient;

namespace QuanLyThuVien.Data
{
    public class DatabaseConnection
    {
        private static readonly string connectionString = ConfigurationManager.ConnectionStrings["QuanLyThuVienDb"].ConnectionString;

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}