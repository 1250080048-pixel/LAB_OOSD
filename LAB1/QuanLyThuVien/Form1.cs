using QuanLyThuVien;
using QuanLyThuVien.Services;
using System;
using System.Windows.Forms;

namespace QuanLyThuVien
{
    
    public partial class Form1 : Form
    {
        private BookService bookService;

        public Form1()
        {
            InitializeComponent();
            bookService = new BookService();

            
            this.Load += Form1_Load;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                // Lấy dữ liệu sách từ CSDL và đổ vào DataGridView
              //  dataGridView1.DataSource = bookService.GetAllBooks();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối cơ sở dữ liệu: " + ex.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            FormMuonTra f = new FormMuonTra(); f.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FormNhanVien f = new FormNhanVien(); f.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            FormThongKe f = new FormThongKe(); f.ShowDialog();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            FormQuanLySach f = new FormQuanLySach();
            f.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FormDocGia f = new FormDocGia();
            f.ShowDialog();
        }
    }
}
