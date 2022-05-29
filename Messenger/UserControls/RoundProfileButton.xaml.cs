using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Messenger.UserControls
{
    /// <summary>
    /// Логика взаимодействия для RoundProfileButton.xaml
    /// </summary>
    public partial class RoundProfileButton : UserControl
    {
        public RoundProfileButton()
        {
            InitializeComponent();
        }

        public ImageSource ProfileImageSource
        {
            get { return (ImageSource)GetValue(ProfileImageSourceProperty); }
            set { SetValue(ProfileImageSourceProperty, value); }
        }

        public static readonly DependencyProperty ProfileImageSourceProperty =
            DependencyProperty.Register("ProfileImageSource", typeof(ImageSource), typeof(RoundProfileButton));


        public bool IsOnline
        {
            get { return (bool)GetValue(IsOnlineProperty); }
            set { SetValue(IsOnlineProperty, value); }
        }

        public static readonly DependencyProperty IsOnlineProperty =
            DependencyProperty.Register("IsOnline", typeof(bool), typeof(RoundProfileButton));
    }
}
