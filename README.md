# 🏪 Warehouse Management System
*Hệ Thống Quản Lý Kho*

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![WPF](https://img.shields.io/badge/WPF-Desktop-0078D4?logo=windows)](https://docs.microsoft.com/en-us/dotnet/desktop/wpf/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

A desktop warehouse/inventory management application built with WPF and .NET 8.0, featuring MVVM architecture and JSON-based data storage. Perfect for small to medium businesses managing dealer transactions, inventory tracking, and debt monitoring.

> 📝 **Vietnamese**: Ứng dụng quản lý kho hàng desktop với WPF và .NET 8.0, sử dụng kiến trúc MVVM và lưu trữ dữ liệu JSON. Phù hợp cho doanh nghiệp vừa và nhỏ quản lý giao dịch đại lý, theo dõi tồn kho và công nợ.

---

## ✨ Features | Tính Năng

### 🔐 Authentication
- Secure login system with user authentication
- Default credentials: `admin` / `admin`

### 📦 Transaction Management | Quản Lý Giao Dịch
- **Import**: Track warehouse stock-in transactions
- **Export**: Track warehouse stock-out transactions
- **CRUD Operations**: Create, view, update, and delete transactions
- **Monthly Filter**: View transactions by month and year
- **Dealer Filter**: Search and filter by dealer name

### 📊 Transaction Details | Chi Tiết Giao Dịch
Each transaction includes:
- Transaction date
- Transaction type (Import/Export)
- Dealer name
- Item name
- Quantity
- Amount (VNĐ)

### 💰 Debt Management | Quản Lý Công Nợ
- Track customer debts
- Monitor unpaid purchases
- Debt status tracking (Paid/Unpaid)
- Customer purchase history

### 📈 Statistics & Reports | Thống Kê & Báo Cáo
- Total monthly import value
- Total monthly export value
- Monthly profit/loss calculation
- Transaction summaries by dealer

### 💾 Data Persistence | Lưu Trữ Dữ Liệu
- JSON-based file storage
- Automatic monthly file generation
- File naming format: `YYYY_MM.json` (e.g., `2026_03.json`)
- Storage location: `bin\Debug\net8.0-windows\Data\`

---

## 🛠️ Tech Stack | Công Nghệ

- **Framework**: .NET 8.0
- **UI**: WPF (Windows Presentation Foundation)
- **Architecture**: MVVM (Model-View-ViewModel)
- **Language**: C#
- **Data Storage**: JSON files
- **Target OS**: Windows 10/11

---

## 📁 Project Structure | Cấu Trúc Dự Án

```
QuanLyKho/
├── Models/                    # Data models
│   ├── Transaction.cs        # Transaction entity
│   ├── Debt.cs              # Debt entity
│   ├── User.cs              # User entity
│   ├── MonthlyData.cs       # Monthly data wrapper
│   └── DebtData.cs          # Debt data wrapper
├── ViewModels/               # MVVM ViewModels
│   ├── BaseViewModel.cs     # Base VM with INotifyPropertyChanged
│   ├── MainViewModel.cs     # Main window VM
│   ├── LoginViewModel.cs    # Login window VM
│   ├── DebtViewModel.cs     # Debt management VM
│   └── RelayCommand.cs      # ICommand implementation
├── Views/                    # XAML views
│   ├── MainWindow.xaml      # Main application window
│   ├── LoginWindow.xaml     # Login screen
│   └── DebtWindow.xaml      # Debt management window
├── Services/                 # Business logic services
│   ├── DataService.cs       # Transaction data service
│   ├── DebtService.cs       # Debt management service
│   └── AuthenticationService.cs # Authentication service
└── Data/                     # JSON data storage
    └── [YYYY_MM.json]       # Monthly transaction files
```

---

## 🚀 Getting Started | Bắt Đầu

### Prerequisites | Yêu Cầu
- Windows 10 or later
- .NET 8.0 SDK or Runtime ([Download](https://dotnet.microsoft.com/download/dotnet/8.0))
- Visual Studio 2022 or later (recommended)

### Installation | Cài Đặt

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/warehouse-management-wpf.git
   cd warehouse-management-wpf
   ```

2. **Open the solution**
   ```bash
   # Open with Visual Studio
   start QuanLyKho.sln
   
   # Or use command line
   dotnet restore
   ```

3. **Build the project**
   ```bash
   dotnet build
   ```

4. **Run the application**
   ```bash
   # Using PowerShell script
   .\run.ps1
   
   # Or using batch file
   .\RunApp.bat
   
   # Or using dotnet CLI
   dotnet run --project QuanLyKho/QuanLyKho.csproj
   ```

---

## 📖 Usage | Hướng Dẫn Sử Dụng

### Login | Đăng Nhập
1. Launch the application
2. Enter credentials: `admin` / `admin`
3. Click "Đăng Nhập" (Login)

### Managing Transactions | Quản Lý Giao Dịch
1. **Add Import Transaction**:
   - Select date
   - Enter dealer name, item name, quantity, and amount
   - Click "Nhập Kho" (Import)

2. **Add Export Transaction**:
   - Fill in transaction details
   - Click "Xuất Kho" (Export)

3. **View Monthly Reports**:
   - Select year and month from dropdowns
   - View filtered transactions and statistics

4. **Delete Transactions**:
   - Select one or more transactions
   - Click "Xóa" (Delete)

### Managing Debts | Quản Lý Công Nợ
1. Click "Quản Lý Công Nợ" (Debt Management)
2. Add new debt entries
3. Mark debts as paid when settled
4. View unpaid debts summary

---

## 🏗️ MVVM Architecture | Kiến Trúc MVVM

### Models
- **Transaction**: Import/Export transaction entity
- **Debt**: Customer debt tracking
- **User**: User authentication
- **MonthlyData**: Monthly transaction collection
- **DebtData**: Debt collection wrapper

### ViewModels
- **BaseViewModel**: Implements `INotifyPropertyChanged`
- **RelayCommand**: Generic `ICommand` implementation
- **MainViewModel**: Core application logic
- **LoginViewModel**: Authentication logic
- **DebtViewModel**: Debt management logic

### Views
- **MainWindow**: Primary application interface
- **LoginWindow**: User authentication interface
- **DebtWindow**: Debt management interface

### Services
- **DataService**: Transaction CRUD operations & JSON persistence
- **DebtService**: Debt management operations
- **AuthenticationService**: User authentication

---

## 📝 Data Format | Định Dạng Dữ Liệu

### Transaction JSON (YYYY_MM.json)
```json
{
  "Year": 2026,
  "Month": 3,
  "Transactions": [
    {
      "Id": "guid",
      "Date": "2026-03-15T10:30:00",
      "Type": 0,
      "DealerName": "Dealer ABC",
      "ItemName": "Product XYZ",
      "Quantity": 100,
      "Amount": 5000000
    }
  ]
}
```

---

## 🤝 Contributing | Đóng Góp

Contributions are welcome! Please feel free to submit a Pull Request.

1. Fork the project
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

---

## 📄 License | Giấy Phép

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 👤 Author | Tác Giả

**Your Name**
- GitHub: [@yourusername](https://github.com/yourusername)

---

## 🙏 Acknowledgments | Cảm Ơn

- Built with [.NET 8.0](https://dotnet.microsoft.com/)
- UI Framework: [WPF](https://docs.microsoft.com/en-us/dotnet/desktop/wpf/)
- Architecture Pattern: [MVVM](https://docs.microsoft.com/en-us/xamarin/xamarin-forms/enterprise-application-patterns/mvvm)

---

## 📞 Support | Hỗ Trợ

If you have any questions or issues, please open an issue on GitHub.

**Made with ❤️ for Vietnamese small businesses**
- **MainViewModel.cs**: ViewModel cho màn hình chính

### Views
- **LoginWindow.xaml**: Giao diện đăng nhập
- **MainWindow.xaml**: Giao diện quản lý chính

### Services
- **AuthenticationService.cs**: Xử lý xác thực người dùng
- **DataService.cs**: Xử lý đọc/ghi file JSON

## Cách Chạy Ứng Dụng

### 1. Build Ứng Dụng
```powershell
dotnet build QuanLyKho.sln
```

### 2. Chạy Ứng Dụng
```powershell
dotnet run --project QuanLyKho\QuanLyKho.csproj
```

Hoặc chạy file exe sau khi build:
```powershell
.\QuanLyKho\bin\Debug\net8.0-windows\QuanLyKho.exe
```

### 3. Publish Ứng Dụng
```powershell
dotnet publish -c Release -r win-x64 --self-contained
```

## Hướng Dẫn Sử Dụng

### Đăng Nhập
1. Khởi động ứng dụng
2. Nhập tên đăng nhập: `admin`
3. Nhập mật khẩu: `admin`
4. Click "ĐĂNG NHẬP"

### Nhập Kho
1. Chọn ngày nhập hàng
2. Nhập tên đại lý
3. Nhập tên mặt hàng
4. Nhập số lượng
5. Nhập số tiền
6. Click "NHẬP KHO"

### Xuất Kho
1. Chọn ngày xuất hàng
2. Nhập tên đại lý
3. Nhập tên mặt hàng
4. Nhập số lượng
5. Nhập số tiền
6. Click "XUẤT KHO"

### Xem Giao Dịch Theo Tháng
1. Chọn tháng từ dropdown "Tháng"
2. Chọn năm từ dropdown "Năm"
3. Danh sách giao dịch sẽ tự động cập nhật

### Xóa Giao Dịch
1. Click chọn giao dịch cần xóa trong bảng
2. Click "XÓA GIAO DỊCH"
3. Xác nhận xóa

## Công Nghệ Sử Dụng

- **.NET 8.0**: Framework chính
- **WPF (Windows Presentation Foundation)**: UI Framework
- **MVVM Pattern**: Kiến trúc ứng dụng
- **System.Text.Json**: Xử lý JSON
- **Data Binding**: Liên kết dữ liệu giữa View và ViewModel
- **ICommand**: Command pattern cho các thao tác

## Cấu Trúc Thư Mục

```
QuanLyKho/
├── Models/
│   ├── Transaction.cs
│   ├── User.cs
│   └── MonthlyData.cs
├── ViewModels/
│   ├── BaseViewModel.cs
│   ├── RelayCommand.cs
│   ├── LoginViewModel.cs
│   └── MainViewModel.cs
├── Views/
│   ├── LoginWindow.xaml
│   └── LoginWindow.xaml.cs
├── Services/
│   ├── AuthenticationService.cs
│   └── DataService.cs
├── Data/
│   └── (Các file JSON sẽ được tạo tự động tại đây)
├── MainWindow.xaml
├── MainWindow.xaml.cs
├── App.xaml
├── App.xaml.cs
└── QuanLyKho.csproj
```

## Lưu Ý

- Ứng dụng chỉ chạy trên Windows
- Dữ liệu được lưu local trong thư mục `Data`
- Mỗi tháng có một file JSON riêng
- Không có giới hạn số lượng giao dịch
- Giao dịch được sắp xếp theo thứ tự ngày mới nhất

## Mở Rộng Trong Tương Lai

- [ ] Thêm chức năng export Excel
- [ ] Báo cáo thống kê nâng cao
- [ ] Quản lý nhiều người dùng
- [ ] Backup và restore dữ liệu
- [ ] Tìm kiếm và lọc nâng cao
- [ ] In báo cáo PDF
- [ ] Dashboard với biểu đồ
- [ ] Quản lý tồn kho

## Tác Giả

Phát triển với ❤️ bằng .NET 8.0 và WPF

## License

MIT License - Tự do sử dụng và chỉnh sửa cho mục đích cá nhân và thương mại.
