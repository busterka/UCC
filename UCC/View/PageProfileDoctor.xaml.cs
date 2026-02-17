using System;
using System.Windows;
using System.Windows.Controls;

namespace UCC.View
{
    public partial class PageProfileDoctor : Page
    {
        private Guid _doctorId;
        private bool _isEditMode = false;

        // Конструктор с передачей ID врача (для админа или просмотра)
        public PageProfileDoctor(Guid doctorId)
        {
            InitializeComponent();
            _doctorId = doctorId;
            LoadDoctorData();
        }

        // Конструктор по умолчанию — только для дизайнера
        public PageProfileDoctor()
        {
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                InitializeComponent();
                return;
            }
            throw new InvalidOperationException("Используйте конструктор с Guid doctorId.");
        }

        private void LoadDoctorData()
        {
            // 🔜 Здесь будет вызов: DoctorService.GetById(_doctorId)
            // Сейчас — заглушка
            TxtFullName.Text = "Петров Алексей Сергеевич";
            TxtSpecialty.Text = "Терапевт";
            TxtDistrict.Text = "Участок №1 – г. Город";
            TxtCabinet.Text = "205";
            TxtEmail.Text = "petrov.as@clinic.local";
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (!_isEditMode)
            {
                // Включить редактирование
                SetEditMode(true);
                BtnEdit.Content = "Сохранить"; // ✅ Теперь правильно!
                _isEditMode = true;
            }
            else
            {
                // Сохранить изменения
                try
                {
                    // 🔜 Здесь: DoctorService.Update(...)
                    MessageBox.Show("Данные успешно сохранены.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    SetEditMode(false);
                    BtnEdit.Content = "Редактировать"; // ✅
                    _isEditMode = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SetEditMode(bool enable)
        {
            TxtFullName.IsReadOnly = !enable;
            TxtSpecialty.IsReadOnly = !enable;
            TxtDistrict.IsReadOnly = !enable;
            TxtCabinet.IsReadOnly = !enable;
            TxtEmail.IsReadOnly = !enable;
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService?.CanGoBack == true)
                NavigationService.GoBack();
            else
                NavigationService?.Navigate(new Uri("View/PageDoctorMenu.xaml", UriKind.Relative));
        }
    }
}