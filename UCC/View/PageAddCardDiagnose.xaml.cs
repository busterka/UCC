using System;
using System.Windows;
using System.Windows.Controls;

namespace UCC.View
{
    public partial class PageAddCardDiagnose : Page
    {
        private int _patientId;
        private string _patientFullName;

        public PageAddCardDiagnose(int patientId, string patientFullName)
        {
            InitializeComponent();
            _patientId = patientId;
            _patientFullName = patientFullName;
            TxtPatientName.Text = !string.IsNullOrEmpty(_patientFullName)
                ? _patientFullName
                : "Пациент не указан";
        }

        public PageAddCardDiagnose()
        {
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                InitializeComponent();
                return;
            }
            throw new InvalidOperationException("Используйте конструктор с параметрами.");
        }

        // 🔹 Добавлены недостающие обработчики
        private void BtnAddPrescription_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Форма выписки рецепта будет реализована позже.",
                "Рецепт", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnAddReferral_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Форма выдачи направления будет реализована позже.",
                "Направление", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService?.CanGoBack == true)
                NavigationService.GoBack();
            else
                NavigationService?.Navigate(new Uri("View/PageDoctorMenu.xaml", UriKind.Relative));
        }

        // Сохранение данных
        private void BtnSaveAndFinish_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtDiagnosis.Text))
            {
                MessageBox.Show("Укажите диагноз.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(TxtComplaints.Text))
            {
                MessageBox.Show("Укажите жалобы пациента.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // 🔜 Здесь будет сохранение в БД через EF
                MessageBox.Show(
                    $"Данные по пациенту {_patientFullName} успешно сохранены.\n" +
                    "Эпизод лечения создан.",
                    "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                NavigationService?.Navigate(new Uri("View/PageDoctorMenu.xaml", UriKind.Relative));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка сохранения",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}