using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using UCC.Model;

namespace UCC.View
{
    public partial class PageManageRoles : Page
    {
        private MedicalRoles _selectedRole;

        public PageManageRoles()
        {
            InitializeComponent();
            LoadRoles();
        }

        private void LoadRoles()
        {
            using (var db = new ECCEntities1())
            {
                DgRoles.ItemsSource = db.MedicalRoles.ToList();
            }
        }

        private void DgRoles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedRole = DgRoles.SelectedItem as MedicalRoles;
            bool hasSelection = (_selectedRole != null);
            BtnEdit.IsEnabled = hasSelection;
            BtnDelete.IsEnabled = hasSelection;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var editor = new WindowRoleEditor(null);
            if (editor.ShowDialog() == true)
            {
                LoadRoles(); // Обновить список
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedRole != null)
            {
                var editor = new WindowRoleEditor(_selectedRole);
                if (editor.ShowDialog() == true)
                {
                    LoadRoles();
                }
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedRole != null)
            {
                if (MessageBox.Show($"Удалить роль «{_selectedRole.RoleName}»?\nЭто повлияет на всех пользователей с этой ролью!",
                    "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    try
                    {
                        using (var db = new ECCEntities1())
                        {
                            // Проверка: есть ли пользователи с этой ролью
                            var usersWithRole = db.Staff.Count(s => s.RoleId == _selectedRole.RoleId);
                            if (usersWithRole > 0)
                            {
                                MessageBox.Show($"Нельзя удалить роль: ей присвоено {usersWithRole} пользователей.\n" +
                                                "Сначала измените их роли.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                return;
                            }

                            db.MedicalRoles.Remove(_selectedRole);
                            db.SaveChanges();
                            LoadRoles();
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