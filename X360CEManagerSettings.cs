using Playnite.SDK;
using Playnite.SDK.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            set
            {
                settings = value;
                OnPropertyChanged();
            }
        }

        public X360CEManagerSettingsViewModel(X360CEManager plugin)
        {
            // Injecting your plugin instance is required for Save/Load method because Playnite saves data to a location based on what plugin requested the operation.
            this.plugin = plugin;

            // Load saved settings.
            var savedSettings = plugin.LoadPluginSettings<X360CEManagerSettings>();

            // LoadPluginSettings returns null if no saved data is available.
            if (savedSettings != null)
            {
                Settings = savedSettings;
            }
            else
            {
                Settings = new X360CEManagerSettings();
            }
        }

        public void BeginEdit()
        {
            // Code executed when settings view is opened and user starts editing values.
            editingClone = Serialization.GetClone(Settings);
        }

        public void CancelEdit()
        {
            // Code executed when user decides to cancel any changes made since BeginEdit was called.
            // This method should revert any changes made to Option1 and Option2.
            Settings = editingClone;
        }

        public void EndEdit()
        {
            // Code executed when user decides to confirm changes made since BeginEdit was called.
            // This method should save settings made to Option1 and Option2.
            plugin.SavePluginSettings(Settings);
        }

        public bool VerifySettings(out List<string> errors)
        {
            // Code execute when user decides to confirm changes made since BeginEdit was called.
            // Executed before EndEdit is called and EndEdit is not called if false is returned.
            // List of errors is presented to user if verification fails.
            errors = new List<string>();
            return true;
        }
    }
}