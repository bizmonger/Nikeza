using System.Windows;
using Microsoft.FSharp.Core;
using Nikeza.Mobile.Profile;
using Nikeza.Mobile.UILogic.Registration;
using static Nikeza.Common;
using static Nikeza.Mobile.Profile.Registration;

namespace Desktop.App
{
    public partial class MainWindow : Window
    {
        ViewModel _viewmodel;

        public MainWindow()
        {
            InitializeComponent();

            var cs_submit = FSharpFunc<ValidatedForm, FSharpResult<ProfileRequest,ValidatedForm>>.FromConverter(Try.submit);
            _viewmodel = new ViewModel(cs_submit);

            Password.PasswordChanged += (s, e) =>
                {
                    _viewmodel.Password = Password.Password;
                    _viewmodel.Validate.Execute(null);
                };

            Confirm.PasswordChanged += (s, e) =>
                {
                    _viewmodel.Confirm = Confirm.Password;
                    _viewmodel.Validate.Execute(null);
                };

            DataContext = _viewmodel;
        }
    }
}
