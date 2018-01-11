using System.Windows.Controls;
using System.Windows.Media;

namespace Desktop.App
{
    static class ControlLogic
    {
        internal static void FocusResonse<ViewModel>(this TextBox textbox, ViewModel viewmodel, string placeholder) where ViewModel : class
        {
            viewmodel = textbox.DataContext as ViewModel;

            if (textbox.Text == placeholder)
                textbox.Text = "";

            else
            {
                textbox.Foreground = new SolidColorBrush(Colors.Black);
                textbox.SelectAll();
            }
        }
    }
}