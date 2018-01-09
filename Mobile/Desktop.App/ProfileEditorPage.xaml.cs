using System.Windows.Controls;

namespace Desktop.App
{
    public partial class ProfileEditorPage : Page
    {
        public ProfileEditorPage()
        {
            InitializeComponent();
            
            FirstName.GotFocus    += (s, e) => { FirstName.FocusResonse(_viewmodel, _viewmodel.FirstNamePlaceholder, DataContext); };
            FirstName.TextChanged += (s, e) => { InputResponse(); };

            LastName.GotFocus     += (s, e) => { LastName.FocusResonse (_viewmodel, _viewmodel.LastNamePlaceholder,  DataContext); };
            LastName.TextChanged  += (s, e) => { InputResponse(); };
                                  
            Email.TextChanged     += (s, e) => { InputResponse(); };
        }
    }
}