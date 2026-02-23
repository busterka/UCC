using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using UCC.Model;

namespace UCC.View
{
    public partial class PagePatientPrescriptions : Page
    {
        private int _patientId;

        public PagePatientPrescriptions(int patientId)
        {
            InitializeComponent();
            _patientId = patientId;
            LoadPrescriptions();
        }

        private void LoadPrescriptions()
        {
            try
            {
                using (var db = new ECCEntities1())
                {
                    var prescriptions = from p in db.Prescriptions
                                        join m in db.Medications on p.MedicationId equals m.MedicationId
                                        join s in db.Staff on p.StaffId equals s.StaffId
                                        where p.PatientId == _patientId
                                        select new
                                        {
                                            p.IssuedAt,
                                            MedicationName = m.MedicationName,
                                            StandardDosage = m.StandardDosage,
                                            p.Instructions,
                                            DoctorName = s.FullName,
                                            p.Status
                                        };

                    DgPrescriptions.ItemsSource = prescriptions.ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки рецептов: {ex.Message}");
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }
    }
}