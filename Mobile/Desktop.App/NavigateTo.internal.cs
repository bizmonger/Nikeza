using Nikeza.Mobile.UILogic;
using System.Windows.Controls;

using ProfileEditor = Nikeza.Mobile.UILogic.Portal.ProfileEditor.ViewModel;
using static Desktop.App.FunctionFactory;

namespace Desktop.App
{
    public static partial class Navigation
    {
        static void ToPortal(Frame appFrame, Pages.PageRequested pageRequested)
        {
            var page = new PortalPage();
            page.DataContext = new ProfileEditor(pageRequested.TryProfile().Value, SaveProfile());
            appFrame.Navigate(page);
        }
    }
}