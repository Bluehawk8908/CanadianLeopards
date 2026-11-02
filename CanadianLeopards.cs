using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using MelonLoader;
using GHPC;
using GHPC.Infantry;
using GHPC.State;
using GHPC.Mission;
using GHPC.Vehicle;

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
        
        void Awake()
        {
            done = false;
            spawned = false;
        }
        void LateUpdate()
        {
            if (done) Destroy(gameObject);
            if (!spawned) { CanadianLeopardsClass.ShowcaseExtras(); spawned = true; }  
            if (GHPC.World.SceneUnitsManager.Instance.AllUnitsInScene.Count == 40) {
                Vehicle[] newList = GameObject.FindObjectsByType<Vehicle>(FindObjectsSortMode.None);                
                foreach (var vehicle in newList)
                {
                    if (vehicle.gameObject.GetComponent<CanLepConverted>() != null) { continue; }
                    if (vehicle.UniqueName == "LEO1A3A3" || vehicle.UniqueName == "LEO1A3")
                    { Leo1A3.Convert(vehicle); }
                }
                done = true;
            }
        }
    }
    public class CanadianLeopardsClass : MelonMod
    {
        public static GameObject gameManager;
        public static MelonPreferences_Entry<int> ammo_loadout;
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
        public static MelonPreferences_Entry<bool> exclude_early_L2;
        public static MelonPreferences_Entry<bool> exclude_late_L2;
        public static MelonPreferences_Entry<int> leo2_mlc;
        public static MelonPreferences_Entry<int> leo2a4_tactical;
        public static MelonPreferences_Entry<int> leo2a4_camo;
        public static MelonPreferences_Entry<int> leo2a4_chevrons;
        public static MelonPreferences_Entry<bool> convert_infantry;
        public static MelonPreferences_Entry<bool> mute_logger;
                
        public static bool graf;
        
        public override void OnInitializeMelon()
        {
            MelonPreferences_Category cfg = MelonPreferences.CreateCategory("Canadian Leopards");
            ammo_loadout = cfg.CreateEntry<int>("Customize ammo loadout", 1);
            ammo_loadout.Comment = "1 for 'historical' Canadian munitions, 2 for 'American': M774 and HEAT, 3 to keep German defaults";

            no_threecolour = cfg.CreateEntry<bool>("Force single colour paint schemes", false);
            no_threecolour.Comment = "Prevents C1 and C2s from appearing in NATO three-colour camo";

            carc_green = cfg.CreateEntry<bool>("C2 in CARC Green", true);
            carc_green.Comment = "Replaces the default German Gelboliv for NATO CARC, a brighter shade of green.";

            decals_outlined = cfg.CreateEntry<bool>("Decals outlined in white", true);
            decals_outlined.Comment = "Turret numbers and maple leaves have white borders when true; plain black when false.";

            additional_decals = cfg.CreateEntry<bool>("Additional decals", true);
            additional_decals.Comment = "Adds tactical unit symbol and MLC number to the hull (C1, C2 and Leo2A3s only).";

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

            exclude_early_L2 = cfg.CreateEntry<bool>("Exclude early Leo 2s from conversion", false);
            exclude_early_L2.Comment = "2A0s and 2A1s will remain in German service and retain old features";

            exclude_late_L2 = cfg.CreateEntry<bool>("Exclude late Leo 2s from conversion", false);
            exclude_late_L2.Comment = "2A2s and 2A3s will remain in German service and retain old features";

            leo2_mlc = cfg.CreateEntry<int>("Leopard 2 MLC decals", 1);
            leo2_mlc.Comment = "0 for NONE, 1 for black and white, 2 for hollow black";

            leo2a4_tactical = cfg.CreateEntry<int>("Leopard 2A4 tactical unit symbols", 2);
            leo2a4_tactical.Comment = "0 for NONE, 1 for 4th CMBG, 2 for generic armoured unit";

            leo2a4_camo = cfg.CreateEntry<int>("Leopard 2A4 camouflage", 1);
            leo2a4_camo.Comment = "0 for NONE, 1 for forest three-colour, 2 for desert tan";

            leo2a4_chevrons = cfg.CreateEntry<int>("Leopard 2A4 allied chevrons", 1);
            leo2a4_chevrons.Comment = "0 for NONE, 1 for white, 2 for gold on green (black always on tan)";

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
                  Position = new Vector3(1411.68f, 26.06f, 1434.42f), 
                  Rotation = Quaternion.Euler(0.563f, 220.48f, 358.23f)});
            UnitSpawner.Instance.SpawnUnit(Leo1A3A3, new GHPC.Mission.Data.UnitMetaData()
                { Allegiance = Faction.Neutral, 
                  Position = new Vector3(1399.87f, 25.7f, 1436.75f), 
                  Rotation = Quaternion.Euler(0.925f, 230.98f, 358.284f) });

            prefab1A3.ReleaseAsset();
            prefab1A3A3.ReleaseAsset();
            Utilities.Log("Spawned additional vehicles on the range.");
        }        

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            if (sceneName == "MainMenu2_Scene" || sceneName == "t64_menu" || sceneName == "MainMenu2-1_Scene") 
            {
                graf = false;
                return;
            }
            if (sceneName == "leo2_menu") 
            {
                graf = false;
                Utilities.FetchResources();
                Menu.MenuLeopards();
                return;
            }

            gameManager = GameObject.Find("_APP_GHPC_");
            if (gameManager == null) { return; }

            if (sceneName == "TR01_showcase" && showcase_extras.Value) { graf = true; }
            StateController.RunOrDefer(GameState.GameReady, new GameStateEventHandler(Conversion), GameStatePriority.Medium);            
        }

        public IEnumerator Conversion(GameState _)
        {
            AmmoSwaps.Initalize();
            Utilities.FetchResources();

            //INFANTRY
            if (convert_infantry.Value) {                
                InfantryUnit[] troops = GameObject.FindObjectsByType<InfantryUnit>(FindObjectsSortMode.None);
                
                foreach (var troop in troops)
                {
                    if (!troop.name.StartsWith("BW Feldanzug")) { continue; }
                    if (troop.gameObject.GetComponent<CanLepConverted>() != null) { continue; }
                    Infantry.Convert(troop);
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
                    M113.Convert(vehicle, vehicle_go);                    
                }

                string short_name = vehicle_go.name.Substring(0, 3);
                if (short_name != "LEO") { continue; }                
                if ((vehicle.UniqueName == "LEO1A1" || vehicle.UniqueName == "LEO1A1A2") && exclude_early_1A1.Value) { continue; }
                if ((vehicle.UniqueName == "LEO1A1A3" || vehicle.UniqueName == "LEO1A1A4") && exclude_late_1A1.Value) { continue; }
                if ((vehicle.UniqueName == "LEO1A3" || vehicle.UniqueName == "LEO1A3A1") && exclude_early_1A3.Value) { continue; }
                if ((vehicle.UniqueName == "LEO1A3A2" || vehicle.UniqueName == "LEO1A3A3") && exclude_late_1A3.Value) { continue; }
                if (vehicle.UniqueName == "LEO1A4" && exclude_1A4.Value) { continue; }                

                if ((vehicle.UniqueName == "LEO2" || vehicle.UniqueName == "LEO2A1") && exclude_early_L2.Value) { continue; }
                if ((vehicle.UniqueName == "LEO2A2" || vehicle.UniqueName == "LEO2A3") && exclude_late_L2.Value) { continue; }
                
                short_name = vehicle_go.name.Substring(0, 4);
                if (short_name == "LEO2") { Leo2.Convert(vehicle); 
                    continue; 
                }
                
                short_name = vehicle_go.name.Substring(0, 6);
                if (short_name == "LEO1A1") { Leo1A1.Convert(vehicle); }
                if (short_name == "LEO1A3" || short_name == "LEO1A4") { Leo1A3.Convert(vehicle); }
            }

            if (graf) { 
                GameObject gizmo = new GameObject("gizmo");
                gizmo.AddComponent<GrafenGizmo>();                
                GameObject.Instantiate(gizmo);
            }
            yield break;
        }
    }    
}
