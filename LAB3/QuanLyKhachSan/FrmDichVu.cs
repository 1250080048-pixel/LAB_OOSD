using System;
using System.Windows.Forms;
using QuanLyKhachSan.Services;

namespace QuanLyKhachSan
{
    public partial class FrmDichVu : Form
    {
        public FrmDichVu()
        {
            InitializeComponent();
        }

        private void FrmDichVu_Load(object sender, EventArgs e)
        {
            try
            {
                // Load dữ liệu ban đầu nếu cần
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnThemDichVu_Click(object sender, EventArgs e)
        {
            try
            {
                string soPhong = cbSoPhong.Text.Trim();
                string maDV = cbDichVu.SelectedValue != null ? cbDichVu.SelectedValue.ToString() : "";

                if (!int.TryParse(txtSoLuong.Text.Trim(), out int soLuong) || soLuong <= 0)
                {
                    MessageBox.Show("Số lượng dịch vụ phải là số nguyên lớn hơn 0!", "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtSoLuong.Focus();
                    return;
                }

                if (string.IsNullOrEmpty(soPhong) || string.IsNullOrEmpty(maDV))
                {
                    MessageBox.Show("Vui lòng chọn phòng và dịch vụ!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Gọi Service để lưu dịch vụ phát sinh
                DichVuService.ThemDichVuChoPhong(soPhong, maDV, soLuong);

                MessageBox.Show("Thêm dịch vụ thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi nghiệp vụ: " + ex.Message, "Thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}