﻿using gbfr.fix.matchmaking.Template.Configuration;

using Reloaded.Mod.Interfaces.Structs;

using System.ComponentModel;

namespace gbfr.fix.matchmaking.Configuration
{
    public class Config : Configurable<Config>
    {
        /*
            User Properties:
                - Please put all of your configurable properties here.
    
            By default, configuration saves as "Config.json" in mod user config folder.    
            Need more config files/classes? See Configuration.cs
    
            Available Attributes:
            - Category
            - DisplayName
            - Description
            - DefaultValue

            // Technically Supported but not Useful
            - Browsable
            - Localizable

            The `DefaultValue` attribute is used as part of the `Reset` button in Reloaded-Launcher.
        */

        [DisplayName("Lobby Distance Filter")]
        [Description("Controls AddRequestLobbyListDistanceFilter. Vanilla GBFR defaults to 'ELobbyDistanceFilterFar'.")]
        [DefaultValue(ELobbyDistanceFilter.ELobbyDistanceFilterWorldwide)]
        public ELobbyDistanceFilter LobbyDistanceFilter { get; set; } = ELobbyDistanceFilter.ELobbyDistanceFilterWorldwide;

        public enum ELobbyDistanceFilter
        {
            ELobbyDistanceFilterClose,
            ELobbyDistanceFilterDefault,
            ELobbyDistanceFilterFar,
            ELobbyDistanceFilterWorldwide,
        }
    }

    /// <summary>
    /// Allows you to override certain aspects of the configuration creation process (e.g. create multiple configurations).
    /// Override elements in <see cref="ConfiguratorMixinBase"/> for finer control.
    /// </summary>
    public class ConfiguratorMixin : ConfiguratorMixinBase
    {
        // 
    }
}
