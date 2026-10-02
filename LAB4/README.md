# eShopping

> Mô tả ngắn gọn về dự án: _(ví dụ: Ứng dụng mua sắm trực tuyến cho phép người dùng xem sản phẩm, thêm vào giỏ hàng và đặt hàng.)_

## Giới thiệu

**eShopping** là một solution .NET (C#) được phát triển bằng Visual Studio 2022. Solution hiện gồm một project chính:

| Project | Đường dẫn |
|---------|-----------|
| eShopping | `eShopping/eShopping.csproj` |

## Tính năng

_(Chỉnh sửa theo tính năng thực tế của dự án)_

- [ ] Xem danh sách và chi tiết sản phẩm
- [ ] Giỏ hàng
- [ ] Đặt hàng và thanh toán
- [ ] Đăng ký / đăng nhập
- [ ] Quản trị sản phẩm và đơn hàng

## Yêu cầu hệ thống

- [Visual Studio 2022](https://visualstudio.microsoft.com/) (phiên bản 17.0 trở lên)
- [.NET SDK](https://dotnet.microsoft.com/download) phù hợp với `TargetFramework` trong `eShopping.csproj`
- _(Nếu có)_ SQL Server / cơ sở dữ liệu khác

## Cài đặt và chạy

### 1. Clone repository

```bash
git clone <url-repository>
cd eShopping
```

### 2. Khôi phục các gói NuGet

```bash
dotnet restore
```

### 3. Cấu hình

_(Nếu dự án có file cấu hình, ví dụ `appsettings.json`, mô tả các giá trị cần thiết như chuỗi kết nối cơ sở dữ liệu tại đây.)_

### 4. Build và chạy

Dùng command line:

```bash
dotnet build
dotnet run --project eShopping/eShopping.csproj
```

Hoặc dùng Visual Studio: mở `eShopping.sln`, chọn project **eShopping** làm startup project và nhấn **F5**.

## Cấu trúc thư mục

```
eShopping.sln
└── eShopping/
    └── eShopping.csproj
```

_(Bổ sung các thư mục như Controllers, Models, Views, Services... nếu có.)_

## Đóng góp

1. Fork repository
2. Tạo nhánh mới: `git checkout -b feature/ten-tinh-nang`
3. Commit thay đổi: `git commit -m "Thêm tính năng ..."`
4. Push và tạo Pull Request

## Giấy phép

_(Ví dụ: MIT License. Thêm file `LICENSE` vào repository.)_

## Liên hệ

- Tác giả: Tằng Minh Hào

