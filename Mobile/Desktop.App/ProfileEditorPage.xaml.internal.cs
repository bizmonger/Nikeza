using Nikeza.Mobile.UILogic.Portal.ProfileEditor;

namespace Desktop.App
{
    partial class ProfileEditorPage
    {
        ViewModel _viewmodel;

        void InputResponse()
        {
            _viewmodel = DataContext as ViewModel;
            _viewmodel.Validate.Execute(null);
        }
    }
}