using System.Windows.Controls;
using static Nikeza.Mobile.UILogic.Pages;

namespace Desktop.App
{
    public static partial class Navigation
    {
        public static void Load(this Frame appFrame, PageRequested requested)
        {
            if (requested.IsPortal)
                ToPortal(appFrame, requested);
        }
    }
}