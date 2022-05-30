using Messenger.ChatDB;
using Messenger.UserControls;
using Messenger.View.Windows;
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

namespace Messenger.View.Pages
{
    /// <summary>
    /// Логика взаимодействия для ChatScreenPage.xaml
    /// </summary>
    public partial class ChatScreenPage : Page, ServiceReference1.IServiceChatCallback
    {
        ChatWindow chatWindow;

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

        public ChatScreenPage(ChatWindow chatWindow)
        {
            InitializeComponent();
            this.chatWindow = chatWindow;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
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
                        if (msg.SenderUserId == chatWindow.onlineUser.Id) msgStatus = "Sent";
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

                    var onlineUsers = chatWindow.client.GetOnlineUsersId(); // ид пользователей в сети

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
                return null;
            }

            return listOfChatUsers;
        }

        public void MsgCallback(string msg, long id)
        {
            try
            {
                string msgStatus = "Received";
                if (id == chatWindow.onlineUser.Id) msgStatus = "Sent";
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
                chatWindow.client.SendMsg(content, senderId);
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
                sendMsg(textMsgTextBox.Text, chatWindow.onlineUser.Id);
                textMsgTextBox.Text = String.Empty;
            }
        }


        private void textMsgTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && textMsgTextBox.Text != "")
            {
                sendMsg(textMsgTextBox.Text, chatWindow.onlineUser.Id);
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
