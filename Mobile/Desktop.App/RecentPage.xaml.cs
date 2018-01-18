using System.Windows.Controls;
using Nikeza.Mobile.UILogic.Portal.Recent;

namespace Desktop.App
{
    public partial class RecentPage : Page
    {
        ViewModel _viewmodel;

        public RecentPage() => InitializeComponent();

        public void Init(ViewModel viewmodel)
        {
            DataContext = _viewmodel = viewmodel;
            _viewmodel.Init();
        }
    }
}