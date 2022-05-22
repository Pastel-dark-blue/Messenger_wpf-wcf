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
    /// Логика взаимодействия для LoginPage.xaml
    /// </summary>
    public partial class LoginPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        Authorization mainWnd { get => Application.Current.MainWindow as Authorization; }
        private void Register_MouseDown(object sender, MouseButtonEventArgs e)
        {
            mainWnd.authorizationWindFrame.Navigate(
                new Uri("pack://application:,,,/View/Pages/RegisterPage.xaml"),
                UriKind.RelativeOrAbsolute);
        }

        private void EnterBtn_Click(object sender, RoutedEventArgs e)
        {
            // получаем из полей информацию, введенную пользователем
            string login = LoginField.Text;
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

            // если валидация пройдена, подключаемся к БД
            if (IsValid)
            {
                using (ChatDBModel db = new ChatDBModel())
                {
                    var chatUser = db.ChatUser.FirstOrDefault(user =>
                        user.Login == login &&
                        user.Password == password);

                    if (chatUser != null)
                    {
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
                    else
                    {
                        MessageBox.Show("Пользователь не найден");
                    }
                }
            }
        }

        // подсказка для поля с не валидными данными
        private ToolTip CreateToolTip(string text)
        {
            // всплывающая подсказка для отображения текста ошибки
            ToolTip tooltip = new ToolTip();

            //tooltip.Background = new SolidColorBrush(Color.FromRgb(71, 63, 74)) { Opacity = 0.7 };
            //tooltip.Foreground = new SolidColorBrush(Color.FromRgb(175, 215, 224));
            //tooltip.FontSize = 16;
            //tooltip.BorderThickness = new Thickness(0, 0, 0, 0);
            //tooltip.Width = 400;

            ControlTemplate template = Application.Current.FindResource("errorToolTipStyle") as ControlTemplate;
            tooltip.Template = template;
            tooltip.Content = text;

            return tooltip;
        }

    }
}
