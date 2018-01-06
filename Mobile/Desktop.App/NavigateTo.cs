using Nikeza.Mobile.UILogic;
using ProfileEditor = Nikeza.Mobile.UILogic.Portal.ProfileEditor.ViewModel;
using static Desktop.App.FunctionFactory;
using System.Windows.Controls;

namespace Desktop.App
{
    public static class Navigation
    {
        public static void Load(this Frame appFrame, Pages.PageRequested pageRequested, ViewModels viewmodels)
        {
            var profile = pageRequested.TryProfile().Value;
            var profileEditor = new ProfileEditor(profile, SaveProfile());

            viewmodels.ProfileEditor = profileEditor;

            appFrame.Navigate(new PortalPage());
        }
    }
}
