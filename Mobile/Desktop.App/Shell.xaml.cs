using System.Windows;
using Nikeza.Mobile.UILogic;

namespace Desktop.App
{
    public partial class Shell : Window
    {
        ViewModels _viewmodels;

        public Shell()
        {
            InitializeComponent();

            _viewmodels = InitViewmodels();

            new Nikeza.Mobile.AppLogic.Navigation(_viewmodels).Requested += (s, pageRequested) =>
                {
                    AppFrame.Load(pageRequested, _viewmodels);
                };

            AppFrame.Navigate(new RegistrationPage());
        }

        public ViewModels ViewModels() => _viewmodels;
    }
}