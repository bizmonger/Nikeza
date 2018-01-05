using System.Windows;
using Microsoft.FSharp.Core;
using Nikeza.Mobile.Profile;
using Nikeza.Mobile.UILogic.Registration;
using static Nikeza.Common;
using static Nikeza.Mobile.Profile.Registration;

namespace Desktop.App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var cs_submit = FSharpFunc<ValidatedForm, FSharpResult<ProfileRequest,ValidatedForm>>.FromConverter(Try.submit);
            DataContext = new ViewModel(cs_submit);
        }
    }
}
