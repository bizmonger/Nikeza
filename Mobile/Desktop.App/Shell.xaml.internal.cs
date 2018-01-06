using Registration = Nikeza.Mobile.UILogic.Registration.ViewModel;
using ProfileEditor = Nikeza.Mobile.UILogic.Portal.ProfileEditor.ViewModel;
using static Desktop.App.FunctionFactory;
using Nikeza.Mobile.UILogic;

namespace Desktop.App
{
    public partial class Shell
    {
        ViewModels InitViewmodels()
        {
            var registration = new Registration(SubmitRegistration());
            var profileEditor_nulled = new ProfileEditor(null, null);

            return new ViewModels(registration, profileEditor_nulled);
        }
    }
}
