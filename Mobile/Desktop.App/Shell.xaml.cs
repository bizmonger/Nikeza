using System.Windows;

namespace Desktop.App
{
    public partial class Shell : Window
    {
        public Shell()
        {
            InitializeComponent();

            var registrationPage = new RegistrationPage();
            var registrationViewmodel = registrationPage.DataContext as Nikeza.Mobile.UILogic.Registration.ViewModel;

            registrationViewmodel.PageRequested += navigateFromRegistration;
            
            AppFrame.Navigate(registrationPage);
        }
    }
}