using System.Windows.Controls;
using System.Windows.Media;

namespace Desktop.App
{
    static class ControlLogic
    {
        internal static void FocusResonse<ViewModel>(this TextBox textbox, ViewModel viewmodel, string compareValue, object dataContext) where ViewModel : class
        {
            viewmodel = dataContext as ViewModel;

            if (textbox.Text == compareValue)
                textbox.Text = "";

            else
            {
                textbox.Foreground = new SolidColorBrush(Colors.Black);
                textbox.SelectAll();
            }
        }

        internal static void FocusResonse<ViewModel>(this PasswordBox textbox, ViewModel viewmodel, string compareValue, object dataContext) where ViewModel : class
        {
            viewmodel = dataContext as ViewModel;

            if (textbox.Password == compareValue)
                textbox.Password = "";

            else textbox.SelectAll();
        }
    }
}