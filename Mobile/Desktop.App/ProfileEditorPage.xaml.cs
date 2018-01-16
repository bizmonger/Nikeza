using System.Windows.Controls;
using System.Windows.Media;
using Nikeza.Mobile.UILogic.Portal.ProfileEditor;

namespace Desktop.App
{
    public partial class ProfileEditorPage : Page
    {
        public ProfileEditorPage()
        {
            InitializeComponent();

            FirstName.Foreground = new SolidColorBrush(Colors.LightGray);
            FirstName.GotFocus    += (s, e) => { FirstName.FocusResonse(  _viewmodel, _viewmodel.FirstNamePlaceholder ); };
            FirstName.TextChanged += (s, e) => { InputResponse(FirstName, _viewmodel.FirstNamePlaceholder); };

            LastName.Foreground = new SolidColorBrush(Colors.LightGray);
            LastName.GotFocus     += (s, e) => { LastName.FocusResonse ( _viewmodel, _viewmodel.LastNamePlaceholder ); };
            LastName.TextChanged  += (s, e) => { InputResponse(LastName, _viewmodel.LastNamePlaceholder); };
                                  
            Email.TextChanged     += (s, e) => { InputResponse(Email, ""); };
        }

        public void Init(ViewModel viewmodel)
        {
            DataContext = _viewmodel = viewmodel;
            viewmodel.Init();
        }
    }
}