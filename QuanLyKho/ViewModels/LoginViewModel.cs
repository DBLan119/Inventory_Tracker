using System.Windows;
using System.Windows.Input;
using QuanLyKho.Services;

namespace QuanLyKho.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly AuthenticationService _authService;
        private string _username;
        private string _password;
        private string _errorMessage;

        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public ICommand LoginCommand { get; }
        public ICommand ExitCommand { get; }

        public LoginViewModel()
        {
            _authService = new AuthenticationService();
            LoginCommand = new RelayCommand(Login, CanLogin);
            ExitCommand = new RelayCommand(Exit);
        }

        private bool CanLogin(object parameter)
        {
            return !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password);
        }

        private void Login(object parameter)
        {
            if (_authService.Authenticate(Username, Password))
            {
                var mainWindow = new MainWindow();
                mainWindow.Show();
                
                // Close login window
                Application.Current.Windows[0]?.Close();
            }
            else
            {
                ErrorMessage = "Tên đăng nhập hoặc mật khẩu không đúng!";
            }
        }

        private void Exit(object parameter)
        {
            Application.Current.Shutdown();
        }
    }
}
