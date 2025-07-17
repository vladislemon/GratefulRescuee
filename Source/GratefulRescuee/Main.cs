using HarmonyLib;
using Verse;
using Verse.AI;

namespace GratefulRescuee
{
    [StaticConstructorOnStartup]
    public class Main
    {
        static Main()
        {
            var harmony = new Harmony("vladislemon.gratefulrescuee");
            harmony.PatchAll();
        }
    }

    // Size of goodwill reward depends on how many times guest was tended by player faction.
    // However, downing or un-downing (only in 1.5) cause guest to forget that count.
    // That's sad, because almost always players tend unconscious guests (at least I do).
    // And then they come to their senses, heal and leave map. No reward for us...
    // So, let's patch code where "forgetting" happens.
    [HarmonyPatch(typeof(Pawn_MindState), nameof(Pawn_MindState.Reset))]
    public class Pawn_MindState_Reset_Patch
    {
        static void Prefix(Pawn_MindState __instance, out int __state)
        {
            __state = __instance.timesGuestTendedToByPlayer;
        }

        static void Postfix(Pawn_MindState __instance, ref int __state, bool clearInspiration, bool clearMentalState)
        {
            if (clearInspiration && clearMentalState)
                // Pawm.Destroy() uses this combination of arg values, it makes sense to actually reset in that case
                return;
            __instance.timesGuestTendedToByPlayer = __state;
        }
    }
}