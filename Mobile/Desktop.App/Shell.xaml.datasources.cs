using Nikeza.Mobile.Portal.DataSources;
using System.Windows.Controls;
using static Nikeza.Common;
using static Desktop.App.FunctionFactory;
using static Nikeza.Mobile.Profile.Events;
using static Nikeza.Mobile.Profile.EventExtentions.SourcesSaveEventExtension;

namespace Desktop.App
{
    public partial class Shell
    {
        static void FromDataSources(Frame AppFrame, SourcesSaveEvent theEvent)
        {
            if (theEvent.IsSourcesSaved)
                ToRecent(AppFrame, theEvent.TryGetProfile().Value);

            else if (theEvent.IsSourcesFailed)
                ToError(AppFrame, theEvent);
        }

        static void ToError(Frame appFrame, SourcesSaveEvent theEvent) =>
            appFrame.Navigate(new ErrorPage());

        static void ToDataSources(Frame AppFrame, ProfileRequest profile)
        {
            var page =      new DataSourcesPage();
            var viewmodel = new ViewModel(profile, GetPlatforms(), SaveSources());

            viewmodel.SaveRequest += (s, e) => FromDataSources(AppFrame, e);

            page.Init(viewmodel);
            AppFrame.Navigate(page);
        }
    }
}