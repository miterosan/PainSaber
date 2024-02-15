using System.Diagnostics;
using HarmonyLib;

namespace PainSaber.Hooks
{
    [HarmonyPatch(typeof(PlayerHeadAndObstacleInteraction))]
    public class HeadInObstacleHook
    {
        [HarmonyPostfix]
        [HarmonyPatch("Update")]
        static void Postfix(PlayerHeadAndObstacleInteraction __instance)
        {
            if (__instance.playerHeadIsInObstacle)
            {
                PainSaberPlugin.HeadInWall(stopwatch.ElapsedMilliseconds);
            } else {
                stopwatch.Reset();
            }
        }

        private static Stopwatch stopwatch = new Stopwatch();
    }
}