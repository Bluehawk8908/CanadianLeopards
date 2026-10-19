using UnityEngine;
using TMPro;
using GHPC;
using GHPC.Vehicle;
using GHPC.Weapons;
using GHPC.Camera;
using GHPC.Equipment.Optics;
using Reticle;
using GHPC.Effects;
using GHPC.Effects.Voices;
using GHPC.AI.Platoons;
using GHPC.Utility;

namespace CanadianLeopards
{

    public class EMESIndexer : MonoBehaviour
    {
        public WeaponSystem MainGun;
        public WeaponSystem Coax;
        public FireControlSystem FCS;
        public GameObject KE;
        public GameObject MZ;
        public GameObject COAX;
        void LateUpdate()
        {
            if (FCS.CurrentWeaponSystem == MainGun) { 
                if (FCS.CurrentAmmoType.Category == AmmoType.AmmoCategory.Penetrator) { KE.SetActive(true); MZ.SetActive(false); COAX.SetActive(false); }
                else { KE.SetActive(false); MZ.SetActive(true); COAX.SetActive(false); }
            }
            if (FCS.CurrentWeaponSystem == Coax) { KE.SetActive(false); MZ.SetActive(false); COAX.SetActive(true); }
        }
    }
    public class Leo1A1
    {
        public static void Convert(Vehicle vehicle, GameObject american_crew_voice, GameObject fn_mag, 
                Texture2D A1_base, Texture2D A1_camomask, Texture2D maple, Texture2D callsigns, Texture2D tac, Texture2D mlc)
        {
            GameObject vehicle_go = vehicle.gameObject;
            vehicle_go.AddComponent<CanLepConverted>();
            vehicle._friendlyName = "Leopard C2";  //New display name

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

            //Importing the EMES-15 sight from the Leo2 and jury-rigging it into a EMES-18
            GameObject new_optic = GameObject.Instantiate(CanadianLeopardsClass.emes15, vehicle.transform.Find("LEO1A1A1_rig/HULL/TURRET/--Turret Scripts--/Sights/"));
            Object.Destroy(new_optic.GetComponent<FireControlSystem>());
            Object.Destroy(new_optic.transform.Find("Optic").gameObject);
            new_optic.name = "EMES-18";
            GameObject emes15_canvas = new_optic.transform.Find("EMES-15 canvas").gameObject;            
            GameObject lrf_holder = new GameObject("Laser Rangefinder");            
            lrf_holder.transform.SetParent(vehicle.transform.Find("LEO1A1A1_rig/HULL/TURRET/--Turret Scripts--/Sights/GPS"));
            lrf_holder.transform.localPosition = new Vector3(0f, 0f, 0.2f);
            lrf_holder.transform.localRotation = Quaternion.identity;
            GHPC.Equipment.DestructibleComponent laser_dest = lrf_holder.AddComponent<GHPC.Equipment.DestructibleComponent>();
            laser_dest._name = "Laser rangefinder";
            laser_dest._damageThreshold = 1f;
            laser_dest._health = 2f;
            laser_dest._fullHealth = 2f;
            laser_dest._pressureTolerance = 1f;            
            laser_dest.ComponentCollision = lrf_holder.GetComponent<MeshCollider>();
            laser_dest.Unit = vehicle;
            //laser_dest.Destroyed += fcs.LaserDestroyed;

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
            fcs._useSeparateLead = true;
            fcs._originalRangeLimits = new Vector2(400f, 9990f);
            UsableOptic dayOptic = fcs.MainOptic;
            dayOptic.ForceHorizontalReticleAlign = true;
            dayOptic.RotateAzimuth = true;
            dayOptic.LocalElevationLimits = new Vector2(-15f, 25f);
            dayOptic.RangeText = emes15_canvas.transform.Find("-- range readout").GetComponent<TextMeshProUGUI>();
            dayOptic.RangeTextDivideBy = 10;
            dayOptic.RangeTextArchetype = "000";
            dayOptic.ReadyToFireObject = emes15_canvas.transform.Find("-- ready to fire").gameObject;
            dayOptic.UnreadyToFireObject = emes15_canvas.transform.Find("-- unready to fire").gameObject;
            UnityEngine.Object.Destroy(fcs.OpticalRangefinder);
            CameraSlot dayCam = dayOptic.slot;
            dayCam.ExclusiveItems = new GameObject[] { emes15_canvas };
            dayCam.DefaultFov = 2.5f;
            dayCam.VibrationBlurScale = 0f;
            dayCam.VibrationShakeMultiplier = 0.1f;
            dayCam.HitZone = laser_dest;
            
            CameraSlot nightCam = new_optic.transform.Find("FLIR").GetComponent<CameraSlot>();
            UsableOptic nightOptic = new_optic.transform.Find("FLIR").GetComponent<UsableOptic>();
            nightOptic.Alignment = OpticAlignment.BoresightStabilized;
            nightOptic.ReticleActive = true;
            nightOptic.StabsActive = true;
            nightOptic.LevelTransform = vehicle.transform.Find("LEO1A1A1_rig/HULL/TURRET/--Turret Scripts--");
            nightOptic.FCS = fcs;
            nightCam.LinkedDaySight = dayCam;
            dayCam.LinkedNightSight = nightCam;
            nightCam.transform.position = dayCam.transform.position;
            nightCam.transform.localPosition = new Vector3(-0.8f, -2f, 0f);

            GameObject blackPanel = emes15_canvas.transform.Find("Panel").gameObject; blackPanel.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 327f);
            dayOptic.ReadyToFireObject.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(-64.47f, 239f);
            dayOptic.UnreadyToFireObject.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(-64.47f, 239f);
            GameObject rangeDisplay = emes15_canvas.transform.Find("-- range readout").gameObject; rangeDisplay.GetComponent<RectTransform>().anchoredPosition = new Vector2(-10f, 540f);
            GameObject symbolKE = emes15_canvas.transform.Find("-- KE").gameObject; symbolKE.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(80f, 239f);
            GameObject symbolMZ = emes15_canvas.transform.Find("-- MZ").gameObject; symbolMZ.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(80f, 239f);
            GameObject symbolCOAX = emes15_canvas.transform.Find("-- COAX").gameObject; symbolCOAX.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(80f, 239f);
            dayOptic.ReadyToFireObject.transform.GetComponent<RectTransform>().localScale = new Vector3(0.8f, 0.8f, 0.8f);
            dayOptic.UnreadyToFireObject.transform.GetComponent<RectTransform>().localScale = new Vector3(0.8f, 0.8f, 0.8f);
            rangeDisplay.GetComponent<RectTransform>().localScale = new Vector3(0.8f, 0.8f, 0.8f);
            symbolKE.GetComponent<RectTransform>().localScale = new Vector3(0.8f, 0.8f, 0.8f);
            symbolMZ.GetComponent<RectTransform>().localScale = new Vector3(0.8f, 0.8f, 0.8f);
            symbolCOAX.GetComponent<RectTransform>().localScale = new Vector3(0.8f, 0.8f, 0.8f);

