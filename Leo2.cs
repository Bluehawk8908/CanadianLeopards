using UnityEngine;
using GHPC;
using GHPC.Camera;
using GHPC.Equipment;
using GHPC.Equipment.Optics;
using GHPC.Vehicle;
using GHPC.Effects;
using GHPC.Effects.Voices;
using GHPC.PhysicsHelpers;
using GHPC.Weapons;
using GHPC.AI.Platoons;
using GHPC.Utility;

namespace CanadianLeopards
{
    public class ChevronsGizmo : MonoBehaviour
    {
        public FlammablesManager FlameManager;
        public Material[] Chevrons;

        void LateUpdate()
        {
            if (FlameManager == null || Chevrons == null) return;
            float scorch = FlameManager.CurrentScorchRatio * 1.35f;
            foreach (Material chevron in Chevrons)
            {
                chevron.SetVector("_Color", new Vector4((1f - scorch), (1f - scorch), (1f - scorch), 1f));
            }
            if (FlameManager._fullyScorched) enabled = false;
        }
    }
    
    class Leo2
    {
        public static void Convert(Vehicle vehicle)
        {
            GameObject vehicle_go = vehicle.gameObject;
            vehicle_go.AddComponent<CanLepConverted>();

            GameObject american_crew_voice = Utilities.american_crew_voice;
            GameObject leo2a1_hull = Utilities.leo2a1_hull;
            GameObject leo2a1_turret = Utilities.leo2a1_turret;
            GameObject emes15 = Utilities.emes15;
            GameObject mg_mount = Utilities.mg_mount;
            GameObject fn_mag = Utilities.fn_mag;
            GameObject bustle_baskets = Utilities.bustle_baskets;
            GameObject basket_colliders = Utilities.basket_colliders;
            GameObject jerry_can = Utilities.jerry_can;
            GameObject new_net = Utilities.new_net;
            Texture2D L2_base = Utilities.L2_base;
            Texture2D L2_camomask = Utilities.L2_camomask;
            Texture2D L2_tan = Utilities.L2_tan;
            Texture2D L2_tan_mask = Utilities.L2_tan_mask;
            Texture2D tan_net = Utilities.tan_net;
            Texture2D maple = Utilities.maple;
            Texture2D maple_black = Utilities.maple_black;
            Texture2D callsigns = Utilities.callsigns;
            Texture2D callsigns_black = Utilities.callsigns_black;
            Texture2D tac = Utilities.tac;
            Texture2D tac_generic = Utilities.tac_generic;
            Texture2D mlc60 = Utilities.mlc60;
            Texture2D mlc60_black = Utilities.mlc60_black;
            Texture2D chevron;
            if (CanadianLeopardsClass.leo2a4_chevrons.Value == 2) chevron = Utilities.chevron_gold;
            else chevron = Utilities.chevron_white;
            Texture2D chevron_black = Utilities.chevron_black;

            if (vehicle.UniqueName == "LEO2") vehicle._friendlyName = "Leopard 2A3"; 
            else vehicle._friendlyName = "Leopard 2A4 CAN";  //New display name

            vehicle.transform.Find("DE Tank Voice").gameObject.SetActive(false); //Adding US Voices
            GameObject new_voice = GameObject.Instantiate(american_crew_voice, vehicle.transform);
            new_voice.transform.localPosition = new Vector3(0, 0, 0);
            new_voice.transform.localEulerAngles = new Vector3(0, 0, 0);
            CrewVoiceHandler handler = new_voice.GetComponent<CrewVoiceHandler>();
            handler._chassis = vehicle._chassis as NwhChassis;
            vehicle._crewVoiceHandler = handler;
            new_voice.SetActive(true);

            Transform turret = vehicle_go.transform.Find("leo2_rig/hull/turret");

            //enforcing FLIR night channel
            if (vehicle.UniqueName == "LEO2")
            {                
                Transform gunSights = vehicle.transform.Find("leo2_rig/hull/turret/---TURRET SCRIPTS---/GPS");
                FireControlSystem fcs = gunSights.GetComponent<FireControlSystem>();
                GameObject new_optic = GameObject.Instantiate(emes15.transform.Find("FLIR").gameObject, gunSights);
                CameraSlot flirCam = new_optic.GetComponent<CameraSlot>();
                UsableOptic flirOptic = new_optic.GetComponent<UsableOptic>();
                CameraSlot dayCam = gunSights.Find("Optic").GetComponent<CameraSlot>();
                CameraSlot auxCam = dayCam.SlotToUseOnDestroy;                

                flirCam.LinkedDaySight = dayCam;
                flirCam.ExclusiveItems = new GameObject[] { gunSights.transform.Find("EMES-15 canvas").gameObject };
                flirCam.IsLinkedNightSight = true;
                dayCam.LinkedNightSight = flirCam;
                auxCam.LinkedNightSight = flirCam;

                flirOptic.FCS = fcs;                
                flirOptic.LevelTransform = turret;
                flirOptic.ReadyToFireObject = gunSights.transform.Find("EMES-15 canvas/-- ready to fire").gameObject;
                flirOptic.UnreadyToFireObject = gunSights.transform.Find("EMES-15 canvas/-- unready to fire").gameObject;
                flirOptic.RangeText = gunSights.transform.Find("EMES-15 canvas/-- range readout").GetComponent<TMPro.TextMeshProUGUI>();
            }

            if (vehicle.UniqueName != "LEO2") //Leopard 2A3 will use c. 1985 ammo still, 2A4s are upgraded
            {
                LoadoutManager loadout_manager = vehicle_go.GetComponent<LoadoutManager>();
                AmmoSwaps.Modern2A4(loadout_manager);
            }

            //Replacing Coax MG
            WeaponSystemInfo coax = vehicle.GetComponent<WeaponsManager>().Weapons[1];
            coax.Name = "7.62mm machine gun C6";
            AmmoFeed coax_ammo = coax.Weapon.Feed;
            coax_ammo._totalCycleTime = 0.08f;
            coax.Weapon.WeaponSound.LoopEventPath = "event:/Weapons/MG_m240_750rmp";

            //replacing loader hatch MG                        
            GameObject mount = GameObject.Instantiate(mg_mount, turret);                      
            mount.transform.localPosition = new Vector3(-0.91f, 0.975f, -0.53f);
            mount.transform.localRotation = Quaternion.Euler(new Vector3(271f, 177.16f, 0f));
            GameObject c6 = GameObject.Instantiate(fn_mag, mount.transform);
            c6.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            c6.transform.localPosition = new Vector3(0f, -0.0004f, -0.0008f);
            c6.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 180f));

