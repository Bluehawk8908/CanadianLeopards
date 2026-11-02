using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;
using GHPC.Weapons;
using GHPC.Weaponry;

namespace CanadianLeopards
{
    public class AmmoSwaps
    {
        public static AmmoCodexScriptable codexScriptable_C35;         //outdated APDS
        public static AmmoClipCodexScriptable clipCodexScriptable_C35;
        public static AmmoType.AmmoClip clip_C35;
        public static GameObject visC35_go;
        public static AmmoType type_C35;

        public static AmmoCodexScriptable codexScriptable_C76;          //early APFSDS
        public static AmmoClipCodexScriptable clipCodexScriptable_C76;
        public static AmmoType.AmmoClip clip_C76;
        public static GameObject visC76_go;
        public static AmmoType type_C76;

        public static AmmoCodexScriptable codexScriptable_C127;           //1990s APFSDS
        public static AmmoClipCodexScriptable clipCodexScriptable_C127;
        public static AmmoType.AmmoClip clip_C127;
        public static GameObject visC127_go;
        public static AmmoType type_C127;

        public static AmmoCodexScriptable codexScriptable_L35;            //HESH
        public static AmmoClipCodexScriptable clipCodexScriptable_L35;
        public static AmmoType.AmmoClip clip_L35;
        public static GameObject visL35_go;
        public static AmmoType type_L35;

        public static AmmoCodexScriptable codexScriptable_DM53;            //Time-travelling APFSDS
        public static AmmoClipCodexScriptable clipCodexScriptable_DM53;
        public static AmmoType.AmmoClip clip_DM53;
        public static GameObject visDM53_go;
        public static AmmoType type_DM53;

        public static void CloneAmmo(AmmoType source, AmmoType destination)
        {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
            FieldInfo[] srcFields = source.GetType().GetFields(flags);
            FieldInfo[] destFields = destination.GetType().GetFields(flags);

            foreach (FieldInfo srcField in srcFields)
            {
                FieldInfo destField = destFields.FirstOrDefault(field => field.Name == srcField.Name);

                if (destField != null && !destField.IsLiteral)
                {
                    if (srcField.FieldType == destField.FieldType)
                        destField.SetValue(destination, srcField.GetValue(source));
                }
            }
        }

