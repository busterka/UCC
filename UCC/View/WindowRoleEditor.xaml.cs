using System;
using System.Windows;
using UCC.Model;

namespace UCC.View
{
    public partial class WindowRoleEditor : Window
    {
        private MedicalRoles _role;

        public WindowRoleEditor(MedicalRoles role = null)
        {
            InitializeComponent();
            _role = role;

            if (_role != null)
            {
                TxtRoleName.Text = _role.RoleName ?? "";
               
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtRoleName.Text))
            {
                MessageBox.Show("Название роли обязательно.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var db = new ECCEntities1())
                {
                    if (_role == null)
                    {
                        _role = new MedicalRoles
                        {
                            RoleName = TxtRoleName.Text.Trim()
                            // Удалено Description
                        };
                        db.MedicalRoles.Add(_role);
                    }
                    else
                    {
                        _role.RoleName = TxtRoleName.Text.Trim();
                        // Удалено Description
                    }
                    db.SaveChanges();
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}