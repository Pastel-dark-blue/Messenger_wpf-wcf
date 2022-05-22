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

namespace Messenger.View.Windows
{
    /// <summary>
    /// Логика взаимодействия для ChatWindow.xaml
    /// </summary>
    public partial class ChatWindow : Window, ServiceReference1.IServiceChatCallback
    {
        ServiceReference1.ServiceChatClient client;
        ChatUser onlineUser;
        bool IsConnected = false;

        public ChatWindow(ChatUser user)
        {
            InitializeComponent();

            client = new ServiceReference1.ServiceChatClient(new System.ServiceModel.InstanceContext(this));

            onlineUser = user;

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

            try
            {
                // выгружаем из БД сообщения
                using (ChatDBModel db = new ChatDBModel())
                {
                    var messages = db.Message;

                    foreach (var msg in messages)
                    {
                        chatBox.Items.Add(msg.CreationDate + " " + msg.SenderUserId + ": " + msg.Content);
                    }

                    //var onlineUsers = client.GetOnlineUsers();
                    //foreach (var u in onlineUsers)
                    //    MessageBox.Show(u.Login);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("При попытке получить данные из базы данных произошла ошибка.\nТекст ошибки:\t" + ex.Message);
                this.Close();
                return;
            }
        }

        public void MsgCallback(string msg, long id)
        {
            try
            {
                chatBox.Items.Add(id + ": " + msg);

            }
            catch(Exception ex)
            {
                MessageBox.Show("Произошла ошибка при попытке прислать вам сообщение.\n" +
                    "Текст ошибки: " + ex.Message);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (IsConnected && client.InnerChannel.State != System.ServiceModel.CommunicationState.Faulted)
            {
                client.Disconnect(onlineUser.Id);  // отключение пользователя от сети
                IsConnected = false;
            }
        }

        // отправка сообщения
        private void msgTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && msgTextBox.Text != "") 
            {
                try
                {
                    // записываем соо в БД
                    using (ChatDBModel db = new ChatDBModel())
                    {
                        Message msg = new Message();
                        msg.Content = msgTextBox.Text;
                        msg.CreationDate = DateTime.Now;
                        msg.SenderUserId = onlineUser.Id;

                        db.Message.Add(msg);
                        db.SaveChanges();
                    }

                    // отправляем соо всем клиентам, подключенным к чату
                    client.SendMsg(msgTextBox.Text, onlineUser.Id);
                    msgTextBox.Text = String.Empty;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Произошла ошибка при попытке отправить ваше сообщение.\n" +
                        "Текст ошибки: " + ex.Message);
                }                
            }
        }
    }
}
