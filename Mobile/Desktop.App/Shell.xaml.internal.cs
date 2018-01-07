using Nikeza.Mobile.Profile;
using static Desktop.App.FunctionFactory;
using RegistrationViewModel = Nikeza.Mobile.UILogic.Registration.ViewModel;
using ProfileEditorViewmodel = Nikeza.Mobile.UILogic.Portal.ProfileEditor.ViewModel;

namespace Desktop.App
{
    public partial class Shell
    {
        RegistrationPage InitRegistration()
        {
            var registrationPage = new RegistrationPage();
            var registration = registrationPage.DataContext as RegistrationViewModel;

            registration.EventOccurred += navigateFromRegistration;
            return registrationPage;
        }

        void navigateFromRegistration(object sender, Events.RegistrationSubmissionEvent args)
        {
            var portalPage = new PortalPage();
            portalPage.DataContext = new ProfileEditorViewmodel(args.TryGetProfile().Value, SaveProfile());
            AppFrame.Navigate(portalPage);
        }
    }
}