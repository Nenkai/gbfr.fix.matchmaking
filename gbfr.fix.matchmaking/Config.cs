using gbfr.fix.matchmaking.Template.Configuration;

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

        [DisplayName("Join Lobby Attempts")]
        [Description("Number of attempts until the game consider joining a lobby a fail. An attempt is made every 3 seconds. \n" +
            "Vanilla is 7-10 attempts. Mod default is 20. 0 will skip any join phase.\n")]
        [DefaultValue(20)]
        [SliderControlParams(minimum: 0.0, maximum: 100.0, smallChange: 1.0, largeChange: 10.0, tickFrequency: 10,
            isSnapToTickEnabled: false,
            tickPlacement: SliderControlTickPlacement.BottomRight,
            showTextField: true,
            isTextFieldEditable: true,
            textValidationRegex: "\\d{1-3}")]
        public int NumAttempts { get; set; } = 20;

        [DisplayName("Lobby Fill Timeout")]
        [Description("Number of seconds to wait for a created lobby to be full (after joining has failed), before being automatically disbanded.\n" +
            "Vanilla is 60s (30s before v1.2.1). Mod default is 60s.\n")]
        [DefaultValue(60)]
        [SliderControlParams(minimum: 20.0, maximum: 300.0, smallChange: 1.0, largeChange: 10.0, tickFrequency: 10,
            isSnapToTickEnabled: false,
            tickPlacement: SliderControlTickPlacement.BottomRight,
            showTextField: true,
            isTextFieldEditable: true,
            textValidationRegex: "\\d{1-3}")]
        public int LobbyFillTimeout { get; set; } = 60;

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
