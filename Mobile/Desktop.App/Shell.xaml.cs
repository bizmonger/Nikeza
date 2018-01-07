using System.Windows;

namespace Desktop.App
{
    public partial class Shell : Window
    {
        public Shell()
        {
            InitializeComponent();

            var registrationPage = InitRegistration();
            AppFrame.Navigate(registrationPage);
        }
    }
}