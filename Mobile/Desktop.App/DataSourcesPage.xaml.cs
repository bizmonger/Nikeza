using Nikeza.Mobile.Portal.DataSources;
using System.Windows.Controls;

namespace Desktop.App
{
    public partial class DataSourcesPage : Page
    {
        ViewModel _viewmodel;

        public DataSourcesPage() => InitializeComponent();

        public void Init(ViewModel viewmodel)
        {
            DataContext = _viewmodel = viewmodel;
            _viewmodel.Init();
        }
    }
}