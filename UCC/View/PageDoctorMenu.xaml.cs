using System;
using System.Windows;
using System.Windows.Controls;
using UCC.Model; // Для работы с DbContext

namespace UCC.View
{
    public partial class PageDoctorMenu : Page
    {
        // Хранит ID текущего врача
        private int _currentStaffId;

        // Конструктор для дизайнера
        public PageDoctorMenu()
        {
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                InitializeComponent();
                return;
            }
            throw new InvalidOperationException("Используйте конструктор с параметром staffId.");
        }

        // Основной конструктор
        public PageDoctorMenu(int staffId)
        {
            InitializeComponent();
            _currentStaffId = staffId;
        }

        private void BtnNewDiagnosis_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new PageSelectPatient());
        }

        private void BtnSearchPatient_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new PagePatientList());
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

        // 🔹 НОВЫЙ МЕТОД: Переход в профиль
        private void BtnProfile_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new PageProfileDoctor(_currentStaffId));
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы действительно хотите выйти?",
                "Подтверждение выхода",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                NavigationService?.Navigate(new Uri("View/PageRegAuthoMenu.xaml", UriKind.Relative));
            }
        }
    }
}