            EMESIndexer indexer = fcs.gameObject.AddComponent<EMESIndexer>();
            indexer.FCS = fcs;
            indexer.MainGun = maingun;
            indexer.Coax = coax.Weapon;
            indexer.KE = symbolKE;
            indexer.MZ = symbolMZ;
            indexer.COAX = symbolCOAX;

            //Disabling PZB-200 and spotlight            
            vehicle.transform.Find("LEO1A1A1_rig/HULL/TURRET/Mantlet/searchlight closed").gameObject.SetActive(false);
            vehicle.transform.Find("LEO1A1A1_rig/HULL/TURRET/Mantlet/searchlight open").gameObject.SetActive(false);
            vehicle.transform.Find("LEO1A1A1_rig/HULL/TURRET/searchlight cord").gameObject.SetActive(false);
            vehicle.transform.Find("LEO1A1_mesh/PZB 200").gameObject.SetActive(false);
            GHPC.Equipment.Lamps.LampInfo[] tempArray = new GHPC.Equipment.Lamps.LampInfo[3];
            tempArray[0] = vehicle.GetComponent<GHPC.Equipment.EquipmentManager>().AllLamps[0];
            tempArray[1] = vehicle.GetComponent<GHPC.Equipment.EquipmentManager>().AllLamps[1];
            tempArray[2] = vehicle.GetComponent<GHPC.Equipment.EquipmentManager>().AllLamps[2];
            vehicle.GetComponent<GHPC.Equipment.EquipmentManager>().AllLamps = tempArray;
            vehicle.GetComponent<GHPC.Equipment.EquipmentManager>().lampsDict.Remove(GHPC.Equipment.Lamps.LampType.GunnerSpotlight);
            vehicle._nightVisionType = NightVisionType.Thermal;
            vehicle._targetSpotterSettings._bestNightVision = GHPC.AI.TargetSpotterSettings.NightVisionQuality.Gen1Thermal;            
            dayCam.NightSightAtNightOnly = false;

