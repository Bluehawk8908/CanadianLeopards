using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;
using MelonLoader;
using MelonLoader.Utils;
using GHPC;
using GHPC.Camera;
using GHPC.Player;
using GHPC.Mission;
using GHPC.Infantry;
using GHPC.AI.Platoons;
using GHPC.State;
using GHPC.Vehicle;
using GHPC.Weapons;
using GHPC.Equipment.Optics;
using Reticle;
using GHPC.Effects.Voices;


namespace CanadianLeopards
{
    public class CanLepConverted : MonoBehaviour
    {
        void Awake()
        {
            enabled = false;
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
        public static MelonPreferences_Entry<bool> exclude_1A4;
        public static MelonPreferences_Entry<bool> convert_infantry;
        public static MelonPreferences_Entry<bool> mute_logger;

        public static GameObject american_crew_voice = null;
        public static GameObject m240_prefab = null;
        public static AmmoFeed cal50 = null;        
        public static ReticleMesh.CachedReticle crosshair;
        static bool activeScene = false;
        static bool grafen = false;
        
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

            exclude_1A4 = cfg.CreateEntry<bool>("Exclude 1A4 from conversion", true);
            exclude_1A4.Comment = "The Leopard 1A4 will remain in German service and retain all its unique features.";

            convert_infantry = cfg.CreateEntry<bool>("Convert FRG infantry and M113s", true);
            convert_infantry.Comment = "West German infantry and M113Gs will be converted to resemble Canadian mechanized infantry.";

            mute_logger = cfg.CreateEntry<bool>("Mute log messages", false);
            mute_logger.Comment = "Mutes log messages in the MelonLoader console.";
        }

        public static void Log(string text)
        {
            if (!mute_logger.Value) { MelonLogger.Msg(text); }
        }

        void ShowcaseExtras()
        {
            var prefabLookups = Object.FindAnyObjectByType<UnitSpawner>().PrefabLookup;
            AssetReference prefab1A3 = prefabLookups.GetPrefab("LEO1A3");
            AssetReference prefab1A3A3 = prefabLookups.GetPrefab("LEO1A3A3");
            GameObject Leo1A3 = Addressables.LoadAssetAsync<GameObject>(prefab1A3).WaitForCompletion();
            GameObject Leo1A3A3 = Addressables.LoadAssetAsync<GameObject>(prefab1A3A3).WaitForCompletion();
            Leo1A3.GetComponent<Vehicle>().Allegiance = Faction.Neutral;
            Leo1A3A3.GetComponent<Vehicle>().Allegiance = Faction.Neutral;
            GameObject.Instantiate(Leo1A3, new Vector3(1423.7f, 26.416f, 1433.9f), Quaternion.Euler(0.573f, 233.87f, 358.75f));
            GameObject.Instantiate(Leo1A3A3, new Vector3(1399.87f, 25.7f, 1436.75f), Quaternion.Euler(0.925f, 230.98f, 358.284f));                      
            Log("Spawned additional vehicles on the range.");
            grafen = true;            
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
            filter.mesh = new Mesh();
            filter.mesh.vertices = new Vector3[] {
                            new Vector3(1f, 0 , 1f), new Vector3(1f, 0, -1f), new Vector3(-1f, 0, 1f), new Vector3(-1f, 0, -1f) };
            filter.mesh.uv = new Vector2[] {
                            new Vector2(1, 1), new Vector2(1, 0), new Vector2(0, 1), new Vector2(0, 0) };
            filter.mesh.triangles = new int[] { 0, 1, 2, 2, 1, 3 };
            filter.mesh.RecalculateNormals();
            render.material = mat;
            render.material.mainTexture = tex;
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            if (sceneName == "MainMenu2_Scene" || sceneName == "t64_menu" || sceneName == "MainMenu2-1_Scene") 
            {
                activeScene = false;
                grafen = false; 
                return;               
            }

            gameManager = GameObject.Find("_APP_GHPC_");
            if (gameManager == null) { return; }
            if (sceneName == "TR01_showcase" && showcase_extras.Value) { ShowcaseExtras(); }

            StateController.RunOrDefer(GameState.GameReady, new GameStateEventHandler(Conversion), GameStatePriority.Medium);
        }

