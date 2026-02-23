using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using UCC.Model;

namespace UCC.View
{
    public partial class PagePatientReferrals : Page
    {
        private int _patientId;

        public PagePatientReferrals(int patientId)
        {
            InitializeComponent();
            _patientId = patientId;
            LoadReferrals();
        }

        private void LoadReferrals()
        {
            try
            {
                using (var db = new ECCEntities1())
                {
                    // Проходим через MedicalCards к Patients
                    var referrals = from r in db.Referrals
                                    join mc in db.MedicalCards on r.CardId equals mc.CardId
                                    join s in db.Staff on r.StaffId equals s.StaffId
                                    join d in db.Departments on r.DepartmentId equals d.DepartmentId
                                    where mc.PatientId == _patientId // ← Фильтруем по PatientId
                                    select new
                                    {
                                        CreatedAt = r.IssuedAt,
                                        ReferralType = d.DepartmentName, // Название отделения как тип
                                        r.Purpose,
                                        DoctorName = s.FullName,
                                        Status = "Активно" // Можно добавить поле Status в БД позже
                                    };

                    DgReferrals.ItemsSource = referrals.ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки направлений: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }
    }
}