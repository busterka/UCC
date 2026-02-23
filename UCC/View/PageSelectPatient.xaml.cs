using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using UCC.Model;

namespace UCC.View
{
    public partial class PageSelectPatient : Page
    {
        private Patients _selectedPatient;

        public PageSelectPatient()
        {
            InitializeComponent();
            LoadPatients();
        }

        private void LoadPatients()
        {
            using (var db = new ECCEntities1())
            {
                DgPatients.ItemsSource = db.Patients.ToList();
            }
        }

        private void DgPatients_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            _selectedPatient = DgPatients.SelectedItem as Patients;
            BtnStartVisit.IsEnabled = (_selectedPatient != null);
        }

        private void BtnStartVisit_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedPatient != null)
            {
                NavigationService?.Navigate(new PageAddCardDiagnose(_selectedPatient.PatientId, _selectedPatient.FullName));
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService?.CanGoBack == true)
                NavigationService.GoBack();
            else
                NavigationService?.Navigate(new Uri("View/PageDoctorMenu.xaml", UriKind.Relative));
        }
    }
}