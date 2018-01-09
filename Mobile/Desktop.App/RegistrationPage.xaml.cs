using System.Windows.Controls;
using Nikeza.Mobile.UILogic.Registration;

namespace Desktop.App
{
    public partial class RegistrationPage : Page
    {
        ViewModel _viewmodel;

        public RegistrationPage()
        {
            InitializeComponent();
            
            _viewmodel = new ViewModel(FunctionFactory.SubmitRegistration());

            Email.GotFocus    += (s, e) => { Email.FocusResonse   (_viewmodel.EmailPlaceholder,    _viewmodel, DataContext); };
            Password.GotFocus += (s, e) => { Password.FocusResonse(_viewmodel.PasswordPlaceholder, _viewmodel, DataContext); };
            Confirm.GotFocus  += (s, e) => { Confirm.FocusResonse (_viewmodel.ConfirmPlaceholder,  _viewmodel, DataContext); };

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