using System.Windows.Controls;
using System.Windows.Media;

namespace Desktop.App
{
    public partial class ProfileEditorPage : Page
    {
        public ProfileEditorPage()
        {
            InitializeComponent();

            FirstName.Foreground = new SolidColorBrush(Colors.LightGray);
            FirstName.GotFocus    += (s, e) => { FirstName.FocusResonse(_viewmodel, _viewmodel.FirstNamePlaceholder); };
            FirstName.TextChanged += (s, e) => { InputResponse(FirstName, _viewmodel != null ? _viewmodel.FirstNamePlaceholder : _viewmodel.FirstNamePlaceholder); };

            LastName.Foreground = new SolidColorBrush(Colors.LightGray);
            LastName.GotFocus     += (s, e) => { LastName.FocusResonse (_viewmodel, _viewmodel.LastNamePlaceholder); };
            LastName.TextChanged  += (s, e) => { InputResponse(LastName, _viewmodel != null ? _viewmodel.LastNamePlaceholder : _viewmodel.LastNamePlaceholder); };
                                  
            Email.TextChanged     += (s, e) => { InputResponse(Email, ""); };
        }
    }
}