            //Augmenting the reticle in the primary sight
            ReticleMesh reticle_mesh = vehicle.transform.Find("LEO1A1A1_rig/HULL/TURRET/--Turret Scripts--/Sights/GPS/Reticle Mesh").GetComponent<ReticleMesh>();
            reticle_mesh.reticleSO.lights[0].color = new Reticle.RGB(4f, 0f, 0f, true);
            ReticleTree.Angular design = reticle_mesh.reticleSO.planes[0].elements[0] as ReticleTree.Angular;
            if (design.elements.Count == 14) {
                ReticleTree.Circle circle = new ReticleTree.Circle(position: new Vector2(0f, 0f), degrees: 0f, radius: 0.25f, thickness: 0.05f, iterations: 12);
                circle.illumination = ReticleTree.Light.Type.Powered;
                circle.visualType = ReticleTree.VisualElement.Type.ReflectedAdditive;
                design.elements.Add(circle);            
                reticle_mesh.Clear(false);
                reticle_mesh.SMR = null;
                reticle_mesh.GenerateMesh();
                reticle_mesh.Load();
            }

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
            Transform loader_station = vehicle.transform.Find("LEO1A1A1_rig/HULL/TURRET/lafette001");
            GameObject fnMag = GameObject.Instantiate(fn_mag, loader_station);
            fnMag.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 180f));
            fnMag.transform.localPosition = new Vector3(0f, 0.05f, -0.08f);
            loader_station.transform.Find("MG3").gameObject.SetActive(false);

            //Configuring Ammunition
            string ammo_loadout = CanadianLeopardsClass.ammo_loadout.Value;
            if (ammo_loadout != "German" || ammo_loadout != "german" || ammo_loadout != "GERMAN")
            {
                LoadoutManager loadout_manager = vehicle.GetComponent<LoadoutManager>();

                if (ammo_loadout == "historical" || ammo_loadout == "Historical" || (ammo_loadout == "HISTORICAL")) { AmmoSwaps.HistoricalLoad(maingun, loadout_manager); }
                else if (ammo_loadout == "American" || ammo_loadout == "american" || ammo_loadout == "AMERICAN") { AmmoSwaps.AmericanLoad(maingun, loadout_manager); }
                else { Utilities.Log("Unknown value for ammo loadout, using mission defaults"); }
            }

            //Texture cosmetics
            GameObject de_markings;
            de_markings = vehicle.transform.Find("LEO1A1_markings").gameObject;
            de_markings.SetActive(false);
            GameObject cross = vehicle.transform.Find("LEO1A1A1_rig/HULL/TURRET/kreuz").gameObject;
            Material cross_mat = cross.GetComponent<MeshRenderer>().material;
            cross.SetActive(false);

            GameObject active_hull;
            MeshRenderer base_mr;
            
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
            
            base_mr = active_hull.GetComponent<MeshRenderer>();
            base_mr.material.SetTexture("_Albedo", A1_base);
            base_mr.material.SetTexture("_PaintMask", A1_camomask);
            if (CanadianLeopardsClass.no_threecolour.Value) base_mr.material.SetFloat("_CamoAmount", 0f);
            
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
            

            //The Iron-Cross decals have weird UVs so we need to create custom meshes
            GameObject turret = vehicle.transform.Find("LEO1A1A1_rig/HULL/TURRET").gameObject;
            GameObject maple_left = new GameObject("Mapleleaf_left");
            maple_left.transform.parent = turret.transform;
            maple_left.transform.position = turret.transform.position;
            Utilities.NewQuad(maple_left, cross_mat, maple);
            maple_left.transform.localScale = new Vector3(0.165f, 0.165f, 0.165f);
            maple_left.transform.localPosition += new Vector3(-1.135f, 0.6f, -0.15f);
            maple_left.transform.localRotation = Quaternion.Euler(new Vector3(0f, 10f, 60f));            
            RendererMaterial maple_left_rm = new RendererMaterial { Renderer = maple_left.GetComponent<MeshRenderer>() };

            GameObject maple_right = new GameObject("Mapleleaf_right");
            maple_right.transform.parent = turret.transform;
            maple_right.transform.position = turret.transform.position;
            Utilities.NewQuad(maple_right, cross_mat, maple);
            maple_right.transform.localScale = new Vector3(0.165f, 0.165f, 0.165f);            
            maple_right.transform.localPosition += new Vector3(1.135f, 0.6f, -0.15f);
            maple_right.transform.localRotation = Quaternion.Euler(new Vector3(0f, 170f, 60f));            
            RendererMaterial maple_right_rm = new RendererMaterial { Renderer = maple_right.GetComponent<MeshRenderer>() };

            FlammablesManager flamables = vehicle.GetComponent<FlammablesManager>();
            flamables._scorchRendererMaterials.Add(maple_left_rm);
            flamables._scorchRendererMaterials.Add(maple_right_rm);

            //Turret numbers: Company [1-4], Troop [1-4], Vic [blank, A B C]
            PlatoonData platoon = vehicle.Platoon;
            int position_in_platoon = 0;
            if (vehicle.Platoon == null) { position_in_platoon = Random.Range(0, 4); }
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
            numbers_go = vehicle.transform.Find("LEO1A1A1_rig/HULL/TURRET/NUMBERS").gameObject;
            MeshRenderer numbers = numbers_go.GetComponent<MeshRenderer>();
            numbers.material.mainTexture = callsigns;            

            numbers_go.transform.localPosition = new Vector3(1.115f, 0.31f, -1.345f); //moving turret numbers to back of the turret
            numbers_go.transform.localRotation = numbers_go.transform.localRotation * Quaternion.Euler(0f, 90f, 7f);
            numbers_go.transform.localScale = new Vector3(0.48f, 0.48f, 0.4f);
            

            Vector3[] num_vertices = numbers_go.GetComponent<MeshFilter>().sharedMesh.vertices;
            
            for (int i = 0; i < 21; i++)
            {
                num_vertices[i] = new Vector3(0f, 0f, 0f); //removing unneeded double image
            }
            num_vertices[26] += new Vector3(-0.03f, 0f, 0f); //flattening out the bottom-right corner
            num_vertices[28] += new Vector3(-0.03f, 0f, 0f);
            num_vertices[29] += new Vector3(-0.03f, 0f, 0f);
            
            numbers_go.GetComponent<MeshFilter>().sharedMesh.vertices = num_vertices;

            GameObject hull_numbers = new GameObject("hull callsign"); //new number decal for the back of the hull
            hull_numbers.transform.parent = active_hull.transform;
            hull_numbers.AddComponent<MeshFilter>();
            hull_numbers.AddComponent<MeshRenderer>();
            hull_numbers.GetComponent<MeshRenderer>().material = numbers.material;
            hull_numbers.GetComponent<MeshFilter>().mesh = numbers_go.GetComponent<MeshFilter>().mesh;
            hull_numbers.transform.position = active_hull.transform.position;            
            hull_numbers.transform.localPosition += new Vector3(1.88f, 3.12f, -8.9f);
            hull_numbers.transform.localRotation = Quaternion.Euler(0f, 89f, -16f);
            hull_numbers.transform.localScale = new Vector3(1f, 1f, 1.1f);
            

            if (position_in_platoon == 0) //centers decals for troop-leaders
            {                
                numbers_go.transform.localPosition += new Vector3(0.07f, 0f, 0f);
                hull_numbers.transform.localPosition += new Vector3(0.2f, 0f, 0f);                
            }

            if (CanadianLeopardsClass.additional_decals.Value)
            {
                //NATO map symbol type decal, front and back
                GameObject hull_tac_front = new GameObject("tactical sign front");
                hull_tac_front.transform.parent = active_hull.transform;
                hull_tac_front.transform.position = active_hull.transform.position;
                Utilities.NewQuad(hull_tac_front, numbers.material, tac);
                hull_tac_front.transform.localPosition += new Vector3(-1.45f, 2.4f, 2.25f);
                hull_tac_front.transform.localRotation = Quaternion.Euler(new Vector3(330f, 180f, 0f));
                hull_tac_front.transform.localScale = new Vector3(0.25f, 0.25f, 0.18f);                

                GameObject hull_tac_rear = new GameObject("tactical sign rear");
                hull_tac_rear.transform.parent = active_hull.transform;
                hull_tac_rear.transform.position = active_hull.transform.position;
                Utilities.NewQuad(hull_tac_rear, numbers.material, tac);
                hull_tac_rear.transform.localPosition += new Vector3(-1.78f, 2.8f, -10.85f);
                hull_tac_rear.transform.localRotation = Quaternion.Euler(new Vector3(-100f, 0f, 0f));
                hull_tac_rear.transform.localScale = new Vector3(0.2f, 0.2f, 0.14f);                

                GameObject mlc_decal = new GameObject("MLC decal");
                mlc_decal.transform.parent = active_hull.transform;
                mlc_decal.transform.position = active_hull.transform.position;
                Utilities.NewQuad(mlc_decal, numbers.material, mlc);
                mlc_decal.transform.localPosition += new Vector3(1.6f, 3f, 1.15f);
                mlc_decal.transform.localRotation = Quaternion.Euler(new Vector3(330f, 180f, 0f));
                mlc_decal.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);                
            }
            Utilities.Log("Conversions complete on " + vehicle_go.name);
        }
    }
}