        public static void ClearAndReload(LoadoutManager loadout_manager)
        {
            int rack_count = loadout_manager.RackLoadouts.Length;
            for (int i = 0; i < rack_count; i++)
            {
                var rack = loadout_manager.RackLoadouts[i].Rack;
                rack.StoredClips = new List<AmmoType.AmmoClip>();
                rack.SlotIndicesByAmmoType = new Dictionary<AmmoType, List<byte>>();
                foreach (Transform transform in rack.VisualSlots)
                {
                    AmmoStoredVisual vis = transform.GetComponentInChildren<AmmoStoredVisual>();
                    if (vis != null && vis.AmmoType != null)
                    {
                        rack.RemoveAmmoVisualFromSlot(transform);
                    }
                }
            }

            WeaponSystem mainGun = loadout_manager._weaponsManager.Weapons[0].Weapon;
            mainGun.Feed.AmmoTypeInBreech = null;
            loadout_manager.SpawnCurrentLoadout();
            mainGun.Feed.Start();
            loadout_manager.RegisterAllBallistics();
        }
        public static void Initalize()
        {
            if (type_C35 != null && visC35_go != null) return;
            MelonLoader.MelonLogger.Msg("Initalizing ammunitions");
            
            AmmoType temp35 = new AmmoType();
            AmmoCodexScriptable default_apds = Resources.FindObjectsOfTypeAll<AmmoCodexScriptable>()
                .Where(o => o.name == "ammo_DM13(105)").FirstOrDefault();
            CloneAmmo(default_apds.AmmoType, temp35);
            type_C35 = temp35;            
            type_C35.Name = "C35 APDS-T"; //copying DM13 

            AmmoCodexScriptable codexC35 = new AmmoCodexScriptable { name = "ammo_C35", AmmoType = type_C35 };
            codexScriptable_C35 = codexC35;

            visC35_go = GameObject.Instantiate<GameObject>(type_C35.VisualModel);
            AmmoStoredVisual vis_C35 = visC35_go.transform.GetComponent<AmmoStoredVisual>();
            vis_C35.AmmoType = type_C35;
            vis_C35.AmmoScriptable = codexScriptable_C35;
            type_C35.VisualModel = visC35_go;

            AmmoType.AmmoClip clipC35 = new AmmoType.AmmoClip { Capacity = 1, Name = "C35 APDS-T", 
                MinimalPattern = new AmmoCodexScriptable[] { codexScriptable_C35 }};            
            clip_C35 = clipC35;

            AmmoClipCodexScriptable clipCodexC35 = new AmmoClipCodexScriptable { name = "clip_C35(105)", ClipType = clip_C35 };            
            clipCodexScriptable_C35 = clipCodexC35;

            /////////
            AmmoType temp76 = new AmmoType();
            AmmoCodexScriptable default_sabot = Resources.FindObjectsOfTypeAll<AmmoCodexScriptable>()
                .Where(o => o.name == "ammo_DM23(105)").FirstOrDefault();
            CloneAmmo(default_sabot.AmmoType, temp76);
            type_C76 = temp76;
            
            type_C76.Name = "C76 APFSDS-T"; //copying DM23, upgrading it approx. to FP105
            type_C76.Mass = 3.6f;            //DM13 is 3.72, DM23 is 4.2, M774 is 3.4, M833 is 4.6
            type_C76.MuzzleVelocity = 1485f; //DM13 is 1478, DM23 is 1455, M774 is 1509, M833 is 1494
            type_C76.RhaPenetration = 420;  //DM13 is 270, DM23 is 390, M774 is 440, M833 is 480

            AmmoCodexScriptable codexC76 = new AmmoCodexScriptable { name = "ammo_C76", AmmoType = type_C76 };
            codexScriptable_C76 = codexC76;

            visC76_go = GameObject.Instantiate<GameObject>(type_C76.VisualModel);
            AmmoStoredVisual vis_C76 = visC76_go.transform.GetComponent<AmmoStoredVisual>();
            vis_C76.AmmoType = type_C76;
            vis_C76.AmmoScriptable = codexScriptable_C76;
            type_C76.VisualModel = visC76_go;

            AmmoType.AmmoClip clipC76 = new AmmoType.AmmoClip { Capacity = 1, Name = "C76 APFSDS-T", 
                MinimalPattern = new AmmoCodexScriptable[] { codexScriptable_C76 } };            
            clip_C76 = clipC76;

            AmmoClipCodexScriptable clipCodexC76 = new AmmoClipCodexScriptable { name = "clip_C76(105)", ClipType = clip_C76 };
            clipCodexScriptable_C76 = clipCodexC76;

            /////////
            AmmoType temp127 = new AmmoType();            
            CloneAmmo(default_sabot.AmmoType, temp127);
            type_C127 = temp127;

            type_C127.Name = "C127 APFSDS-T"; //copying DM23, upgrading it approx. to C127
            type_C127.Mass = 3.44f;            //DM13 is 3.72, DM23 is 4.2,  M774 is 3.4,  M833 is 4.6
            type_C127.MuzzleVelocity = 1555f;  //DM13 is 1478, DM23 is 1455, M774 is 1509, M833 is 1494
            type_C127.RhaPenetration = 470;    //DM13 is 270,  DM23 is 390,  M774 is 440,  M833 is 480

            AmmoCodexScriptable codexC127 = new AmmoCodexScriptable { name = "ammo_C127", AmmoType = type_C127 };
            codexScriptable_C127 = codexC127;

            visC127_go = GameObject.Instantiate<GameObject>(type_C127.VisualModel);
            AmmoStoredVisual vis_C127 = visC127_go.transform.GetComponent<AmmoStoredVisual>();
            vis_C127.AmmoType = type_C127;
            vis_C127.AmmoScriptable = codexScriptable_C127;
            type_C127.VisualModel = visC127_go;

            AmmoType.AmmoClip clipC127 = new AmmoType.AmmoClip
            {
                Capacity = 1,
                Name = "C127 APFSDS-T",
                MinimalPattern = new AmmoCodexScriptable[] { codexScriptable_C127 }
            };
            clip_C127 = clipC127;

            AmmoClipCodexScriptable clipCodexC127 = new AmmoClipCodexScriptable { name = "clip_C127(105)", ClipType = clip_C127 };
            clipCodexScriptable_C127 = clipCodexC127;

            /////////
            AmmoType tempHesh = new AmmoType();
            AmmoCodexScriptable default_hesh = Resources.FindObjectsOfTypeAll<AmmoCodexScriptable>()
                .Where(o => o.name == "ammo_DM512(105)").FirstOrDefault();
            CloneAmmo(default_hesh.AmmoType, tempHesh);
            type_L35 = tempHesh;
            
            type_L35.Name = "L35 HESH-T"; //copying DM512
            type_L35.Mass = 11.35f;          //DM512 is 20 (weight of the full cartridge instead of projectile?!)
            type_L35.TntEquivalentKg = 4.8f; //DM512 is 3.9

            AmmoCodexScriptable codexL35 = new AmmoCodexScriptable { name = "ammo_L35", AmmoType = type_L35 };
            codexScriptable_L35 = codexL35;

            visL35_go = GameObject.Instantiate<GameObject>(type_L35.VisualModel);
            AmmoStoredVisual vis_L35 = visL35_go.transform.GetComponent<AmmoStoredVisual>();
            vis_L35.AmmoType = type_L35;
            vis_L35.AmmoScriptable = codexScriptable_L35;
            type_L35.VisualModel = visL35_go;

            AmmoType.AmmoClip clipL35 = new AmmoType.AmmoClip { Capacity = 1, Name = "L35 HESH-T",
                MinimalPattern = new AmmoCodexScriptable[] { codexScriptable_L35 } };
            clip_L35 = clipL35;

            AmmoClipCodexScriptable clipCodexL35 = new AmmoClipCodexScriptable { name = "clip_L35(105)", ClipType = clip_L35 };
            clipCodexScriptable_L35 = clipCodexL35;

            /////////
            AmmoType temp120sabot = new AmmoType();
            AmmoCodexScriptable default_120sabot = Resources.FindObjectsOfTypeAll<AmmoCodexScriptable>()
                .Where(o => o.name == "ammo_DM23(120)").FirstOrDefault();
            CloneAmmo(default_120sabot.AmmoType, temp120sabot);
            type_DM53 = temp120sabot;

            type_DM53.Name = "DM53 APFSDS-T"; //copying DM23 (120)
            type_DM53.Mass = 5f;           //DM23 is 4.6
            type_DM53.MuzzleVelocity = 1670f; //DM23 is 1650
            type_DM53.RhaPenetration = 650;   //DM23 is 520

            AmmoCodexScriptable codex120sabot = new AmmoCodexScriptable { name = "ammo_DM53(120)", AmmoType = type_DM53 };
            codexScriptable_DM53 = codex120sabot;

            visDM53_go = GameObject.Instantiate<GameObject>(type_DM53.VisualModel);
            AmmoStoredVisual vis_DM53 = visDM53_go.transform.GetComponent<AmmoStoredVisual>();
            vis_DM53.AmmoType = type_DM53;
            vis_DM53.AmmoScriptable = codexScriptable_DM53;
            type_DM53.VisualModel = visDM53_go;

            AmmoType.AmmoClip clip120sabot = new AmmoType.AmmoClip
            {
                Capacity = 1,
                Name = "DM53 APFSDS-T",
                MinimalPattern = new AmmoCodexScriptable[] { codexScriptable_DM53 }
            };
            clip_DM53 = clip120sabot;

            AmmoClipCodexScriptable clipCodex120sabot = new AmmoClipCodexScriptable { name = "clip_DM53(120)", ClipType = clip_DM53 };
            clipCodexScriptable_DM53 = clipCodex120sabot;
        }

