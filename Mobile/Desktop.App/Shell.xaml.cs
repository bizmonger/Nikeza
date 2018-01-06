using System.Windows;
using Nikeza.Mobile.AppLogic;
using Nikeza.Mobile.UILogic;
using Registration = Nikeza.Mobile.UILogic.Registration.ViewModel;
using ProfileEditor = Nikeza.Mobile.UILogic.Portal.ProfileEditor.ViewModel;
using static Desktop.App.FunctionFactory;

namespace Desktop.App
{
    public partial class Shell : Window
    {
        ViewModels _viewmodels;

        public Shell()
        {
            InitializeComponent();

            var registration =         new Registration(SubmitRegistration());
            var profileEditor_nulled = new ProfileEditor(null, null);
            _viewmodels = new ViewModels(registration, profileEditor_nulled);

            var navigation = new Navigation(_viewmodels);
            navigation.Requested += (s, e) => 
                {
                    if (e.IsPortal) HandleProfileEditor(e);
                };

            AppFrame.Navigate(new RegistrationPage());
        }

        void HandleProfileEditor(Pages.PageRequested pageRequested)
        {
            AppFrame.Navigate(new PortalPage());

            var profile =       pageRequested.TryProfile().Value;
            var profileEditor = new ProfileEditor(profile, SaveProfile());

            _viewmodels.ProfileEditor = profileEditor;
        }

        public ViewModels ViewModels() => _viewmodels;
    }
}