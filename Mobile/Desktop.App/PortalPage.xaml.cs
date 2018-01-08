using System.Windows.Controls;

namespace Desktop.App
{
    public partial class PortalPage : Page
    {
        public PortalPage()
        {
            InitializeComponent();
            
            FirstName.GotFocus    += (s, e) => { FocusResonse(FirstName, _viewmodel.FirstNameDefault); };
            FirstName.TextChanged += (s, e) => { InputResponse(); };

            LastName.GotFocus     += (s, e) => { FocusResonse(LastName,  _viewmodel.LastNameDefault); };
            LastName.TextChanged  += (s, e) => { InputResponse(); };
                                  
            Email.TextChanged     += (s, e) => { InputResponse(); };
        }
    }
}