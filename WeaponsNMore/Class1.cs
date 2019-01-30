using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using Unity;
using UnityEngine;
using TerraTech;
using System.Reflection;
using Nuterra.BlockInjector;


namespace WeaponsNMore
{
    class QPatch
    {
        public static void Main()
        {
            var harmony = HarmonyInstance.Create("mindless.ttmm.changeteamanywhere.mod");
            harmony.PatchAll(System.Reflection.Assembly.GetExecutingAssembly());
            {

                var HECombatComputer = new BlockPrefabBuilder("GSOLightStud(111)", true)
                    .SetBlockID(730192, "cbea0c11d1655dd4")
                .SetMass(6)
                .SetName("Hawkeye Combat Computer")
                .SetDescription("The programmers at Hawkeye HQ have long been attempting to find a way to improve the accuracy of their weapons. They have finally done it. The Combat Computer can effectively control the weapons on a tech and make them lead their target- resulting in a lot less missed shots.")
                .SetPrice(2000)
                .SetHP(1000)
                .SetFaction(FactionSubTypes.HE)
                .SetCategory(BlockCategories.Accessories)
                .SetModel(GameObjectJSON.MeshFromFile("HE_COMBAT_COMPUTER.obj"), true, GameObjectJSON.GetObjectFromGameResources<Material>("HE_Main"))
                .SetIcon(GameObjectJSON.SpriteFromImage(GameObjectJSON.ImageFromFile("HECombatComputerIcon.png")))
                .SetSizeManual(
                    new IntVector3[] {
                    new IntVector3(0,0,0),
                    },
                    new Vector3[] {
                    new Vector3(0f, -.5f, 0f),
                    new Vector3(0f, .5f, 0f),
                    new Vector3(0f, 0f, .5f)
});

                HECombatComputer.Prefab.AddComponent<ModuleVelocityAim>();
                HECombatComputer.RegisterLater();

            }
        }
    }

    static class Despacito
    {
        private static FieldInfo m_TargetPosition;
        [HarmonyPatch(typeof(TargetAimer), "UpdateTarget")] //when the target is updated in the target aimer class, harmony will go "hey that's my cue"
        static class Despacito2
        {

            private static void Postfix(ref TargetAimer __instance)
            {
                try
                {
                    if (__instance.HasTarget)
                    {
                        if (Despacito.m_TargetPosition == null)
                        {
                            Despacito.m_TargetPosition = typeof(TargetAimer).GetField("m_TargetPosition", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                            if (Despacito.m_TargetPosition == null)
                            {
                                Console.WriteLine("FIEL INFOS ARE NULLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLL");
                            }
                        }
                        FireData mw = __instance.GetComponentInParent<FireData>();
                        if (!(mw == null))
                        {
                            Tank Parent = __instance.GetComponentInParent<Tank>();
                            if (!(Parent == null) && !(__instance.Target.rbody == null) && Parent.GetComponentInChildren<ModuleVelocityAim>() != null)
                            {
                                Vector3 PredictedTravel = (__instance.Target.rbody.velocity - Parent.rbody.velocity) * (__instance.Target.rbody.transform.position - Parent.rbody.transform.position).magnitude / mw.m_MuzzleVelocity;
                                Despacito.m_TargetPosition.SetValue(__instance, PredictedTravel * 1.1f + (Vector3)Despacito.m_TargetPosition.GetValue(__instance));
                            }
                        }
                    }
                }
                catch
                {
                }
            }
        }

    }
    public class ModuleVelocityAim : Module
    {

    }
}

