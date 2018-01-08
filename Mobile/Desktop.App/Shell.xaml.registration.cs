using System.Windows.Controls;
using static Nikeza.Mobile.Profile.Events;
using RegistrationViewModel = Nikeza.Mobile.UILogic.Registration.ViewModel;

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
                        ToProfileEditor(AppFrame, theEvent);

            else if (theEvent.IsRegistrationFailed)
                        ToError(AppFrame, theEvent);
        }
        
        static void ToError(Frame AppFrame, RegistrationSubmissionEvent theEvent) =>
            AppFrame.Navigate(new ErrorPage());
    }
}