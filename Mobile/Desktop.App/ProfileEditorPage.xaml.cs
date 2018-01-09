using System.Windows.Controls;

namespace Desktop.App
{
    public partial class ProfileEditorPage : Page
    {
        public ProfileEditorPage()
        {
            InitializeComponent();
            
            FirstName.GotFocus    += (s, e) => { FirstName.FocusResonse(_viewmodel.FirstNamePlaceholder, _viewmodel, DataContext); };
            FirstName.TextChanged += (s, e) => { InputResponse(); };

            LastName.GotFocus     += (s, e) => { LastName.FocusResonse(_viewmodel.LastNamePlaceholder,   _viewmodel, DataContext); };
            LastName.TextChanged  += (s, e) => { InputResponse(); };
                                  
            Email.TextChanged     += (s, e) => { InputResponse(); };
        }
    }
}