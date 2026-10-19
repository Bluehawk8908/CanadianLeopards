using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;
using MelonLoader;
using MelonLoader.Utils;
using GHPC;
using GHPC.State;
using GHPC.Mission;
using GHPC.Vehicle;
using GHPC.Weapons;
using Reticle;
using GHPC.Effects.Voices;
using GHPC.Infantry;

namespace CanadianLeopards
{
    public class CanLepConverted : MonoBehaviour
    {
        void Awake()
        {
            enabled = false;
        }
    }

    public class GrafenGizmo : MonoBehaviour
    {
        static bool done;
        static bool spawned;
        public GameObject american_crew_voice;
        public ReticleMesh.CachedReticle marder_crosshair;
        public GameObject fn_mag;
        public Texture2D A3_base;
        public Texture2D A3_camomask;
        public Texture2D maple;
        public Texture2D callsigns;
        public Texture2D tac;
        public Texture2D mlc;
        
        void LateUpdate()
        {
            if (done) return;
            if (!spawned) { CanadianLeopardsClass.ShowcaseExtras(); spawned = true; }  
            if (GHPC.World.SceneUnitsManager.Instance.AllUnitsInScene.Count == 40) {
                Vehicle[] newList = GameObject.FindObjectsByType<Vehicle>(FindObjectsSortMode.None);                
                foreach (var vehicle in newList)
                {
                    if (vehicle.gameObject.GetComponent<CanLepConverted>() != null) { continue; }
                    if (vehicle.UniqueName == "LEO1A3A3" || vehicle.UniqueName == "LEO1A3")
                    { Leo1A3.Convert(vehicle, american_crew_voice, marder_crosshair, fn_mag, A3_base, A3_camomask, maple, callsigns, tac, mlc); }
                }
                done = true;
            }
        }
    }
    public class CanadianLeopardsClass : MelonMod
    {
        public static GameObject gameManager;
        public static MelonPreferences_Entry<string> ammo_loadout;
        public static MelonPreferences_Entry<bool> carc_green;
        public static MelonPreferences_Entry<bool> no_threecolour;
        public static MelonPreferences_Entry<bool> decals_outlined;
        public static MelonPreferences_Entry<bool> additional_decals;
        public static MelonPreferences_Entry<bool> showcase_extras;
        public static MelonPreferences_Entry<bool> exclude_early_1A1;
        public static MelonPreferences_Entry<bool> exclude_late_1A1;
        public static MelonPreferences_Entry<bool> exclude_early_1A3;
        public static MelonPreferences_Entry<bool> exclude_late_1A3;
        public static MelonPreferences_Entry<bool> exclude_1A4;
        public static MelonPreferences_Entry<bool> convert_infantry;
        public static MelonPreferences_Entry<bool> mute_logger;

        public static GameObject american_crew_voice = null;                
        public static AmmoFeed cal50 = null;        
        public static ReticleMesh.CachedReticle marder_crosshair;
        public static GameObject emes15;
        public static bool graf;
        
