using BepInEx;
using R2API;
using RoR2;
using EntityStates;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2.ContentManagement;
using System.Linq;
using RoR2.CharacterAI;
using KinematicCharacterController;
//using BepInEx.Configuration;

namespace HalcyoniteTweaks
{
    [BepInDependency("com.Moffein.RiskyTweaks", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]

    public class HalcyoniteTweaks : BaseUnityPlugin
    {
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "Jeffdev";
        public const string PluginName = "HalcyoniteTweaks";
        public const string PluginVersion = "1.0.0";

        private GameObject halcyoniteMaster = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC2/Halcyonite/HalcyoniteMaster.prefab").WaitForCompletion();
        public void Awake()
        {
            Log.Init(Logger);

            foreach (AISkillDriver item in halcyoniteMaster.GetComponents<AISkillDriver>().ToList())
            {
                if (item.customName == "Golden Swipe")
                {
                    item.maxDistance = 17f;
                    item.minDistance = 12f;
                    item.selectionRequiresAimTarget = true;
                    item.selectionRequiresTargetLoS = true;
                }
                if (item.customName == "Golden Slash")
                {
                    item.maxDistance = 12f;
                    item.selectionRequiresTargetLoS = true;
                }
            }

            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.RiskyTweaks")) return;

            // Halcyonite Stun/Freeze fix, credit to Moffein for writing this part, I was given permission to utilize it here by him in the modding discord
            GameObject bodyObject = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC2/Halcyonite/HalcyoniteBody.prefab").WaitForCompletion();
            var allStateMachines = bodyObject.GetComponents<EntityStateMachine>();
            SetStateOnHurt ssoh = bodyObject.GetComponent<SetStateOnHurt>();
            ssoh.idleStateMachine = [.. allStateMachines.Where(esm => esm != ssoh.targetStateMachine)];

        }
    }
}
