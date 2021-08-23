using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Wizard.Mobile.ViewModels
{
    public class CreationViewModel : BindableBase
    {
        private readonly Services.DataService _dataService;

        public CreationViewModel()
        {
            string dbPath = DependencyService.Get<IDataBaseAccess>().DatabasePath();
            _dataService = new Services.DataService(dbPath);
            AddPlayerCommand = new Command(() =>
            {
                if (Names.Count < 6)
                {
                    Names.Add(string.Empty);
                }
            });
            ClearPlayersCommand = new Command(() => Names.Clear());
            StartCommand = new Command(async () =>
            {
                if (Names.Count > 3)
                {
                    var game = _dataService.Create(Names);

                    if (game != null)
                    {
                        var navPage = Application.Current.MainPage as NavigationPage;
                        var page = await navPage.PopAsync();

                        await navPage.PushAsync(new Pages.MainPage(game.Id));
                    }
                }
            });
        }

        #region Commands

        public ICommand AddPlayerCommand { get; }
        public ICommand ClearPlayersCommand { get; }
        public ICommand StartCommand { get; }

        #endregion Commands

        public ObservableCollection<string> Names { get; } = new ObservableCollection<string> { string.Empty, string.Empty, string.Empty, string.Empty };
    }
}
