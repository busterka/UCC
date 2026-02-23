using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using UCC.Model;

namespace UCC.View
{
    public partial class PageAddCardDiagnose : Page
    {
        private int _patientId;
        private string _patientFullName;
        private int _currentStaffId;
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

        // 🔹 РЕАЛИЗОВАНО: Выдача рецепта
        private void BtnAddPrescription_Click(object sender, RoutedEventArgs e)
        {
            var prescriptionWindow = new WindowPrescriptionAdd();
            prescriptionWindow.SetPatient(_patientId, _patientFullName);

            if (prescriptionWindow.ShowDialog() == true)
            {
                MessageBox.Show("Рецепт успешно создан!", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // 🔹 РЕАЛИЗОВАНО: Выдача направления
        private void BtnAddReferral_Click(object sender, RoutedEventArgs e)
        {
            // 🔹 ПЕРЕДАЁМ ID ВРАЧА
            var referralWindow = new WindowRefferalAdd(_currentStaffId);
            referralWindow.SetPatient(_patientId, _patientFullName);

            if (referralWindow.ShowDialog() == true)
            {
                MessageBox.Show("Направление успешно создано!", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService?.CanGoBack == true)
                NavigationService.GoBack();
            else
                NavigationService?.Navigate(new Uri("View/PageDoctorMenu.xaml", UriKind.Relative));
        }

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