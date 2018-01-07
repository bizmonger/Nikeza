using Nikeza.Mobile.Profile;
using static Desktop.App.FunctionFactory;
using ProfileEditorViewmodel = Nikeza.Mobile.UILogic.Portal.ProfileEditor.ViewModel;

namespace Desktop.App
{
    public partial class Shell
    {
        RegistrationPage InitRegistration()
        {
            var registrationPage = new RegistrationPage();
            var registrationViewmodel = registrationPage.DataContext as Nikeza.Mobile.UILogic.Registration.ViewModel;

            registrationViewmodel.PageRequested += navigateFromRegistration;
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