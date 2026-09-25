using System;
using System.Windows.Forms;
using QuanLyKhachSan.Services;

namespace QuanLyKhachSan
{
    public partial class FrmDatPhong : Form
    {
        public FrmDatPhong()
        {
            InitializeComponent();
        }

        private void btnDatPhong_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Lấy dữ liệu từ giao diện
                string soPhieu = txtSoPhieu.Text.Trim();
                string maKhach = txtMaKhach.Text.Trim();
                string maNV = txtMaNV.Text.Trim();
                string soPhong = cbSoPhong.Text.Trim();
                DateTime ngayNhan = dtpNgayNhan.Value;
                DateTime ngayTra = dtpNgayTra.Value;

                if (!int.TryParse(txtSoNguoi.Text.Trim(), out int soNguoi))
                {
                    MessageBox.Show("Số người ở phải là số nguyên hợp lệ!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!decimal.TryParse(txtTienCoc.Text.Trim(), out decimal tienCoc))
                {
                    tienCoc = 0;
                }

                string kenhDat = cbKenhDat.Text.Trim();

                // 2. Kiểm tra dữ liệu đầu vào cơ bản
                if (string.IsNullOrEmpty(soPhieu) || string.IsNullOrEmpty(soPhong))
                {
                    MessageBox.Show("Vui lòng nhập số phiếu và chọn phòng!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (ngayNhan >= ngayTra)
                {
                    MessageBox.Show("Ngày trả phòng phải lớn hơn ngày nhận phòng!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 3. Gọi Service kiểm tra sức chứa, trùng lịch và lưu vào cơ sở dữ liệu
                DatPhongService.TaoPhieuDat(soPhieu, maKhach, maNV, ngayNhan, ngayTra, tienCoc, kenhDat, soPhong, soNguoi);

                MessageBox.Show("Đặt phòng thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                // Bắt lỗi vi phạm nghiệp vụ (vượt sức chứa tối đa hoặc trùng lịch đặt)
                MessageBox.Show("Lỗi nghiệp vụ: " + ex.Message, "Thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}