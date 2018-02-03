using Xamarin.Forms;

namespace XForms
{
    public partial class RegistrationPage1 : ContentPage
	{
		public RegistrationPage1() => InitializeComponent();

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            ControlTemplate = getTemplate(width, height);
        }

        ControlTemplate getTemplate(double width, double height) =>
            (height > width) ? Resources["PortraitTemplate"]  as ControlTemplate 
                             : Resources["LandscapeTemplate"] as ControlTemplate;
    }
}