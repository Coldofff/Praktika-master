using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
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
    /// Логика взаимодействия для CaptchaWindow.xaml
    /// </summary>
    public partial class CaptchaWindow : Window
    {
        Random random = new Random();
        public CaptchaWindow()
        {
            InitializeComponent();
            textBlock.Text = FillCapctha();
        }

        string FillCapctha()
        {
            string combination = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            StringBuilder captcha = new StringBuilder();
            for (int i = 0; i < 10; i++)
            {
                captcha.Append(combination[random.Next(combination.Length)]);
            }
            return captcha.ToString();
        }

        private void textBoxCaptcha_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key==Key.Enter )
                if (textBoxCaptcha.Text == textBlock.Text) Close();
                else
                {
                    textBlock.Text = FillCapctha();
                    textBoxCaptcha.Clear();
                }
        }
    }
}
