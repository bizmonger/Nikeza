using System.Windows;
using Nikeza.Mobile.UILogic;

using AppNavigation = Nikeza.Mobile.AppLogic.Navigation;

namespace Desktop.App
{
    public partial class Shell : Window
    {
        ViewModels _viewmodels;

        public Shell()
        {
            InitializeComponent();

            _viewmodels = InitViewmodels();

            new AppNavigation(_viewmodels).Requested += (sender, pageRequested) =>
                {
                    AppFrame.Load(pageRequested, _viewmodels);
                };

            AppFrame.Navigate(new RegistrationPage());
        }

        public ViewModels ViewModels() => _viewmodels;
    }
}