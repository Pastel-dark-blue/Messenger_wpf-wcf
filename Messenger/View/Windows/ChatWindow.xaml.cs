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

namespace Messenger.View.Windows
{
    /// <summary>
    /// Логика взаимодействия для ChatWindow.xaml
    /// </summary>
    public partial class ChatWindow : Window, ServiceReference1.IServiceChatCallback
    {
        ServiceReference1.ServiceChatClient client;
        ChatUser onlineUser; // текущий пользователь
        bool IsConnected = false;

        //#region иконки (пункты) левого меню
        //public class MenuItems
        //{
        //    public BitmapImage ImageSource { get; set; }
        //    public int ListItemHeight { get; set; }
        //    public bool IsItemSelected { get; set; }
        //}

        //public List<MenuItems> ItemList
        //{
        //    get
        //    {
        //        return new List<MenuItems>
        //        {
        //            new MenuItems(){ ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/messages.png")), IsItemSelected = true},
        //            new MenuItems(){ ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/user.png")) },
        //        };
        //    }
        //}
        //#endregion

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

        #region пользователи из панели слева
        public class ChatListUser
        {
            public string Photo { get; set; }

            public bool IsOnline { get; set; }

            public string Login { get; set; }

            public string About { get; set; }

            public string LastTimeOnline { get; set; }
        }
        #endregion

        #region сообщения чата
        public class ConversationMessages
        {
            public string MessageStatus { get; set; }

            public string Message { get; set; }

            public string Timestamp { get; set; }

            public string SenderPhoto { get; set; }
        }
        #endregion

        public ChatWindow(ChatUser user)
        {
            InitializeComponent();

            // создаем клиента
            client = new ServiceReference1.ServiceChatClient(new System.ServiceModel.InstanceContext(this));

            // наш пользователь
            onlineUser = user;

            // иконки в самой правой панели
            MenuList.UCListBox.ItemsSource = ItemList;

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

            // подгружаем юзеров в панель 
            UsersListBox.UCListBox.ItemsSource = GetUsers();
            UsersListBox.UCListBox.SelectedIndex = 0; // первй юзер будет выбран по умолчанию

            try
            {
                // выгружаем из БД сообщения 
                using (ChatDBModel db = new ChatDBModel())
                {
                    // листбокс с сообщениями
                    var messages = db.Message;
                    List<ConversationMessages> msgList = new List<ConversationMessages>();
                    string msgStatus = "Received";
                    foreach (var msg in messages)
                    {
                        if (msg.SenderUserId == onlineUser.Id) msgStatus = "Sent";
                        else msgStatus = "Received";

                        ConversationListBox.UCListBox.Items.Add(
                            new ConversationMessages
                            {
                                MessageStatus = msgStatus,
                                Message = msg.Content,
                                Timestamp = msg.CreationDate.ToString(),
                                //SenderPhoto=msg.ChatUser?.Photo.ToString(),
                            });
                    }
                    
                    // автоматически пороматываем к последнему соо
                    ConversationListBox.UCListBox.ScrollIntoView(ConversationListBox.UCListBox.Items[ConversationListBox.UCListBox.Items.Count - 1]);
                        
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("При попытке получить данные из базы данных произошла ошибка.\nТекст ошибки:\t" + ex.Message);
                this.Close();
                return;
            }
        }

        // получаем всех пользователей со статусами (онлайн/офлайн)
        private List<ChatListUser> GetUsers()
        {
            // у ChatListUsers есть св-во - онлайн/офлайн, так что лист с этим типом, а не ChatUser, будет заполнять панель окна чата
            List<ChatListUser> listOfChatUsers = new List<ChatListUser>();

            try
            {
                using (ChatDBModel db = new ChatDBModel())
                {
                    // листбокс с последними 20 зарегистрировавшимися юзерами
                    var users = db.ChatUser; // получаем список всех зарегистрированных пользователей

                    var onlineUsers = client.GetOnlineUsersId(); // ид пользователей в сети

                    bool IsUserOnline = false;
                    foreach (var u in users)
                    {
                        foreach (var onlineu in onlineUsers)
                        {
                            if (onlineu == u.Id) // если ид юзера из бд совпадает с юзером онлайн
                            {
                                IsUserOnline = true;
                                break;
                            }
                            else // иначе
                            {
                                IsUserOnline = false;
                            }
                        }

                        listOfChatUsers.Add(
                                    new ChatListUser
                                    {
                                        Photo = u.Photo?.ToString(),
                                        Login = u.Login,
                                        IsOnline = IsUserOnline,
                                        About = u.About,
                                        LastTimeOnline = u.LastTimeOnline.ToString(),
                                    }
                                );
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("При попытке получить данные из базы данных произошла ошибка.\nТекст ошибки:\t" + ex.Message);
                this.Close();
                return null;
            }

            return listOfChatUsers;
        }

        public void MsgCallback(string msg, long id)
        {
            try
            {
                string msgStatus = "Received";
                if (id == onlineUser.Id) msgStatus = "Sent";
                else msgStatus = "Received";

                ConversationMessages newMsg = new ConversationMessages
                {
                    MessageStatus = msgStatus,
                    Message = msg,
                    Timestamp = DateTime.Now.ToString(),
                    //SenderPhoto=msg.ChatUser?.Photo.ToString(),
                };
                ConversationListBox.UCListBox.Items.Add(newMsg);
                // автоматически пороматываем к последнему соо
                ConversationListBox.UCListBox.ScrollIntoView(ConversationListBox.UCListBox.Items[ConversationListBox.UCListBox.Items.Count - 1]);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при попытке прислать вам сообщение.\n" +
                    "Текст ошибки: " + ex.Message);
            }
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

        //отправка сообщения
        private void sendMsg(string content, long senderId)
        {
            try
            {
                // записываем соо в БД
                using (ChatDBModel db = new ChatDBModel())
                {
                    Message msg = new Message();
                    msg.Content = content;
                    msg.CreationDate = DateTime.Now;
                    msg.SenderUserId = senderId;

                    db.Message.Add(msg);
                    db.SaveChanges();
                }

                // отправляем соо всем клиентам, подключенным к чату
                client.SendMsg(content, senderId);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при попытке отправить ваше сообщение.\n" +
                    "Текст ошибки: " + ex.Message);
            }
        }

        private void sendMsgButton_Click(object sender, RoutedEventArgs e)
        {
            if (textMsgTextBox.Text != "")
            {
                sendMsg(textMsgTextBox.Text, onlineUser.Id);
                textMsgTextBox.Text = String.Empty;
            }
        }


        private void textMsgTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && textMsgTextBox.Text != "")
            {
                sendMsg(textMsgTextBox.Text, onlineUser.Id);
                textMsgTextBox.Text = String.Empty;
            }
        }

        private void UsersListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ChatList chatList = sender as ChatList;

            ChatListUser cu = chatList.UCListBox.SelectedItem as ChatListUser;
            UserInfoStackPanel.DataContext = cu;
            InfoPanel.Width = new GridLength(310);
        }

        private void CloseInfoPanel(object sender, MouseButtonEventArgs e)
        {
            InfoPanel.Width = new GridLength(0);
        }

        private void RefreshUsersList(object sender, MouseButtonEventArgs e)
        {
            UsersListBox.UCListBox.ItemsSource = GetUsers();
        }
    }
}