        public override void OnInitializeMelon()
        {
            MelonPreferences_Category cfg = MelonPreferences.CreateCategory("Canadian Leopards");
            ammo_loadout = cfg.CreateEntry<string>("Customize ammo loadout", "historical");
            ammo_loadout.Comment = "'historical' for DM-23/13 and HESH, 'American' for M774 and HEAT, 'German' to keep mission defaults";

            no_threecolour = cfg.CreateEntry<bool>("Force single colour paint schemes", false);
            no_threecolour.Comment = "Prevents C1s from appearing in NATO three-colour camo";

            carc_green = cfg.CreateEntry<bool>("NATO/CARC Green", true);
            carc_green.Comment = "Replaces the default German Gelboliv ('yellow-olive') for NATO CARC, a brighter shade of green.";

            decals_outlined = cfg.CreateEntry<bool>("Decals outlined in white", true);
            decals_outlined.Comment = "Turret numbers and maple leaves have white borders when true; plain black when false.";

            additional_decals = cfg.CreateEntry<bool>("Additional decals", true);
            additional_decals.Comment = "Adds tactical unit symbol and MLC number to the hull.";

            showcase_extras = cfg.CreateEntry<bool>("Add 1A3s to Showcase", true);
            showcase_extras.Comment = "Adds 1A3-based Leopard C1s to the Grafenwoehr Showcase.";

            exclude_early_1A1 = cfg.CreateEntry<bool>("Exclude early 1A1s from conversion", false);
            exclude_early_1A1.Comment = ("1A1A1 and 'A2 will remain in German service and retain old features");

            exclude_late_1A1 = cfg.CreateEntry<bool>("Exclude late 1A1s from conversion", false);
            exclude_late_1A1.Comment = ("1A1A3 and 'A4 will remain in German service and retain old features");

            exclude_early_1A3 = cfg.CreateEntry<bool>("Exclude early 1A3s from conversion", false);
            exclude_early_1A3.Comment = ("1A3 and 1A3A1 will remain in German service and retain old features");

            exclude_late_1A3 = cfg.CreateEntry<bool>("Exclude late 1A3s from conversion", false);
            exclude_late_1A3.Comment = ("1A3A3 and '3A4 will remain in German service and retain old features");

            exclude_1A4 = cfg.CreateEntry<bool>("Exclude 1A4 from conversion", true);
            exclude_1A4.Comment = "The Leopard 1A4 will remain in German service and retain all its unique features.";

            convert_infantry = cfg.CreateEntry<bool>("Convert FRG infantry and M113s", true);
            convert_infantry.Comment = "West German infantry and M113Gs will be converted to resemble Canadian mechanized infantry.";

            mute_logger = cfg.CreateEntry<bool>("Mute log messages", false);
            mute_logger.Comment = "Mutes log messages in the MelonLoader console.";
            Utilities.mute_logger = mute_logger.Value;
        }
        
        public static void ShowcaseExtras()
        {
            var prefabLookups = UnitSpawner.Instance.PrefabLookup;
            AssetReference prefab1A3 = prefabLookups.GetPrefab("LEO1A3");
            AssetReference prefab1A3A3 = prefabLookups.GetPrefab("LEO1A3A3");            
            GameObject Leo1A3 = Addressables.LoadAssetAsync<GameObject>(prefab1A3).WaitForCompletion();
            GameObject Leo1A3A3 = Addressables.LoadAssetAsync<GameObject>(prefab1A3A3).WaitForCompletion();

            UnitSpawner.Instance.SpawnUnit(Leo1A3, new GHPC.Mission.Data.UnitMetaData() 
                { Allegiance = Faction.Neutral, 
                  Position = new Vector3(1423.7f, 26.416f, 1433.9f), 
                  Rotation = Quaternion.Euler(0.573f, 233.87f, 358.75f)});
            UnitSpawner.Instance.SpawnUnit(Leo1A3A3, new GHPC.Mission.Data.UnitMetaData()
                { Allegiance = Faction.Neutral, 
                  Position = new Vector3(1399.87f, 25.7f, 1436.75f), 
                  Rotation = Quaternion.Euler(0.925f, 230.98f, 358.284f) });
            
            Utilities.Log("Spawned additional vehicles on the range.");
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            if (sceneName == "MainMenu2_Scene" || sceneName == "t64_menu" || sceneName == "MainMenu2-1_Scene" || sceneName == "leo2_menu") 
            {
                graf = false;
                return;               
            }

            gameManager = GameObject.Find("_APP_GHPC_");
            if (gameManager == null) { return; }

            if (sceneName == "TR01_showcase" && showcase_extras.Value) { graf = true; }
            StateController.RunOrDefer(GameState.GameReady, new GameStateEventHandler(Conversion), GameStatePriority.Medium);            
        }