        public IEnumerator Conversion(GameState _)
        {
            if (activeScene == true) { yield break; }
            activeScene = true;
            Vehicle[] list = GameObject.FindObjectsByType<Vehicle>(FindObjectsSortMode.None);
            
            
            // PREFABS
            if (m240_prefab == null || american_crew_voice == null || cal50 == null) 
            {
                Vehicle abrams = null;
                foreach (var vehicle in list)
                {
                    if (vehicle.UniqueName != "M1") { continue; }
                    abrams = vehicle;                        
                    m240_prefab = abrams.transform.Find("IPM1_rig/HULL/TURRET/Turret Scripts/M240_loader").gameObject;
                    cal50 = abrams.transform.Find("IPM1_rig/HULL/TURRET/CUPOLA/CUPOLA_GUN/12.7mm Machine Gun M48").GetComponent<AmmoFeed>();
                    Log("Abrams found in scene");
                    break;                  
                }
                
                if (abrams == null)
                {
                    var prefabLookups = Object.FindAnyObjectByType<UnitSpawner>().PrefabLookup;
                    AssetReference prefab = prefabLookups.GetPrefab("M1");
                    abrams = Addressables.LoadAssetAsync<GameObject>(prefab).WaitForCompletion().GetComponent<Vehicle>();
                    m240_prefab = abrams.transform.Find("Turret Scripts/M240_loader").gameObject;
                    Log("Dummy Abrams fetched");
                    cal50 = abrams.transform.Find("Cupola Scripts/12.7mm Machine Gun M48").GetComponent<AmmoFeed>(); ;
                    if (cal50 != null) { Log("M2 Browning AmmoFeed found"); }
                }               
                american_crew_voice = abrams.GetComponentInChildren<CrewVoiceHandler>().gameObject;                
            }
            
            if (crosshair.mesh == null)
            { 
                Vehicle marder = null;
                foreach (var vehicle in list)
                {
                    if (!vehicle.UniqueName.StartsWith("MARDER")) { continue; }
                    marder = vehicle;
                    crosshair = marder.transform.Find("Marder1A1_rig/hull/turret/PERI Z11/NFOV reticle").GetComponent<ReticleMesh>().reticle;
                    Log("Marder found in scene");
                    break;
                }

                if (marder == null)
                {
                    var prefabLookups = Object.FindAnyObjectByType<UnitSpawner>().PrefabLookup;
                    AssetReference prefab = prefabLookups.GetPrefab("MARDERA1PLUS");
                    marder = Addressables.LoadAssetAsync<GameObject>(prefab).WaitForCompletion().GetComponent<Vehicle>();
                    ReticleMesh prefabReticle = marder.transform.Find("FCS and sights/PERI Z11/NFOV reticle").GetComponent<ReticleMesh>();
                    prefabReticle.Load();
                    crosshair = prefabReticle.reticle;                    
                    Log("Dummy Marder fetched");
                }                
            }

            //TEXTURES
            string maplePath = decals_outlined.Value ? "Mods/CanadianLeopards/maple.png" : "Mods/CanadianLeopards/maple_black.png";
            Texture2D maple = FetchTex(128, 128, maplePath);
            string A1_basePath = carc_green.Value ? "Mods/CanadianLeopards/green.png" : "Mods/CanadianLeopards/1A1_base.png";
            Texture2D A1_base = FetchTex(2048, 2048, A1_basePath);
            Texture2D A3_base = FetchTex(2048, 2048, "Mods/CanadianLeopards/1A3_base.png"); 
            string callsignsPath = decals_outlined.Value ? "Mods/CanadianLeopards/callsigns.png": "Mods/CanadianLeopards/callsigns_black.png";            
            Texture2D callsigns = FetchTex(512, 64, callsignsPath);             
            Texture2D A1_camomask = FetchTex(2048, 2048, "Mods/CanadianLeopards/A1_mask.png");            
            Texture2D A3_camomask = FetchTex(2048, 2048, "Mods/CanadianLeopards/A3_mask.png");
            Texture2D tac = FetchTex(128, 98, "Mods/CanadianLeopards/tac.png");            
            string mlcPath = decals_outlined.Value ? "Mods/CanadianLeopards/mlc.png": "Mods/CanadianLeopards/mlc_black.png";
            Texture2D mlc = FetchTex(128, 128, mlcPath);            
            Texture2D canInf = FetchTex(1024, 1024, "Mods/CanadianLeopards/can_inf.png");            
            Texture2D canInf_nm = FetchTex(1024, 1024, "Mods/CanadianLeopards/can_inf_nm.png", true);
            Texture2D canInf_sm = FetchTex(1024, 1024, "Mods/CanadianLeopards/can_inf_sm.png", true);
            Texture2D apc = FetchTex(2048, 2048, "Mods/CanadianLeopards/apc.png"); 
            Texture2D flag = FetchTex(196, 98, "Mods/CanadianLeopards/flag.png");
            Texture2D mlcAPC = FetchTex(128, 128, "Mods/CanadianLeopards/mlc_apc.png");
            Texture2D tacAPC = FetchTex(148, 88, "Mods/CanadianLeopards/tac_apc.png");
            Texture2D callsignsAPC = (!decals_outlined.Value) ? callsigns : FetchTex(512, 64, "Mods/CanadianLeopards/callsigns_black.png");
            Texture2D mapleAPC = (!decals_outlined.Value) ? maple : FetchTex(128, 128, "Mods/CanadianLeopards/maple_black.png");            

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
                    dress.material.SetTexture("_Albedo", canInf);
                    dress.material.SetTexture("_Normal", canInf_nm);                    
                    accoutrements.material.SetTexture("_Albedo", canInf);                    
                    helmet.material.SetTexture("_Albedo", canInf);
                    helmet.material.SetTexture("_Normal", canInf_nm);
                    helmet.material.SetTexture("_Smoothness", canInf_sm);                                        
                    webbing.material.SetTexture("_Albedo", canInf);

                    AarVisual aarVis = troop.transform.Find("Troop Base").GetComponent<AarVisual>();
                    aarVis.OriginalMaterials[dress] = new System.Collections.Generic.List<Material> { dress.material };
                    aarVis.OriginalMaterials[accoutrements] = new System.Collections.Generic.List<Material> { accoutrements.material };
                    aarVis.OriginalMaterials[helmet] = new System.Collections.Generic.List<Material> { helmet.material };
                    aarVis.OriginalMaterials[webbing] = new System.Collections.Generic.List<Material> { webbing.material };
                    
                    //troop.transform.Find("Troop Base/TRP_SKELETON/weaponmain/Troop Weapons/--PRIMARY WEAPONS/M16A1").gameObject.SetActive(true);
                    //troop.transform.Find("Troop Base/TRP_SKELETON/weaponmain/G3A3").gameObject.SetActive(false);

                    //InfantryAnimation infAnimation = troop.transform.Find("Troop Base").GetComponent<InfantryAnimation>();
                    //InfantryWeaponSystem m16 = troop.transform.Find("Troop Base/TRP_SKELETON/weaponmain/Troop Weapons/--PRIMARY WEAPONS/M16A1").GetComponent<InfantryWeaponSystem>();
                    //TroopWeaponAnimationData m16_anim = troop.transform.Find("Troop Base/TRP_SKELETON/weaponmain/Troop Weapons/--PRIMARY WEAPONS/M16A1").GetComponent<TroopWeaponAnimationData>();
                    //InfantryWeaponsManager iwm = troop.GetComponent<InfantryWeaponsManager>();
                    //InfantryRagdoll ird = troop.transform.Find("Troop Base").GetComponent<InfantryRagdoll>();                    
                    //iwm._weapons[0] = m16;
                    //iwm._equippableWeapons[0] = m16;
                    //iwm.EquippedWeapon = m16;
                    //ird._weaponRagdolls[0] = m16.transform.GetComponent<WeaponRagdoll>();
                    //infAnimation._primaryWeapon = m16_anim;
                    //infAnimation._targetWeapon = m16_anim;
                    //infAnimation._activeWeapon = m16_anim;

                    var c1a1_bundle = AssetBundle.LoadFromFile(Path.Combine(MelonEnvironment.ModsDirectory + "/CanadianLeopards", "c1a1"));
                    if (c1a1_bundle == null) MelonLogger.Error("Could not load test asset bundle");                                     
                    
                    GameObject c1a1_rifle = GameObject.Instantiate(c1a1_bundle.LoadAsset("assets/C1A1.obj") as GameObject);                    

                    GameObject default_rifle = troop.transform.Find("Troop Base/TRP_SKELETON/weaponmain/G3A3").gameObject;                   
                    c1a1_rifle.transform.parent = default_rifle.transform;
                    c1a1_rifle.transform.position = default_rifle.transform.position;
                    c1a1_rifle.transform.localPosition = new Vector3(0f, 0.06f, 0.235f);
                    c1a1_rifle.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                    c1a1_rifle.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);                    
                    c1a1_rifle.transform.Find("default").GetComponent<MeshRenderer>().material.color = new Color(0.6604f, 0.6604f, 0.6604f, 1);
                    
