using System;
using System.Windows;
using System.Windows.Controls;

namespace UCC.View
{
    /// <summary>
    /// Логика взаимодействия для PageDoctorMenu.xaml
    /// </summary>
    public partial class PageDoctorMenu : Page
    {
        public PageDoctorMenu()
        {
            InitializeComponent();
        }

        private void BtnNewDiagnosis_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new PageSelectPatient());
        }

        private void BtnSearchPatient_Click(object sender, RoutedEventArgs e)
        {
            // Поиск пациента — открываем карточку с возможностью поиска
            NavigationService?.Navigate(new PagePatientList());
        }


        private void BtnDiagnoses_Click(object sender, RoutedEventArgs e)
        {
            // Открываем справочник диагнозов (МКБ-10) как модальное окно
            var window = new WindowDiagnoseDictionary();
            window.Title = "Справочник диагнозов (МКБ-10)";
            window.ShowDialog();
        }

        private void BtnMedications_Click(object sender, RoutedEventArgs e)
        {
            // Открываем справочник лекарств (в том же окне или отдельном)
            // Пока используем то же окно — можно добавить вкладку позже
            var window = new WindowMedicalDictionary();
            window.Title = "Справочник лекарств";
            window.ShowDialog();
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