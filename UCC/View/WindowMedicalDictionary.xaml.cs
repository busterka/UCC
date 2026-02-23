using System.Collections.Generic;
using System.Linq;
using System.Windows;
using UCC.Model; // ← обратите внимание: Model, а не Models!

namespace UCC.View
{
    public partial class WindowMedicalDictionary : Window
    {
        public WindowMedicalDictionary()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            using (var db = new ECCEntities1())
            {
                
       

                // Лекарства
                DgMedications.ItemsSource = db.Medications.ToList();

                // Участки
                DgDepartments.ItemsSource = db.Departments.ToList();
            }
        }

        private void BtnAddDiagnosis_Click(object sender, RoutedEventArgs e)
        {
            // Пока заглушка
            MessageBox.Show("Добавление диагноза — в разработке.");
        }

        private void TxtSearchDiagnosis_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            // Можно реализовать фильтрацию позже
        }
    }
}