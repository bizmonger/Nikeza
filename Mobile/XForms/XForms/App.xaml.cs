
using Xamarin.Forms;

namespace XForms
{
    public partial class App : Application
	{
		public App ()
		{
			InitializeComponent();

			MainPage = new XForms.RegistrationPage();
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