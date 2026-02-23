using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using UCC.Class;

namespace UCC.View
{
    public partial class WindowRegPatient : Window
    {
        private readonly AuthService _authService = new AuthService();
        private byte[] _selectedPhotoBytes; // ← храним выбранное фото

        public WindowRegPatient()
        {
            InitializeComponent();
        }

        private void BtnSelectPhoto_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpeg;*.jpg;*.bmp)|*.png;*.jpeg;*.jpg;*.bmp|All files (*.*)|*.*",
                Title = "Выберите фото"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    // Загружаем превью
                    var bitmap = new BitmapImage(new Uri(openFileDialog.FileName));
                    ImgPreview.Source = bitmap;

                    // Сохраняем байты для регистрации
                    _selectedPhotoBytes = File.ReadAllBytes(openFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки фото:\n{ex.Message}",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    _selectedPhotoBytes = null;
                    ImgPreview.Source = new BitmapImage(new Uri("pack://application:,,,/Images/default-avatar.png"));
                }
            }
        }

        private async void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtFullName.Text) ||
                dpBirthDate.SelectedDate == null ||
                string.IsNullOrEmpty(txtEmail.Text) ||
                string.IsNullOrEmpty(txtPassword.Password))
            {
                MessageBox.Show("Заполните все обязательные поля!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                bool success = await _authService.RegisterPatientAsync(
                    txtFullName.Text,
                    dpBirthDate.SelectedDate.Value,
                    txtPhone.Text,
                    "", // адрес
                    txtEmail.Text,
                    txtPassword.Password,
                    _selectedPhotoBytes // ← передаём фото (может быть null)
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
                MessageBox.Show($"Ошибка регистрации: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}