using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using UCC.Model; // ваша EF-модель

namespace UCC.View
{
    public partial class PagePatientCard : Page
    {
        private int _patientId; // ← используем int, как в БД

        public PagePatientCard(int patientId)
        {
            InitializeComponent();
            _patientId = patientId;
            LoadPatientData();
            LoadEpisodeHistory();
        }

        // Конструктор для дизайнера
        public PagePatientCard()
        {
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                InitializeComponent();
                return;
            }
            throw new InvalidOperationException("Используйте конструктор с int patientId.");
        }

        private void LoadPatientData()
        {
            try
            {
                using (var db = new ECCEntities1())
                {
                    var patient = db.Patients.FirstOrDefault(p => p.PatientId == _patientId);
                    if (patient == null)
                    {
                        MessageBox.Show("Пациент не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Отображаем только доступные поля
                    TxtFullName.Text = patient.FullName ?? "—";
                    TxtBirthDate.Text = patient.DateOfBirth?.ToString("dd.MM.yyyy") ?? "—";
                    TxtPhone.Text = patient.Phone ?? "—";
                    TxtInsurance.Text = $"ID: {_patientId}"; // временно вместо полиса
                    TxtDistrict.Text = "—"; // участок пока не используется
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadEpisodeHistory()
        {
            try
            {
                using (var db = new ECCEntities1())
                {
                    // Находим MedicalCard по PatientId
                    var medicalCard = db.MedicalCards.FirstOrDefault(mc => mc.PatientId == _patientId);
                    if (medicalCard == null)
                    {
                        DgEpisodes.ItemsSource = new List<EpisodeItem>();
                        return;
                    }

                    // Загружаем эпизоды
                    var episodes = db.CardDiagnoses
                        .Where(cd => cd.CardId == medicalCard.CardId)
                        .ToList()
                        .Select(cd => new EpisodeItem
                        {
                            StartDate = cd.OpenedAt ?? DateTime.Now,
                            EndDate = cd.ClosedAt,
                            Diagnosis = GetDiagnosisName(db, cd.DiagnosisId),
                            Status = (cd.IsClosed == 1) ? "Закрыта" : "Активна",
                            DoctorName = GetDoctorName(db, cd.StaffId)
                        })
                        .ToList();

                    DgEpisodes.ItemsSource = episodes;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка истории: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string GetDiagnosisName(ECCEntities1 db, int? diagnosisId)
        {
            if (!diagnosisId.HasValue) return "Без диагноза";
            var diag = db.Diagnoses.FirstOrDefault(d => d.DiagnosisId == diagnosisId.Value);
            return diag?.DiseaseName ?? "Неизвестен";
        }

        private string GetDoctorName(ECCEntities1 db, int? staffId)
        {
            if (!staffId.HasValue) return "Неизвестен";
            var doctor = db.Staff.FirstOrDefault(s => s.StaffId == staffId.Value);
            return doctor?.FullName ?? "Неизвестен";
        }

        private void BtnStartVisit_Click(object sender, RoutedEventArgs e)
        {
            var diagnosePage = new PageAddCardDiagnose(_patientId, TxtFullName.Text);
            NavigationService?.Navigate(diagnosePage);
        }

        private void BtnEditPatient_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Редактирование данных будет реализовано позже.",
                "В разработке", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService?.CanGoBack == true)
                NavigationService.GoBack();
            else
                NavigationService?.Navigate(new Uri("View/PageDoctorMenu.xaml", UriKind.Relative));
        }

        // Вспомогательный класс
        public class EpisodeItem
        {
            public DateTime StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public string Diagnosis { get; set; } = string.Empty;
            public string Status { get; set; } = string.Empty;
            public string DoctorName { get; set; } = string.Empty;
        }
    }
}