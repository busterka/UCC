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
        public PageAdminMenu()
        {
            InitializeComponent();
        }

        private void BtnDiagnoses_Click(object sender, RoutedEventArgs e)
        {
            // Открываем справочник диагнозов как модальное окно
            var window = new WindowMedicalDictionary();
            window.Title = "Справочник диагнозов (МКБ-10)";
            window.ShowDialog(); // Блокирующий вызов
        }

        private void BtnMedications_Click(object sender, RoutedEventArgs e)
        {
            // Если у вас есть отдельное окно для лекарств — создайте его.
            // Пока используем то же окно с переключением контента,
            // или сообщаем, что функция в разработке.
            MessageBox.Show("Справочник лекарств будет добавлен в следующей версии.",
                "В разработке", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnDepartments_Click(object sender, RoutedEventArgs e)
        {
            // Управление участками — предположим, это часть справочника
            var window = new WindowMedicalDictionary();
            window.Title = "Участки поликлиники";
            window.ShowDialog();
        }

        private void BtnRoles_Click(object sender, RoutedEventArgs e)
        {
            // Роли персонала — можно реализовать в том же окне или отдельном
            MessageBox.Show("Управление ролями доступа — в разработке.",
                "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnPatients_Click(object sender, RoutedEventArgs e)
        {
            // Переход на страницу управления пациентами
            NavigationService?.Navigate(new PagePatientCard());
        }

        private void BtnStaff_Click(object sender, RoutedEventArgs e)
        {
            // Переход на профиль врача (админ может редактировать всех)
            NavigationService?.Navigate(new PageProfileDoctor());
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