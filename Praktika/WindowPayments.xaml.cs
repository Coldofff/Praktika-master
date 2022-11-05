using CsvHelper;
using Syncfusion.UI.Xaml.Grid.Converter;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Praktika
{
    /// <summary>
    /// Логика взаимодействия для WindowPayments.xaml
    /// </summary>
    public partial class WindowPayments : Window
    {
        List<payments> paymentsOfUser = new List<payments>();
        users currentUser;
        DateTime dateAuthorization = new DateTime();
        int addPaymentsCount = 0;
        int editPaymentsCount = 0;
        int deletePaymentsCount = 0;
        public WindowPayments(users user)
        {
            InitializeComponent();
            paymentsOfUser = Instances.db.payments.Where(p => p.FK_user_id == user.PK_users_id).ToList();
            dataGridPayments.ItemsSource = paymentsOfUser;
            comboBoxCategory.ItemsSource = Instances.db.categories.ToList();
            currentUser = user;
            dateAuthorization = DateTime.Now;     
        }

        void UpdatePayments()
        {
            paymentsOfUser = Instances.db.payments.Where(p => p.FK_user_id == currentUser.PK_users_id).ToList();
            if (comboBoxCategory.SelectedIndex > -1)
            {
                paymentsOfUser = paymentsOfUser.Where(p => p.products.FK_category_id == (comboBoxCategory.SelectedItem as categories).PK_category_id).ToList();
            }
            if(datePickerFrom.SelectedDate!=null)
                paymentsOfUser = paymentsOfUser.Where(p => p.date >= datePickerFrom.SelectedDate).ToList();
            else if (datePickerTo.SelectedDate != null)
            {
                paymentsOfUser = paymentsOfUser.Where(p => p.date <= datePickerTo.SelectedDate).ToList();
            }
            else if (datePickerTo.SelectedDate != null && datePickerFrom.SelectedDate != null)
                paymentsOfUser = paymentsOfUser.Where(p => p.date <= datePickerTo.SelectedDate && p.date >= datePickerFrom.SelectedDate).ToList();
            paymentsOfUser = paymentsOfUser.Where(p => p.products.product_name.ToLower().Contains(textBoxSearch.Text.ToLower())).ToList();
            dataGridPayments.ItemsSource = paymentsOfUser;
        }

        private void buttonSelect_Click(object sender, RoutedEventArgs e)
        {
            UpdatePayments();
        }

        private void buttonClear_Click(object sender, RoutedEventArgs e)
        {
            ClearData();
        }

        private void ClearData()
        {
            datePickerFrom.SelectedDate = null;
            datePickerTo.SelectedDate = null;
            comboBoxCategory.SelectedIndex = -1;
            dataGridPayments.ItemsSource = Instances.db.payments.Where(p => p.FK_user_id == currentUser.PK_users_id).ToList();
        }

        private void buttonReport_Click(object sender, RoutedEventArgs e)
        {
            var document = dataGridPayments.ExportToPdf();
            document.Save($"Report { DateTime.Now.ToString("d")}.pdf");

            MessageBox.Show("Отчет сохранен");
        }

        private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            Window addPaymentWindow;
            payments payment = dataGridPayments.SelectedItem as payments;
            addPaymentWindow = new AddPaymentWindow(currentUser, payment);
            bool result = (bool)addPaymentWindow.ShowDialog();
            if (!result)
            {
                if (payment != null) editPaymentsCount++;
                else addPaymentsCount++;
                ClearData();
            }
        }

        private void buttonDelete_Click(object sender, RoutedEventArgs e)
        {
            var paymentsForRemoving = dataGridPayments.SelectedItems.Cast<payments>().ToList();
            if (paymentsForRemoving.Count == 1) {
                var item = dataGridPayments.SelectedItem as payments;
                if (MessageBox.Show($"Вы хотите удалить платеж {item.payment_key}\nКатегория: {item.purpose}\nНаименование: {item.products.product_name}\nСтоимость: {item.sum}", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    removeItems(paymentsForRemoving);
                    deletePaymentsCount++;
                }
            }
            else
            {
                if (MessageBox.Show($"Вы хотите удалить {paymentsForRemoving.Count} платежей с общей стоимостью {paymentsForRemoving.Sum(p=> p.sum)}", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    removeItems(paymentsForRemoving);
                    deletePaymentsCount++;
                }
            }
        }

        private void removeItems(List<payments> paymentsForRemoving)
        {
            try
            {
                Instances.db.payments.RemoveRange(paymentsForRemoving);
                Instances.db.SaveChanges();
                MessageBox.Show("Данные удалены");
                dataGridPayments.ItemsSource = Instances.db.payments.Where(p => p.FK_user_id == currentUser.PK_users_id).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            AddAnalysis();
            string path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Payments", $"{currentUser.login}_{DateTime.Now.ToString("d")}.csv");
            Directory.CreateDirectory(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Payments"));
            bool exists = File.Exists(path);
            MakeReport(path, exists);
        }

        private void AddAnalysis()
        {
            analyzes analysis = new analyzes()
            {
                date = DateTime.Today,
                changed_count = editPaymentsCount,
                deleted_count = deletePaymentsCount,
                added_count = addPaymentsCount,
                FK_user_id = currentUser.PK_users_id
            };
            Instances.db.analyzes.Add(analysis);
            Instances.db.SaveChanges();
        }

        void MakeReport(string path, bool exists)
        {
            List<String> stringsInfo = new List<string>() { "Дата и время авторизации", "Дата и время выхода из приложения", "Записей добавлено", "Записей изменено", "Записей удалено", "Общее количество затронутых записей" };
            List<String> listResults = new List<string>() { $"{dateAuthorization}", $"{DateTime.Now}", $"{addPaymentsCount}", $"{editPaymentsCount}", $"{deletePaymentsCount}", $"{addPaymentsCount + editPaymentsCount + deletePaymentsCount}" };
            
            using (StreamWriter streamWriter = new StreamWriter(path, true, Encoding.Default, 10))
            {
                using (CsvWriter csvWriter = new CsvWriter(streamWriter, CultureInfo.GetCultureInfo("ru-RU")))
                {
                    if (!exists) csvWriter.WriteField(stringsInfo);
                    csvWriter.NextRecord();
                    csvWriter.WriteField(listResults);
                }
            }
        }

        private void textBoxSearch_KeyDown(object sender, KeyEventArgs e)
        {
            UpdatePayments();
        }

        private void buttonAnalysis_Click(object sender, RoutedEventArgs e)
        {
            AddAnalysis();
            new AnalysisWindow(currentUser).ShowDialog();
        }
    }
}
    