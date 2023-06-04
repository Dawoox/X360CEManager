using Playnite.SDK;
using Playnite.SDK.Events;
using Playnite.SDK.Models;
using Playnite.SDK.Plugins;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Controls;

namespace X360CEManager
{
    // ReSharper disable once ClassNeverInstantiated.Global
    // ReSharper disable once InconsistentNaming
    public class X360CEManager : GenericPlugin
    {
        private static readonly ILogger Logger = LogManager.GetLogger();
        private static readonly Process X360CeProcess = new Process();
        private static Tag _flagTag = Tag.Empty;
        private static bool _emulatorStarted;

        private X360CEManagerSettingsViewModel Settings { get; set; }
        public override Guid Id { get; } = Guid.Parse("b69dfabb-9de7-4235-be99-690b4230d9f6");

        public X360CEManager(IPlayniteAPI api) : base(api)
        {
            Settings = new X360CEManagerSettingsViewModel(this);
            Properties = new GenericPluginProperties { HasSettings = true };
        }

        public override void OnGameStarting(OnGameStartingEventArgs args)
        {
            // If the game is marked start with the flag-tag and needed settings are true start the emulator
            if (Settings.Settings.startWithSelectedGames && args.Game.Tags.Contains(_flagTag))
            {
                Logger.Debug("X360CE scheduled to start with " + args.Game.Name);
                StartEmulator();
            }
        }

        public override void OnGameStopped(OnGameStoppedEventArgs args)
        {
            // If x360ce started with playnite or config to not start with games, return
            if (Settings.Settings.startWithPlaynite) return;
            // If x360ce started with the game, closed it
            if (args.Game.Tags.Contains(_flagTag))
            {
                Logger.Debug("X360CE scheduled to stop with " + args.Game.Name);
                StopEmulator();
            }
        }

        public override void OnApplicationStarted(OnApplicationStartedEventArgs args)
        {
            UpdateTag(); // Make sure the reference to the flagtag is set right

            if (Settings.Settings.startWithPlaynite)    // Start x360ce if config so 
            {
                Logger.Debug("X360CE scheduled to start with Playnite");
                StartEmulator();
            }
        }

        public override void OnApplicationStopped(OnApplicationStoppedEventArgs args)
        {
            // Always closing x360ce when closing (to be sure)
            Logger.Debug("X360CE scheduled to stop");
            StopEmulator();
        }

        // To add new game menu items override GetGameMenuItems
        public override IEnumerable<GameMenuItem> GetGameMenuItems(GetGameMenuItemsArgs args)
        {
            yield return new GameMenuItem { MenuSection = "X360CEManager" };
        }

        private void UpdateTag()
        {
            var found = false;
            foreach (var tag in PlayniteApi.Database.Tags)
            {
                if (tag.Name == "[X360CE] Start with")
                {
                    _flagTag = tag;
                    found = true;
                    Logger.Debug("FlagTag found, updating the reference");
                    break; 
                }
            }

            if (!found) // If the tag is not present, add it to the tags database
            {
                _flagTag = new Tag("[X360CE] Start with");
                PlayniteApi.Database.Tags.Add(_flagTag);
            }
        }
        
        private void StartEmulator() 
        {
            if (GetEmulatorPath().Equals(null)) // If the path is not set return
            {
                Logger.Error("X360CE path not set");
                return;
            }
            
            if (_emulatorStarted)   // If already started return
            {
                Logger.Error("X360CE already started");
                return;
            }
            X360CeProcess.StartInfo.FileName = Settings.Settings.x360cePath;
            X360CeProcess.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
            X360CeProcess.Start();
            _emulatorStarted = true;
            Logger.Debug("X360CE started, " + Settings.Settings.x360cePath);
        }

        private static void StopEmulator()
        {
            X360CeProcess.Kill();
            _emulatorStarted = false;
            Logger.Debug("X360CE stopped");
        }

        private string GetEmulatorPath()
        {
            var path = Settings.Settings.x360cePath;
            // If not set return null
            return path.Equals(string.Empty) ? null : path;
        }
        
        public override ISettings GetSettings(bool firstRunSettings) { return Settings; }
        public override UserControl GetSettingsView(bool firstRunSettings) { return new X360CEManagerSettingsView(); }
    }
}