using System.Windows;
using static Nikeza.Mobile.UILogic.TestAPI;

namespace Desktop.App
{
    public partial class Shell : Window
    {
        public Shell()
        {
            InitializeComponent();

            //var registrationPage = InitRegistration();
            //AppFrame.Navigate(registrationPage);

            ToRecent(AppFrame, someProfile);
        }
    }
}