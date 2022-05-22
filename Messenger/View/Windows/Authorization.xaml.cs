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

namespace Messenger.View.Windows
{
    /// <summary>
    /// Логика взаимодействия для Authorization.xaml
    /// </summary>
    public partial class Authorization : Window
    {
        //public static ServiceReference1.ServiceChatClient client; // ServiceReference1 - из Connected Services
        
        //Authorization mainWnd { get => Application.Current.MainWindow as Authorization; }

        public Authorization()
        {
            InitializeComponent();

            // изначально показываем страницу LoginPage
            authorizationWindFrame.Navigate(
                new Uri("pack://application:,,,/View/Pages/LoginPage.xaml"), 
                UriKind.RelativeOrAbsolute);
        }
    }
}
