using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;
using MelonLoader;
using GHPC.Weapons;
using GHPC.Weaponry;


namespace CanadianLeopards
{
    public class AmmoSwaps
    {
        public static void HistoricalLoad(WeaponSystem maingun, LoadoutManager loadout_manager, bool mute_logger)
        {
            WeaponSystem gun = maingun;
            LoadoutManager lm = loadout_manager;
            AmmoCodexScriptable[] codex_scriptables = Resources.FindObjectsOfTypeAll<AmmoCodexScriptable>();
            AmmoCodexScriptable ammo_codex_hesh = codex_scriptables.Where(o => o.name == "ammo_DM512(105)").FirstOrDefault();
            AmmoType ammo_hesh = ammo_codex_hesh.AmmoType;            
            AmmoType.AmmoClip clip_hesh = new AmmoType.AmmoClip();
            AmmoClipCodexScriptable clip_codex_hesh = new AmmoClipCodexScriptable();
            clip_hesh.Capacity = 1;
            //clip_hesh.Name = "DM512 HESH-T";
            clip_hesh.Name = "L35 HESH-T";
            ammo_hesh.Name = "L35 HESH-T";
            clip_hesh.MinimalPattern = new AmmoCodexScriptable[] { ammo_codex_hesh };
            clip_codex_hesh.name = "clip_DM512(105)";
            clip_codex_hesh.ClipType = clip_hesh;            
            loadout_manager.LoadedAmmoList.AmmoClips[1] = clip_codex_hesh;

            AmmoType.AmmoClip clip_sabot = loadout_manager.LoadedAmmoList.AmmoClips[0].ClipType;
            AmmoCodexScriptable codex_sabot = clip_sabot.MinimalPattern[0];
            AmmoType ammo_sabot = codex_sabot.AmmoType;            
            if (clip_sabot.Name == "DM23 APFSDS-T") {
                if (!mute_logger) { MelonLogger.Msg("DM-23 being renamed"); }
                ammo_sabot.Name = "C76 APFSDS-T";
                clip_sabot.Name = "C76 APFSDS-T";
                
            }
            if (clip_sabot.Name == "DM13 APDS-T") { 
                if (!mute_logger) { MelonLogger.Msg("DM-13 being renamed"); }
                clip_sabot.Name = "C35 APDS-T";
                ammo_sabot.Name = "C35 APDS-T";
            }
            loadout_manager.LoadedAmmoList.AmmoClips[0].ClipType = clip_sabot;

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
            MethodInfo removeVis = typeof(GHPC.Weapons.AmmoRack).GetMethod("RemoveAmmoVisualFromSlot", BindingFlags.Instance | BindingFlags.NonPublic);
            PropertyInfo stored_clips = typeof(GHPC.Weapons.AmmoRack).GetProperty("StoredClips");
            int rack_count = loadout_manager.RackLoadouts.Length;
            for (int i = 0; i < rack_count; i++)
            {
                var rack = loadout_manager.RackLoadouts[i].Rack;
                stored_clips.SetValue(rack, new List<AmmoType.AmmoClip>());
                rack.SlotIndicesByAmmoType = new Dictionary<AmmoType, List<byte>>();
                foreach (Transform transform in rack.VisualSlots)
                {
                    AmmoStoredVisual vis = transform.GetComponentInChildren<AmmoStoredVisual>();
                    if (vis != null && vis.AmmoType != null)
                    {
                        removeVis.Invoke(rack, new object[] { transform });
                    }
                }                
            }
            maingun.Feed.AmmoTypeInBreech = null;            
            loadout_manager.SpawnCurrentLoadout();            
            maingun.Feed.Start();            
            loadout_manager.RegisterAllBallistics();
            if (!mute_logger) { MelonLogger.Msg("Swapped out HEAT for HESH"); }            
        }
        public static void AmericanLoad(WeaponSystem maingun, LoadoutManager loadout_manager, bool mute_logger)
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
            MethodInfo removeVis = typeof(GHPC.Weapons.AmmoRack).GetMethod("RemoveAmmoVisualFromSlot", BindingFlags.Instance | BindingFlags.NonPublic);
            PropertyInfo stored_clips = typeof(GHPC.Weapons.AmmoRack).GetProperty("StoredClips");
            int rack_count = loadout_manager.RackLoadouts.Length;
            for (int i = 0; i < rack_count; i++)
            {
                var rack = loadout_manager.RackLoadouts[i].Rack;
                stored_clips.SetValue(rack, new List<AmmoType.AmmoClip>());
                rack.SlotIndicesByAmmoType = new Dictionary<AmmoType, List<byte>>();
                foreach (Transform transform in rack.VisualSlots)
                {
                    AmmoStoredVisual vis = transform.GetComponentInChildren<AmmoStoredVisual>();
                    if (vis != null && vis.AmmoType != null)
                    {
                        removeVis.Invoke(rack, new object[] { transform });
                    }
                }                
            }
            maingun.Feed.AmmoTypeInBreech = null;            
            loadout_manager.SpawnCurrentLoadout();            
            maingun.Feed.Start();            
            loadout_manager.RegisterAllBallistics();
            if (!mute_logger) { MelonLogger.Msg("Loaded M774 sabot and M456A2 heat"); }
        }
    }
}
