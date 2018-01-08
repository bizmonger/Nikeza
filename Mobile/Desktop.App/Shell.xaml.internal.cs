using System.Windows.Controls;
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

        void FromRegistration(RegistrationSubmissionEvent disEvent)
        {
            if      (disEvent.IsRegistrationSucceeded)
                        To.Portal(AppFrame, disEvent);

            else if (disEvent.IsRegistrationFailed)
                        To.Error(AppFrame, disEvent);
        }

        static void FromPortalEditor(Frame AppFrame, ProfileEvent disEvent)
        {
            if      (disEvent.IsProfileSaved)
                        To.DataSources(AppFrame, disEvent.TryGetProfile().Value);

            else if (disEvent.IsProfileSaveFailed)
                        To.Error(AppFrame, disEvent);
        }

        private class To
        {
            internal static void Portal(Frame AppFrame, RegistrationSubmissionEvent disEvent)
            {
                var portalPage = new PortalPage();
                var viewmodel =  new ProfileEditorViewmodel(disEvent.TryGetProfile().Value, SaveProfile());

                viewmodel.EventOccurred += (s, e) => FromPortalEditor(AppFrame, e);
                portalPage.DataContext =   viewmodel;

                AppFrame.Navigate(portalPage);
            }
            
            internal static void Error(Frame AppFrame, RegistrationSubmissionEvent disEvent) =>
                AppFrame.Navigate(new ErrorPage());

            internal static void Error(Frame AppFrame, ProfileEvent disEvent) =>
                AppFrame.Navigate(new ErrorPage());

            internal static void DataSources(Frame AppFrame, ProfileRequest profile) =>
                AppFrame.Navigate(new DataSourcesPage());
        }
    }
}