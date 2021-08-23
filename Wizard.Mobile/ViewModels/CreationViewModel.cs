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
                    Names.Add(new NameViewModel($"Player {Names.Count + 1}"));
                    RaisePropertyChanged(nameof(NumberOfPlayers));
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

            RemovePlayerCommand = new Command(() =>
            {
                if (Names.Count > 4)
                {
                    Names.RemoveAt(Names.Count - 1);
                    RaisePropertyChanged(nameof(NumberOfPlayers));
                }
            });
        }

        public int NumberOfPlayers => Names.Count;

        #region Commands

        public ICommand AddPlayerCommand { get; }
        public ICommand RemovePlayerCommand { get; }
        public ICommand ClearPlayersCommand { get; }
        public ICommand StartCommand { get; }

        #endregion Commands

        public ObservableCollection<NameViewModel> Names { get; } = new ObservableCollection<NameViewModel>
        {
            new NameViewModel("Player 1 (1st dealer)"),
            new NameViewModel("Player 2"),
            new NameViewModel("Player 3"),
            new NameViewModel("Player 4")
        };
    }

    public class NameViewModel : BindableBase
    {
        public NameViewModel(string number)
        {
            Name = string.Empty;
            PlayerNumber = number;
        }

        private string name;
        public string Name { get => name; set => SetProperty(ref name, value); }

        public string PlayerNumber { get; }
    }
}
