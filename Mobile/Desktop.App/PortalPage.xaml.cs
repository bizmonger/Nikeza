using Nikeza.Mobile.UILogic.Portal.ProfileEditor;
using System.Windows.Controls;

namespace Desktop.App
{
    public partial class PortalPage : Page
    {
        ViewModel _viewmodel;

        public PortalPage()
        {
            InitializeComponent();
            
            FirstName.GotFocus +=    (s, e) => {
                _viewmodel = DataContext as ViewModel;
                FirstName.Text = ""; };

            FirstName.TextChanged += (s, e) => {
                _viewmodel = DataContext as ViewModel;
                _viewmodel.Validate.Execute(null); };

            LastName.GotFocus  +=    (s, e) => {
                _viewmodel = DataContext as ViewModel;
                LastName.Text =  ""; };

            LastName.TextChanged +=  (s, e) => {
                _viewmodel = DataContext as ViewModel;
                _viewmodel.Validate.Execute(null); };
                                     
            Email.TextChanged +=     (s, e) => {
                _viewmodel = DataContext as ViewModel;
                _viewmodel.Validate.Execute(null); };
        }
    }
}
