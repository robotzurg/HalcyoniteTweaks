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
        public const string PluginVersion = "1.0.1";

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

            On.RoR2.CharacterBody.Start += CharacterBody_Start;

            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.RiskyTweaks")) return;

            // Halcyonite Stun/Freeze fix, credit to Moffein for writing this part, I was given permission to utilize it here by him in the modding discord
            GameObject bodyObject = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC2/Halcyonite/HalcyoniteBody.prefab").WaitForCompletion();
            var allStateMachines = bodyObject.GetComponents<EntityStateMachine>();
            SetStateOnHurt ssoh = bodyObject.GetComponent<SetStateOnHurt>();
            ssoh.idleStateMachine = [.. allStateMachines.Where(esm => esm != ssoh.targetStateMachine)];

        }

        private void CharacterBody_Start(On.RoR2.CharacterBody.orig_Start orig, CharacterBody self)
        {
            if (self.name == "HalcyoniteBody(Clone)")
            {
                CapsuleCollider component = self.GetComponent<CapsuleCollider>();
                component.center = new Vector3(0f, 0f, 0f);
                component.height = -1.25f;
                KinematicCharacterMotor component2 = self.GetComponent<KinematicCharacterMotor>();
                component2.CapsuleYOffset = 0f;
                component2.CapsuleHeight = -1.25f;
                self.modelLocator.modelTransform.GetChild(4).localScale = new Vector3(4f, 5f, 12f);
            }
        }
    }
}