        public static void HistoricalC2(LoadoutManager loadout_manager)
        {
            loadout_manager.LoadedAmmoList.AmmoClips[0] = clipCodexScriptable_C127;
            loadout_manager.LoadedAmmoList.AmmoClips[1] = clipCodexScriptable_L35;

            if (loadout_manager.LoadedAmmoList.AmmoClips.Length == 3)
            {
                loadout_manager.LoadedAmmoList.AmmoClips[2] = null;
                AmmoClipCodexScriptable[] new_clips = new AmmoClipCodexScriptable[2];
                new_clips[0] = loadout_manager.LoadedAmmoList.AmmoClips[0];
                new_clips[1] = loadout_manager.LoadedAmmoList.AmmoClips[1];
                loadout_manager.LoadedAmmoList.AmmoClips = new_clips;

                loadout_manager._totalAmmoTypes = 2;
                loadout_manager._totalAmmoCount = 55;
                loadout_manager.TotalAmmoCounts = new int[] { 21, 34 };
            }

            ClearAndReload(loadout_manager);
        }
        public static void HistoricalC1(LoadoutManager loadout_manager)
        {
            string defaultSabot = loadout_manager.LoadedAmmoList.AmmoClips[0].name;            
            if (defaultSabot == "clip_DM23(105)") { loadout_manager.LoadedAmmoList.AmmoClips[0] = clipCodexScriptable_C76; }
            else { loadout_manager.LoadedAmmoList.AmmoClips[0] = clipCodexScriptable_C35; }            
            loadout_manager.LoadedAmmoList.AmmoClips[1] = clipCodexScriptable_L35;    

            if (loadout_manager.LoadedAmmoList.AmmoClips.Length == 3)
            {
                loadout_manager.LoadedAmmoList.AmmoClips[2] = null;
                AmmoClipCodexScriptable[] new_clips = new AmmoClipCodexScriptable[2];                
                new_clips[0] = loadout_manager.LoadedAmmoList.AmmoClips[0];
                new_clips[1] = loadout_manager.LoadedAmmoList.AmmoClips[1];
                loadout_manager.LoadedAmmoList.AmmoClips = new_clips;                

                loadout_manager._totalAmmoTypes = 2;
                loadout_manager._totalAmmoCount = 55;
                loadout_manager.TotalAmmoCounts = new int[] { 21, 34 };                
            }

            ClearAndReload(loadout_manager);
        }

