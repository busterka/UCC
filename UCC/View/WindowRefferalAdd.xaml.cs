using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using UCC.Model;

namespace UCC.View
{
    public partial class WindowRefferalAdd : Window
    {
        private int _currentDoctorId; // ← int, а не Guid!

        public WindowRefferalAdd(int currentDoctorId)
        {
            InitializeComponent();
            _currentDoctorId = currentDoctorId;
            LoadPatients(); // загружает MedicalCards + Patients
        }

        private void LoadPatients()
        {
            using (var db = new ECCEntities1())
            {
                var cards = db.MedicalCards
                    .Select(mc => new
                    {
                        CardId = mc.CardId,
                        FullName = db.Patients
                            .Where(p => p.PatientId == mc.PatientId)
                            .Select(p => p.FullName)
                            .FirstOrDefault()
                    })
                    .ToList();

                CmbPatients.ItemsSource = cards;
                CmbPatients.DisplayMemberPath = "FullName";
                CmbPatients.SelectedValuePath = "CardId";
            }
        }

        private void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            if (CmbPatients.SelectedValue == null)
            {
                MessageBox.Show("Выберите пациента.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (CmbReferralType.SelectedItem == null)
            {
                MessageBox.Show("Выберите тип направления.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var db = new ECCEntities1())
                {
                    var cardId = (int)CmbPatients.SelectedValue;
                    var referralType = (CmbReferralType.SelectedItem as ComboBoxItem)?.Content.ToString();

                    var referral = new Referrals
                    {
                        CardId = cardId,
                        Purpose = $"{referralType}\n{TxtPurpose.Text}".Trim(),
                        IssuedAt = DateTime.Now,
                        StaffId = _currentDoctorId
                    };

                    db.Referrals.Add(referral);
                    db.SaveChanges();

                    MessageBox.Show("Направление создано.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    DialogResult = true;
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        public void SetPatient(int patientId, string patientName)
        {
            // Загружаем всех пациентов
            using (var db = new ECCEntities1())
            {
                var patients = db.Patients.ToList();
                CmbPatients.ItemsSource = patients;

                // Выбираем текущего пациента
                var currentPatient = patients.FirstOrDefault(p => p.PatientId == patientId);
                if (currentPatient != null)
                {
                    CmbPatients.SelectedItem = currentPatient;
                    CmbPatients.IsEnabled = false; // Фиксируем выбор
                }
            }
        }
    }
}