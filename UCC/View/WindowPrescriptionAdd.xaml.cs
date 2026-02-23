using System;
using System.Linq;
using System.Windows;
using UCC.Model;

namespace UCC.View
{
    public partial class WindowPrescriptionAdd : Window
    {
        private int _patientId;

        public WindowPrescriptionAdd()
        {
            InitializeComponent();
            LoadMedications();
        }

        public void SetPatient(int patientId, string patientName)
        {
            _patientId = patientId;
            TxtPatientName.Text = patientName;
        }

        private void LoadMedications()
        {
            try
            {
                using (var db = new ECCEntities1())
                {
                    var medications = db.Medications.ToList();
                    CmbMedications.ItemsSource = medications;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки лекарств: {ex.Message}");
            }
        }

        private void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            if (CmbMedications.SelectedItem == null)
            {
                MessageBox.Show("Выберите лекарство.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var db = new     ECCEntities1())
                {
                    var selectedMed = CmbMedications.SelectedItem as Medications;
                    var prescription = new Prescriptions
                    {
                        PatientId = _patientId,
                        MedicationId = selectedMed.MedicationId,
                        Instructions = TxtInstructions.Text,
                        IssuedAt = DateTime.Now,
                        Status = "Active"
                    };
                    db.Prescriptions.Add(prescription);
                    db.SaveChanges();
                }
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка создания рецепта: {ex.Message}");
            }
        }
    }
}