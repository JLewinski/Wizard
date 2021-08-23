using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Wizard.Mobile.ViewModels
{
    public class HomeViewModel : BindableBase
    {
        private readonly string _dbPath;
        private readonly Services.DataService _dataService;

        public HomeViewModel()
        {
            _dbPath = DependencyService.Get<IDataBaseAccess>().DatabasePath();
            _dataService = new Services.DataService(_dbPath);
            LoadCommand = new Command<int>(async x => await LoadPage(x));
            DeleteCommand = new Command<int>(id =>
            {
                if (!_dataService.Delete(id))
                {
                    //ERROR
                }
                else
                {
                    Refresh();
                }
            });
            NewGameCommand = new Command(async () =>
            {
                var navPage = Application.Current.MainPage as NavigationPage;
                await navPage.PushAsync(new Pages.CreationPage());
            });
            RefreshCommand = new Command(Refresh);
            Refresh();
        }

        public void Refresh()
        {
            Games = _dataService.GetSummary();
        }

        public Models.GameSummary SelectedSummary { get; set; }

        private List<Models.GameSummary> games;
        public List<Models.GameSummary> Games { get => games; set => SetProperty(ref games, value); }

        public ICommand LoadCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand NewGameCommand { get; }
        public ICommand RefreshCommand { get; }

        public async Task LoadPage()
        {
            var navPage = Application.Current.MainPage as NavigationPage;
            await navPage.PushAsync(new Pages.MainPage(SelectedSummary.Id));
        }

        public async Task LoadPage(int id)
        {
            var navPage = Application.Current.MainPage as NavigationPage;
            await navPage.PushAsync(new Pages.MainPage(id));
        }
    }
}
