using System.Collections.Generic;
using Wizard.Models;
using Wizard.Services;

namespace Wizard.MauiMobile.ViewModels
{
    public class MainPageViewModel : BindableBase
    {
        private readonly IDataService _dataService;
        private List<GameSummary> _gameList;

        public MainPageViewModel(IDataService dataService)
        {
            _dataService = dataService;
        }
        
    }
}