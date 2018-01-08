using Nikeza.Mobile.UILogic.Portal.ProfileEditor;
using System.Windows.Controls;

namespace Desktop.App
{
    partial class PortalPage
    {
        ViewModel _viewmodel;

        void InputResponse()
        {
            _viewmodel = DataContext as ViewModel;
            _viewmodel.Validate.Execute(null);
        }

        void FocusResonse(TextBox textbox, string compareValue)
        {
            _viewmodel = DataContext as ViewModel;

            textbox.SelectAll();

            if (textbox.Text == compareValue)
                textbox.Text = "";
        }
    }
}