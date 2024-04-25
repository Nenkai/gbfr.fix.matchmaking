using gbfr.fix.matchmaking.Configuration;
using gbfr.fix.matchmaking.Template;

using Reloaded.Hooks.ReloadedII.Interfaces;
using Reloaded.Mod.Interfaces;

using System.Diagnostics;

using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.ReloadedII.Interfaces;
using Reloaded.Mod.Interfaces;
using Reloaded.Memory.Interfaces;
using Reloaded.Memory.SigScan.ReloadedII.Interfaces;
using IReloadedHooks = Reloaded.Hooks.ReloadedII.Interfaces.IReloadedHooks;
using System;
using Reloaded.Hooks.Definitions.Structs;

namespace gbfr.fix.matchmaking;

/// <summary>
/// Your mod logic goes here.
/// </summary>
public unsafe class Mod : ModBase // <= Do not Remove.
{
    /// <summary>
    /// Provides access to the mod loader API.
    /// </summary>
    private readonly IModLoader _modLoader;

    /// <summary>
    /// Provides access to the Reloaded.Hooks API.
    /// </summary>
    /// <remarks>This is null if you remove dependency on Reloaded.SharedLib.Hooks in your mod.</remarks>
    private readonly IReloadedHooks? _hooks;

    /// <summary>
    /// Provides access to the Reloaded logger.
    /// </summary>
    private readonly ILogger _logger;

    /// <summary>
    /// Entry point into the mod, instance that created this class.
    /// </summary>
    private readonly IMod _owner;

    /// <summary>
    /// Provides access to this mod's configuration.
    /// </summary>
    private Config _configuration;

    /// <summary>
    /// The configuration of the currently executing mod.
    /// </summary>
    private readonly IModConfig _modConfig;

    private static IStartupScanner? _startupScanner = null!;

    private IHook<SetNumAttempts> _setNumAttemptsHook;
    private IHook<CreateLobbyInternal> _createLobbyInternal;

    public delegate void SetNumAttempts(byte* a1);
    public delegate void CreateLobbyInternal(byte* a1, byte* a2, byte* a3, byte* a4);

    public Mod(ModContext context)
    {
        _modLoader = context.ModLoader;
        _hooks = context.Hooks;
        _logger = context.Logger;
        _owner = context.Owner;
        _configuration = context.Configuration;
        _modConfig = context.ModConfig;

        var startupScannerController = _modLoader.GetController<IStartupScanner>();
        if (startupScannerController == null || !startupScannerController.TryGetTarget(out _startupScanner))
        {
            return;
        }

        var memory = Reloaded.Memory.Memory.Instance;

        // Change func argument
        SigScan("BA ?? ?? ?? ?? FF 50 ?? 48 8B 03", "", address =>
        {
            memory.SafeWrite((nuint)address + 1, new byte[] { (byte)_configuration.LobbyDistanceFilter });
            _logger.WriteLine($"[gbfr.fix.matchmaking] Successfully set ELobbyDistanceFilter to {_configuration.LobbyDistanceFilter} ({(byte)_configuration.LobbyDistanceFilter}) at 0x{address:X8}", _logger.ColorGreen);
        });

        // Join lobby attempts are done 8-10 times, one attempt every 3 seconds. Once the counter reaches 0, joining a lobby fails
        // The number of attempts is more or less randomized using std::random_device
        //
        // To more easily find this function, a function in it has a string "invalid random_device value"
        // Find the function that matches:
        //   *(_DWORD *)(a1 + 136) = (unsigned __int8)(BYTE4(randValue) + 20) / 3u;
        SigScan("56 57 B8 ?? ?? ?? ?? E8 ?? ?? ?? ?? 48 29 C4 48 89 CE", "", address =>
        {
            _setNumAttemptsHook = _hooks.CreateHook<SetNumAttempts>(SetNumAttemptsHook, address).Activate();
            _logger.WriteLine($"[gbfr.fix.matchmaking] Successfully hooked SetNumAttempts (num attempts: {_configuration.NumAttempts})", _logger.ColorGreen);
        });

        // Part of a function called inside a function of hw::network::LobbySystem
        SigScan("55 41 57 41 56 41 55 41 54 56 57 53 48 81 EC ?? ?? ?? ?? 48 8D AC 24 ?? ?? ?? ?? 48 83 E4 ?? 48 89 E3 48 89 AB ?? ?? ?? ?? 48 C7 45 ?? ?? ?? ?? ?? 4D 89 CE 4C 89 C7", "", address =>
        {
            _createLobbyInternal = _hooks.CreateHook<CreateLobbyInternal>(CreateLobbyInternalHook, address).Activate();
            _logger.WriteLine($"[gbfr.fix.matchmaking] Successfully hooked CreateLobbyInternal", _logger.ColorGreen);
        });
    }

    private static void SigScan(string pattern, string name, Action<nint> action)
    {
        var baseAddress = Process.GetCurrentProcess().MainModule!.BaseAddress;
        _startupScanner?.AddMainModuleScan(pattern, result =>
        {
            if (!result.Found)
            {
                return;
            }
            action(result.Offset + baseAddress);
        });
    }

    private void SetNumAttemptsHook(byte* a1)
    {
        _setNumAttemptsHook.OriginalFunction(a1);

        if (a1 is not null && *(int*)(a1 + 0x88) > 0)
        {
            _logger.WriteLine($"[gbfr.fix.matchmaking] SetNumAttempts intercepted (original num attempts: {*(int*)(a1 + 0x88)}, new: {_configuration.NumAttempts})");
            *(int*)(a1 + 0x88) = _configuration.NumAttempts;
        }
    }

    private void CreateLobbyInternalHook(byte* a1, byte* a2, byte* a3, byte* a4)
    {
        _createLobbyInternal.OriginalFunction(a1, a2, a3, a4);
        _logger.WriteLine($"[gbfr.fix.matchmaking] CreateLobbyInternalHook intercepted (original timeout: {(*(int*)(a1 + 0x18)) / 1000}ms, new: {_configuration.LobbyFillTimeout})");

        *(int*)(a1 + 0x18) = _configuration.LobbyFillTimeout * 1000;
    }

    #region Standard Overrides
    public override void ConfigurationUpdated(Config configuration)
    {
        // Apply settings from configuration.
        // ... your code here.
        _configuration = configuration;
        _logger.WriteLine($"[{_modConfig.ModId}] Config Updated: Applying");
    }
    #endregion

    #region For Exports, Serialization etc.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public Mod() { }
#pragma warning restore CS8618
    #endregion
}