using System.Windows.Controls;
using Nikeza.Mobile.UILogic.Portal.Recent;
using static Desktop.App.FunctionFactory;
using static Nikeza.Common;

namespace Desktop.App
{
    public partial class Shell
    {
        static void ToRecent(Frame AppFrame, ProfileRequest profile)
        {
            var page = new RecentPage();
            var viewmodel = new ViewModel(profile.Id , GetRecent(), GetPortfolio());

            page.Init(viewmodel);
            AppFrame.Navigate(page);
        }
    }
}