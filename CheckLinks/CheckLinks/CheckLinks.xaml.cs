using CheckLinks.checkoutUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace CheckLinks
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void checkAllLinks(object Sender, RoutedEventArgs e)
        {
            String sUrl="";
            sUrl = inputURL.Text;
            if (sUrl.Length == 0)
                statusLine("Введите адрес", Colors.Firebrick);
            else
            {
                if (!Regex.IsMatch(sUrl, @"w*\.\w*"))
                    statusLine("Введите корректный адрес", Colors.Firebrick);
                else
                {
                    if (!Regex.IsMatch(sUrl, @"http\w*"))
                        sUrl = "http://" + sUrl;
                    ConfirmLinks cl = new ConfirmLinks();
                    statusLine("Ожидайте...", Colors.Gold);
                    cl.getNodesTagsA(sUrl);
                    Score score = new Score();
                    score.bconfirmLinksClick += scoreConfirmLinks;
                    score.Show();

                }
            }
            

        }

        private void scoreConfirmLinks(object sender, RoutedEventArgs e)
        {

        }

        private void checkSitemap(object sender, RoutedEventArgs e)
        {

        }

        private void addInList(object sender, RoutedEventArgs e)
        {

        }

        public void statusLine(String value, Color color)
        {
            statusText.Text = value;
            statusLamp.Fill = new SolidColorBrush(color);
        }
    }
}
