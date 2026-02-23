using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using UCC.Model; // ← ваша EF-модель

namespace UCC.View
{
    public partial class WindowDiagnoseDictionary : Window
    {
        private List<Diagnoses> _allDiagnoses;
        private List<Diagnoses> _filteredDiagnoses;

        public WindowDiagnoseDictionary()
        {
            InitializeComponent();
            LoadDiagnoses();
            DgDiagnoses.SelectionChanged += DgDiagnoses_SelectionChanged;
        }

        private void LoadDiagnoses()
        {
            try
            {
                using (var db = new ECCEntities1())
                {
                    _allDiagnoses = db.Diagnoses.ToList();
                    _filteredDiagnoses = new List<Diagnoses>(_allDiagnoses);
                    DgDiagnoses.ItemsSource = _filteredDiagnoses;

                    // Очищаем поле поиска после загрузки
                    TxtSearch.Text = "Поиск по названию...";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки диагнозов:\n{ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterDiagnoses();
        }

        private void BtnClearSearch_Click(object sender, RoutedEventArgs e)
        {
            TxtSearch.Text = "";
            FilterDiagnoses();
        }

        private void FilterDiagnoses()
        {
            // 🔹 ДОБАВЛЕНА ПРОВЕРКА НА NULL
            if (_allDiagnoses == null)
                return;

            var query = TxtSearch.Text?.Trim();
            if (string.IsNullOrEmpty(query))
            {
                _filteredDiagnoses = new List<Diagnoses>(_allDiagnoses);
            }
            else
            {
                var q = query.ToLower();
                _filteredDiagnoses = _allDiagnoses
                    .Where(d => d.DiagnosisId.ToString().Contains(q) ||
                               (d.DiseaseName != null && d.DiseaseName.ToLower().Contains(q)) ||
                               (d.Description != null && d.Description.ToLower().Contains(q)))
                    .ToList();
            }

            DgDiagnoses.ItemsSource = _filteredDiagnoses;
            // DgDiagnoses.Items.Refresh(); ← ЭТО НЕ НУЖНО
        }

        private void DgDiagnoses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool hasSelection = DgDiagnoses.SelectedItem != null;
            BtnEdit.IsEnabled = hasSelection;
            BtnDelete.IsEnabled = hasSelection;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var editor = new WindowDiagnosisEditor(); // новый диагноз
            if (editor.ShowDialog() == true)
            {
                using (var db = new ECCEntities1())
                {
                    var newDiag = new Diagnoses
                    {
                        DiseaseName = editor.TxtName.Text,
                        Description = editor.TxtDescription.Text
                    };
                    db.Diagnoses.Add(newDiag);
                    db.SaveChanges();
                }
                LoadDiagnoses(); // обновить список
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var selected = DgDiagnoses.SelectedItem as Diagnoses;
            if (selected != null)
            {
                var editor = new WindowDiagnosisEditor();
                editor.TxtCode.Text = selected.DiagnosisId.ToString();
                editor.TxtName.Text = selected.DiseaseName ?? "";
                editor.TxtDescription.Text = selected.Description ?? "";
                editor.TxtCode.IsReadOnly = true;

                if (editor.ShowDialog() == true)
                {
                    using (var db = new ECCEntities1())
                    {
                        var diag = db.Diagnoses.Find(selected.DiagnosisId);
                        if (diag != null)
                        {
                            diag.DiseaseName = editor.TxtName.Text;
                            diag.Description = editor.TxtDescription.Text;
                            db.SaveChanges();
                        }
                    }
                    LoadDiagnoses();
                }
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selected = DgDiagnoses.SelectedItem as Diagnoses;
            if (selected != null)
            {
                if (MessageBox.Show($"Удалить диагноз «{selected.DiseaseName}»?", "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    using (var db = new ECCEntities1())
                    {
                        db.Diagnoses.Remove(selected);
                        db.SaveChanges();
                    }
                    LoadDiagnoses();
                }
            }
        }
    }
}