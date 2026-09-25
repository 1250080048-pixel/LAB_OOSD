using System;
using System.Data;
using System.Windows.Forms;

namespace QuanLyKhachSan
{
    public partial class FrmThongKe : Form
    {
        public FrmThongKe()
        {
            InitializeComponent();
        }

        private void FrmThongKe_Load(object sender, EventArgs e)
        {
            LoadDuLieuThongKe();
        }

        private void LoadDuLieuThongKe()
        {
            try
            {
                // Câu lệnh gọi trực tiếp View thống kê doanh thu từ SQL Server
                string query = "SELECT * FROM vw_ThongKeDoanhThu";

                // Sử dụng DatabaseHelper để lấy dữ liệu dạng DataTable
                DataTable dt = DatabaseHelper.ExecuteQuery(query);

                // Đổ dữ liệu vào DataGridView đúng tên dgvThongKe trên giao diện
                dgvThongKe.DataSource = dt;

                // Tùy chỉnh tiêu đề hiển thị các cột cho đẹp mắt
                if (dgvThongKe.Columns.Count >= 7)
                {
                    dgvThongKe.Columns["MaHD"].HeaderText = "Mã HĐ";
                    dgvThongKe.Columns["MaDatPhong"].HeaderText = "Mã Đặt Phòng";
                    dgvThongKe.Columns["TenKhachHang"].HeaderText = "Tên Khách Hàng";
                    dgvThongKe.Columns["CMND"].HeaderText = "CMND/CCCD";
                    dgvThongKe.Columns["TenNhanVien"].HeaderText = "Nhân Viên Lập";
                    dgvThongKe.Columns["NgayThanhToan"].HeaderText = "Ngày Thanh Toán";
                    dgvThongKe.Columns["TongTien"].HeaderText = "Tổng Tiền (VNĐ)";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu thống kê: " + ex.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnXemThongKe_Click(object sender, EventArgs e)
        {
            LoadDuLieuThongKe();
        }
    }
}