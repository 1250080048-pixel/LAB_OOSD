using System;
using System.Windows.Forms;

namespace QuanLyKhachSan
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
        }

        private void btnDanhMuc_Click(object sender, EventArgs e)
        {
            try
            {
                FrmDanhMucDichVu frm = new FrmDanhMucDichVu();
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi mở form Danh mục: " + ex.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDatNhanPhong_Click(object sender, EventArgs e)
        {
            try
            {
                FrmDatPhong frm = new FrmDatPhong();
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi mở form Đặt / Nhận phòng: " + ex.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSuDungDichVu_Click(object sender, EventArgs e)
        {
            try
            {
                FrmDichVu frm = new FrmDichVu();
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi mở form Sử dụng dịch vụ: " + ex.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnTraPhong_Click(object sender, EventArgs e)
        {
            try
            {
                FrmTraPhong frm = new FrmTraPhong();
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi mở form Trả phòng - Thanh toán: " + ex.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnThongKe_Click(object sender, EventArgs e)
        {
            try
            {
                FrmThongKe frm = new FrmThongKe();
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi mở form Thống kê: " + ex.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnPhongTienNghi_Click(object sender, EventArgs e)
        {
            try
            {
                FrmPhong frm = new FrmPhong();
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi mở form Phòng - Tiện nghi: " + ex.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Bạn có chắc chắn muốn thoát ứng dụng không?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        // Các hàm phụ trợ đồng bộ với Designer (tránh lỗi phát sinh khi build)
        private void btnPhongTienNghi_Click_1(object sender, EventArgs e) => btnPhongTienNghi_Click(sender, e);
        private void btnDichVu_Click_1(object sender, EventArgs e) => btnSuDungDichVu_Click(sender, e);
        private void btnThoat_Click_1(object sender, EventArgs e) => btnThoat_Click(sender, e);
        private void FrmMain_Load(object sender, EventArgs e) { }

        private void btnDanhMuc_Click_1(object sender, EventArgs e)
        {
            btnDanhMuc_Click(sender, e);
        }

        private void btnDatPhong_Click(object sender, EventArgs e)
        {
            btnDatNhanPhong_Click(sender, e);
        }

        private void btnTraPhong_Click_1(object sender, EventArgs e)
        {
            btnTraPhong_Click(sender, e);
        }

        private void btnThongKe_Click_1(object sender, EventArgs e)
        {
            btnThongKe_Click(sender, e);
        }
    }
}