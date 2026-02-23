// Только официальный PdfSharp для .NET Framework
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using UCC.Model;

namespace UCC.View
{
    /// <summary>
    /// Логика взаимодействия для PagePatientMenu.xaml
    /// </summary>
    public partial class PagePatientMenu : Page
    {
        private int _currentPatientId;

        // Конструктор для дизайнера
        public PagePatientMenu()
        {
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                InitializeComponent();
                return;
            }
            throw new InvalidOperationException("Используйте конструктор с параметром int patientId.");
        }

        // Основной конструктор
        public PagePatientMenu(int patientId)
        {
            InitializeComponent();
            _currentPatientId = patientId;

            // Проверка валидности ID
            if (_currentPatientId <= 0)
            {
                MessageBox.Show("Неверный ID пациента!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                NavigationService?.Navigate(new Uri("View/PageRegAuthoMenu.xaml", UriKind.Relative));
            }
        }

        private void BtnMyCard_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new PagePatientCard(_currentPatientId));
        }

        private void BtnHistory_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new PagePatientCard(_currentPatientId));
        }

        private void BtnPrescriptions_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Список ваших рецептов будет отображён здесь.\n" +
                            "Функция в разработке.",
                "Рецепты", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnReferrals_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Ваши направления к врачам и на исследования.",
                "Направления", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnLabTests_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Результаты лабораторных анализов будут доступны здесь.",
                "Анализы", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы действительно хотите выйти?",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                NavigationService?.Navigate(new Uri("View/PageRegAuthoMenu.xaml", UriKind.Relative));
            }
        }

        private void BtnDownloadPdf_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Загружаем данные из БД
                PatientInfo patient = null;
                List<EpisodeInfo> episodes = new List<EpisodeInfo>();

                using (var db = new ECCEntities1())
                {
                    var p = db.Patients.Find(_currentPatientId);
                    if (p == null)
                        throw new Exception("Пациент не найден");

                    patient = new PatientInfo
                    {
                        Id = p.PatientId,
                        FullName = p.FullName ?? "—",
                        DateOfBirth = p.DateOfBirth?.ToString("dd.MM.yyyy") ?? "—",
                        Phone = p.Phone ?? "—"
                    };

                    // Загружаем историю болезней
                    var card = db.MedicalCards.FirstOrDefault(mc => mc.PatientId == _currentPatientId);
                    if (card != null)
                    {
                        var diagnoses = db.CardDiagnoses
                            .Where(cd => cd.CardId == card.CardId)
                            .ToList();

                        foreach (var d in diagnoses)
                        {
                            string diagnosisName = "Без диагноза";
                            if (d.DiagnosisId > 0)
                            {
                                var diag = db.Diagnoses.Find(d.DiagnosisId);
                                diagnosisName = diag?.DiseaseName ?? "Неизвестен";
                            }

                            string doctorName = "Неизвестен";
                            if (d.StaffId > 0)
                            {
                                var staff = db.Staff.Find(d.StaffId);
                                doctorName = staff?.FullName ?? "Неизвестен";
                            }

                            episodes.Add(new EpisodeInfo
                            {
                                StartDate = d.OpenedAt?.ToString("dd.MM.yyyy") ?? "—",
                                Diagnosis = diagnosisName,
                                Status = (d.IsClosed == 1) ? "Закрыта" : "Активна",
                                Doctor = doctorName
                            });
                        }
                    }
                }

                // Создаём PDF
                var document = new PdfDocument();
                var page = document.AddPage();
                var gfx = XGraphics.FromPdfPage(page);

                // Используем шрифт, который точно есть в Windows
                var font = new XFont("Arial", 10);
                var boldFont = new XFont("Arial", 10, XFontStyle.Bold);
                var titleFont = new XFont("Arial", 14, XFontStyle.Bold);

                float y = 50;
                const float lineHeight = 16;

                // Заголовок
                gfx.DrawString("МЕДИЦИНСКАЯ КАРТА ПАЦИЕНТА", titleFont, XBrushes.Black, 50, y);
                y += 30;

                // Данные пациента
                gfx.DrawString($"ID: {patient.Id}", boldFont, XBrushes.Black, 50, y); y += lineHeight;
                gfx.DrawString($"ФИО: {patient.FullName}", boldFont, XBrushes.Black, 50, y); y += lineHeight;
                gfx.DrawString($"Дата рождения: {patient.DateOfBirth}", font, XBrushes.Black, 50, y); y += lineHeight;
                gfx.DrawString($"Телефон: {patient.Phone}", font, XBrushes.Black, 50, y); y += lineHeight * 2;

                // История болезней
                gfx.DrawString("ИСТОРИЯ БОЛЕЗНЕЙ", new XFont("Arial", 12, XFontStyle.Bold), XBrushes.Black, 50, y);
                y += 20;

                // Заголовки таблицы
                gfx.DrawString("Дата начала", boldFont, XBrushes.Black, 50, y);
                gfx.DrawString("Диагноз", boldFont, XBrushes.Black, 150, y);
                gfx.DrawString("Статус", boldFont, XBrushes.Black, 350, y);
                gfx.DrawString("Врач", boldFont, XBrushes.Black, 450, y);
                y += lineHeight + 5;

                // Данные
                foreach (var ep in episodes)
                {
                    gfx.DrawString(ep.StartDate, font, XBrushes.Black, 50, y);
                    gfx.DrawString(ep.Diagnosis, font, XBrushes.Black, 150, y);
                    gfx.DrawString(ep.Status, font, XBrushes.Black, 350, y);
                    gfx.DrawString(ep.Doctor, font, XBrushes.Black, 450, y);
                    y += lineHeight;

                    if (y > 780)
                    {
                        page = document.AddPage();
                        gfx = XGraphics.FromPdfPage(page);
                        y = 50;
                    }
                }

                // Сохраняем
                string fileName = $"Карта_пациента_{_currentPatientId}_{DateTime.Now:yyyyMMdd}.pdf";
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName);
                document.Save(path);

                MessageBox.Show($"PDF сохранён:\n{path}", "Успех", MessageBoxButton.OK);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка создания PDF:\n{ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Вспомогательные классы
        private class PatientInfo
        {
            public int Id { get; set; }
            public string FullName { get; set; }
            public string DateOfBirth { get; set; }
            public string Phone { get; set; }
        }

        private class EpisodeInfo
        {
            public string StartDate { get; set; }
            public string Diagnosis { get; set; }
            public string Status { get; set; }
            public string Doctor { get; set; }
        }
    }
}