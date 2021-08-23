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
                    Names.Add(new NameViewModel());
                }
            });
            ClearPlayersCommand = new Command(() => Names.Clear());
            StartCommand = new Command(async () =>
            {
                if (Names.Count > 3 && Names.All(x => !string.IsNullOrWhiteSpace(x.Name)))
                {
                    var game = _dataService.Create(Names.Select(x => x.Name));

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

        public ObservableCollection<NameViewModel> Names { get; } = new ObservableCollection<NameViewModel>
        {
            new NameViewModel(),
            new NameViewModel(),
            new NameViewModel(),
            new NameViewModel()
        };
    }

    public class NameViewModel : BindableBase
    {
        private string name = string.Empty;
        public string Name { get => name; set => SetProperty(ref name, value); }
    }
}
