using System.Windows.Controls;
using static Desktop.App.FunctionFactory;
using static Nikeza.Mobile.Profile.Events;
using RegistrationViewModel =  Nikeza.Mobile.UILogic.Registration.ViewModel;
using ProfileEditorViewmodel = Nikeza.Mobile.UILogic.Portal.ProfileEditor.ViewModel;
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

        void FromRegistration(RegistrationSubmissionEvent theEvent)
        {
            if      (theEvent.IsRegistrationSucceeded)
                        To.Portal(AppFrame, theEvent);

            else if (theEvent.IsRegistrationFailed)
                        To.Error(AppFrame, theEvent);
        }

        static void FromPortalEditor(Frame AppFrame, ProfileEditorEvent theEvent)
        {
            if      (theEvent.IsProfileSaved)
                        To.DataSources(AppFrame, theEvent.TryGetProfile().Value);

            else if (theEvent.IsProfileSaveFailed)
                        To.Error(AppFrame, theEvent);
        }

        private class To
        {
            internal static void Portal(Frame AppFrame, RegistrationSubmissionEvent theEvent)
            {
                var portalPage = new PortalPage();
                var viewmodel =  new ProfileEditorViewmodel(theEvent.TryGetProfile().Value, SaveProfile());

                viewmodel.EventOccurred += (s, e) => FromPortalEditor(AppFrame, e);
                portalPage.DataContext =   viewmodel;

                AppFrame.Navigate(portalPage);
            }
            
            internal static void Error(Frame AppFrame, RegistrationSubmissionEvent theEvent) =>
                AppFrame.Navigate(new ErrorPage());

            internal static void Error(Frame AppFrame, ProfileEditorEvent theEvent) =>
                AppFrame.Navigate(new ErrorPage());

            internal static void DataSources(Frame AppFrame, ProfileRequest profile) =>
                AppFrame.Navigate(new DataSourcesPage());
        }
    }
}