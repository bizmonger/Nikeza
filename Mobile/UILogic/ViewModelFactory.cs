using Nikeza.Mobile.UILogic.Registration;

namespace XForms
{
    public static class ViewModelFactory
    {
        public static ViewModel GetViewModel()
        {
            var actions = new Actions(null);
            var observers = new Observers(null);

            var dependencies = new Dependencies(actions, observers);
            return new ViewModel(dependencies);
        }
    }
}