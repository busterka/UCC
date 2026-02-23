using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using UCC.Model;
using System.Data.Entity; // Для Include()

namespace UCC.View
{
    public partial class PageStaffList : Page
    {
        private System.Collections.Generic.List<Staff> _allStaff;
        private System.Collections.Generic.List<Staff> _filteredStaff;

        public PageStaffList()
        {
            InitializeComponent();
            LoadStaff();
        }

        private void LoadStaff()
        {
            try
            {
                using (var db = new ECCEntities1())
                {
                    // ✅ Исправлено: присваиваем _allStaff
                    _allStaff = db.Staff.Include(s => s.MedicalRoles).ToList();
                    _filteredStaff = new System.Collections.Generic.List<Staff>(_allStaff);
                    DgStaff.ItemsSource = _filteredStaff;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterStaff();
        }

        private void BtnClearSearch_Click(object sender, RoutedEventArgs e)
        {
            TxtSearch.Text = "";
            FilterStaff();
        }

        private void FilterStaff()
        {
            if (_allStaff == null) return;

            var query = TxtSearch.Text?.Trim();
            if (string.IsNullOrEmpty(query))
            {
                _filteredStaff = new System.Collections.Generic.List<Staff>(_allStaff);
            }
            else
            {
                var q = query.ToLower();
                _filteredStaff = _allStaff
                    .Where(s => !string.IsNullOrEmpty(s.FullName) && s.FullName.ToLower().Contains(q))
                    .ToList();
            }
            DgStaff.ItemsSource = _filteredStaff;
        }

        private void DgStaff_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool hasSelection = DgStaff.SelectedItem != null;
            BtnEdit.IsEnabled = hasSelection;
            BtnDelete.IsEnabled = hasSelection;
        }

        private void DgStaff_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (DgStaff.SelectedItem is Staff selected)
            {
                NavigationService?.Navigate(new PageProfileDoctor(selected.StaffId));
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Добавление сотрудника будет реализовано позже.",
                "В разработке", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (DgStaff.SelectedItem is Staff selected)
            {
                MessageBox.Show("Редактирование сотрудника будет реализовано позже.",
                    "В разработке", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (DgStaff.SelectedItem is Staff selected)
            {
                if (MessageBox.Show($"Удалить сотрудника «{selected.FullName}»?",
                    "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        using (var db = new ECCEntities1())
                        {
                            db.Staff.Remove(selected);
                            db.SaveChanges();
                            LoadStaff();
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