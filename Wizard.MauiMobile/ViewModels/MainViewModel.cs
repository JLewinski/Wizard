using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wizard.Models;
using Wizard.Services;
using Wizard.Services.Settings;

namespace Wizard.MauiMobile.ViewModels
{
    public class MainViewModel : BindableBase
    {
        private readonly IDataService _dataService;
        private List<GameSummary> _gameList;

        public MainViewModel(IDataService dataService, ISettingsService settings)
            : base(settings)
        {
            _dataService = dataService;
            _gameList = dataService.GetSummaries();
        }
    }
}
