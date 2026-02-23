using System;
using System.Windows;
using System.Windows.Controls;

namespace UCC.View
{
    /// <summary>
    /// Логика взаимодействия для PageAdminMenu.xaml
    /// </summary>
    public partial class PageAdminMenu : Page
    {
        private int _currentStaffId;

        // Конструктор для дизайнера Visual Studio
        public PageAdminMenu()
        {
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                InitializeComponent();
                return;
            }
            throw new InvalidOperationException("Используйте конструктор с параметром staffId.");
        }

        // Основной конструктор
        public PageAdminMenu(int staffId)
        {
            InitializeComponent();
            _currentStaffId = staffId;

            // Проверка валидности ID
            if (_currentStaffId <= 0)
            {
                MessageBox.Show("Неверный ID администратора!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                NavigationService?.Navigate(new Uri("View/PageRegAuthoMenu.xaml", UriKind.Relative));
            }
        }

        private void BtnDiagnoses_Click(object sender, RoutedEventArgs e)
        {
            var window = new WindowDiagnoseDictionary();
            window.Title = "Справочник диагнозов (МКБ-10)";
            window.ShowDialog();
        }

        private void BtnMedications_Click(object sender, RoutedEventArgs e)
        {
            var window = new WindowMedicalDictionary();
            window.Title = "Справочник лекарств";
            window.ShowDialog();
        }

        private void BtnDepartments_Click(object sender, RoutedEventArgs e)
        {
            var window = new WindowMedicalDictionary();
            window.Title = "Участки поликлиники";
            window.ShowDialog();
        }

        private void BtnRoles_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new PageManageRoles());
        }

        private void BtnPatients_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new PagePatientList());
        }

        private void BtnStaff_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new PageStaffList());
        }

        private void BTNlogout_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите выйти?",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                NavigationService?.Navigate(new Uri("View/PageRegAuthoMenu.xaml", UriKind.Relative));
            }
        }
    }
}