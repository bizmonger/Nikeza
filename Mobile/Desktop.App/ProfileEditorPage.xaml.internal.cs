using Nikeza.Mobile.UILogic.Portal.ProfileEditor;
using System.Windows.Controls;
using System.Windows.Media;

namespace Desktop.App
{
    partial class ProfileEditorPage
    {
        ViewModel _viewmodel;

        void InputResponse(TextBox textbox, string placeholder)
        {
            _viewmodel = DataContext as ViewModel;
            _viewmodel.Validate.Execute(null);

            if (textbox.Text == "" && !textbox.IsFocused)
                textbox.Text = placeholder;

            if (textbox.Text != placeholder)
                 textbox.Foreground = new SolidColorBrush(Colors.Black);

            else textbox.Foreground = new SolidColorBrush(Colors.LightGray);
        }
    }
}