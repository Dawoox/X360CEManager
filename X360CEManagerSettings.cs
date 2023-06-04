using Playnite.SDK;
using Playnite.SDK.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// ReSharper disable All
namespace X360CEManager
{
    public class X360CEManagerSettings : ObservableObject
    {
        private bool _startWithPlayniteDefault = false;
        private bool _startWithAllGamesDefault = false;
        private bool _startWithSelectedGamesDefault = false;
        private string _x360cePathDefault = string.Empty; 
        
        public bool startWithPlaynite { get => _startWithPlayniteDefault; set => SetValue(ref _startWithPlayniteDefault, value); }
        public bool startWithAllGames { get => _startWithAllGamesDefault; set => SetValue(ref _startWithAllGamesDefault, value); }
        public bool startWithSelectedGames { get => _startWithSelectedGamesDefault; set => SetValue(ref _startWithSelectedGamesDefault, value); }
        public string x360cePath { get => _x360cePathDefault; set => SetValue(ref _x360cePathDefault, value); }
    }

    
    public class X360CEManagerSettingsViewModel : ObservableObject, ISettings
    {
        private readonly X360CEManager plugin;
        private X360CEManagerSettings editingClone { get; set; }

        private X360CEManagerSettings settings;
        public X360CEManagerSettings Settings
        {
            get => settings;
            set { settings = value; OnPropertyChanged(); }
        }

        public X360CEManagerSettingsViewModel(X360CEManager plugin)
        {
            // Injecting your plugin instance is required for Save/Load method because Playnite saves data
            // to a location based on what plugin requested the operation.
            this.plugin = plugin;

            // Load saved settings.
            var savedSettings = plugin.LoadPluginSettings<X360CEManagerSettings>();

            // LoadPluginSettings returns null if no saved data is available.
            if (savedSettings != null) { Settings = savedSettings; }
            else { Settings = new X360CEManagerSettings(); }
        }

        public void BeginEdit() { editingClone = Serialization.GetClone(Settings); }

        public void CancelEdit() { Settings = editingClone; }

        public void EndEdit() { plugin.SavePluginSettings(Settings); }

        public bool VerifySettings(out List<string> errors)
        {
            errors = new List<string>();
            if (Settings.x360cePath.Equals(string.Empty)) { errors.Add("x360ce path not set"); }
            return errors.Count == 0;
        }
    }
}