                    AarVisual weapon_AAR = troop.transform.Find("Troop Base/TRP_SKELETON/weaponmain/G3A3").GetComponent<AarVisual>();
                    weapon_AAR._renderers = new System.Collections.Generic.List<Renderer> { c1a1_rifle.transform.Find("default").GetComponent<MeshRenderer>() };
                    weapon_AAR.OriginalMaterials = new System.Collections.Generic.Dictionary<Renderer, System.Collections.Generic.List<Material>>
                        { [c1a1_rifle.transform.Find("default").GetComponent<MeshRenderer>()] = new System.Collections.Generic.List<Material> 
                            { c1a1_rifle.transform.Find("default").GetComponent<MeshRenderer>().material } };

                    troop.transform.Find("Troop Base/TRP_SKELETON/weaponmain/G3A3/G3A3/G3A3").gameObject.SetActive(false);
                    string[] singleShotPaths = { "event:/Infantry/Weapons/MG_HKG3_600rpm" };                    
                    troop.transform.Find("Troop Base/TRP_SKELETON/weaponmain/G3A3/FMODWeaponAudio").GetComponent<WeaponAudio>().SingleShotEventPaths = singleShotPaths;
                    troop.transform.Find("Troop Base/TRP_SKELETON/weaponmain/G3A3/G3A3 Rigidbody/WPN_G3A3/G3A3").gameObject.SetActive(false);
                    
                    c1a1_bundle.Unload(false);

                    int seed = System.DateTime.Now.Millisecond;                    
                    if (seed <= 600) //approx. 60% chance for spawning a field-dressing taped to the soldier's webbing
                    {
                        var dressing_bundle = AssetBundle.LoadFromFile(Path.Combine(MelonEnvironment.ModsDirectory + "/CanadianLeopards", "dressing"));
                        if (dressing_bundle == null) MelonLogger.Error("Could not load test asset bundle");

                        GameObject dressing = GameObject.Instantiate(dressing_bundle.LoadAsset("assets/field-dressing.obj") as GameObject);
                        Transform chest = troop.transform.Find("Troop Base/TRP_SKELETON/soldierHip/soldierSpine1/soldierSpine2/soldierSpine3/soldierChest");
                        dressing.transform.parent = chest;
                        dressing.transform.position = chest.position;
                        dressing.transform.localPosition = new Vector3(-0.1f, 0.075f, 0.12f);
                        dressing.transform.localRotation = Quaternion.Euler(45f, 275f, 0f);
                        dressing.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);

                        dressing_bundle.Unload(false);
                    }                    