            //enforcing 2A3 cosmetics
            Transform mesh = vehicle_go.transform.Find("LEO2_mesh");
            if (vehicle.UniqueName == "LEO2") { 
                GameObject.Instantiate(leo2a1_hull, mesh);
                GameObject newTurret = GameObject.Instantiate(leo2a1_turret, mesh);
                SkinnedMeshRenderer newTurret_mr = newTurret.GetComponent<SkinnedMeshRenderer>();
                Transform[] bones = new Transform[] { vehicle_go.transform.Find("leo2_rig/hull/turret/mantlet"),
                                                      vehicle_go.transform.Find("leo2_rig/hull/turret"),
                                                      vehicle_go.transform.Find("leo2_rig/hull/turret/mantlet/muzzle"),
                                                      vehicle_go.transform.Find("leo2_rig/hull/turret/peri") };
                newTurret_mr.bones = bones;
                newTurret_mr.rootBone = bones[1];
                newTurret_mr.material.SetTexture("_Albedo", L2_base);
                newTurret_mr.material.SetTexture("_PaintMask", L2_camomask);
                newTurret_mr.material.SetFloat("_CamoAmount", 0f);
            }

            mesh.transform.Find("hull 2A0").gameObject.SetActive(false);
            mesh.transform.Find("turret A0").gameObject.SetActive(false);

