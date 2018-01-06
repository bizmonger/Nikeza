using Nikeza.Mobile.UILogic;
using System.Windows.Controls;

using ProfileEditor = Nikeza.Mobile.UILogic.Portal.ProfileEditor.ViewModel;
using static Desktop.App.FunctionFactory;

namespace Desktop.App
{
    public static partial class Navigation
    {
        static void ToPortal(Frame appFrame, Pages.PageRequested pageRequested, ViewModels viewmodels)
        {
            var profile = pageRequested.TryProfile().Value;
            var profileEditor = new ProfileEditor(profile, SaveProfile());

            viewmodels.ProfileEditor = profileEditor;

            appFrame.Navigate(new PortalPage());
        }
    }
}
