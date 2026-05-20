using System.Collections.Generic;
using UnityEngine;
using GHPC.AI.Platoons;
using GHPC.Vehicle;
using GHPC.Utility;
using GHPC.Weapons;

namespace CanadianLeopards
{
    public class M113
    {
        public static void Convert(Vehicle vehicle, GameObject vehicle_go, AmmoFeed cal50, bool additional_decals, 
            Texture2D mapleAPC, Texture2D canInf, Texture2D canInf_nm, Texture2D canInf_sm, Texture2D callsigns, Texture2D apc, Texture2D flag, Texture2D tacAPC, Texture2D mlcAPC) 
        {
            Transform M2_init = vehicle_go.transform.Find("M2HB_rig (1)");
            if (M2_init != null) { M2_init.gameObject.SetActive(true); }
            Transform M2HB_t = vehicle_go.transform.Find("M113G_rig/HULL/Turret ring/MG elevation/MG ball pivot AZ/MG ball pivot EL/M2HB_rig (1)");
            if (M2HB_t != null) { M2HB_t.GetComponent<Reparent>().enabled = true; }
            GameObject weapon_scripts = vehicle_go.transform.Find("M113G_rig/HULL/Turret ring/MG elevation/MG ball pivot AZ/MG ball pivot EL/weapon scripts").gameObject;
            Transform mg3 = weapon_scripts.transform.Find("mg3");
            mg3.gameObject.SetActive(false);
            Transform M2HB_pintle = M2HB_t.transform.Find("PINTLE");
            M2HB_pintle.localPosition = new Vector3 ( 0.15f, 0f, 0.1f );
            
            WeaponSystem wep = weapon_scripts.transform.Find("7.62mm Machine Gun MG3").GetComponent<WeaponSystem>();
            AmmoFeed ammoFeed = weapon_scripts.transform.Find("7.62mm Machine Gun MG3").GetComponent<AmmoFeed>();
            GHPC.Weapons.AmmoRack ammoRack = weapon_scripts.transform.Find("MG3 ready rack").GetComponent<GHPC.Weapons.AmmoRack>();

            vehicle_go.GetComponent<WeaponsManager>().Weapons[0].Name = "12.7mm machine gun M2HB";
            wep.WeaponSound.LoopEventPath = "event:/Weapons/MG_m2_550rmp";
            wep._maxBurstSeconds = 1f;
            wep._minBurstSeconds = 0.3f;
            wep._muzzleIdentity.transform.localPosition = new Vector3(0.145f, 2.44f, 2.1f);

            Transform gunsight_cam = weapon_scripts.transform.Find("GUNSIGHT CAM 2");
            gunsight_cam.localPosition = new Vector3(0.1423f, 2.5385f, 0.373f);

            ammoRack.ClipTypes = cal50.ReadyRack.ClipTypes;
            List<AmmoType.AmmoClip> storedClips = new List<AmmoType.AmmoClip>();
            for (int i = 0; i < 10; i++)
            {
                storedClips.Add(ammoRack.ClipTypes[0]);
            }
            ammoRack.StoredClips = storedClips;
            ammoFeed._totalCycleTime = cal50.TotalCycleTime;            
            ammoFeed._feedClipMain = cal50.LoadedClip; //may need fixing            
            ammoFeed.QueuedClipType = ammoRack.ClipTypes[0];            
            ammoFeed.LoadedClipType = ammoRack.ClipTypes[0];

            GameObject gunner = vehicle_go.transform.Find("M113G_rig/HULL/Turret ring").GetComponent<LateFollowTarget>().LateFollowers[0].transform.Find("Commander").gameObject;            
            SkinnedMeshRenderer helmet = gunner.transform.Find("BLU_FAZ63_OLIVE/helmet").GetComponent<SkinnedMeshRenderer>();            
            SkinnedMeshRenderer dress = gunner.transform.Find("BLU_FAZ63_OLIVE/dress").GetComponent<SkinnedMeshRenderer>();            
            SkinnedMeshRenderer webbing = gunner.transform.Find("BLU_FAZ63_OLIVE/webbing").GetComponent<SkinnedMeshRenderer>();            
            helmet.material.SetTexture("_Albedo", canInf);
            helmet.material.SetTexture("_Normal", canInf_nm);
            helmet.material.SetTexture("_Smoothness", canInf_sm);
            dress.material.SetTexture("_Albedo", canInf);
            dress.material.SetTexture("_Normal", canInf_nm);
            webbing.material.SetTexture("_Albedo", canInf);            
           
            vehicle_go.transform.Find("M113G_markings/cross").gameObject.SetActive(false);
            Material cross = vehicle_go.transform.Find("M113G_markings/cross").GetComponent<MeshRenderer>().material;
            vehicle_go.transform.Find("M113G_markings/license number").gameObject.SetActive(false);            
            vehicle_go.transform.Find("M113G_markings/unit tactical").gameObject.SetActive(false);
            MeshRenderer numbers = vehicle_go.transform.Find("M113G_markings/digits").GetComponent<MeshRenderer>();
            numbers.material.mainTexture = callsigns;            
            Transform numbers_t = vehicle_go.transform.Find("M113G_markings/digits");
            numbers_t.localPosition = new Vector3(-1.2797f, 1.6f, -0.171f);
            numbers_t.localScale = new Vector3(1.5f, 1.5f, 1f);

            //Hull numbers: Company [1-4], Troop [1-4], Section [blank, A B C]
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
            MergedVehicleNumberControl[] components = vehicle_go.transform.Find("M113G_markings").GetComponents<MergedVehicleNumberControl>();
            MergedVehicleNumberControl hull_numbers = new MergedVehicleNumberControl();
            foreach (var mvnc in components)
            {
                if (mvnc.Type == VehicleDecalType.UnitNumber) { hull_numbers = mvnc; }
            }
            if (hull_numbers._allValues[0] == 9) { hull_numbers._allValues[0] = 1; }
            else if (hull_numbers._allValues[0] > 4) { hull_numbers._allValues[0] -= 4; } //5-8 get remapped onto 1-4
            if (hull_numbers._allValues[1] == 9) { hull_numbers._allValues[1] = 1; }
            else if (hull_numbers._allValues[1] > 4) { hull_numbers._allValues[1] -= 4; }
            else if (hull_numbers._allValues[1] == 0) { hull_numbers._allValues[1] = 1; }
            hull_numbers._allValues[2] = position_in_platoon + 6; // 6-9 in the texture will become letter codes
            hull_numbers.RefreshDecals();

            GameObject rear_numbers = new GameObject("rear callsign");
            rear_numbers.transform.parent = vehicle.transform;
            rear_numbers.AddComponent<MeshFilter>();
            rear_numbers.AddComponent<MeshRenderer>();
            rear_numbers.GetComponent<MeshRenderer>().material = numbers.material;
            rear_numbers.GetComponent<MeshFilter>().mesh = numbers_t.GetComponent<MeshFilter>().mesh;
            rear_numbers.transform.localPosition = new Vector3(-0.4f, 1.88f, 0.5f);
            rear_numbers.transform.localRotation = Quaternion.Euler(new Vector3(350f, 0f, 0f));            

            GameObject maple_left = new GameObject("maple_left");
            maple_left.transform.parent = vehicle_go.transform;            
            CanadianLeopardsClass.NewQuad(maple_left, cross, mapleAPC);
            maple_left.transform.localPosition = new Vector3(-1.29f, 1.745f, 1.5f);
            maple_left.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 90f));
            maple_left.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);

            GameObject maple_right = new GameObject("maple_right");
            maple_right.transform.parent = vehicle_go.transform;
            CanadianLeopardsClass.NewQuad(maple_right, cross, mapleAPC);
            maple_right.transform.localPosition = new Vector3(1.29f, 1.745f, 1.5f);
            maple_right.transform.localRotation = Quaternion.Euler(new Vector3(0f, 180f, 90f));
            maple_right.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);

            GameObject flag_front = new GameObject("flag_front");
            flag_front.transform.parent = vehicle_go.transform;
            CanadianLeopardsClass.NewQuad(flag_front, numbers.material, flag);
            flag_front.transform.localPosition = new Vector3(1.075f, 1.5f, 2.21f);
            flag_front.transform.localRotation = Quaternion.Euler(new Vector3(315f, 180f, 0f));
            flag_front.transform.localScale = new Vector3(0.08f, 1f, 0.045f);

            GameObject flag_rear = new GameObject("flag_rear");
            flag_rear.transform.parent = vehicle_go.transform;
            CanadianLeopardsClass.NewQuad(flag_rear, numbers.material, flag);
            flag_rear.transform.localPosition = new Vector3(-1.05f, 1.5f, -1.91f);
            flag_rear.transform.localRotation = Quaternion.Euler(new Vector3(280f, 180f, 180f));
            flag_rear.transform.localScale = new Vector3(0.08f, 1f, 0.045f);

            if (additional_decals) { 
                GameObject tac_front = new GameObject("tac_front");
                tac_front.transform.parent = vehicle_go.transform;
                CanadianLeopardsClass.NewQuad(tac_front, numbers.material, tacAPC);
                tac_front.transform.localPosition = new Vector3(-1.06f, 1.2f, 2.51f);
                tac_front.transform.localRotation = Quaternion.Euler(new Vector3(315f, 180f, 0f));
                tac_front.transform.localScale = new Vector3(0.1f, 1f, 0.07f);
                vehicle.transform.Find("M113G_mesh/low track links").gameObject.SetActive(false);

                GameObject tac_rear = new GameObject("tac_rear");
                tac_rear.transform.parent = vehicle_go.transform;
                CanadianLeopardsClass.NewQuad(tac_rear, numbers.material, tacAPC);
                tac_rear.transform.localPosition = new Vector3(1.045f, 1.5f, -1.91f);
                tac_rear.transform.localRotation = Quaternion.Euler(new Vector3(280f, 180f, 180f));
                tac_rear.transform.localScale = new Vector3(0.1f, 1f, 0.07f);

                GameObject mlc = new GameObject("mlc");
                mlc.transform.parent = vehicle_go.transform;
                CanadianLeopardsClass.NewQuad(mlc, numbers.material, mlcAPC);
                mlc.transform.localPosition = new Vector3(1.075f, 1.315f, 2.4f);
                mlc.transform.localRotation = Quaternion.Euler(new Vector3(315f, 180f, 0f));
                mlc.transform.localScale = new Vector3(0.08f, 1f, 0.08f);
            }

            MeshRenderer hull = vehicle.transform.Find("M113G_mesh/hull").GetComponent<MeshRenderer>();            
            SkinnedMeshRenderer running_gear = vehicle.transform.Find("M113G_mesh/running gear").GetComponent<SkinnedMeshRenderer>();            
            MeshRenderer drivers_hatch = vehicle.transform.Find("M113G_mesh/driver's hatch").GetComponent<MeshRenderer>();            
            SkinnedMeshRenderer cupola = vehicle.transform.Find("M113G_mesh/cupola").GetComponent<SkinnedMeshRenderer>();            
            hull.material.SetTexture("_Albedo", apc);            
            running_gear.material.SetTexture("_Albedo", apc);            
            drivers_hatch.material.SetTexture("_Albedo", apc);            
            cupola.material.SetTexture("_Albedo", apc);            

            vehicle._friendlyName = "M113A1";
            vehicle_go.AddComponent<CanLepConverted>();
        }
    }
}
