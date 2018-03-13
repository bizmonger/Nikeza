
using Nikeza.Mobile.UILogic.Registration;
using Xamarin.Forms;

namespace XForms
{
    public partial class App : Application
	{
		public App ()
        {
            InitializeComponent();
            MainPage = new RegistrationPage1 { BindingContext = new ViewModel1() };
        }

        protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}