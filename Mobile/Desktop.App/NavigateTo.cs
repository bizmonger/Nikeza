using Nikeza.Mobile.UILogic;
using System.Windows.Controls;

namespace Desktop.App
{
    public static partial class Navigation
    {
        public static void Load(this Frame appFrame, Pages.PageRequested requested, ViewModels viewmodels)
        {
            if (requested.IsPortal)
                ToPortal(appFrame, requested, viewmodels);
        }
    }
}
