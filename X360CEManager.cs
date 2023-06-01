using Playnite.SDK;
using Playnite.SDK.Events;
using Playnite.SDK.Models;
using Playnite.SDK.Plugins;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace X360CEManager
{
    public class X360CEManager : GenericPlugin
    {
        private static readonly ILogger logger = LogManager.GetLogger();

        private static Process x360ceProcess = new Process();

        private X360CEManagerSettingsViewModel settings { get; set; }

        public override Guid Id { get; } = Guid.Parse("b69dfabb-9de7-4235-be99-690b4230d9f6");

        public X360CEManager(IPlayniteAPI api) : base(api)
        {
            settings = new X360CEManagerSettingsViewModel(this);
            Properties = new GenericPluginProperties
            {
                HasSettings = true
            };
        }

        public override void OnGameStarting(OnGameStartingEventArgs args)
        {
            // Add code to be executed when game is preparing to be started.
            if (args.Game.Tags.Contains("[X360CE]Start with"))
            {
                logger.Debug("X360CE scheduled to start with INSERT GAME NAME");
                startEmulator();
            }
        }

        public override void OnGameStopped(OnGameStoppedEventArgs args)
        {
            // Add code to be executed when game is stopped
            if (args.Game.Tags.Contains("[X360CE]Start with"))
            {
                logger.Debug("X360CE scheduled to stop with INSERT GAME NAME");
                stopEmulator();
            }
        }

        public override void OnApplicationStarted(OnApplicationStartedEventArgs args)
        {
            // Start x360ce if config so 
            if (!settings.Settings.startWithPlaynite) { return; }
            
            logger.Debug("X360CE scheduled to start with Playnite");
            startEmulator();
        }

        public override void OnApplicationStopped(OnApplicationStoppedEventArgs args)
        {
            // Always closing x360ce when closing (to be sure)
            logger.Debug("X360CE scheduled to stop");
            stopEmulator();
        }

        public override ISettings GetSettings(bool firstRunSettings)
        {
            return settings;
        }

        public override UserControl GetSettingsView(bool firstRunSettings)
        {
            return new X360CEManagerSettingsView();
        }
        
        // To add new game menu items override GetGameMenuItems
        public override IEnumerable<GameMenuItem> GetGameMenuItems(GetGameMenuItemsArgs args)
        {
            yield return new GameMenuItem
            {
                MenuSection = "X360CEManager"
            };
        }

        private void startEmulator() 
        {
            if (getEmulatorPath.Equals(null))
            {
                logger.Error("X360CE path not set");
                return;
            }
            x360ceProcess.StartInfo.FileName = settings.Settings.x360cePath;
            x360ceProcess.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
            x360ceProcess.start();
            logger.Debug("X360CE started, " + settings.Settings.x360cePath);
        }

        private void stopEmulator()
        {
            x360ceProcess.Kill();
            logger.Debug("X360CE stopped");
        }

        private String? getEmulatorPath()
        {
            var path = settings.Settings.x360cePath;
            if (path.Equals(String.Empty)) { return null; }
            return settings.Settings.x360cePath;
        }
    }
}