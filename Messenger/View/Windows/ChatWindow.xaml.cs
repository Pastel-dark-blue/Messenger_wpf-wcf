using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using WcfService;
using Messenger.ChatDB;
using Messenger.UserControls;
using Messenger.View.Pages;

namespace Messenger.View.Windows
{
    /// <summary>
    /// Логика взаимодействия для ChatWindow.xaml
    /// </summary>
    public partial class ChatWindow : Window
    {
        public ServiceReference1.ServiceChatClient client;
        public ChatUser onlineUser; // текущий пользователь
        bool IsConnected = false;

        #region иконки (пункты) левого меню, к-рые посередине
        public class MenuItems
        {
            public string PathData { get; set; }

            public int ListItemHeight { get; set; }

            public bool IsItemSelected { get; set; }
        }

        public List<MenuItems> ItemList
        {
            get
            {
                return new List<MenuItems>
                {
                    new MenuItems(){ PathData = "M19 7.001c0 3.865-3.134 7-7 7s-7-3.135-7-7c0-3.867 3.134-7.001 7-7.001s7 3.134 7 7.001zm-1.598 7.18c-1.506 1.137-3.374 1.82-5.402 1.82-2.03 " +
                    "0-3.899-.685-5.407-1.822-4.072 1.793-6.593 7.376-6.593 9.821h24c0-2.423-2.6-8.006-6.598-9.819z" },
                    new MenuItems(){ PathData = "M6,3A2,2,0,0,0,4,5L4,11A2,2,0,0,0,5.3398438,12.884766A2,2,0,0,0,6,13A2,2,0,0,0,6.0214844,13L18,15 6.0214844,17.001953A2,2,0,0,0,6," +
                    "17A2,2,0,0,0,5.3378906,17.115234A2,2,0,0,0,4,19L4,25A2,2,0,0,0,6,27A2,2,0,0,0,6.9921875,26.734375L6.9941406,26.734375 27.390625,15.921875 27.392578,15.917969A1,1,0,0,0," +
                    "28,15A1,1,0,0,0,27.390625,14.078125L6.9941406,3.265625A2,2,0,0,0,6,3z", IsItemSelected=true },
                };
            }
        }

        #endregion

        public ChatWindow(ChatUser user)
        {
            InitializeComponent();

            // наш пользователь
            onlineUser = user;

            // иконки в самой правой панели
            MenuList.UCListBox.ItemsSource = ItemList;

        }

        private void userDisconnect()
        {
            if (IsConnected && client.InnerChannel.State != System.ServiceModel.CommunicationState.Faulted)
            {
                onlineUser.LastTimeOnline = DateTime.Now;
                client.Disconnect(onlineUser.Id);  // отключение пользователя от сети
                IsConnected = false;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            userDisconnect();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {

            userDisconnect();
            Authorization authWindow = new Authorization();
            authWindow.Show();
            this.Close();
        }

        // при выборе пункта из меню - меню у нас UserControl с ListBox'ом, так что все вот так вот сложно с обращением к выбранному item'у
        private void MenuList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           MenuListControl menuListControl = sender as MenuListControl;
            
            if (menuListControl.UCListBox.SelectedIndex == 0)
            {
                // создаем страницу ChatScreenPage
                ProfilePage profilePage = new ProfilePage(ref onlineUser, this);
                // показываем страницу ChatScreenPage
                chatWindFrame.Navigate(profilePage);
            }
            else if (menuListControl.UCListBox.SelectedIndex == 1)
            {
                // создаем страницу ChatScreenPage
                ChatScreenPage chatScreenPage = new ChatScreenPage(this);

                // создаем клиента
                client = new ServiceReference1.ServiceChatClient(new System.ServiceModel.InstanceContext(chatScreenPage));
                // подключение пользователя к сети
                try
                {
                    if (client.Connect(onlineUser.Id))     // подключение пользователя к сети
                        IsConnected = true; // метод Connect возвращает true, если подключение прошло успешно
                }
                catch (Exception ex)
                {
                    MessageBox.Show("При подключении к сервису произошла ошибка.\nТекст ошибки:\t" + ex.Message);
                    this.Close();
                    return;
                }

                // показываем страницу ChatScreenPage
                chatWindFrame.Navigate(chatScreenPage);

            }
        }
    }
}
