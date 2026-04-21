using HarmonyLib;
using System.Reflection;

/// <summary>
/// SharedStorage mod entry point.
/// Registers Harmony patches on load via IModApi.
/// </summary>
public class SharedStorage : IModApi
{
    public void InitMod(Mod _mod)
    {
        Log.Out("[SharedStorage] Initializing...");
        var harmony = new Harmony(GetType().ToString());
        harmony.PatchAll(Assembly.GetExecutingAssembly());
        Log.Out("[SharedStorage] Harmony patches applied.");
    }
}

/// <summary>
/// Patch TileEntity.IsUserAccessing so that it returns false for storage containers,
/// allowing multiple players to open the same container simultaneously.
///
/// Vanilla behaviour: when Player A opens a container, lockedByEntityID is set
/// to A's entity ID. IsUserAccessing() returns true → "container in use" toast
/// → Player B is blocked.
///
/// Patched behaviour: IsUserAccessing returns false for TileEntityLootContainer
/// and TileEntityCollector, so every player can open any storage regardless of
/// who else has it open. All other TileEntity types (secure doors, land claims,
/// etc.) use the original method.
///
/// Why patch TileEntity instead of TileEntityLootContainer:
/// IsUserAccessing() is declared in the abstract base class TileEntity and is not
/// overridden in TileEntityLootContainer. HarmonyX requires the method to be
/// declared in the patched type, so patching TileEntityLootContainer directly
/// fails at runtime with "Could not find method".
///
/// Safety note: 7DTD uses a server-authoritative item model. If two players
/// attempt to take the same item, the server resolves the conflict and sends
/// the authoritative state to both clients. Simultaneous access therefore does
/// not cause item duplication or loss.
/// </summary>
[HarmonyPatch(typeof(TileEntity), "IsUserAccessing")]
public static class Patch_TileEntity_IsUserAccessing
{
    static bool Prefix(TileEntity __instance, ref bool __result)
    {
        // Only override for storage containers; leave other tile entities
        // (secure doors, land claims, etc.) untouched.
        if (__instance is TileEntityLootContainer || __instance is TileEntityCollector)
        {
            __result = false;
            return false; // skip original method
        }
        return true; // run original method for everything else
    }
}