            if (vehicle.UniqueName != "LEO2")
            {
                if (mesh.transform.Find("hull 2A1") != null) mesh.transform.Find("hull 2A1").gameObject.SetActive(false);
                if (mesh.transform.Find("hull 2A1 late") != null)
                {
                    mesh.transform.Find("hull 2A1 late").gameObject.SetActive(true);
                    mesh.transform.Find("hull 2A1 late").GetComponent<MeshRenderer>().material.SetTexture("_Albedo", L2_base);
                    mesh.transform.Find("hull 2A1 late").GetComponent<MeshRenderer>().material.SetTexture("_PaintMask", L2_camomask);
                }

                if (mesh.transform.Find("turret A2") != null) mesh.transform.Find("turret A2").gameObject.SetActive(false);
                if (mesh.transform.Find("turret A1") != null) mesh.transform.Find("turret A1").gameObject.SetActive(true);
            }

            Material common2a4Mat = mesh.Find("hull common").GetComponent<MeshRenderer>().material;
            common2a4Mat.SetTexture("_Albedo", L2_base);
            common2a4Mat.SetTexture("_PaintMask", L2_camomask);
            if (vehicle.UniqueName == "LEO2A2") { common2a4Mat.SetFloat("_CamoAmount", 0.97f); } //2A1 and 2A2 both upgrade to 2A4, the latter will have camo
            if (CanadianLeopardsClass.leo2a4_camo.Value == 0) { common2a4Mat.SetFloat("_CamoAmount", 0f); }
            if (CanadianLeopardsClass.leo2a4_camo.Value == 2) { 
                common2a4Mat.SetTexture("_CamoLayer", L2_tan); 
                common2a4Mat.SetTexture("_PaintMask", L2_tan_mask);
            }
            mesh.Find("hatches").GetComponent<SkinnedMeshRenderer>().material = common2a4Mat;
            mesh.Find("running gear").GetComponent<SkinnedMeshRenderer>().material = common2a4Mat;            
            mesh.Find("rear skirt down").GetComponent<MeshRenderer>().material = common2a4Mat;            
            mesh.Find("rear skirt unclipped").GetComponent<SkinnedMeshRenderer>().material = common2a4Mat;
            if (vehicle.UniqueName != "LEO2") { 
                mesh.transform.Find("turret A1").GetComponent<SkinnedMeshRenderer>().material = common2a4Mat;
                mesh.transform.Find("hull 2A1 late").GetComponent<MeshRenderer>().material = common2a4Mat; 
            }
            turret.transform.Find("turret common parts").GetComponent<MeshRenderer>().material = common2a4Mat;            
            turret.transform.Find("blowoff panel").GetComponent<MeshRenderer>().material = common2a4Mat;

            GameObject baskets = GameObject.Instantiate(bustle_baskets, turret);
            baskets.transform.GetComponent<MeshRenderer>().material = common2a4Mat;
            baskets.transform.localPosition = new Vector3(0f, -1.5f, -0.25f);
            RendererMaterial baskets_rm = new RendererMaterial { Renderer = baskets.GetComponent<MeshRenderer>() };

            GameObject turret_armours = turret.GetComponent<LateFollowTarget>().LateFollowers[0].transform.Find("LEO2_armour_turret").gameObject;
            GameObject basketArmourContainer = GameObject.Instantiate(basket_colliders, turret_armours.transform);            
            GameObject basketArmour = basketArmourContainer.transform.Find("default").gameObject;
            basketArmour.tag = "Penetrable";
            basketArmour.gameObject.layer = 8;
            basketArmourContainer.transform.localPosition = new Vector3(0f, 0.1f, 0.57f); 
            MeshCollider basketCollider = basketArmour.AddComponent<MeshCollider>();            
            basketCollider.enabled = false;
            UniformArmor doghouse_armour = basketArmour.AddComponent<UniformArmor>();
            doghouse_armour._name = "turret bustle basket";
            doghouse_armour._surfaceMaterial = ParticleEffectsManager.SurfaceMaterial.Steel;
            doghouse_armour.PrimaryHeatRha = 3f;
            doghouse_armour.PrimarySabotRha = 3f;
            doghouse_armour._canShatterLongRods = false;
            doghouse_armour.AngleMatters = false;
            doghouse_armour._normalizesHits = false;
            vehicle_go.GetComponent<RaycastColliderGroup>().childColliders.Add(basketCollider);

