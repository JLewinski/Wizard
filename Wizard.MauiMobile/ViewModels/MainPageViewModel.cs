using System.Collections.Generic;
using Wizard.Models;
using Wizard.Services;
using Wizard.Services.Settings;

namespace Wizard.MauiMobile.ViewModels
{
    public class MainPageViewModel : BindableBase
    {
        private readonly IDataService _dataService;
        private List<GameSummary> _gameList;

        public MainPageViewModel(IDataService dataService, ISettingsService settings)
            : base(settings)
        {
            _dataService = dataService;
        }
        
    }
}