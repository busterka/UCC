using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using UCC.Model;
using System.Data.Entity; // Для Include()

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
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // 🔹 ДОБАВЛЕН МЕТОД ДЛЯ КНОПКИ "НАЗАД"
        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService?.CanGoBack == true)
                NavigationService.GoBack();
            else
                NavigationService?.Navigate(new Uri("View/PageAdminMenu.xaml", UriKind.Relative));
        }
    }
}