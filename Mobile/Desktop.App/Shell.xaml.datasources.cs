using System.Windows.Controls;
using static Nikeza.Common;

namespace Desktop.App
{
    public partial class Shell
    {
        static void ToDataSources(Frame AppFrame, ProfileRequest profile) =>
            AppFrame.Navigate(new DataSourcesPage());
    }
}