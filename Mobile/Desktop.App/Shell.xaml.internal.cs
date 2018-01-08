using System.Windows.Controls;
using Nikeza.Mobile.Profile;
using Nikeza.Mobile.Profile.EventExtraction;
using static Desktop.App.FunctionFactory;
using RegistrationViewModel = Nikeza.Mobile.UILogic.Registration.ViewModel;
using ProfileEditorViewmodel = Nikeza.Mobile.UILogic.Portal.ProfileEditor.ViewModel;
using static Nikeza.Mobile.Profile.Events;
using static Nikeza.Common;

namespace Desktop.App
{
    public partial class Shell
    {
        RegistrationPage InitRegistration()
        {
            var registrationPage = new RegistrationPage();
            var registration =     registrationPage.DataContext as RegistrationViewModel;

            registration.EventOccurred += (s,e) => FromRegistration(e);
            return registrationPage;
        }

        void FromRegistration(RegistrationSubmissionEvent args)
        {
            if      (args.IsRegistrationSucceeded)
                        To.Portal(AppFrame, args);

            else if (args.IsRegistrationFailed)
                        To.RegistrationError(AppFrame, args);
        }

        static void FromPortalEditor(Frame AppFrame, ProfileEvent args)
        {
            if (args.IsProfileSaved)
                To.DataSources(AppFrame, args.TryGetProfile().Value);
        }

        private class To
        {
            internal static void Portal(Frame AppFrame, RegistrationSubmissionEvent args)
            {
                var portalPage = new PortalPage();
                var viewmodel =  new ProfileEditorViewmodel(args.TryGetProfile().Value, SaveProfile());

                viewmodel.EventOccurred += (s, e) => FromPortalEditor(AppFrame, e);
                portalPage.DataContext =   viewmodel;

                AppFrame.Navigate(portalPage);
            }
            
            internal static void RegistrationError(Frame AppFrame, RegistrationSubmissionEvent args) =>
                AppFrame.Navigate(new RegistrationErrorPage());

            internal static void DataSources(Frame AppFrame, ProfileRequest profile) =>
                AppFrame.Navigate(new DataSourcesPage());
        }
    }
}