        public IEnumerator Conversion(GameState _)
        {
            //if (showcase_extras.Value && !PlayerInput.Instance.IsMapActive) PlayerInput.Instance.SetPlayerUnit(PlayerInput.Instance.CurrentPlayerUnit); //resets camera            

            //PREFABS
            if (american_crew_voice == null || cal50 == null)
            {
                var prefabLookups = Object.FindAnyObjectByType<UnitSpawner>().PrefabLookup;
                AssetReference prefab = prefabLookups.GetPrefab("M1");
                GameObject abrams = Addressables.LoadAssetAsync<GameObject>(prefab).WaitForCompletion();
                cal50 = abrams.transform.Find("Cupola Scripts/12.7mm Machine Gun M48").GetComponent<AmmoFeed>();
                american_crew_voice = abrams.GetComponentInChildren<CrewVoiceHandler>().gameObject;
                Utilities.Log("12.7 ammo and US voices fetched from prefab");
            }

            if (marder_crosshair.mesh == null)
            {
                var prefabLookups = Object.FindAnyObjectByType<UnitSpawner>().PrefabLookup;
                AssetReference prefab = prefabLookups.GetPrefab("MARDERA1PLUS");
                GameObject marder = Addressables.LoadAssetAsync<GameObject>(prefab).WaitForCompletion();
                ReticleMesh prefabReticle = marder.transform.Find("FCS and sights/PERI Z11/NFOV reticle").GetComponent<ReticleMesh>();
                prefabReticle.Load();
                marder_crosshair = prefabReticle.reticle;
                Utilities.Log("TZF graticule fetched from prefab");
            }

            if (emes15 == null)
            {
                var prefabLookups = Object.FindAnyObjectByType<UnitSpawner>().PrefabLookup;
                AssetReference prefab = prefabLookups.GetPrefab("LEO2A3");
                GameObject leo2 = Addressables.LoadAssetAsync<GameObject>(prefab).WaitForCompletion();                
                emes15 = leo2.transform.Find("---TURRET SCRIPTS---/GPS").gameObject;
                Utilities.Log("EMES-15 fetched from prefab");
            }

            //EXTERNAL ASSETS
            var fnMag_bundle = AssetBundle.LoadFromFile(Path.Combine(MelonEnvironment.ModsDirectory + "/CanadianLeopards", "fn_mag"));
            if (fnMag_bundle == null) MelonLogger.Error("Could not load FN MAG asset bundle");
            GameObject fn_mag = fnMag_bundle.LoadAsset("assets/FN_MAG.obj") as GameObject;            
            fnMag_bundle.Unload(false);

            var c1a1_bundle = AssetBundle.LoadFromFile(Path.Combine(MelonEnvironment.ModsDirectory + "/CanadianLeopards", "c1a1"));
            if (c1a1_bundle == null) MelonLogger.Error("Could not load C1A1 FAL asset bundle");
            GameObject c1a1_prefab = c1a1_bundle.LoadAsset("assets/C1A1.obj") as GameObject;
            Texture2D green = c1a1_bundle.LoadAsset<Texture2D>("assets/green.png");
            c1a1_bundle.Unload(false);

            var dressing_bundle = AssetBundle.LoadFromFile(Path.Combine(MelonEnvironment.ModsDirectory + "/CanadianLeopards", "dressing"));
            if (dressing_bundle == null) MelonLogger.Error("Could not load field-dressing asset bundle");
            GameObject dressing_prefab = dressing_bundle.LoadAsset("assets/field-dressing.obj") as GameObject;
            dressing_bundle.Unload(false);

            //TEXTURES
            string maplePath = decals_outlined.Value ? "Mods/CanadianLeopards/maple.png" : "Mods/CanadianLeopards/maple_black.png";
            Texture2D maple = Utilities.FetchTex(128, 128, maplePath);
            string A1_basePath = carc_green.Value ? "Mods/CanadianLeopards/green.png" : "Mods/CanadianLeopards/1A1_base.png";
            Texture2D A1_base = Utilities.FetchTex(2048, 2048, A1_basePath);
            Texture2D A3_base = Utilities.FetchTex(2048, 2048, "Mods/CanadianLeopards/1A3_base.png"); 
            string callsignsPath = decals_outlined.Value ? "Mods/CanadianLeopards/callsigns.png": "Mods/CanadianLeopards/callsigns_black.png";            
            Texture2D callsigns = Utilities.FetchTex(512, 64, callsignsPath);             
            Texture2D A1_camomask = Utilities.FetchTex(2048, 2048, "Mods/CanadianLeopards/A1_mask.png");            
            Texture2D A3_camomask = Utilities.FetchTex(2048, 2048, "Mods/CanadianLeopards/A3_mask.png");
            Texture2D tac = Utilities.FetchTex(128, 98, "Mods/CanadianLeopards/tac.png");            
            string mlcPath = decals_outlined.Value ? "Mods/CanadianLeopards/mlc.png": "Mods/CanadianLeopards/mlc_black.png";
            Texture2D mlc = Utilities.FetchTex(128, 128, mlcPath);            
            Texture2D canInf = Utilities.FetchTex(1024, 1024, "Mods/CanadianLeopards/can_inf.png");            
            Texture2D canInf_nm = Utilities.FetchTex(1024, 1024, "Mods/CanadianLeopards/can_inf_nm.png", true);
            Texture2D canInf_sm = Utilities.FetchTex(1024, 1024, "Mods/CanadianLeopards/can_inf_sm.png", true);            
            Texture2D apc = Utilities.FetchTex(2048, 2048, "Mods/CanadianLeopards/apc.png"); 
            Texture2D flag = Utilities.FetchTex(196, 98, "Mods/CanadianLeopards/flag.png");
            Texture2D mlcAPC = Utilities.FetchTex(128, 128, "Mods/CanadianLeopards/mlc_apc.png");
            Texture2D tacAPC = Utilities.FetchTex(148, 88, "Mods/CanadianLeopards/tac_apc.png");
            Texture2D callsignsAPC = (!decals_outlined.Value) ? callsigns : Utilities.FetchTex(512, 64, "Mods/CanadianLeopards/callsigns_black.png");
            Texture2D mapleAPC = (!decals_outlined.Value) ? maple : Utilities.FetchTex(128, 128, "Mods/CanadianLeopards/maple_black.png");            

            //INFANTRY
            if (convert_infantry.Value) {                
                InfantryUnit[] troops = GameObject.FindObjectsByType<InfantryUnit>(FindObjectsSortMode.None);
                
                foreach (var troop in troops)
                {
                    if (!troop.name.StartsWith("BW Feldanzug")) { continue; }
                    if (troop.gameObject.GetComponent<CanLepConverted>() != null) { continue; }
                    
                    SkinnedMeshRenderer dress = troop.transform.Find("Troop Base/BLU_FAZ63_OLIVE/dress").GetComponent<SkinnedMeshRenderer>();
                    SkinnedMeshRenderer accoutrements = troop.transform.Find("Troop Base/BLU_FAZ63_OLIVE/accoutrements").GetComponent<SkinnedMeshRenderer>();
                    SkinnedMeshRenderer helmet = troop.transform.Find("Troop Base/BLU_FAZ63_OLIVE/helmet").GetComponent<SkinnedMeshRenderer>();
                    SkinnedMeshRenderer webbing = troop.transform.Find("Troop Base/BLU_FAZ63_OLIVE/webbing").GetComponent<SkinnedMeshRenderer>();
                    Material canInf_mat = dress.material; 
                    canInf_mat.SetTexture("_Albedo", canInf);
                    canInf_mat.SetTexture("_Normal", canInf_nm);
                    canInf_mat.SetTexture("_Smoothness", canInf_sm);
                    canInf_mat.SetTexture("_regions", green);
                    accoutrements.material = canInf_mat;                 
                    helmet.material = canInf_mat;                                                         
                    webbing.material = canInf_mat;
                    
                    AarVisual aarVis = troop.transform.Find("Troop Base").GetComponent<AarVisual>();
                    aarVis.OriginalMaterials[dress] = new System.Collections.Generic.List<Material> { dress.material };
                    aarVis.OriginalMaterials[accoutrements] = new System.Collections.Generic.List<Material> { accoutrements.material };
                    aarVis.OriginalMaterials[helmet] = new System.Collections.Generic.List<Material> { helmet.material };
                    aarVis.OriginalMaterials[webbing] = new System.Collections.Generic.List<Material> { webbing.material };
                    
                    GameObject default_rifle = troop.transform.Find("Troop Base/TRP_SKELETON/weaponmain/G3A3").gameObject;
                    GameObject c1a1_rifle = GameObject.Instantiate(c1a1_prefab, default_rifle.transform); 
                    c1a1_rifle.transform.localPosition = new Vector3(0f, 0.05f, 0.22f);
                    c1a1_rifle.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                    c1a1_rifle.transform.localScale = new Vector3(0.51f, 0.51f, 0.51f);                    
                    c1a1_rifle.transform.Find("default").GetComponent<MeshRenderer>().material.color = new Color(0.6604f, 0.6604f, 0.6604f, 1);
                    
                    AarVisual weapon_AAR = troop.transform.Find("Troop Base/TRP_SKELETON/weaponmain/G3A3").GetComponent<AarVisual>();
                    weapon_AAR._renderers = new System.Collections.Generic.List<Renderer> { c1a1_rifle.transform.Find("default").GetComponent<MeshRenderer>() };
                    weapon_AAR.OriginalMaterials = new System.Collections.Generic.Dictionary<Renderer, System.Collections.Generic.List<Material>>
                        { [c1a1_rifle.transform.Find("default").GetComponent<MeshRenderer>()] = new System.Collections.Generic.List<Material> 
                            { c1a1_rifle.transform.Find("default").GetComponent<MeshRenderer>().material } };
                    
                    troop.transform.Find("Troop Base/TRP_SKELETON/weaponmain/G3A3/G3A3/G3A3").gameObject.SetActive(false);                    
                    troop.transform.Find("Troop Base/TRP_SKELETON/weaponmain/G3A3/G3A3 Rigidbody/WPN_G3A3/G3A3").gameObject.SetActive(false);
                    
                    int seed = System.DateTime.Now.Millisecond;                    
                    if (seed <= 600) //approx. 60% chance for spawning a field-dressing taped to the soldier's webbing
                    {
                        Transform chest = troop.transform.Find("Troop Base/TRP_SKELETON/soldierHip/soldierSpine1/soldierSpine2/soldierSpine3/soldierChest");
                        GameObject dressing = GameObject.Instantiate(dressing_prefab, chest);
                        dressing.transform.parent = chest;
                        dressing.transform.position = chest.position;
                        dressing.transform.localPosition = new Vector3(-0.1f, 0.075f, 0.12f);
                        dressing.transform.localRotation = Quaternion.Euler(45f, 275f, 0f);
                        dressing.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);                       
                    }

                    troop.gameObject.AddComponent<CanLepConverted>();
                    Utilities.Log(troop.name + " converted to CF Infantry");
                }
            }

