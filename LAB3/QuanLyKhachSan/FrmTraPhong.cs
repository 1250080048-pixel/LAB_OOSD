using System;
using System.Windows.Forms;
using QuanLyKhachSan.Services;

namespace QuanLyKhachSan
{
    public partial class FrmTraPhong : Form
    {
        public FrmTraPhong()
        {
            InitializeComponent();
        }

        private void btnTinhTien_Click(object sender, EventArgs e)
        {
            try
            {
                string soPhong = cbSoPhong.Text.Trim();

                if (string.IsNullOrEmpty(soPhong))
                {
                    MessageBox.Show("Vui lòng nhập hoặc chọn số phòng cần thanh toán!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 1. Gọi Service tính tổng tiền dịch vụ phát sinh của phòng đó
                decimal tienDV = TraPhongService.TinhTongTienDichVu(soPhong);
                lblTienDichVu.Text = tienDV.ToString("N0") + " VNĐ";

                // 2. Tiền phòng và tiền cọc (có thể lấy từ CSDL hoặc tạm tính)
                decimal tienPhong = 1000000; // Tiền phòng mẫu
                decimal tienCoc = 200000;     // Tiền cọc đã nhận lúc đặt

                // 3. Tính tổng tiền thực tế cần thanh toán
                decimal tongTien = tienPhong + tienDV - tienCoc;
                lblTongTien.Text = tongTien.ToString("N0") + " VNĐ";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tính tiền: " + ex.Message, "Thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {
            // Không cần viết gì trong này cũng được
        }
        private void btnThanhToan_Click(object sender, EventArgs e)
        {
            try
            {
                string soPhieu = txtSoPhieu.Text.Trim();
                string soPhong = cbSoPhong.Text.Trim();

                if (string.IsNullOrEmpty(soPhieu) || string.IsNullOrEmpty(soPhong))
                {
                    MessageBox.Show("Vui lòng nhập số phiếu và số phòng để thanh toán!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DialogResult dr = MessageBox.Show("Xác nhận thanh toán hóa đơn và hoàn tất trả phòng?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == DialogResult.Yes)
                {
                    // Gọi Service cập nhật trạng thái phiếu và chuyển phòng về Trống
                    TraPhongService.ThanhToanVaTraPhong(soPhieu, soPhong, 0);

                    MessageBox.Show("Thanh toán thành công! Phòng đã được giải phóng.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi thanh toán: " + ex.Message, "Thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}