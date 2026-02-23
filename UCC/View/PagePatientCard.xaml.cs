using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Win32; // Для OpenFileDialog
using UCC.Model;

namespace UCC.View
{
    public partial class PagePatientCard : Page
    {
        private int _patientId;

        public PagePatientCard(int patientId)
        {
            InitializeComponent();
            _patientId = patientId;
            LoadPatientData();
            LoadEpisodeHistory();
        }

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

                    TxtFullName.Text = patient.FullName ?? "—";
                    TxtBirthDate.Text = patient.DateOfBirth?.ToString("dd.MM.yyyy") ?? "—";
                    TxtPhone.Text = patient.Phone ?? "—";
                    TxtInsurance.Text = $"ID: {_patientId}";
                    TxtDistrict.Text = "—";

                    LoadProfileImageFromBytes(patient.Image);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadProfileImageFromBytes(byte[] imageData)
        {
            try
            {
                if (imageData != null && imageData.Length > 0)
                {
                    using (var ms = new MemoryStream(imageData))
                    {
                        var bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.StreamSource = ms;
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.EndInit();
                        bitmap.Freeze();
                        ImgProfile.Source = bitmap;
                    }
                    return;
                }
            }
            catch { /* Игнорируем ошибки */ }

            // Заглушка по умолчанию
            try
            {
                ImgProfile.Source = new BitmapImage(new Uri("pack://application:,,,/Images/default-avatar.png"));
            }
            catch
            {
                ImgProfile.Source = null;
            }
        }

        // 🔹 НОВЫЙ МЕТОД: Смена фото
        private void BtnChangePhoto_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpeg;*.jpg;*.bmp)|*.png;*.jpeg;*.jpg;*.bmp|All files (*.*)|*.*",
                Title = "Выберите новое фото"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    // Загружаем изображение для превью
                    var bitmap = new BitmapImage(new Uri(openFileDialog.FileName));

                    // Преобразуем в byte[]
                    byte[] imageBytes = File.ReadAllBytes(openFileDialog.FileName);

                    // Сохраняем в БД
                    SavePhotoToDatabase(imageBytes);

                    // Обновляем отображение
                    ImgProfile.Source = bitmap;

                    MessageBox.Show("Фото успешно обновлено!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки фото:\n{ex.Message}",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // 🔹 НОВЫЙ МЕТОД: Сохранение фото в БД
        private void SavePhotoToDatabase(byte[] imageData)
        {
            using (var db = new ECCEntities1())
            {
                var patient = db.Patients.FirstOrDefault(p => p.PatientId == _patientId);
                if (patient != null)
                {
                    patient.Image = imageData;
                    db.SaveChanges();
                }
            }
        }

        // 🔹 ЗАГРУЗКА ИСТОРИИ БОЛЕЗНЕЙ
        private void LoadEpisodeHistory()
        {
            try
            {
                using (var db = new ECCEntities1())
                {
                    var medicalCard = db.MedicalCards.FirstOrDefault(mc => mc.PatientId == _patientId);
                    if (medicalCard == null)
                    {
                        DgEpisodes.ItemsSource = new List<EpisodeItem>();
                        return;
                    }

                    var episodes = db.CardDiagnoses
                        .Where(cd => cd.CardId == medicalCard.CardId)
                        .ToList()
                        .Select(cd => new EpisodeItem
                        {
                            StartDate = cd.OpenedAt ?? DateTime.Now,
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
                MessageBox.Show($"Ошибка загрузки истории: {ex.Message}", "Ошибка",
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

        // Вспомогательный класс для DataGrid
        public class EpisodeItem
        {
            public DateTime StartDate { get; set; }
            public string Diagnosis { get; set; } = string.Empty;
            public string Status { get; set; } = string.Empty;
            public string DoctorName { get; set; } = string.Empty;
        }
    }
}