using System.Windows;
using Microsoft.FSharp.Core;
using Nikeza.Mobile.AppLogic;
using Nikeza.Mobile.UILogic;
using static Nikeza.Common;
using static Nikeza.Mobile.Profile.Registration;
using static Nikeza.Mobile.AppLogic.TestAPI;

namespace Desktop.App
{
    public partial class Shell : Window
    {
        ViewModels _viewmodels;

        public Shell()
        {
            InitializeComponent();

            var cs_submit = FSharpFunc<ValidatedForm, FSharpResult<ProfileRequest, ValidatedForm>>.FromConverter(mockSubmit);
            var registration = new Nikeza.Mobile.UILogic.Registration.ViewModel(cs_submit);
            _viewmodels = new ViewModels(registration);

            var navigation = new Navigation(_viewmodels);
            navigation.Requested += (s, e) => { if (e.IsPortal) AppFrame.Navigate(new PortalPage()); };

            AppFrame.Navigate(new RegistrationPage());
        }

        public ViewModels ViewModels() => _viewmodels;
    }
}