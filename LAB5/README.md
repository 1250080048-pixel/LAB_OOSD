
LAB 5 - Quản lý công ty du lịch Văn Hóa Việt

Bài thực hành môn Phân tích thiết kế hướng đối tượng (HUTECH): phân tích nghiệp vụ, mô hình hóa bằng UML, thiết kế cơ sở dữ liệu SQL Server và cài đặt ứng dụng Windows Forms bằng C# theo kiến trúc UI - Service - Data.

Sinh viên: Tằng Minh Hào
MSSV: 1250080048
Lớp: 12_DH_CNPM1
Mô tả bài toán

Công ty du lịch Văn Hóa Việt (TP.HCM) quản lý:

Tour và hành trình: điểm dừng theo thứ tự, phương tiện theo từng chặng, điểm tham quan. Mọi tour xuất phát và kết thúc tại TP.HCM.
Khách lẻ (dưới 12 người): đăng ký theo chuyến tại điểm bán vé, thanh toán ngay.
Khách theo đoàn (trên 12 người): đặt cọc trước, danh sách người đi khi mua bảo hiểm, thanh toán kinh phí sau khi kết thúc tour. Đoàn không đi thì mất cọc.
Phân công hướng dẫn viên (HDV): không chồng chéo lịch; mỗi chuyến lẻ có đúng một HDV.
Lương HDV = lương căn bản + thù lao các tour kết thúc trong tháng.
Kết thúc tour: thanh toán sau tour của đoàn và phiếu khảo sát khách hàng.
Chức năng chính
Module	Mô tả
Danh mục	Phương tiện, điểm bán vé, hướng dẫn viên, điểm tham quan
Tour - hành trình	Tour, điểm dừng (hạng khách sạn 2-5 sao), phương tiện theo chặng, điểm tham quan
Lịch chuyến khách lẻ	Tạo chuyến (ngày về tự tính), đóng đăng ký
Đăng ký khách lẻ	Đăng ký theo chuyến, tính thành tiền và thanh toán vé
Đăng ký theo đoàn	Lập phiếu đoàn trong transaction, danh sách người đi, hủy phiếu (mất cọc)
Phân công HDV	Phân công cho chuyến lẻ hoặc đoàn, kiểm tra trùng lịch
Kết thúc tour - khảo sát	Thanh toán sau tour của đoàn, gửi và ghi nhận phiếu khảo sát
Lương - thống kê	Bảng lương HDV theo tháng, thống kê tổng hợp theo khoảng ngày
Công nghệ sử dụng
C# Windows Forms, .NET Framework 4.7.2
Visual Studio 2022
SQL Server (LocalDB)
ADO.NET (SqlConnection, SqlCommand, SqlDataAdapter)
Cấu trúc thư mục
LAB5/
├── README.md
├── Database/
│   └── QuanLyCongTyDuLich.sql      # Tạo CSDL, ràng buộc và dữ liệu mẫu
└── QuanLyCongTyDuLich/
    ├── QuanLyCongTyDuLich.sln      # Solution Visual Studio
    └── QuanLyCongTyDuLich/
        ├── QuanLyCongTyDuLich.csproj
        ├── App.config              # Chuỗi kết nối CSDL
        ├── Program.cs
        ├── Data/
        │   └── Db.cs               # Truy cập SQL Server, tham số hóa câu lệnh
        ├── Services/               # Nghiệp vụ và kiểm tra quy định
        │   ├── Models.cs           # KetQuaXuLy, ThanhVienDoanItem, QuyDinh
        │   ├── DanhMucService.cs
        │   ├── TourService.cs
        │   ├── ChuyenLeService.cs
        │   ├── DangKyLeService.cs
        │   ├── DangKyDoanService.cs
        │   ├── PhanCongService.cs
        │   ├── KetThucService.cs
        │   └── ThongKeService.cs
        └── Forms/                  # Giao diện (không chứa SQL)
            ├── FormHelper.cs
            ├── FrmMain.cs / .Designer.cs
            ├── FrmDanhMuc.cs / .Designer.cs
            ├── FrmTour.cs / .Designer.cs
            ├── FrmChuyenLe.cs / .Designer.cs
            ├── FrmDangKyLe.cs / .Designer.cs
            ├── FrmDangKyDoan.cs / .Designer.cs
            ├── FrmPhanCongHDV.cs / .Designer.cs
            ├── FrmKetThucKhaoSat.cs / .Designer.cs
            └── FrmLuongThongKe.cs / .Designer.cs
Kiến trúc
Forms (UI): nhận thao tác người dùng, gọi Service và hiển thị thông báo. Không viết câu lệnh SQL.
Services: chứa toàn bộ nghiệp vụ và kiểm tra quy định. Mỗi phương thức trả về KetQuaXuLy (thành công hoặc thất bại kèm lý do).
Data (Db.cs): thực thi câu lệnh SQL có tham số.
Transaction: lập phiếu đoàn và hủy đoàn chạy trong một transaction, không để dữ liệu dở dang.
Cài đặt và chạy
Tạo cơ sở dữ liệu. Mở SQL Server Management Studio hoặc Visual Studio (SQL Server Object Explorer), kết nối (localdb)\MSSQLLocalDB, mở và thực thi Database/QuanLyCongTyDuLich.sql. Script tạo CSDL QuanLyCongTyDuLich kèm dữ liệu mẫu.
Mở solution. Mở QuanLyCongTyDuLich/QuanLyCongTyDuLich.sln bằng Visual Studio 2022 (cần workload .NET desktop development).
Kiểm tra chuỗi kết nối trong App.config:
xml
   <add name="QuanLyCongTyDuLichDB"
        connectionString="Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=QuanLyCongTyDuLich;Integrated Security=True"
        providerName="System.Data.SqlClient" />

Nếu dùng SQL Server khác LocalDB, sửa Data Source cho đúng. 4. Build và chạy. Chọn Build > Rebuild Solution, sau đó nhấn F5.

Dữ liệu mẫu

Script tạo sẵn 3 tour (T001 Miền Tây, T002 Đà Lạt, T003 Hà Nội - Hạ Long), 4 phương tiện, 2 điểm bán vé, 3 hướng dẫn viên, 5 điểm tham quan, 3 chuyến khách lẻ, 2 phiếu lẻ, 2 phiếu đoàn, 3 phân công và 1 phiếu khảo sát.

Lưu ý khi kiểm thử

Các ngày đi của chuyến và đoàn mẫu đã qua so với hiện tại. Khi kiểm thử đăng ký mới, hãy chọn ngày trong tương lai (ví dụ dùng ngày 05/01/2027 như TC08). Với các trường hợp kiểm thử khác, xem bảng test case trong báo cáo.

Quy định bổ sung

Đề không cho một số thông tin, nên bài làm xử lý như sau:

Thù lao từng tour do người phân công nhập khi phân công HDV.
Mức cọc do người lập phiếu nhập.
Trường hợp đúng 12 người bị từ chối ở cả khách lẻ và khách đoàn.
Tài liệu liên quan
Báo cáo: BaoCao_Bai6_QuanLyCongTyDuLich.docx (phân tích, UML, CSDL, giao diện, test case)
Bộ UML: Use Case, Class, Activity, Sequence
