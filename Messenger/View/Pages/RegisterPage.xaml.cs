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
using Messenger.ChatDB;
using Messenger.View.Windows;

namespace Messenger.View.Pages
{
    /// <summary>
    /// Логика взаимодействия для RegisterPage.xaml
    /// </summary>
    public partial class RegisterPage : Page
    {
        public RegisterPage()
        {
            InitializeComponent();
        }

        Authorization mainWnd { get => Application.Current.MainWindow as Authorization; }

        private void Login_MouseDown(object sender, MouseButtonEventArgs e)
        {
            mainWnd.authorizationWindFrame.Navigate(
               new Uri("pack://application:,,,/View/Pages/LoginPage.xaml"),
               UriKind.RelativeOrAbsolute);
        }

        private void RegisterBtn_Click(object sender, RoutedEventArgs e)
        {

            // получаем из полей информацию, введенную пользователем
            string login = LoginField.Text;
            string email = EmailField.Text;
            string password = PasswordField.passwrdBox.Password;

            #region Валидация
            bool IsValid = true;    // переменная равна true только, когда все поля валидны


            // валидация логина
            string loginError = ChatUser.LoginValidation(login);
            // если есть ошибка, то есть метод LoginValidation не вернул null
            if (loginError != null)
            {
                LoginField.placeholderBorderUC.Background = new SolidColorBrush(Colors.Red) { Opacity = 0.1 };
                LoginField.ToolTip = CreateToolTip(loginError);
                IsValid = false;
            }
            // если валидно
            else
            {
                LoginField.placeholderBorderUC.Background = Brushes.Transparent;
                LoginField.ToolTip = null;
            }

            // валидация email
            string emailError = ChatUser.EmailValidation(email);
            if (emailError != null)
            {
                EmailField.placeholderBorderUC.Background = new SolidColorBrush(Colors.Red) { Opacity = 0.1 };
                EmailField.ToolTip = CreateToolTip(emailError);
                IsValid = false;
            }
            else
            {
                EmailField.placeholderBorderUC.Background = Brushes.Transparent;
                EmailField.ToolTip = null;
            }

            // валидация пароля
            string passwordError = ChatUser.PasswordValidation(password);
            if (passwordError != null)
            {
                PasswordField.placeholderBorderUC.Background = new SolidColorBrush(Colors.Red) { Opacity = 0.1 };
                PasswordField.ToolTip = CreateToolTip(passwordError);
                IsValid = false;
            }
            else
            {
                PasswordField.placeholderBorderUC.Background = Brushes.Transparent;
                PasswordField.ToolTip = null;
            }
            #endregion

            if (IsValid)
            {
                ChatUser chatUser = new ChatUser();
                try
                {
                    using (ChatDBModel db = new ChatDBModel())
                    {
                        chatUser.Login = login;
                        chatUser.Email = email;
                        chatUser.Password = password;

                        db.ChatUser.Add(chatUser);
                        db.SaveChanges();

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Произошла ошибка при попытке зарегистрировать пользователя.\n" +
                        "Текст ошибки: " + ex.Message);

                    // закрываем окно авторизации
                    mainWnd.Close();

                    return;
                }

                try
                {
                    // открываем окно чата и передаем ему Id вошедшего пользователя
                    ChatWindow chat = new ChatWindow(chatUser);
                    chat.Show();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Произошла ошибка при попытке открыть окно чата.\n" +
                        "Текст ошибки: " + ex.Message);
                }
                finally
                {
                    // закрываем окно авторизации
                    mainWnd.Close();
                }

            }
        }

        // подсказка для поля с не валидными данными
        private ToolTip CreateToolTip(string text)
        {
            // всплывающая подсказка для отображения текста ошибки
            ToolTip tooltip = new ToolTip();

            Style style = Application.Current.FindResource("errorToolTipStyle") as Style;
            tooltip.Style = style;
            tooltip.Content = text;

            return tooltip;
        }

    }
}

