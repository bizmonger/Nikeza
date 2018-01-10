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
            FirstName.GotFocus    += (s, e) => { FirstName.FocusResonse(_viewmodel, _viewmodel.FirstNamePlaceholder, DataContext); };
            FirstName.TextChanged += (s, e) => { InputResponse(FirstName, _viewmodel != null ? _viewmodel.FirstNamePlaceholder : "first name"); };

            LastName.Foreground = new SolidColorBrush(Colors.LightGray);
            LastName.GotFocus     += (s, e) => { LastName.FocusResonse (_viewmodel, _viewmodel.LastNamePlaceholder,  DataContext); };
            LastName.TextChanged  += (s, e) => { InputResponse(LastName, _viewmodel != null ? _viewmodel.LastNamePlaceholder : "last name"); };
                                  
            Email.TextChanged     += (s, e) => { InputResponse(Email, ""); };
        }
    }
}