                    troop.gameObject.AddComponent<CanLepConverted>();
                    Log(troop.name + " converted to CF Infantry");
                }
            }

            //VEHICLES
            foreach (var vehicle in list)
            {
                GameObject vehicle_go = vehicle.gameObject;
                if (vehicle_go == null) { continue; }
                if (vehicle_go.GetComponent<CanLepConverted>() != null) { continue; }

                if (vehicle.UniqueName == "M113G" && convert_infantry.Value) {
                    M113.Convert(vehicle, vehicle_go, cal50, additional_decals.Value,
                        mapleAPC, canInf, canInf_nm, canInf_sm, callsignsAPC, apc, flag, tacAPC, mlcAPC);
                    Log("Conversions complete on " + vehicle_go.name);
                }

                string short_name = vehicle_go.name.Substring(0, 3);
                if (short_name != "LEO") { continue; }
                vehicle_go.AddComponent<CanLepConverted>();
                Log("Found vic named: " + vehicle_go.name);
                bool leo1a3 = false;
                short_name = vehicle_go.name.Substring(0, 6);
                if (short_name == "LEO1A3" || short_name == "LEO1A4") { leo1a3 = true; }
                if (short_name == "LEO1A4" && exclude_1A4.Value == true) { continue; }
                vehicle._friendlyName = "Leopard C1";  //New display name

                vehicle.transform.Find("DE Tank Voice").gameObject.SetActive(false); //Adding US Voices
                GameObject new_voice = GameObject.Instantiate(american_crew_voice, vehicle.transform);
                new_voice.transform.localPosition = new Vector3(0, 0, 0);
                new_voice.transform.localEulerAngles = new Vector3(0, 0, 0);
                CrewVoiceHandler handler = new_voice.GetComponent<CrewVoiceHandler>();
                handler._chassis = vehicle._chassis as NwhChassis;
                vehicle._crewVoiceHandler = handler;
                new_voice.SetActive(true);

                WeaponSystem maingun = vehicle.GetComponent<WeaponsManager>().Weapons[0].Weapon;
                WeaponSystemInfo coax = vehicle.GetComponent<WeaponsManager>().Weapons[1];
                FireControlSystem fcs = vehicle.GetComponentInChildren<FireControlSystem>();

                //Adding Laser-Range Finder and Lead-Calculator
                GameObject lrf_holder = new GameObject("Laser Rangefinder");
                lrf_holder.transform.SetParent(vehicle.transform.Find("LEO1A1A1_rig/HULL/TURRET/--Turret Scripts--/Sights/GPS"));
                if (leo1a3) { lrf_holder.transform.localPosition = new Vector3(0f, 0f, 0.5f); }
                else { lrf_holder.transform.localPosition = new Vector3(0f, 0f, 0.2f); }
                lrf_holder.transform.localRotation = Quaternion.identity;
                GHPC.Equipment.DestructibleComponent laser_dest = lrf_holder.AddComponent<GHPC.Equipment.DestructibleComponent>();
                laser_dest._health = 5f;
                laser_dest._fullHealth = 5f;
                laser_dest._pressureTolerance = 1f;
                laser_dest._shockResistance = 0.30f;
                laser_dest._name = "Laser Rangefinder";

                fcs.LaserAim = LaserAimMode.ImpactPoint;
                fcs.LaserComponent = laser_dest;
                fcs.LaserOrigin = lrf_holder.transform;
                fcs.MaxLaserRange = 4000f;
                fcs.DynamicLead = true;
                fcs.SuperleadWeapon = true;
                fcs.SuperelevateWeapon = true;
                fcs.TraverseBufferSeconds = 0.5f;
                fcs._autoDumpViaPalmSwitches = true;
                fcs._originalSuperleadMode = true;
                fcs.ComputerNeedsPower = true;
                fcs.RecordTraverseRateBuffer = true;
                fcs._useSeparateLead = false;
                //fcs._manualModeOnRangeSet = true;
                //fcs._autoModeOnLase = true;
                UsableOptic sabca = fcs.MainOptic;
                sabca.ForceHorizontalReticleAlign = true;
                sabca.RotateAzimuth = true;

                UnityEngine.Object.Destroy(fcs.OpticalRangefinder);
                GameObject sabca_go = sabca.gameObject;
                CameraSlot sabca_cam = sabca_go.GetComponent<CameraSlot>();

                //Ensuring PZB-200 Night Sight
                if (fcs.NightOptic == null || fcs.NightOptic.name == "PERI-R12")
                {
                    GameObject pzb_go;
                    GameObject aux_go;
                    if (leo1a3)
                    {
                        pzb_go = vehicle.transform.Find("LEO1A1A1_rig/HULL/TURRET/1A3 mantlet/--Gun Scripts--/PZB-200").gameObject;
                        aux_go = vehicle.transform.Find("LEO1A1A1_rig/HULL/TURRET/1A3 mantlet/--Gun Scripts--/Aux sight TZF1A").gameObject;
                    }
                    else
                    {
                        pzb_go = vehicle.transform.Find("LEO1A1A1_rig/HULL/TURRET/Mantlet/--Gun Scripts--/PZB-200").gameObject;
                        aux_go = vehicle.transform.Find("LEO1A1A1_rig/HULL/TURRET/Mantlet/--Gun Scripts--/Aux sight TZF1A").gameObject;
                    }
                    pzb_go.SetActive(true);
                    UsableOptic pzb = pzb_go.GetComponent<UsableOptic>();
                    pzb.ReticleActive = true;
                    pzb.StabsActive = true;
                    CameraSlot pzb_cam = pzb_go.GetComponent<CameraSlot>();
                    CameraSlot aux_cam = aux_go.GetComponent<CameraSlot>();
                    pzb_cam.LinkedDaySight = sabca_cam;
                    sabca_cam.LinkedNightSight = pzb_cam;
                    aux_cam.LinkedNightSight = pzb_cam;
                    pzb_cam._pairedOptic = pzb;
                    pzb_cam.IsLinkedNightSight = true;
                    pzb_cam._isUsableByWeapon = true;
                    pzb_cam.NightSightAtNightOnly = false;
                    fcs.NightOptic = pzb;
                    fcs.RegisterOptic(pzb);
                    if (leo1a3)
                    {
                        vehicle.transform.Find("LEO1A3_mesh/1A3_PZB200").gameObject.SetActive(true);
                        vehicle.transform.Find("LEO1A3_mesh/PERI R12").gameObject.SetActive(false);
                    }
                    else { vehicle.transform.Find("LEO1A1_mesh/PZB 200").gameObject.SetActive(true); }
                    Log("Swapping night sights");
                }

                //Changing the reticle in the primary sight
                GameObject reticle_mesh_go = vehicle.transform.Find("LEO1A1A1_rig/HULL/TURRET/--Turret Scripts--/Sights/GPS/Reticle Mesh").gameObject;
                ReticleMesh reticle_mesh = reticle_mesh_go.GetComponent<ReticleMesh>();
                reticle_mesh.reticleSO = crosshair.tree;
                reticle_mesh.reticle = crosshair;
                reticle_mesh.SMR = null;
                reticle_mesh.Load();
                reticle_mesh.enabled = false;
                ReticleTree.Light new_light = new ReticleTree.Light();
                new_light.color = new RGB(4f, 3f, 0, true);
                new_light.type = ReticleTree.Light.Type.Powered;
                reticle_mesh.lights[0].light = new_light;
                reticle_mesh.lightCols[1] = new Vector4(4f, 3f, 0f, 1f);
                sabca_cam.DefaultFov = 9.52f;
                sabca_cam.OtherFovs = new float[] { 3f };
                sabca_cam.AllowFreeZoom = true;
                sabca_cam.ZoomInAudioEvent = "event:/Effects/Optic/Optic_Zoom_In";
                sabca_cam.ZoomOutAudioEvent = "event:/Effects/Optic/Optic_Zoom_Out";
                GameObject old_scale = vehicle.transform.Find("LEO1A1A1_rig/HULL/TURRET/--Turret Scripts--/" +
                    "Sights/GPS/E-Scale").gameObject;
                old_scale.transform.Find("e-scale").gameObject.SetActive(false);
                old_scale.transform.Find("index mark").gameObject.SetActive(false);
                Transform old_scale_red = vehicle.transform.Find("LEO1A1A1_rig/HULL/TURRET/--Turret Scripts--/" +
                    "Sights/GPS/E-Scale red");
                if (old_scale_red != null)
                {
                    old_scale_red.transform.Find("e-scale").gameObject.SetActive(false);
                    old_scale_red.transform.Find("index mark").gameObject.SetActive(false);
                }

                maingun.WeaponData.FriendlyName = "105mm Gun L7A4 L/52";

                //Replacing Coax MG
                coax.Name = "7.62mm machine gun C6";
                AmmoFeed coax_ammo = coax.Weapon.Feed;
                coax_ammo._totalCycleTime = 0.08f;
                coax.Weapon.WeaponSound.LoopEventPath = "event:/Weapons/MG_m240_750rmp";

                //Replacing loader-hatch MG
                Transform loader_station;
                if (leo1a3) { loader_station = vehicle.transform.Find("LEO1A1A1_rig/HULL/TURRET/lafette002"); }
                else { loader_station = vehicle.transform.Find("LEO1A1A1_rig/HULL/TURRET/lafette001"); }
                GameObject loader_C6 = GameObject.Instantiate(m240_prefab, loader_station);
                Transform loader_MG3;
                if (leo1a3) { loader_MG3 = loader_station.transform.Find("MG004"); }
                else { loader_MG3 = loader_station.transform.Find("MG3"); }
                Transform MG3_box;
                if (leo1a3) { MG3_box = loader_station.transform.Find("MGbox002"); }
                else { MG3_box = loader_station.transform.Find("MGbox001"); }
                loader_C6.transform.localPosition = loader_MG3.localPosition + new Vector3(0f, 0f, 0.15f);
                loader_C6.transform.localRotation = loader_MG3.localRotation;
                loader_C6.transform.localEulerAngles = new Vector3(-90f, 90f, 90f);
                loader_MG3.gameObject.SetActive(false);
                MG3_box.gameObject.SetActive(false);
                MeshFilter old_pintle = loader_station.gameObject.GetComponent<MeshFilter>();
                old_pintle.mesh = null;

                //Configuring Ammunition                
                if (ammo_loadout.Value != "German" || ammo_loadout.Value != "german")
                {
                    LoadoutManager loadout_manager = vehicle.GetComponent<LoadoutManager>();

                    if (ammo_loadout.Value == "historical") { AmmoSwaps.HistoricalLoad(maingun, loadout_manager); }
                    else if (ammo_loadout.Value == "American" || ammo_loadout.Value == "american") { AmmoSwaps.AmericanLoad(maingun, loadout_manager); }
                    else
                    {
                        Log("Unknown value for ammo loadout, using mission defaults");
                    }

                }

                //Texture cosmetics
                GameObject de_markings;
                if (leo1a3) { de_markings = vehicle.transform.Find("LEO1A3_markings").gameObject; }
                else { de_markings = vehicle.transform.Find("LEO1A1_markings").gameObject; }
                de_markings.SetActive(false);
                GameObject cross = vehicle.transform.Find("LEO1A1A1_rig/HULL/TURRET/kreuz").gameObject;
                Material cross_mat = cross.GetComponent<MeshRenderer>().material;
                cross.SetActive(false);

                GameObject active_hull;
                MeshRenderer base_mr;
                if (leo1a3)
                {
                    active_hull = vehicle.transform.Find("LEO1A3_mesh/1A3_hull").gameObject;
                }
                else
                {
                    GameObject hull_early = vehicle.transform.Find("LEO1A1_mesh/hull_early").gameObject;
                    GameObject hull_mid = vehicle.transform.Find("LEO1A1_mesh/hull_mid").gameObject;
                    GameObject hull_late = vehicle.transform.Find("LEO1A1_mesh/hull_late").gameObject;
                    if (hull_early.activeSelf == true)
                    {
                        active_hull = hull_early;
                    }
                    else if (hull_mid.activeSelf == true)
                    {
                        active_hull = hull_mid;
                    }
                    else
                    {
                        active_hull = hull_late;
                    }
                }
                base_mr = active_hull.GetComponent<MeshRenderer>();
                if (leo1a3) { base_mr.material.SetTexture("_Albedo", A3_base); }
                else { base_mr.material.SetTexture("_Albedo", A1_base); }
                if (leo1a3) { base_mr.material.SetTexture("_PaintMask", A3_camomask); }
                else { base_mr.material.SetTexture("_PaintMask", A1_camomask); }
                if (no_threecolour.Value) base_mr.material.SetFloat("_CamoAmount", 0f);

                if (leo1a3)
                {   
                    GameObject a3_turret = vehicle.transform.Find("LEO1A3_mesh/A3 turret").gameObject;
                    GameObject a3_skirt_cut = vehicle.transform.Find("LEO1A3_mesh/a3_skirt_cut").gameObject;
                    GameObject a3_skirt_full = vehicle.transform.Find("LEO1A3_mesh/a3_skirt_full").gameObject;
                    GameObject a3_wheels = vehicle.transform.Find("LEO1A3_mesh/running gear").gameObject;
                    a3_turret.GetComponent<SkinnedMeshRenderer>().material = base_mr.material;
                    a3_skirt_cut.GetComponent<MeshRenderer>().material = base_mr.material;
                    a3_skirt_full.GetComponent<MeshRenderer>().material = base_mr.material;
                    a3_wheels.GetComponent<SkinnedMeshRenderer>().material = base_mr.material;                    
                }
                else
                {    
                    GameObject turret_early = vehicle.transform.Find("LEO1A1A1_rig/HULL/TURRET/turret_early").gameObject;
                    GameObject turret_late = vehicle.transform.Find("LEO1A1A1_rig/HULL/TURRET/turret_late").gameObject;
                    GameObject gun_barrel = vehicle.transform.Find("LEO1A1_mesh/gun").gameObject;
                    GameObject side_skirts = vehicle.transform.Find("LEO1A1_mesh/side skirts").gameObject;
                    GameObject skirt_full = vehicle.transform.Find("LEO1A1_mesh/skirt_full").gameObject;
                    GameObject skirts_cut0 = vehicle.transform.Find("LEO1A1_mesh/skirts_cut0").gameObject;
                    GameObject skirts_cut1 = vehicle.transform.Find("LEO1A1_mesh/skirts_cut1").gameObject;
                    GameObject skirts_cut2 = vehicle.transform.Find("LEO1A1_mesh/skirts_cut2").gameObject;
                    GameObject wheels = vehicle.transform.Find("LEO1A1_mesh/running gear").gameObject;
                    turret_early.GetComponent<MeshRenderer>().material = base_mr.material;
                    turret_late.GetComponent<MeshRenderer>().material = base_mr.material;
                    gun_barrel.GetComponent<SkinnedMeshRenderer>().material = base_mr.material;
                    side_skirts.GetComponent<MeshRenderer>().material = base_mr.material;
                    skirt_full.GetComponent<MeshRenderer>().material = base_mr.material;
                    skirts_cut0.GetComponent<MeshRenderer>().material = base_mr.material;
                    skirts_cut1.GetComponent<MeshRenderer>().material = base_mr.material;
                    skirts_cut2.GetComponent<MeshRenderer>().material = base_mr.material;
                    wheels.GetComponent<SkinnedMeshRenderer>().material = base_mr.material;                                      
                }
                Log("Vehicle repainted");

                //The Iron-Cross decals have weird UVs so we need to create custom meshes
                GameObject turret = vehicle.transform.Find("LEO1A1A1_rig/HULL/TURRET").gameObject;
                GameObject maple_left = new GameObject("Mapleleaf_left");
                maple_left.transform.parent = turret.transform;
                maple_left.transform.position = turret.transform.position;
                NewQuad(maple_left, cross_mat, maple);
                maple_left.transform.localScale = new Vector3(0.165f, 0.165f, 0.165f);
                if (leo1a3)
                {
                    maple_left.transform.localPosition += new Vector3(-1.15f, 0.628f, 0.08f);
                    maple_left.transform.rotation = turret.transform.rotation * Quaternion.Euler(new Vector3(3f, 0f, 63f));                    
                }
                else
                {
                    maple_left.transform.localPosition += new Vector3(-1.135f, 0.6f, -0.15f);
                    maple_left.transform.rotation = turret.transform.rotation * Quaternion.Euler(new Vector3(0f, 10f, 60f));                    
                }                

                GameObject maple_right = new GameObject("Mapleleaf_right");
                maple_right.transform.parent = turret.transform;
                maple_right.transform.position = turret.transform.position;
                NewQuad(maple_right, cross_mat, maple);
                maple_right.transform.localScale = new Vector3(0.165f, 0.165f, 0.165f);
                if (leo1a3) 
                {
                    maple_right.transform.localPosition += new Vector3(1.15f, 0.628f, 0.1f);
                    maple_right.transform.rotation = turret.transform.rotation * Quaternion.Euler(new Vector3(0f, 180f, 62f));                    
                }
                else 
                { 
                    maple_right.transform.localPosition += new Vector3(1.135f, 0.6f, -0.15f);
                    maple_right.transform.rotation = turret.transform.rotation * Quaternion.Euler(new Vector3(0f, 170f, 60f));
                }                                

                //Turret numbers: Company [1-4], Troop [1-4], Vic [blank, A B C]
                PlatoonData platoon = vehicle.Platoon;
                int position_in_platoon = 0;
                if (vehicle.Platoon == null) { position_in_platoon = UnityEngine.Random.Range(0, 4); }
                else
                {
                    int platoon_size = vehicle.Platoon.Units.Count;
                    for (int i = 0; i < platoon_size; i++)
                    {
                        if (platoon.Units[i] == vehicle) { position_in_platoon = i; } //0 is first ... 3 is fourth
                    }
                }
                MergedVehicleNumberControl[] components = de_markings.GetComponents<MergedVehicleNumberControl>();
                MergedVehicleNumberControl turret_numbers = new MergedVehicleNumberControl();
                foreach (var mvnc in components)
                {
                    if (mvnc.Type == VehicleDecalType.UnitNumber) { turret_numbers = mvnc; }
                }
                if (turret_numbers._allValues[0] == 9) { turret_numbers._allValues[0] = 1; }
                else if (turret_numbers._allValues[0] > 4) { turret_numbers._allValues[0] -= 4; } //5-8 get remapped onto 1-4
                if (turret_numbers._allValues[1] == 9) { turret_numbers._allValues[1] = 1; }
                else if (turret_numbers._allValues[1] > 4) { turret_numbers._allValues[1] -= 4; } 
                else if (turret_numbers._allValues[1] == 0) { turret_numbers._allValues[1] = 1; }
                turret_numbers._allValues[2] = position_in_platoon + 6; // 6-9 in the texture will become letter codes
                turret_numbers.RefreshDecals();
                
                GameObject numbers_go;
                if (leo1a3) { numbers_go = vehicle.transform.Find("LEO1A1A1_rig/HULL/TURRET/numbers").gameObject; }
                else { numbers_go = vehicle.transform.Find("LEO1A1A1_rig/HULL/TURRET/NUMBERS").gameObject; }
                MeshRenderer numbers = numbers_go.GetComponent<MeshRenderer>();
                numbers.material.mainTexture = callsigns;
                numbers.material.color = new Vector4(1f, 1f, 1f, 1f);
                numbers.material.SetFloat("_Metallic", 0.698f);
                numbers.material.SetFloat("_Glossiness", 0.346f);

                if (leo1a3) {
                    numbers_go.transform.localPosition = new Vector3(-0.42f, 0.55f, -1.245f);  //moving turret numbers to back of the turret
                    numbers_go.transform.localRotation = Quaternion.Euler(270f, 270f, 0f);
                    numbers_go.transform.localScale = new Vector3(1f, 0.85f, 1f);
                }
                else 
                {
                    numbers_go.transform.localPosition = new Vector3(1.115f, 0.31f, -1.345f); 
                    numbers_go.transform.localRotation = numbers_go.transform.localRotation * Quaternion.Euler(0f, 90f, 7f);
                    numbers_go.transform.localScale = new Vector3(0.48f, 0.48f, 0.4f);
                }
                
                Vector3[] num_vertices = numbers_go.GetComponent<MeshFilter>().sharedMesh.vertices;
                if (leo1a3) 
                {
                    for (int i = 0; i < 12; i++)
                    {
                        num_vertices[i] = new Vector3(0f, 0f, 0f);
                    }
                    num_vertices[14] += new Vector3(0.13f, 0f, 0f); //changing angle of decal to play nice with the back of the turret
                    num_vertices[15] += new Vector3(0.13f, 0f, 0f);
                    num_vertices[18] += new Vector3(0.13f, 0f, 0f);
                    num_vertices[19] += new Vector3(0.13f, 0f, 0f);
                    num_vertices[22] += new Vector3(0.13f, 0f, 0f);
                    num_vertices[23] += new Vector3(0.13f, 0f, 0f);
                }
                else 
                { 
                    for (int i = 0; i < 21; i++)
                    {
                        num_vertices[i] = new Vector3(0f, 0f, 0f); //removing unneeded double image
                    }  
                    num_vertices[26] += new Vector3(-0.03f, 0f, 0f); //flattening out the bottom-right corner
                    num_vertices[28] += new Vector3(-0.03f, 0f, 0f);
                    num_vertices[29] += new Vector3(-0.03f, 0f, 0f);                    
                }
                numbers_go.GetComponent<MeshFilter>().sharedMesh.vertices = num_vertices;
                
                GameObject hull_numbers = new GameObject("hull callsign"); //new number decal for the back of the hull
                hull_numbers.transform.parent = active_hull.transform;
                hull_numbers.AddComponent<MeshFilter>();
                hull_numbers.AddComponent<MeshRenderer>();
                hull_numbers.GetComponent<MeshRenderer>().material = numbers.material;
                hull_numbers.GetComponent<MeshFilter>().mesh = numbers_go.GetComponent<MeshFilter>().mesh;
                hull_numbers.transform.position = active_hull.transform.position;
                if (leo1a3) 
                {
                    hull_numbers.transform.localPosition += new Vector3(-0.6f, 4.35f, 1.6f);
                    hull_numbers.transform.localRotation = Quaternion.Euler(355f, 0f, 270f);
                    hull_numbers.transform.localScale = new Vector3(1f, 1f, 1f);
                } 
                else 
                {                 
                    hull_numbers.transform.localPosition += new Vector3(1.88f, 3.12f, -8.9f);
                    hull_numbers.transform.localRotation = Quaternion.Euler(0f, 89f, -16f);
                    hull_numbers.transform.localScale = new Vector3(1f, 1f, 1.1f);
                }

                if (position_in_platoon == 0) //centers decals for troop-leaders
                {
                    if (leo1a3) 
                    {
                        numbers_go.transform.localPosition += new Vector3(0.105f, 0f, 0f);
                        hull_numbers.transform.localPosition += new Vector3(0.1f, 0f, 0f);
                    }
                    else 
                    { 
                        numbers_go.transform.localPosition += new Vector3(0.07f, 0f, 0f);
                        hull_numbers.transform.localPosition += new Vector3(0.2f, 0f, 0f);
                    }
                }

                if (additional_decals.Value) { 
                    //NATO map symbol type decal, front and back
                    GameObject hull_tac_front = new GameObject("tactical sign front"); 
                    hull_tac_front.transform.parent = active_hull.transform;
                    hull_tac_front.transform.position = active_hull.transform.position;
                    NewQuad(hull_tac_front, numbers.material, tac);
                    if (leo1a3)
                    {
                        hull_tac_front.transform.localPosition += new Vector3(-0.75f, -1.15f, 1.2f);
                        hull_tac_front.transform.localRotation = Quaternion.Euler(new Vector3(310f, 0f, 180f));
                        hull_tac_front.transform.localScale = new Vector3(0.1f, 0.1f, 0.066f);
                    }
                    else
                    {
                        hull_tac_front.transform.localPosition += new Vector3(-1.45f, 2.4f, 2.25f);
                        hull_tac_front.transform.localRotation = Quaternion.Euler(new Vector3(330f, 180f, 0f));
                        hull_tac_front.transform.localScale = new Vector3(0.25f, 0.25f, 0.18f);
                    }                    

                    GameObject hull_tac_rear = new GameObject("tactical sign rear");
                    hull_tac_rear.transform.parent = active_hull.transform;                    
                    hull_tac_rear.transform.position = active_hull.transform.position;
                    NewQuad(hull_tac_rear, numbers.material, tac);
                    if (leo1a3)
                    {
                        hull_tac_rear.transform.localPosition += new Vector3(-0.88f, 5.418f, 1.4f);
                        hull_tac_rear.transform.localRotation = Quaternion.Euler(new Vector3(348f, 0f, 0f));
                        hull_tac_rear.transform.localScale = new Vector3(0.1f, 0.1f, 0.066f);
                    }
                    else
                    {
                        hull_tac_rear.transform.localPosition += new Vector3(-1.78f, 2.8f, -10.85f);
                        hull_tac_rear.transform.localRotation = Quaternion.Euler(new Vector3(-100f, 0f, 0f));
                        hull_tac_rear.transform.localScale = new Vector3(0.2f, 0.2f, 0.14f);
                    }

                    GameObject mlc_decal = new GameObject("MLC decal");
                    mlc_decal.transform.parent = active_hull.transform;
                    mlc_decal.transform.position = active_hull.transform.position;
                    NewQuad(mlc_decal, numbers.material, mlc);
                    if (leo1a3)
                    {
                        mlc_decal.transform.localPosition += new Vector3(0.84f, -0.62f, 1.48f);
                        mlc_decal.transform.localRotation = Quaternion.Euler(new Vector3(306f, 0f, 180f));
                        mlc_decal.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                    }
                    else
                    {
                        mlc_decal.transform.localPosition += new Vector3(1.6f, 3f, 1.15f);
                        mlc_decal.transform.localRotation = Quaternion.Euler(new Vector3(330f, 180f, 0f));
                        mlc_decal.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                    }                    
                }
                Log("Conversions complete on " + vehicle_go.name);
            }

            if (grafen) {
                Unit newVic = Object.FindAnyObjectByType<Unit>();
                gameManager.GetComponent<PlayerInput>().SetPlayerUnit(newVic);
            }
            activeScene = false;
            yield break;            
        }
    }    
}