        public static void Modern2A4(LoadoutManager loadout_manager)
        {
            loadout_manager.LoadedAmmoList.AmmoClips[0] = clipCodexScriptable_DM53;
            AmmoClipCodexScriptable[] new_clips = new AmmoClipCodexScriptable[2];
            new_clips[0] = loadout_manager.LoadedAmmoList.AmmoClips[0];
            new_clips[1] = loadout_manager.LoadedAmmoList.AmmoClips[1];
            loadout_manager.LoadedAmmoList.AmmoClips = new_clips;
            ClearAndReload(loadout_manager);
        }
        public static void AmericanLoad(LoadoutManager loadout_manager)
        {
            AmmoClipCodexScriptable[] clip_codex_scriptables = Resources.FindObjectsOfTypeAll<AmmoClipCodexScriptable>();
            AmmoClipCodexScriptable clip_codex_sabot = clip_codex_scriptables.Where(o => o.name == "clip_M774").FirstOrDefault();
            AmmoClipCodexScriptable clip_codex_heat = clip_codex_scriptables.Where(o => o.name == "clip_M456A2").FirstOrDefault();            
            loadout_manager.LoadedAmmoList.AmmoClips[0] = clip_codex_sabot;
            loadout_manager.LoadedAmmoList.AmmoClips[1] = clip_codex_heat;
            if (loadout_manager.LoadedAmmoList.AmmoClips.Length == 3)
            {
                loadout_manager.LoadedAmmoList.AmmoClips[2] = null;
                AmmoClipCodexScriptable[] new_clips = new AmmoClipCodexScriptable[2];
                new_clips[0] = loadout_manager.LoadedAmmoList.AmmoClips[0];
                new_clips[1] = loadout_manager.LoadedAmmoList.AmmoClips[1];
                loadout_manager.LoadedAmmoList.AmmoClips = new_clips;

                loadout_manager._totalAmmoTypes = 2;
                loadout_manager._totalAmmoCount = 55;
                loadout_manager.TotalAmmoCounts = new int[] { 21, 34 };                
            }

            ClearAndReload(loadout_manager);
        }
    }
}
