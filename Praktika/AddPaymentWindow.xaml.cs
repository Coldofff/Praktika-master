using System;
using System.Collections.Generic;
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
    /// Логика взаимодействия для AddPaymentWindow.xaml
    /// </summary>
    public partial class AddPaymentWindow : Window
    {
        users user;
        int pkId = 0;
            public AddPaymentWindow(users currentUser, payments payment)
        {
            InitializeComponent();
            comboBoxCategory.ItemsSource = Instances.db.categories.ToList();
            user = currentUser;
            if(payment != null)
            {
                pkId = payment.PK_payment_id;
                comboBoxCategory.Text = payment.products.categories.category_name;
                comboBoxProduct.Text = payment.products.product_name;
                numericUpDownCount.Value = payment.count;
                textBoxPrice.Text = payment.price.ToString();
            }
        }

        private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            if (comboBoxCategory.SelectedIndex > -1 && comboBoxProduct.SelectedIndex > -1 &&
                numericUpDownCount.Value > 0 && double.TryParse(textBoxPrice.Text, out double Value))
            {
                int id = Instances.db.payments.ToList().Count;
                string paymentId = $"{(comboBoxProduct.SelectedItem as products).categories.category_name.ToString().Substring(0, 1)}-{id + 1}-{DateTime.Now.ToString("d")}";
                try
                {
                    if(pkId==0)
                    {
                        var paymentNew = new payments()
                        {
                            payment_key = paymentId,
                            count = (int)numericUpDownCount.Value,
                            price = (decimal)Convert.ToDouble(textBoxPrice.Text),
                            sum = (decimal)(numericUpDownCount.Value * Convert.ToDouble(textBoxPrice.Text)),
                            FK_product_id = (comboBoxProduct.SelectedItem as products).PK_product_id,
                            FK_user_id = user.PK_users_id,
                            date = DateTime.Now,
                            purpose = (comboBoxCategory.SelectedItem as categories).category_name
                        };
                        Instances.db.payments.Add(paymentNew);
                    }
                    else
                    {
                        var payment = Instances.db.payments.Where(p => p.PK_payment_id == pkId).FirstOrDefault();
                        payment.count = (int)numericUpDownCount.Value;
                        payment.price = (decimal)Convert.ToDouble(textBoxPrice.Text);
                        payment.sum = (decimal)(numericUpDownCount.Value * Convert.ToDouble(textBoxPrice.Text));
                        payment.FK_product_id = (comboBoxProduct.SelectedItem as products).PK_product_id;
                        payment.FK_user_id = user.PK_users_id;
                        payment.date = DateTime.Now;
                        payment.purpose = (comboBoxCategory.SelectedItem as categories).category_name;
                        Instances.db.SaveChanges();
                    }
                    Instances.db.SaveChanges();
                    MessageBox.Show("Успех");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else MessageBox.Show("Проверьте введенные данные!");
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void comboBoxCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int id = (comboBoxCategory.SelectedItem as categories).PK_category_id;
            comboBoxProduct.ItemsSource = Instances.db.products.Where(p=> p.FK_category_id == id).ToList();
        }
    }
}
