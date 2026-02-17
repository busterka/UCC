using System.Windows;
using UCC.Model;

namespace UCC.View
{
    public partial class WindowDiagnosisEditor : Window
    {
        private Diagnoses _diagnosis;

        public WindowDiagnosisEditor(Diagnoses existing = null)
        {
            InitializeComponent();
            _diagnosis = existing;

            if (_diagnosis != null)
            {
                TxtCode.Text = _diagnosis.DiagnosisId.ToString();
                TxtName.Text = _diagnosis.DiseaseName ?? "";
                TxtDescription.Text = _diagnosis.Description ?? "";
                TxtCode.IsReadOnly = true;
            }
            else
            {
                TxtCode.Text = "";
                TxtCode.IsReadOnly = true;
            }
        }
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtName.Text))
            {
                MessageBox.Show("Название диагноза обязательно.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DialogResult = true;
            Close();
        }
    }
}