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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Praktika
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int tries = 3;
        DispatcherTimer timer = new DispatcherTimer();
        public MainWindow()
        {
            InitializeComponent();
            textBoxName.Text = "garkusha_dz_mor";
            textBoxPassword.Password = "3db575ee7f3dd0b139a169174ba53596f6dcabb9d9e6a63552d076526600ca56";
            timer.Tick += Timer_Tick;
            timer.Interval = new TimeSpan(0, 0, 30); 
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            textBoxName.IsEnabled = true;
            textBoxPassword.IsEnabled = true;
            buttonLogin.IsEnabled = true;
            timer.Stop();
        }

        private void buttonExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void buttonLogin_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(textBoxName.Text) && !string.IsNullOrEmpty(textBoxPassword.Password))
            {
                var user = Instances.db.users.Where(p => p.login == textBoxName.Text && p.password == textBoxPassword.Password).FirstOrDefault();
                if (user != null && Hashing.hashSHA256(user.login).ToLower()==user.password)
                {
                    MessageBox.Show("Успешная авторизация");
                    Window windowPayment = new WindowPayments(user); //garkusha_dz_mor 3db575ee7f3dd0b139a169174ba53596f6dcabb9d9e6a63552d076526600ca56
                    windowPayment.Show();
                    Close();
                }
                else
                {
                    MessageBox.Show("Неверные данные для авторизации");
                    new CaptchaWindow().ShowDialog();
                    tries--;
                    if (tries <= 0)
                    {
                        textBoxName.IsEnabled = false;
                        textBoxPassword.IsEnabled = false;
                        buttonLogin.IsEnabled = false;
                        MessageBox.Show("Блокировка 30 секунд");
                        timer.Start();
                    }
                    else MessageBox.Show($"Число оставшихся попыток - {tries} ");
                }
            }
            else
            {
                MessageBox.Show("Заполните поля");
            }
        }
    }
}
