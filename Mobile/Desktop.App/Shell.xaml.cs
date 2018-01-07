using System.Windows;

using AppNavigation = Nikeza.Mobile.AppLogic.Navigation;

namespace Desktop.App
{
    public partial class Shell : Window
    {
        public Shell()
        {
            InitializeComponent();
            
            new AppNavigation().Requested += (sender, requested) => AppFrame.Load(requested);

            AppFrame.Navigate(new RegistrationPage());
        }
    }
}