            //maple leaves
            Transform markings = vehicle_go.transform.Find("leo2_rig/hull/turret/LEO2_markings");
            Material cross_mat = markings.transform.Find("cross-big").GetComponent<MeshRenderer>().material;
            cross_mat.renderQueue = 2450;
            if (vehicle.UniqueName != "LEO2") cross_mat.SetVector("_background", new Vector4(-1.5f, -1.5f, -1.5f, 1f));
            markings.transform.Find("cross-big").gameObject.SetActive(false);
            markings.transform.Find("cross-big-late").gameObject.SetActive(false);
            markings.transform.Find("cross-small").gameObject.SetActive(false);            

            GameObject maple_left = new GameObject("Mapleleaf_left");
            maple_left.transform.parent = markings.transform;
            maple_left.transform.position = markings.transform.position;
            if (vehicle.UniqueName == "LEO2") { Utilities.NewQuad(maple_left, cross_mat, maple); }
            else { Utilities.NewQuad(maple_left, cross_mat, maple_black); }
            maple_left.transform.localScale = new Vector3(0.165f, 0.165f, 0.165f);
            maple_left.transform.localPosition = new Vector3(-1.23f, 2.175f, 0.16f);
            maple_left.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 90f));
            RendererMaterial maple_left_rm = new RendererMaterial { Renderer = maple_left.GetComponent<MeshRenderer>() };

            GameObject maple_right = new GameObject("Mapleleaf_right");
            maple_right.transform.parent = markings.transform;
            maple_right.transform.position = markings.transform.position;
            if (vehicle.UniqueName == "LEO2") { Utilities.NewQuad(maple_right, cross_mat, maple); }
            else { Utilities.NewQuad(maple_right, cross_mat, maple_black); }
            maple_right.transform.localScale = new Vector3(0.165f, 0.165f, 0.165f);
            maple_right.transform.localPosition = new Vector3(1.33f, 2.1f, 0.1f);
            maple_right.transform.localRotation = Quaternion.Euler(new Vector3(0f, 180f, 90f));
            RendererMaterial maple_right_rm = new RendererMaterial { Renderer = maple_right.GetComponent<MeshRenderer>() };

            GameObject maple_lower_front = new GameObject("Mapleleaf_lower_front");
            maple_lower_front.transform.parent = vehicle_go.transform.Find("leo2_rig/hull");
            maple_lower_front.transform.position = vehicle_go.transform.Find("leo2_rig/hull").position;
            Utilities.NewQuad(maple_lower_front, cross_mat, maple_black);
            maple_lower_front.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
            maple_lower_front.transform.localPosition = new Vector3(0.21f, 0.145f, 3.435f);
            maple_lower_front.transform.localRotation = Quaternion.Euler(new Vector3(0f, 90f, 135f));
            maple_lower_front.GetComponent<MeshRenderer>().material.SetVector("_background", new Vector4(-1.3f, -1.2f, -1.3f, 0f));

            GameObject maple_lower_rear = new GameObject("Mapleleaf_lower_rear");
            maple_lower_rear.transform.parent = vehicle_go.transform.Find("leo2_rig/hull");
            maple_lower_rear.transform.position = vehicle_go.transform.Find("leo2_rig/hull").position;
            Utilities.NewQuad(maple_lower_rear, cross_mat, maple_black);
            maple_lower_rear.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
            maple_lower_rear.transform.localPosition = new Vector3(0.31f, 0.33f, -3.92f);
            maple_lower_rear.transform.localRotation = Quaternion.Euler(new Vector3(0f, 270f, 120f));
            maple_lower_rear.GetComponent<MeshRenderer>().material.SetVector("_background", new Vector4(-1.3f, -1.2f, -1.3f, 0f));

            FlammablesManager flammables = vehicle.GetComponent<FlammablesManager>();
            flammables._scorchRendererMaterials.Add(maple_left_rm);
            flammables._scorchRendererMaterials.Add(maple_right_rm);
            flammables._scorchRendererMaterials.Add(baskets_rm);


            //callsigns - Company [1-4], Troop [1-4], Vic [blank, A B C]
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
            MergedVehicleNumberControl[] components = markings.GetComponents<MergedVehicleNumberControl>();
            MergedVehicleNumberControl turret_numbers = new MergedVehicleNumberControl();
            MergedVehicleNumberControl cfr_numbers = new MergedVehicleNumberControl();
            foreach (var mvnc in components)
            {
                if (mvnc.Type == VehicleDecalType.UnitNumber) { turret_numbers = mvnc; }
                if (mvnc.Type == VehicleDecalType.LicensePlate) { cfr_numbers = mvnc; }
            }

            if (turret_numbers._allValues[0] == 9) { turret_numbers._allValues[0] = 1; }
            else if (turret_numbers._allValues[0] > 4) { turret_numbers._allValues[0] -= 4; } //5-8 get remapped onto 1-4
            if (turret_numbers._allValues[1] == 9) { turret_numbers._allValues[1] = 1; }
            else if (turret_numbers._allValues[1] > 4) { turret_numbers._allValues[1] -= 4; }
            else if (turret_numbers._allValues[1] == 0) { turret_numbers._allValues[1] = 1; }
            turret_numbers._allValues[2] = position_in_platoon + 6; // 6-9 in the texture will become letter codes
            turret_numbers.RefreshDecals();

            GameObject numbers = vehicle_go.transform.Find("leo2_rig/hull/turret/LEO2_markings/numbers").gameObject;
            if (vehicle.UniqueName == "LEO2") { numbers.GetComponent<MeshRenderer>().material.mainTexture = callsigns; }
            else { 
                numbers.GetComponent<MeshRenderer>().material.mainTexture = callsigns_black;
                numbers.GetComponent<MeshRenderer>().material.SetVector("_Color", new Vector4(1f, 1f, 1f, 1f));
            }

            cfr_numbers._allValues[0] = 8; //CFR numbers for Leo2 hulls are 85-72XXX to 85-73XXX
            cfr_numbers._allValues[1] = 5;
            cfr_numbers._allValues[2] = 36; //hyphen
            cfr_numbers._allValues[3] = 7;
            if (cfr_numbers._allValues[4] > 3 || cfr_numbers._allValues[4] < 2) cfr_numbers._allValues[4] = 2;            
            cfr_numbers.RefreshDecals();
            Material cfr_mat = vehicle_go.transform.Find("leo2_rig/hull/license number").GetComponent<MeshRenderer>().material;
            cfr_mat.SetVector("_fieldcolour", new Vector4(0.1f, 0.1f, 0.1f, 0f));
            cfr_mat.SetVector("_numeralcolours", new Vector4(0.1f, 0.1f, 0f, 0f));

            Transform number_decal = markings.Find("numbers");
            if (position_in_platoon == 0) { number_decal.localPosition = new Vector3(1.16f, 2.06f, -2.11f); }
            else { number_decal.localPosition = new Vector3(1.1f, 2.06f, -2.11f); }
            number_decal.localRotation = Quaternion.Euler(new Vector3(270f, 90f, 0f));
            number_decal.localScale = new Vector3(0.8f, 0.8f, 0.8f);

            turret.Find("Camo Nets/nato_camonet_folded").localPosition = new Vector3(1.07f, 0.6281f, -1.45f);
            turret.Find("Camo Nets/nato_camonet_folded").localRotation = Quaternion.Euler(new Vector3(0f, 75f, 0.4176f));
            turret.Find("Camo Nets/nato_camonet_folded").localScale = new Vector3(1.05f, 0.816f, 0.8f);

            turret.Find("Camo Nets/nato_camonet_folded (2)").localPosition = new Vector3(2.5f, 0.646f, -1.19f);
            turret.Find("Camo Nets/nato_camonet_folded (2)").localRotation = Quaternion.Euler(new Vector3(0f, 0f, 176.8327f));
            turret.Find("Camo Nets/nato_camonet_folded (2)").localScale = new Vector3(1.3f, 0.8f, 1f);

            GameObject jerry = GameObject.Instantiate(jerry_can, turret);
            jerry.transform.localPosition = new Vector3(1.18f, 0.25f, -2.8f);
            jerry.transform.localRotation = Quaternion.Euler(new Vector3(270f, 20f, 0f));

            GameObject turret_net = turret.transform.Find("net_turret").gameObject;
            if (turret_net.activeSelf)
            {
                GameObject newNet = GameObject.Instantiate(new_net, turret);
                Material camoNet_mat = turret_net.GetComponent<MeshRenderer>().material;
                newNet.transform.Find("default").GetComponent<MeshRenderer>().material = camoNet_mat;
                newNet.transform.localPosition = new Vector3(0f, -1.42f, -0.3f);
                newNet.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
                turret_net.SetActive(false);
                RendererMaterial newNet_flutter = new RendererMaterial();
                newNet_flutter.Renderer = newNet.transform.Find("default").GetComponent<MeshRenderer>();
                FlutterController flutCon = vehicle_go.transform.Find("LEO2_camo").GetComponent<FlutterController>();
                flutCon._flutterRendererMaterials.Add(newNet_flutter);
                Transform[] transList = new Transform[5];
                for (int i = 0; i < 4; i++) { transList[i] = flutCon._flutterRenderersParents[i]; }
                transList[4] = newNet.transform.Find("default");
                flutCon._flutterRenderersParents = transList;
                flutCon._maxVelocity = 25f;

                if (vehicle.UniqueName != "LEO2" && CanadianLeopardsClass.leo2a4_camo.Value == 2)
                {
                    newNet.transform.Find("default").GetComponent<MeshRenderer>().material.SetTexture("_basecolour", tan_net);
                    turret.Find("mantlet/net_gun").GetComponent<MeshRenderer>().material.SetTexture("_basecolour", tan_net);
                    turret.Find("mantlet/muzzle/net_muzzle").GetComponent<MeshRenderer>().material.SetTexture("_basecolour", tan_net);
                    vehicle_go.transform.Find("LEO2_camo/net_hull").GetComponent<MeshRenderer>().material.SetTexture("_basecolour", tan_net);
                }
            }

            //MLC number and tactical sign
            if (CanadianLeopardsClass.leo2_mlc.Value != 0)
            {
                GameObject mlc_decal = new GameObject("MLC decal");
                mlc_decal.transform.parent = vehicle_go.transform.Find("leo2_rig/hull");
                mlc_decal.transform.position = vehicle_go.transform.Find("leo2_rig/hull").position;
                if (CanadianLeopardsClass.leo2_mlc.Value == 1) { Utilities.NewQuad(mlc_decal, numbers.GetComponent<MeshRenderer>().material, mlc60); }
                else { Utilities.NewQuad(mlc_decal, numbers.GetComponent<MeshRenderer>().material, mlc60_black); }
                mlc_decal.transform.localPosition += new Vector3(1.41f, 0.385f, 3.58f);
                mlc_decal.transform.localRotation = Quaternion.Euler(new Vector3(300f, 180f, 0f));
                mlc_decal.transform.localScale = new Vector3(0.08f, 0.08f, 0.08f);
            }
                        
            GameObject tactical = new GameObject("tactical unit symbol");
            tactical.transform.parent = vehicle_go.transform.Find("leo2_rig/hull");
            tactical.transform.position = vehicle_go.transform.Find("leo2_rig/hull").position;
            if ((vehicle.UniqueName == "LEO2" && CanadianLeopardsClass.additional_decals.Value) || 
                (vehicle.UniqueName != "LEO2" && CanadianLeopardsClass.leo2a4_tactical.Value == 1)) 
                { Utilities.NewQuad(tactical, numbers.GetComponent<MeshRenderer>().material, tac); }
            else if (vehicle.UniqueName != "LEO2" && CanadianLeopardsClass.leo2a4_tactical.Value == 2) 
                { Utilities.NewQuad(tactical, numbers.GetComponent<MeshRenderer>().material, tac_generic); }
            tactical.transform.localPosition += new Vector3(-1.42f, 0.385f, 3.58f);
            tactical.transform.localRotation = Quaternion.Euler(new Vector3(300f, 180f, 0f));
            tactical.transform.localScale = new Vector3(0.08f, 0.08f, 0.07f);

            //chevrons
            if (vehicle.UniqueName != "LEO2" && CanadianLeopardsClass.leo2a4_chevrons.Value != 0) {
                GameObject chevron_hleft = new GameObject("chevron_hull_left");
                chevron_hleft.transform.parent = vehicle_go.transform.Find("leo2_rig/hull");
                chevron_hleft.transform.position = vehicle_go.transform.Find("leo2_rig/hull").position;
                if (CanadianLeopardsClass.leo2a4_camo.Value == 2 && common2a4Mat.GetFloat("_CamoAmount") > 0.5f) { 
                    Utilities.NewQuad(chevron_hleft, numbers.GetComponent<MeshRenderer>().material, chevron_black); }
                else { Utilities.NewQuad(chevron_hleft, numbers.GetComponent<MeshRenderer>().material, chevron); }
                Material chevMat = chevron_hleft.GetComponent<MeshRenderer>().material;
                chevMat.SetFloat("_Glossiness", 0f);
                chevron_hleft.transform.localScale = new Vector3(0.3f, 0.3f, 0.2f);
                chevron_hleft.transform.localPosition = new Vector3(-1.88f, 0.28f, 1.62f);
                chevron_hleft.transform.localRotation = Quaternion.Euler(new Vector3(270f, 90f, 0f));                

                GameObject chevron_hright = new GameObject("chevron_hull_right");
                chevron_hright.transform.parent = vehicle_go.transform.Find("leo2_rig/hull");
                chevron_hright.transform.position = vehicle_go.transform.Find("leo2_rig/hull").position;
                if (CanadianLeopardsClass.leo2a4_camo.Value == 2 && common2a4Mat.GetFloat("_CamoAmount") > 0.5f) { 
                    Utilities.NewQuad(chevron_hright, chevMat, chevron_black); }
                else { Utilities.NewQuad(chevron_hright, chevMat, chevron); }
                chevron_hright.transform.localScale = new Vector3(0.3f, 0.3f, 0.2f);
                chevron_hright.transform.localPosition = new Vector3(1.88f, 0.28f, 1.64f);
                chevron_hright.transform.localRotation = Quaternion.Euler(new Vector3(270f, 270f, 0f));                

                GameObject chevron_tleft = new GameObject("chevron_turret_left");
                chevron_tleft.transform.parent = markings;
                chevron_tleft.transform.position = markings.position;
                if (CanadianLeopardsClass.leo2a4_camo.Value == 2 && common2a4Mat.GetFloat("_CamoAmount") > 0.5f) {
                    Utilities.NewQuad(chevron_tleft, chevMat, chevron_black); }
                else { Utilities.NewQuad(chevron_tleft, chevMat, chevron); }
                chevron_tleft.transform.localScale = new Vector3(0.25f, 0.25f, 0.18f);
                chevron_tleft.transform.localPosition = new Vector3(-1.23f, 1.82f, 0.55f);
                chevron_tleft.transform.localRotation = Quaternion.Euler(new Vector3(270f, 90f, 0f));                

                GameObject chevron_tright = new GameObject("chevron_turret_right");
                chevron_tright.transform.parent = markings;
                chevron_tright.transform.position = markings.position;
                if (CanadianLeopardsClass.leo2a4_camo.Value == 2 && common2a4Mat.GetFloat("_CamoAmount") > 0.5f) {
                    Utilities.NewQuad(chevron_tright, chevMat, chevron_black); }
                else { Utilities.NewQuad(chevron_tright, chevMat, chevron); }
                chevron_tright.transform.localScale = new Vector3(0.25f, 0.25f, 0.18f);
                chevron_tright.transform.localPosition = new Vector3(1.32f, 1.85f, 0.4f);
                chevron_tright.transform.localRotation = Quaternion.Euler(new Vector3(270f, 270f, 0f));

                Material[] chevrons = new Material[] { 
                    chevron_hleft.GetComponent<MeshRenderer>().material,
                    chevron_hright.GetComponent<MeshRenderer>().material,
                    chevron_tleft.GetComponent<MeshRenderer>().material,
                    chevron_tright.GetComponent<MeshRenderer>().material
                };
                ChevronsGizmo chevronsGizmo = vehicle_go.AddComponent<ChevronsGizmo>();
                chevronsGizmo.FlameManager = vehicle_go.GetComponent<FlammablesManager>();
                chevronsGizmo.Chevrons = chevrons;
            }

            //C-tech armour upgrades
            ArmorCodexScriptable ctechScriptable = ScriptableObject.CreateInstance<ArmorCodexScriptable>();
            ctechScriptable.name = "C-Tech composite";
            ArmorType ctechType = new ArmorType();
            ctechType.Name = "C-Tech composite";
            ctechType.CanRicochet = true;
            ctechType.CanShatterLongRods = true;
            ctechType.NormalizesHits = true;
            ctechType.ThicknessSource = ArmorType.RhaSource.Multipliers;
            ctechType.SpallAngleMultiplier = 0.9f;
            ctechType.SpallPowerMultiplier = 0.8f;
            ctechType.RhaeMultiplierCe = 2f;
            ctechType.RhaeMultiplierKe = 1f;
            ctechType.CrushThicknessModifier = 1f;
            ctechScriptable.ArmorType = ctechType;

            GameObject turretArmours = turret.GetComponent<LateFollowTarget>().LateFollowers[0]
                .transform.Find("LEO2_armour_turret/turret armour").gameObject;
            GameObject hullArmours = vehicle_go.GetComponent<LateFollowTarget>().LateFollowers[0]
                .transform.Find("LEO2_armour_hull").gameObject;
            GameObject mantletArmour = turret.Find("mantlet").GetComponent<LateFollowTarget>().LateFollowers[0]
                .transform.Find("LEO2_armour_turret/mantlet armour").gameObject;
            Material aarLighterBlue = new Material(Shader.Find("Standard"));
            aarLighterBlue.color = new Color(0f, 0.6325f, 1f, 0.4f);
            aarLighterBlue.renderQueue = 3000;
            aarLighterBlue.SetFloat("_Mode", 3f);
            aarLighterBlue.SetFloat("_DstBlend", 10f);
            aarLighterBlue.SetFloat("_SrcBlend", 1f);
            aarLighterBlue.shaderKeywords = new string[] { "_ALPHAPREMULTIPLY_ON" };
            aarLighterBlue.name = "see-through azure";

            if (vehicle.UniqueName != "LEO2") {
                VariableArmor[] compBlocks = new VariableArmor[] {
                    turretArmours.transform.Find("array left").GetComponent<VariableArmor>(),
                    turretArmours.transform.Find("array right").GetComponent<VariableArmor>(),
                    turretArmours.transform.Find("cheek array left A").GetComponent<VariableArmor>(),
                    turretArmours.transform.Find("cheek array right A").GetComponent<VariableArmor>(),
                    turretArmours.transform.Find("cheek array right B").GetComponent<VariableArmor>(),
                    hullArmours.transform.Find("lower glacis array").GetComponent<VariableArmor>(),
                    hullArmours.transform.Find("upper glacis array").GetComponent<VariableArmor>(),
                    hullArmours.transform.Find("front left array").GetComponent<VariableArmor>(),
                    hullArmours.transform.Find("front right array").GetComponent<VariableArmor>(),
                    mantletArmour.transform.Find("mantlet").GetComponent<VariableArmor>()};
                    
                foreach (var compBlock in compBlocks)
                {
                    compBlock._armorType = ctechScriptable;
                    compBlock.AverageRha += 400;
                    compBlock.Renderer.material = aarLighterBlue;
                    compBlock.gameObject.GetComponent<AarVisual>().AarMaterial = aarLighterBlue;                    
                }
            }

            Utilities.Log("Conversions complete on " + vehicle_go.name);
        }
    }
}
