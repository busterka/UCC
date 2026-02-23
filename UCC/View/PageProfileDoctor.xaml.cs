using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Win32; // Для OpenFileDialog
using UCC.Model;
using System.Data.Entity;

namespace UCC.View
{
    public partial class PageProfileDoctor : Page
    {
        private int _staffId;

        public PageProfileDoctor(int staffId)
        {
            InitializeComponent();
            _staffId = staffId;
            LoadStaffData();
        }

        public PageProfileDoctor()
        {
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                InitializeComponent();
                return;
            }
            throw new InvalidOperationException("Используйте конструктор с int staffId.");
        }

        private void LoadStaffData()
        {
            try
            {
                using (var db = new ECCEntities1())
                {
                    var staff = db.Staff
                        .Include(s => s.MedicalRoles)
                        .FirstOrDefault(s => s.StaffId == _staffId);

                    if (staff == null)
                    {
                        MessageBox.Show("Сотрудник не найден.", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    TxtFullName.Text = staff.FullName ?? "—";
                    TxtEmail.Text = staff.Email ?? "—";
                    TxtRole.Text = staff.MedicalRoles?.RoleName ?? "—";
                    TxtCabinet.Text = staff.Cabinet ?? "—";

                    LoadProfileImageFromBytes(staff.Image);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // 🔹 МЕТОД ЗАГРУЗКИ ФОТО ИЗ BYTE[]
        private void LoadProfileImageFromBytes(byte[] imageData)
        {
            try
            {
                if (imageData != null && imageData.Length > 0)
                {
                    using (var ms = new MemoryStream(imageData))
                    {
                        var bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.StreamSource = ms;
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.EndInit();
                        bitmap.Freeze();
                        ImgProfile.Source = bitmap;
                    }
                    return;
                }
            }
            catch { /* Игнорируем ошибки */ }

            // Заглушка по умолчанию
            try
            {
                ImgProfile.Source = new BitmapImage(new Uri("pack://application:,,,/Images/default-avatar.png"));
            }
            catch
            {
                ImgProfile.Source = null;
            }
        }

        // 🔹 НОВЫЙ МЕТОД: Выбор нового фото
        private void BtnChangePhoto_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpeg;*.jpg;*.bmp)|*.png;*.jpeg;*.jpg;*.bmp|All files (*.*)|*.*",
                Title = "Выберите фото профиля"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    // Загружаем изображение
                    var bitmap = new BitmapImage(new Uri(openFileDialog.FileName));
                    ImgProfile.Source = bitmap;

                    // Сохраняем в базу данных
                    SavePhotoToDatabase(File.ReadAllBytes(openFileDialog.FileName));
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки фото: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    LoadStaffData(); // Восстанавливаем оригинальное фото
                }
            }
        }

        // 🔹 НОВЫЙ МЕТОД: Сохранение фото в БД
        private void SavePhotoToDatabase(byte[] imageData)
        {
            try
            {
                using (var db = new ECCEntities1())
                {
                    var staff = db.Staff.FirstOrDefault(s => s.StaffId == _staffId);
                    if (staff != null)
                    {
                        staff.Image = imageData;
                        db.SaveChanges();
                        MessageBox.Show("Фото успешно обновлено!", "Успех",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения фото: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService?.CanGoBack == true)
                NavigationService.GoBack();
            else
                NavigationService?.Navigate(new Uri("View/PageAdminMenu.xaml", UriKind.Relative));
        }
    }
}