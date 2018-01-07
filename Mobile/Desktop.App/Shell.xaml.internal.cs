using Nikeza.Mobile.Profile;
using static Desktop.App.FunctionFactory;
using ProfileEditorViewmodel = Nikeza.Mobile.UILogic.Portal.ProfileEditor.ViewModel;

namespace Desktop.App
{
    public partial class Shell
    {
        void navigateFromRegistration(object sender, Events.RegistrationSubmissionEvent args)
        {
            var portalPage = new PortalPage();
            portalPage.DataContext = new ProfileEditorViewmodel(args.TryGetProfile().Value, SaveProfile());
            AppFrame.Navigate(portalPage);
        }
    }
}