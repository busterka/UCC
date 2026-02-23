using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using UCC.Model;

namespace UCC.View
{
    public partial class PagePatientList : Page
    {
        private System.Collections.Generic.List<Patients> _allPatients;
        private System.Collections.Generic.List<Patients> _filteredPatients;

        public PagePatientList()
        {
            InitializeComponent();
            LoadPatients();
        }

        private void LoadPatients()
        {
            try
            {
                using (var db = new ECCEntities1())
                {
                    _allPatients = db.Patients.ToList();
                    _filteredPatients = new System.Collections.Generic.List<Patients>(_allPatients);
                    DgPatients.ItemsSource = _filteredPatients;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterPatients();
        }

        private void BtnClearSearch_Click(object sender, RoutedEventArgs e)
        {
            TxtSearch.Text = "";
            FilterPatients();
        }

        private void FilterPatients()
        {
            if (_allPatients == null) return;

            var query = TxtSearch.Text?.Trim();
            if (string.IsNullOrEmpty(query))
            {
                _filteredPatients = new System.Collections.Generic.List<Patients>(_allPatients);
            }
            else
            {
                var q = query.ToLower();
                _filteredPatients = _allPatients
                    .Where(p => !string.IsNullOrEmpty(p.FullName) && p.FullName.ToLower().Contains(q))
                    .ToList();
            }
            DgPatients.ItemsSource = _filteredPatients;
        }

        // 🔹 НЕДОСТАЮЩИЕ МЕТОДЫ
        private void DgPatients_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool hasSelection = DgPatients.SelectedItem != null;
            BtnEdit.IsEnabled = hasSelection;
            BtnDelete.IsEnabled = hasSelection;
        }

        private void DgPatients_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (DgPatients.SelectedItem is Patients selected)
            {
                NavigationService?.Navigate(new PagePatientCard(selected.PatientId));
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Добавление пациента будет реализовано позже.",
                "В разработке", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (DgPatients.SelectedItem is Patients selected)
            {
                MessageBox.Show("Редактирование пациента будет реализовано позже.",
                    "В разработке", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (DgPatients.SelectedItem is Patients selected)
            {
                if (MessageBox.Show($"Удалить пациента «{selected.FullName}»?",
                    "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        using (var db = new ECCEntities1())
                        {
                            var hasMedicalCard = db.MedicalCards.Any(mc => mc.PatientId == selected.PatientId);
                            if (hasMedicalCard)
                            {
                                MessageBox.Show("Нельзя удалить пациента: у него есть медицинская карта.",
                                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                return;
                            }

                            db.Patients.Remove(selected);
                            db.SaveChanges();
                            LoadPatients();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка удаления: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
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