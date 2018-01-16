using System.Windows.Controls;
using static Nikeza.Mobile.Profile.EventExtentions.ProfileEditorEventExtension;
using static Nikeza.Mobile.Profile.EventExtentions.RegistrationSubmissionEventExtension;
using static Nikeza.Mobile.Profile.Events;
using static Desktop.App.FunctionFactory;
using ProfileEditorViewmodel = Nikeza.Mobile.UILogic.Portal.ProfileEditor.ViewModel;
using Nikeza.Mobile.UILogic.Portal.ProfileEditor;

namespace Desktop.App
{
    public partial class Shell
    {
        static void FromProfileEditor(Frame AppFrame, ProfileSaveEvent theEvent)
        {
            if (theEvent.IsProfileSaved)
                ToDataSources(AppFrame, theEvent.TryGetProfile().Value);

            else if (theEvent.IsProfileSaveFailed)
                ToError(AppFrame, theEvent);
        }

        static void ToProfileEditor(Frame AppFrame, RegistrationSubmissionEvent theEvent)
        {
            var profileEditorPage = new ProfileEditorPage();
            var inject =            new Dependencies(theEvent.TryGetProfile().Value, SaveProfile(), GetTopics());
            var viewmodel =         new ProfileEditorViewmodel(inject);

            viewmodel.SaveEvent += (s, e) => FromProfileEditor(AppFrame, e);
            profileEditorPage.Init(viewmodel);

            viewmodel.Init();

            AppFrame.Navigate(profileEditorPage);
        }

        static void ToError(Frame AppFrame, ProfileSaveEvent theEvent) =>
            AppFrame.Navigate(new ErrorPage());
    }
}