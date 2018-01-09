using System.Windows.Controls;

namespace Desktop.App
{
    static class ControlLogic
    {
        internal static void FocusResonse<ViewModel>(this TextBox textbox, string compareValue, ViewModel viewmodel, object dataContext) where ViewModel : class
        {
            viewmodel = dataContext as ViewModel;

            if (textbox.Text == compareValue)
                textbox.Text = "";

            else textbox.SelectAll();
        }

        internal static void FocusResonse<ViewModel>(this PasswordBox textbox, string compareValue, ViewModel viewmodel, object dataContext) where ViewModel : class
        {
            viewmodel = dataContext as ViewModel;

            if (textbox.Password == compareValue)
                textbox.Password = "";

            else textbox.SelectAll();
        }
    }
}