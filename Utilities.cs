using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;
using MelonLoader;
using MelonLoader.Utils;
using GHPC.Mission;

namespace CanadianLeopards
{
    public class Utilities
    {
        public static bool mute_logger;

        public static GameObject american_crew_voice;
        public static GHPC.Weapons.AmmoFeed cal50;
        public static GameObject jerry_can;
        public static Reticle.ReticleMesh.CachedReticle marder_crosshair;
        public static GameObject emes15;
        public static GameObject mg_mount;
        public static GameObject leo2a1_hull;
        public static GameObject leo2a1_turret;
        public static GameObject leo1a5_turret;
        public static GameObject bustle_baskets;
        public static GameObject basket_colliders;
        public static GameObject new_net;
        public static GameObject emes18_model;
        public static GameObject emes18_aar;
        public static GameObject fn_mag;
        public static GameObject fn_mag_aar;
        public static GameObject new_gps_aar;
        public static GameObject c1a1_prefab;
        public static Texture2D blood_mask;
        public static GameObject dressing_prefab;
        public static Texture2D maple;
        public static Texture2D maple_black;
        public static Texture2D A1_base;
        public static Texture2D carc_green;
        public static Texture2D A3_base;
        public static Texture2D callsigns;
        public static Texture2D callsigns_black;
        public static Texture2D A1_camomask;
        public static Texture2D A3_camomask;
        public static Texture2D tac;
        public static Texture2D tac_generic;
        public static Texture2D panther;
        public static Texture2D mlc46;
        public static Texture2D mlc46_black;
        public static Texture2D canInf;
        public static Texture2D canInf_nm;
        public static Texture2D canInf_sm;
        public static Texture2D apc;
        public static Texture2D flag;
        public static Texture2D mlcAPC;
        public static Texture2D tacAPC;
        public static Texture2D L2_base;
        public static Texture2D L2_camomask;
        public static Texture2D L2_tan_mask;
        public static Texture2D L2_tan;
        public static Texture2D tan_net;
        public static Texture2D mlc60;
        public static Texture2D mlc60_black;
        public static Texture2D chevron_white;
        public static Texture2D chevron_gold;
        public static Texture2D chevron_black;