            //VEHICLES
            Vehicle[] list = GameObject.FindObjectsByType<Vehicle>(FindObjectsSortMode.None);
            
            foreach (var vehicle in list)
            {
                GameObject vehicle_go = vehicle.gameObject;
                if (vehicle_go == null) { continue; }
                if (vehicle_go.GetComponent<CanLepConverted>() != null) { continue; }

                if (vehicle.UniqueName == "M113G" && convert_infantry.Value) {
                    M113.Convert(vehicle, vehicle_go, cal50, additional_decals.Value,
                        mapleAPC, canInf, canInf_nm, canInf_sm, callsignsAPC, apc, flag, tacAPC, mlcAPC);
                    Utilities.Log("Conversions complete on " + vehicle_go.name);
                }

                string short_name = vehicle_go.name.Substring(0, 3);
                if (short_name != "LEO") { continue; }                
                if ((vehicle.UniqueName == "LEO1A1" || vehicle.UniqueName == "LEO1A1A2") && exclude_early_1A1.Value) { continue; }
                if ((vehicle.UniqueName == "LEO1A1A3" || vehicle.UniqueName == "LEO1A1A4") && exclude_late_1A1.Value) { continue; }
                if ((vehicle.UniqueName == "LEO1A3" || vehicle.UniqueName == "LEO1A3A1") && exclude_early_1A3.Value) { continue; }
                if ((vehicle.UniqueName == "LEO1A3A2" || vehicle.UniqueName == "LEO1A3A3") && exclude_late_1A3.Value) { continue; }
                if (vehicle.UniqueName == "LEO1A4" && exclude_1A4.Value) { continue; }                

                short_name = vehicle_go.name.Substring(0, 4);
                if (short_name == "LEO2") { Leo2.Convert(vehicle); continue; }
                
                short_name = vehicle_go.name.Substring(0, 6);
                if (short_name == "LEO1A1") { Leo1A1.Convert(vehicle, american_crew_voice, fn_mag, A1_base, A1_camomask, maple, callsigns, tac, mlc); }
                if (short_name == "LEO1A3" || short_name == "LEO1A4") { Leo1A3.Convert(vehicle, american_crew_voice, marder_crosshair, fn_mag, A3_base, A3_camomask, maple, callsigns, tac, mlc); }
            }

            if (graf) { 
                GameObject gizmo = new GameObject("gizmo");
                GrafenGizmo graf = gizmo.AddComponent<GrafenGizmo>();
                graf.american_crew_voice = american_crew_voice;
                graf.marder_crosshair = marder_crosshair;
                graf.fn_mag = fn_mag;
                graf.A3_base = A3_base;
                graf.A3_camomask = A3_camomask;
                graf.maple = maple;
                graf.callsigns = callsigns;
                graf.tac = tac;
                graf.mlc = mlc;
                GameObject.Instantiate(gizmo);
            }
            yield break;
        }
    }    
}
