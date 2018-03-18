using Xamarin.Forms;
using Nikeza.Mobile.UILogic.Login;
using Nikeza.Mobile.AppLogic;

namespace XForms
{
    public partial class App : Application
	{
		public App ()
        {
            InitializeComponent();
            MainPage = new LoginPage { BindingContext = new ViewModel(Login.dependencies) };
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