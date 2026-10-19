using UnityEngine;
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
    public class Leo1A3
    {
        public static void Convert(Vehicle vehicle, GameObject american_crew_voice, ReticleMesh.CachedReticle crosshair, GameObject fn_mag, 
                Texture2D A3_base, Texture2D A3_camomask, Texture2D maple, Texture2D callsigns, Texture2D tac, Texture2D mlc)
        {
            GameObject vehicle_go = vehicle.gameObject;
            vehicle_go.AddComponent<CanLepConverted>();
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
            lrf_holder.transform.localPosition = new Vector3(0f, 0f, 0.5f);
            lrf_holder.transform.localRotation = Quaternion.identity;
            GHPC.Equipment.DestructibleComponent laser_dest = lrf_holder.AddComponent<GHPC.Equipment.DestructibleComponent>();
            laser_dest._health = 5f;
            laser_dest._fullHealth = 5f;
            laser_dest._pressureTolerance = 1f;
            laser_dest._name = "Laser Rangefinder";

            fcs.LaserAim = LaserAimMode.ImpactPoint;
            fcs.LaserComponent = laser_dest;
            fcs.LaserOrigin = lrf_holder.transform;
            fcs.MaxLaserRange = 4000f;
            //fcs.DynamicLead = true;
            fcs.SuperleadWeapon = true;
            fcs.SuperelevateWeapon = true;
            fcs.TraverseBufferSeconds = 0.5f;
            fcs._autoDumpViaPalmSwitches = true;
            fcs._originalSuperleadMode = true;
            fcs.ComputerNeedsPower = true;
            fcs.RecordTraverseRateBuffer = true;
            fcs._useSeparateLead = false;
            UsableOptic sabca = fcs.MainOptic;
            sabca.ForceHorizontalReticleAlign = true;
            sabca.RotateAzimuth = false;
            UnityEngine.Object.Destroy(fcs.OpticalRangefinder);
            CameraSlot sabca_cam = sabca.slot;

            //Ensuring PZB-200 Night Sight
            if (fcs.NightOptic == null || fcs.NightOptic.name == "PERI-R12")
            {
                GameObject pzb_go;
                GameObject aux_go;
                pzb_go = vehicle.transform.Find("LEO1A1A1_rig/HULL/TURRET/1A3 mantlet/--Gun Scripts--/PZB-200").gameObject;
                aux_go = vehicle.transform.Find("LEO1A1A1_rig/HULL/TURRET/1A3 mantlet/--Gun Scripts--/Aux sight TZF1A").gameObject;                
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
                fcs.NightOptic = pzb;

                vehicle.transform.Find("LEO1A3_mesh/1A3_PZB200").gameObject.SetActive(true);
                vehicle.transform.Find("LEO1A3_mesh/PERI R12").gameObject.SetActive(false);                
            }
            sabca_cam.NightSightAtNightOnly = false;

            //Changing the reticle in the primary sight
            GameObject reticle_mesh_go = vehicle.transform.Find("LEO1A1A1_rig/HULL/TURRET/--Turret Scripts--/Sights/GPS/Reticle Mesh").gameObject;
            ReticleMesh reticle_mesh = reticle_mesh_go.GetComponent<ReticleMesh>();
            reticle_mesh.reticleSO = crosshair.tree;
            reticle_mesh.reticle = crosshair;
            reticle_mesh.SMR = null;
            reticle_mesh.Load();

            RT_FocalPlane plane = reticle_mesh_go.transform.Find("FFP").GetComponent<RT_FocalPlane>();
            ReticleTree.Angular ang = (ReticleTree.Angular)plane.fp.elements[0];
            if (ang.align != ReticleTree.GroupBase.Alignment.Impact)
            {
                ang.align = ReticleTree.GroupBase.Alignment.Impact;
                ReticleTree.Angular aiming_cross = (ReticleTree.Angular)ang.elements[3];
                aiming_cross.align = ReticleTree.GroupBase.Alignment.None;

                ReticleTree.Line upper_line = new ReticleTree.Line(position: new Vector2(0f, 18f), degrees: 90f, length: 2f, thickness: 0.2f);
                ReticleTree.Line lower_line = new ReticleTree.Line(position: new Vector2(0f, -18f), degrees: 90f, length: 2f, thickness: 0.2f);
                ReticleTree.Line left_line = new ReticleTree.Line(position: new Vector2(18f, 0f), degrees: 0f, length: 2.5f, thickness: 0.2f);
                ReticleTree.Line right_line = new ReticleTree.Line(position: new Vector2(-18f, 0f), degrees: 0f, length: 2.5f, thickness: 0.2f);
                ReticleTree.Circle mysterious_dot = new ReticleTree.Circle(position: new Vector2(0f, 0f), degrees: 0f, radius: 0.5236f, thickness: 0.12f);
                upper_line.illumination = ReticleTree.Light.Type.NightIllumination;
                lower_line.illumination = ReticleTree.Light.Type.NightIllumination;
                left_line.illumination = ReticleTree.Light.Type.NightIllumination;
                right_line.illumination = ReticleTree.Light.Type.NightIllumination;
                mysterious_dot.illumination = ReticleTree.Light.Type.NightIllumination;

                plane.fp.elements.Add(new ReticleTree.Angular(position: new ReticleTree.Position(-12f, 12f), parent: null, align: ReticleTree.GroupBase.Alignment.Boresight)); //dot
                ReticleTree.Angular original_ret = (ReticleTree.Angular)plane.fp.elements[0];
                ReticleTree.Angular new_ang = (ReticleTree.Angular)plane.fp.elements[1];

                original_ret.elements.Add(upper_line); original_ret.elements.Add(lower_line);
                original_ret.elements.Add(left_line); original_ret.elements.Add(right_line);
                new_ang.elements = new System.Collections.Generic.List<ReticleTree.TransformElement> { mysterious_dot };
                reticle_mesh.Clear(false);
                reticle_mesh.SMR = null;
                reticle_mesh.GenerateMesh();
                reticle_mesh.Load();
            }

            ReticleTree.Light new_light = new ReticleTree.Light
            {
                color = new RGB(3f, 2f, 0, true),
                type = ReticleTree.Light.Type.Powered
            };
            reticle_mesh.lights[0].light = new_light;
            reticle_mesh.lightCols[1] = new Vector4(3f, 2f, 0f, 1f);
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
            Transform loader_station = vehicle.transform.Find("LEO1A1A1_rig/HULL/TURRET/lafette002");
            GameObject fnMag = GameObject.Instantiate(fn_mag, loader_station);
            fnMag.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 180f));
            fnMag.transform.localPosition = new Vector3(0f, 0.05f, -0.08f);
            loader_station.transform.Find("MG004").gameObject.SetActive(false);                     

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
            de_markings = vehicle.transform.Find("LEO1A3_markings").gameObject;           
            de_markings.SetActive(false);
            GameObject cross = vehicle.transform.Find("LEO1A1A1_rig/HULL/TURRET/kreuz").gameObject;
            Material cross_mat = cross.GetComponent<MeshRenderer>().material;
            cross.SetActive(false);

            GameObject active_hull;
            MeshRenderer base_mr;
            active_hull = vehicle.transform.Find("LEO1A3_mesh/1A3_hull").gameObject;            
            base_mr = active_hull.GetComponent<MeshRenderer>();
            base_mr.material.SetTexture("_Albedo", A3_base);             
            base_mr.material.SetTexture("_PaintMask", A3_camomask);            
            if (CanadianLeopardsClass.no_threecolour.Value) base_mr.material.SetFloat("_CamoAmount", 0f);

            GameObject a3_turret = vehicle.transform.Find("LEO1A3_mesh/A3 turret").gameObject;
            GameObject a3_skirt_cut = vehicle.transform.Find("LEO1A3_mesh/a3_skirt_cut").gameObject;
            GameObject a3_skirt_full = vehicle.transform.Find("LEO1A3_mesh/a3_skirt_full").gameObject;
            GameObject a3_wheels = vehicle.transform.Find("LEO1A3_mesh/running gear").gameObject;
            a3_turret.GetComponent<SkinnedMeshRenderer>().material = base_mr.material;
            a3_skirt_cut.GetComponent<MeshRenderer>().material = base_mr.material;
            a3_skirt_full.GetComponent<MeshRenderer>().material = base_mr.material;
            a3_wheels.GetComponent<SkinnedMeshRenderer>().material = base_mr.material;

            //The Iron-Cross decals have weird UVs so we need to create custom meshes
            GameObject turret = vehicle.transform.Find("LEO1A1A1_rig/HULL/TURRET").gameObject;
            GameObject maple_left = new GameObject("Mapleleaf_left");
            maple_left.transform.parent = turret.transform;
            maple_left.transform.position = turret.transform.position;
            Utilities.NewQuad(maple_left, cross_mat, maple);
            maple_left.transform.localScale = new Vector3(0.165f, 0.165f, 0.165f);
            maple_left.transform.localPosition += new Vector3(-1.15f, 0.628f, 0.08f);
            maple_left.transform.localRotation = Quaternion.Euler(new Vector3(3f, 0f, 63f));            
            RendererMaterial maple_left_rm = new RendererMaterial { Renderer = maple_left.GetComponent<MeshRenderer>() };

            GameObject maple_right = new GameObject("Mapleleaf_right");
            maple_right.transform.parent = turret.transform;
            maple_right.transform.position = turret.transform.position;
            Utilities.NewQuad(maple_right, cross_mat, maple);
            maple_right.transform.localScale = new Vector3(0.165f, 0.165f, 0.165f);
            maple_right.transform.localPosition += new Vector3(1.15f, 0.628f, 0.1f);
            maple_right.transform.localRotation = Quaternion.Euler(new Vector3(0f, 180f, 62f));            
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
            numbers_go = vehicle.transform.Find("LEO1A1A1_rig/HULL/TURRET/numbers").gameObject; 
            MeshRenderer numbers = numbers_go.GetComponent<MeshRenderer>();
            numbers.material.mainTexture = callsigns;
            //numbers.material.color = new Vector4(1f, 1f, 1f, 1f);
            //numbers.material.SetFloat("_Metallic", 0.698f);
            //numbers.material.SetFloat("_Glossiness", 0.346f);

            numbers_go.transform.localPosition = new Vector3(-0.42f, 0.55f, -1.245f);  //moving turret numbers to back of the turret
            numbers_go.transform.localRotation = Quaternion.Euler(270f, 270f, 0f);
            numbers_go.transform.localScale = new Vector3(1f, 0.85f, 1f);

            Vector3[] num_vertices = numbers_go.GetComponent<MeshFilter>().sharedMesh.vertices;
            
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

            numbers_go.GetComponent<MeshFilter>().sharedMesh.vertices = num_vertices;

            GameObject hull_numbers = new GameObject("hull callsign"); //new number decal for the back of the hull
            hull_numbers.transform.parent = active_hull.transform;
            hull_numbers.AddComponent<MeshFilter>();
            hull_numbers.AddComponent<MeshRenderer>();
            hull_numbers.GetComponent<MeshRenderer>().material = numbers.material;
            hull_numbers.GetComponent<MeshFilter>().mesh = numbers_go.GetComponent<MeshFilter>().mesh;
            hull_numbers.transform.position = active_hull.transform.position;
            hull_numbers.transform.localPosition += new Vector3(-0.6f, 4.35f, 1.6f);
            hull_numbers.transform.localRotation = Quaternion.Euler(355f, 0f, 270f);
            hull_numbers.transform.localScale = new Vector3(1f, 1f, 1f);

            if (position_in_platoon == 0) //centers decals for troop-leaders
            {
                numbers_go.transform.localPosition += new Vector3(0.105f, 0f, 0f);
                hull_numbers.transform.localPosition += new Vector3(0.1f, 0f, 0f);
            }

            if (CanadianLeopardsClass.additional_decals.Value)
            {
                //NATO map symbol type decal, front and back
                GameObject hull_tac_front = new GameObject("tactical sign front");
                hull_tac_front.transform.parent = active_hull.transform;
                hull_tac_front.transform.position = active_hull.transform.position;
                Utilities.NewQuad(hull_tac_front, numbers.material, tac);
                hull_tac_front.transform.localPosition += new Vector3(-0.75f, -1.15f, 1.2f);
                hull_tac_front.transform.localRotation = Quaternion.Euler(new Vector3(310f, 0f, 180f));
                hull_tac_front.transform.localScale = new Vector3(0.1f, 0.1f, 0.066f);

                GameObject hull_tac_rear = new GameObject("tactical sign rear");
                hull_tac_rear.transform.parent = active_hull.transform;
                hull_tac_rear.transform.position = active_hull.transform.position;
                Utilities.NewQuad(hull_tac_rear, numbers.material, tac);
                hull_tac_rear.transform.localPosition += new Vector3(-0.88f, 5.418f, 1.4f);
                hull_tac_rear.transform.localRotation = Quaternion.Euler(new Vector3(348f, 0f, 0f));
                hull_tac_rear.transform.localScale = new Vector3(0.1f, 0.1f, 0.066f);                

                GameObject mlc_decal = new GameObject("MLC decal");
                mlc_decal.transform.parent = active_hull.transform;
                mlc_decal.transform.position = active_hull.transform.position;
                Utilities.NewQuad(mlc_decal, numbers.material, mlc);
                mlc_decal.transform.localPosition += new Vector3(0.84f, -0.62f, 1.48f);
                mlc_decal.transform.localRotation = Quaternion.Euler(new Vector3(306f, 0f, 180f));
                mlc_decal.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            }
            Utilities.Log("Conversions complete on " + vehicle_go.name);
        }
    }
}
