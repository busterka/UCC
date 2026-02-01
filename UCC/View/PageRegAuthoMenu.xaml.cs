using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UCC.Class;

namespace UCC.View
{
    /// <summary>
    /// Логика взаимодействия для PageRegAuthoMenu.xaml
    /// </summary>
    public partial class PageRegAuthoMenu : Page
    {
        private readonly AuthService _authService = new AuthService();

        public PageRegAuthoMenu()
        {
            InitializeComponent();
        }

        private async void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Password;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                string role = await _authService.LoginAsync(email, password);

                if (role == "Patient")
                {
                    NavigateToPatientMenu();
                }
                else if (role == "Doctor")
                {
                    NavigateToDoctorMenu();
                }
                else if (role == "Admin")
                {
                    NavigateToAdminMenu();
                }
                else
                {
                    MessageBox.Show("Неверный email или пароль!\nПроверьте правильность ввода.",
                                   "Ошибка авторизации", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (System.Data.Entity.Core.EntityException)
            {
                MessageBox.Show("Не удалось подключиться к базе данных!\nПроверьте:\n1. Запущен ли SQL Server\n2. Строка подключения в App.config",
                               "Ошибка БД", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            var regWindow = new WindowRegPatient();
            regWindow.Owner = Window.GetWindow(this);
            regWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            regWindow.ShowDialog();
        }

        private void NavigateToPatientMenu()
        {
            var mainWindow = Window.GetWindow(this) as MainWindow;
            mainWindow?.MainFrame.Navigate(new PagePatientMenu());
        }

        private void NavigateToDoctorMenu()
        {
            var mainWindow = Window.GetWindow(this) as MainWindow;
            mainWindow?.MainFrame.Navigate(new PageDoctorMenu());
        }

        private void NavigateToAdminMenu()
        {
            var mainWindow = Window.GetWindow(this) as MainWindow;
            mainWindow?.MainFrame.Navigate(new PageAdminMenu());
        }
    }
}