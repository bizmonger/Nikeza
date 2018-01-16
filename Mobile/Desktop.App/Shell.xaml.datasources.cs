using Nikeza.Mobile.Portal.DataSources;
using System.Windows.Controls;
using static Nikeza.Common;

namespace Desktop.App
{
    public partial class Shell
    {
        static void ToDataSources(Frame AppFrame, ProfileRequest profile)
        {
            var page =      new DataSourcesPage();
            var viewmodel = new ViewModel(FunctionFactory.GetPlatforms());

            page.Init(viewmodel);
            AppFrame.Navigate(page);
        }
    }
}