        public static void FetchResources()
        {
            UnitSpawner spawner = Object.FindAnyObjectByType<UnitSpawner>();
            if (spawner == null)
            {
                MelonLogger.Warning("Could not find a UnitSpawner in scene!");
                goto Assets;
            }
            
            if (american_crew_voice == null || cal50 == null || jerry_can == null)
            {                
                AssetReference prefab = spawner.PrefabLookup.GetPrefab("M1");
                GameObject abrams = Addressables.LoadAssetAsync<GameObject>(prefab).WaitForCompletion();
                cal50 = abrams.transform.Find("Cupola Scripts/12.7mm Machine Gun M48").GetComponent<GHPC.Weapons.AmmoFeed>();
                american_crew_voice = abrams.GetComponentInChildren<GHPC.Effects.Voices.CrewVoiceHandler>().gameObject;
                jerry_can = abrams.transform.Find("Decorations Manager/turret decorations parent/us_20L_water").gameObject;
                Utilities.Log("Jerry can, 12.7 ammo and US voices fetched from prefab");
                prefab.ReleaseAsset();
            }

            if (marder_crosshair.mesh == null)
            {                
                AssetReference prefab = spawner.PrefabLookup.GetPrefab("MARDERA1PLUS");
                GameObject marder = Addressables.LoadAssetAsync<GameObject>(prefab).WaitForCompletion();
                Reticle.ReticleMesh prefabReticle = marder.transform.Find("FCS and sights/PERI Z11/NFOV reticle").GetComponent<Reticle.ReticleMesh>();
                prefabReticle.Load();
                marder_crosshair = prefabReticle.reticle;
                Utilities.Log("TZF graticule fetched from prefab");
                prefab.ReleaseAsset();
            }

            if (emes15 == null || leo2a1_turret == null || leo2a1_hull == null)
            {                
                AssetReference prefab = spawner.PrefabLookup.GetPrefab("LEO2A3");
                GameObject leo2 = Addressables.LoadAssetAsync<GameObject>(prefab).WaitForCompletion();
                emes15 = leo2.transform.Find("---TURRET SCRIPTS---/GPS").gameObject;
                leo2a1_turret = leo2.transform.Find("LEO2_mesh/turret A1").gameObject;
                leo2a1_hull = leo2.transform.Find("LEO2_mesh/hull 2A1 late").gameObject;
                Utilities.Log("EMES-15, 2A1 turret and hull fetched from prefab");
                prefab.ReleaseAsset();
            }

            Assets:
            if (mg_mount == null)
            {
                var lafette_bundle = AssetBundle.LoadFromFile(Path.Combine(MelonEnvironment.ModsDirectory + "/CanadianLeopards", "lafette"));
                if (lafette_bundle == null) MelonLogger.Error("Could not load Leo 1A5 asset bundle");
                mg_mount = lafette_bundle.LoadAsset("assets/lafette_detached.fbx") as GameObject;
                Utilities.Log("lafette mg mount read from disk");
                lafette_bundle.Unload(false);
            }

            if (leo1a5_turret == null)
            {
                var leo1a5_bundle = AssetBundle.LoadFromFile(Path.Combine(MelonEnvironment.ModsDirectory + "/CanadianLeopards", "leo1a5"));
                if (leo1a5_bundle == null) MelonLogger.Error("Could not load Leo 1A5 asset bundle");
                leo1a5_turret = leo1a5_bundle.LoadAsset("assets/Leo1A5_turret.fbx") as GameObject;
                Utilities.Log("1A5-style turret read from disk");
                leo1a5_bundle.Unload(false);
            }

            if (bustle_baskets == null || basket_colliders == null || new_net == null)
            {
                var bustle_bundle = AssetBundle.LoadFromFile(Path.Combine(MelonEnvironment.ModsDirectory + "/CanadianLeopards", "bustle_baskets"));
                if (bustle_bundle == null) MelonLogger.Error("Could not load Leo2 bustle asset bundle");
                bustle_baskets = bustle_bundle.LoadAsset("assets/Leo2_baskets.fbx") as GameObject;
                new_net = bustle_bundle.LoadAsset("assets/new_turret_net.obj") as GameObject;
                basket_colliders = bustle_bundle.LoadAsset("assets/baskets_collider.obj") as GameObject;
                Utilities.Log("Leo2 bustle mods read from disk");
                bustle_bundle.Unload(false);
            }

            if (emes18_model == null || emes18_aar == null)
            {
                var emes18_bundle = AssetBundle.LoadFromFile(Path.Combine(MelonEnvironment.ModsDirectory + "/CanadianLeopards", "emes-18"));
                if (emes18_bundle == null) MelonLogger.Error("Could not load EMES-18 asset bundle");
                emes18_model = emes18_bundle.LoadAsset("assets/emes-18.obj") as GameObject;
                emes18_aar = emes18_bundle.LoadAsset("assets/emes-18_aar.obj") as GameObject;
                Utilities.Log("EMES-18 doghouse read from disk");
                emes18_bundle.Unload(false);
            }

            if (fn_mag == null) { 
                var fnMag_bundle = AssetBundle.LoadFromFile(Path.Combine(MelonEnvironment.ModsDirectory + "/CanadianLeopards", "fn_mag"));
                if (fnMag_bundle == null) MelonLogger.Error("Could not load FN MAG asset bundle");
                fn_mag = fnMag_bundle.LoadAsset("assets/FN_MAG.obj") as GameObject;
                Utilities.Log("FN MAG read from disk");
                fnMag_bundle.Unload(false);
            }

            if (fn_mag_aar == null)
            {
                var fnMagAAR_bundle = AssetBundle.LoadFromFile(Path.Combine(MelonEnvironment.ModsDirectory + "/CanadianLeopards", "fn_mag_aar"));
                if (fnMagAAR_bundle == null) MelonLogger.Error("Could not load FN MAG AAR asset bundle");
                fn_mag_aar = fnMagAAR_bundle.LoadAsset("assets/FN_MAG_aar.obj") as GameObject;
                new_gps_aar = fnMagAAR_bundle.LoadAsset("assets/new_gps_aar.obj") as GameObject;
                Utilities.Log("AAR assets read from disk");
                fnMagAAR_bundle.Unload(false);
            }

            if (c1a1_prefab == null || blood_mask == null)
            {
                var c1a1_bundle = AssetBundle.LoadFromFile(Path.Combine(MelonEnvironment.ModsDirectory + "/CanadianLeopards", "c1a1"));
                if (c1a1_bundle == null) MelonLogger.Error("Could not load C1A1 FAL asset bundle");
                c1a1_prefab = c1a1_bundle.LoadAsset("assets/C1A1.obj") as GameObject;
                blood_mask = c1a1_bundle.LoadAsset<Texture2D>("assets/green.png");
                Utilities.Log("FN FAL asset read from disk");
                c1a1_bundle.Unload(false);
            }

            if (dressing_prefab == null)
            {
                var dressing_bundle = AssetBundle.LoadFromFile(Path.Combine(MelonEnvironment.ModsDirectory + "/CanadianLeopards", "dressing"));
                if (dressing_bundle == null) MelonLogger.Error("Could not load field-dressing asset bundle");
                dressing_prefab = dressing_bundle.LoadAsset("assets/field-dressing.obj") as GameObject;
                Utilities.Log("Field dressing asset read from disk");
                dressing_bundle.Unload(false);
            }

            if (maple == null || A1_base == null || A3_base == null) //arbitrary number of checks
            {   
                var texture_bundle = AssetBundle.LoadFromFile(Path.Combine(MelonEnvironment.ModsDirectory + "/CanadianLeopards", "textures"));
                if (texture_bundle == null) MelonLogger.Error("Could not load texture asset bundle!");
                maple = texture_bundle.LoadAsset<Texture2D>("assets/tex/maple.png");
                maple_black = texture_bundle.LoadAsset<Texture2D>("assets/tex/maple_black.png");
                A1_base = texture_bundle.LoadAsset<Texture2D>("assets/tex/1A1_base.png");
                carc_green = texture_bundle.LoadAsset<Texture2D>("assets/tex/green.png");
                A3_base = texture_bundle.LoadAsset<Texture2D>("assets/tex/1A3_base.png");
                callsigns = texture_bundle.LoadAsset<Texture2D>("assets/tex/callsigns.png");
                callsigns_black = texture_bundle.LoadAsset<Texture2D>("assets/tex/callsigns_black.png");
                A1_camomask = texture_bundle.LoadAsset<Texture2D>("assets/tex/A1_mask.png");
                A3_camomask = texture_bundle.LoadAsset<Texture2D>("assets/tex/A3_mask.png");
                tac = texture_bundle.LoadAsset<Texture2D>("assets/tex/tac.png");
                tac_generic = texture_bundle.LoadAsset<Texture2D>("assets/tex/tac_generic.png");
                panther = texture_bundle.LoadAsset<Texture2D>("assets/tex/CAT87.png");
                mlc46 = texture_bundle.LoadAsset<Texture2D>("assets/tex/mlc46.png");
                mlc46_black = texture_bundle.LoadAsset<Texture2D>("assets/tex/mlc46_black.png");
                canInf = texture_bundle.LoadAsset<Texture2D>("assets/tex/can_inf.png");
                canInf_nm = texture_bundle.LoadAsset<Texture2D>("assets/tex/can_inf_nm.png");
                canInf_sm = texture_bundle.LoadAsset<Texture2D>("assets/tex/can_inf_sm.png");
                apc = texture_bundle.LoadAsset<Texture2D>("assets/tex/apc.png");
                flag = texture_bundle.LoadAsset<Texture2D>("assets/tex/flag.png");
                mlcAPC = texture_bundle.LoadAsset<Texture2D>("assets/tex/mlc_apc.png");
                tacAPC = texture_bundle.LoadAsset<Texture2D>("assets/tex/tac_apc.png");
                L2_base = texture_bundle.LoadAsset<Texture2D>("assets/tex/L2_base.png");
                L2_camomask = texture_bundle.LoadAsset<Texture2D>("assets/tex/L2_mask.png");
                L2_tan_mask = texture_bundle.LoadAsset<Texture2D>("assets/tex/L2_tan_mask.png");
                L2_tan = texture_bundle.LoadAsset<Texture2D>("assets/tex/Leo2_tan.png");
                tan_net = texture_bundle.LoadAsset<Texture2D>("assets/tex/tan_net.png");
                mlc60 = texture_bundle.LoadAsset<Texture2D>("assets/tex/mlc60.png");
                mlc60_black = texture_bundle.LoadAsset<Texture2D>("assets/tex/mlc60_black.png");
                chevron_white = texture_bundle.LoadAsset<Texture2D>("assets/tex/chevron_white.png");
                chevron_gold = texture_bundle.LoadAsset<Texture2D>("assets/tex/chevron_gold.png");
                chevron_black = texture_bundle.LoadAsset<Texture2D>("assets/tex/chevron_black.png");
                Utilities.Log("Texture files loaded from disk");
                texture_bundle.Unload(false);
            }
        }
        public static Texture2D FetchTex(int x, int y, string path)
        {
            Texture2D temp = new Texture2D(x, y);
            try
            {
                byte[] data = File.ReadAllBytes(path);
                temp.LoadImage(data);
            }
            catch (FileNotFoundException e) { MelonLogger.Error(e); }
            return temp;
        }

