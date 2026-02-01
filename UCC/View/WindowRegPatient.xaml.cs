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
using System.Windows.Shapes;
using UCC.Class;

namespace UCC.View
{
    /// <summary>
    /// Логика взаимодействия для WindowRegPatient.xaml
    /// </summary>
    public partial class WindowRegPatient : Window
    {
        private readonly AuthService _authService = new AuthService();

        public WindowRegPatient()
        {
            InitializeComponent();
        }

        private async void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtFullName.Text) ||
                dpBirthDate.SelectedDate == null ||
                string.IsNullOrEmpty(txtEmail.Text) ||
                string.IsNullOrEmpty(txtPassword.Password))
            {
                MessageBox.Show("Заполните все обязательные поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                bool success = await _authService.RegisterPatientAsync(
                    txtFullName.Text,
                    dpBirthDate.SelectedDate.Value,
                    txtPhone.Text,
                    "", // адрес можно добавить позже
                    txtEmail.Text,
                    txtPassword.Password
                );

                if (success)
                {
                    MessageBox.Show("Регистрация успешно завершена!\nТеперь вы можете войти в систему.",
                                   "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Пользователь с таким email уже существует!",
                                   "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка регистрации: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}