using Nikeza.Mobile.Profile;
using static Desktop.App.FunctionFactory;
using RegistrationViewModel = Nikeza.Mobile.UILogic.Registration.ViewModel;
using ProfileEditorViewmodel = Nikeza.Mobile.UILogic.Portal.ProfileEditor.ViewModel;
using System.Windows.Controls;

namespace Desktop.App
{
    public partial class Shell
    {
        RegistrationPage InitRegistration()
        {
            var registrationPage = new RegistrationPage();
            var registration =     registrationPage.DataContext as RegistrationViewModel;

            registration.EventOccurred += navigateFromRegistration;
            return registrationPage;
        }

        void navigateFromRegistration(object sender, Events.RegistrationSubmissionEvent args)
        {
            if  (args.IsRegistrationSucceeded)
                To.Portal(AppFrame, args);

            else if (args.IsRegistrationFailed)
                To.RegistrationError(AppFrame, args);
        }

        public class To
        {
            public static void Portal(Frame AppFrame, Events.RegistrationSubmissionEvent args)
            {
                var portalPage = new PortalPage();
                portalPage.DataContext = new ProfileEditorViewmodel(args.TryGetProfile().Value, SaveProfile());
                AppFrame.Navigate(portalPage);
            }

            public static void RegistrationError(Frame AppFrame, Events.RegistrationSubmissionEvent args) =>
                AppFrame.Navigate(new RegistrationErrorPage());
        }
    }
}