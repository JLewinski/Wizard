using System;
using System.Linq;
using Xamarin.Forms;
using Wizard.Mobile.Pages;
using Xamarin.Forms.Xaml;

namespace Wizard.Mobile
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new HomePage());
            //MainPage = new HomePage();

        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