        public static Texture2D FetchTex(int x, int y, string path, bool linear)
        {
            Texture2D temp = new Texture2D(x, y, TextureFormat.DXT5, true, linear);
            try
            {
                byte[] data = File.ReadAllBytes(path);
                temp.LoadImage(data);
            }
            catch (FileNotFoundException e) { MelonLogger.Error(e); }
            return temp;
        }

        public static void NewQuad(GameObject go, Material mat, Texture2D tex)
        {
            MeshFilter filter = go.AddComponent<MeshFilter>();
            MeshRenderer render = go.AddComponent<MeshRenderer>();
            filter.mesh = new Mesh
            {
                vertices = new Vector3[] { new Vector3(1f, 0, 1f), new Vector3(1f, 0, -1f), new Vector3(-1f, 0, 1f), new Vector3(-1f, 0, -1f) },
                uv = new Vector2[] { new Vector2(1, 1), new Vector2(1, 0), new Vector2(0, 1), new Vector2(0, 0) },
                triangles = new int[] { 0, 1, 2, 2, 1, 3 }
            };
            filter.mesh.RecalculateNormals();
            render.material = mat;
            if (mat.shader.name == "ghpc_roundel") { render.material.SetTexture("_colour", tex); }
            else if (mat.shader.name == "GHPC/VehicleShader") { render.material.SetTexture("_Albedo", tex); }
            else render.material.mainTexture = tex;
        }
        public static void Log(string text)
        {
            if (!mute_logger) { MelonLogger.Msg(text); }
        }
    }
}
