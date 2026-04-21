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
/// Patch TileEntityLootContainer.IsUserAccessing so that it always returns false,
/// allowing multiple players to open the same container simultaneously.
///
/// Vanilla behaviour: when Player A opens a container, lockedByEntityID is set
/// to A's entity ID. Player B's OnBlockActivated calls IsUserAccessing(B.entityId),
/// which returns true → "container in use" toast → B is blocked.
///
/// Patched behaviour: IsUserAccessing always returns false, so every player can
/// open any container regardless of who else has it open.
///
/// Safety note: 7DTD uses a server-authoritative item model. If two players
/// attempt to take the same item, the server resolves the conflict and sends
/// the authoritative state to both clients. Simultaneous access therefore does
/// not cause item duplication or loss.
/// </summary>
[HarmonyPatch(typeof(TileEntityLootContainer), "IsUserAccessing")]
public static class Patch_TileEntityLootContainer_IsUserAccessing
{
    static bool Prefix(ref bool __result)
    {
        // Always report "no one else is accessing" → allow all players to open
        __result = false;
        return false; // skip original method
    }
}
