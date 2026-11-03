using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using GHPC.Vehicle;

namespace CanadianLeopards
{
    class Menu
    {
        public static void MenuLeopards()
        {
            GameObject mg_mount = Utilities.mg_mount;
            GameObject fn_mag = Utilities.fn_mag;
            GameObject bustle_baskets = Utilities.bustle_baskets;
            GameObject jerry_can = Utilities.jerry_can;
            GameObject new_net = Utilities.new_net;
            Texture2D L2_base = Utilities.L2_base;                       
            Texture2D maple = Utilities.maple;
            Texture2D mapleBlack = Utilities.maple_black;
            Texture2D callsigns = Utilities.callsigns;            
            Texture2D tac = Utilities.tac;            
            Texture2D mlc60 = Utilities.mlc60;
            Texture2D mlc60Black = Utilities.mlc60_black;

            GameObject[] props = GameObject.FindGameObjectsWithTag("Vehicle");            
            foreach (GameObject prop in props)
            {
                Transform hull = prop.transform.Find("leo2_rig/hull");
                Transform turret = prop.transform.Find("leo2_rig/hull/turret");
                Transform mesh = prop.transform.Find("LEO2_mesh");                

                GameObject mount = GameObject.Instantiate(mg_mount, turret);
                mount.transform.localPosition = new Vector3(-0.91f, 0.975f, -0.53f);
                mount.transform.localRotation = Quaternion.Euler(new Vector3(271f, 177.16f, 0f));
                GameObject c6 = GameObject.Instantiate(fn_mag, mount.transform);
                c6.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                c6.transform.localPosition = new Vector3(0f, -0.0004f, -0.0008f);
                c6.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 180f));               

                Material commonMat = mesh.transform.Find("hull common").GetComponent<MeshRenderer>().material;
                commonMat.SetTexture("_Albedo", L2_base);
                mesh.transform.Find("running gear").GetComponent<SkinnedMeshRenderer>().material = commonMat;
                mesh.transform.Find("turret A1").GetComponent<SkinnedMeshRenderer>().material = commonMat;
                turret.transform.Find("turret common parts").GetComponent<MeshRenderer>().material = commonMat;
                turret.transform.Find("blowoff panel").GetComponent<MeshRenderer>().material = commonMat;

                GameObject baskets = GameObject.Instantiate(bustle_baskets, turret);
                baskets.transform.GetComponent<MeshRenderer>().material = commonMat;
                baskets.transform.localPosition = new Vector3(0f, -1.5f, -0.25f);

                turret.Find("Camo Nets/nato_camonet_folded").localPosition = new Vector3(1.07f, 0.6281f, -1.45f);
                turret.Find("Camo Nets/nato_camonet_folded").localRotation = Quaternion.Euler(new Vector3(0f, 75f, 0.4176f));
                turret.Find("Camo Nets/nato_camonet_folded").localScale = new Vector3(1.05f, 0.816f, 0.8f);

                turret.Find("Camo Nets/nato_camonet_folded (2)").localPosition = new Vector3(2.5f, 0.646f, -1.19f);
                turret.Find("Camo Nets/nato_camonet_folded (2)").localRotation = Quaternion.Euler(new Vector3(0f, 0f, 176.8327f));
                turret.Find("Camo Nets/nato_camonet_folded (2)").localScale = new Vector3(1.3f, 0.8f, 1f);                

                GameObject turret_net = turret.transform.Find("net_turret").gameObject;
                if (turret_net.activeSelf)
                {
                    GameObject newNet = GameObject.Instantiate(new_net, turret);
                    Material camoNet_mat = turret_net.GetComponent<MeshRenderer>().material;
                    newNet.transform.Find("default").GetComponent<MeshRenderer>().material = camoNet_mat;
                    newNet.transform.localPosition = new Vector3(0f, -1.42f, -0.3f);
                    newNet.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
                    turret_net.SetActive(false);
                }

                Transform markings = prop.transform.Find("leo2_rig/hull/turret/LEO2_markings");
                Material cross_mat = markings.transform.Find("cross-big").GetComponent<MeshRenderer>().material;                               
                markings.transform.Find("cross-big").gameObject.SetActive(false);
                markings.transform.Find("cross-big-late").gameObject.SetActive(false);
                markings.transform.Find("cross-small").gameObject.SetActive(false);

                GameObject maple_left = new GameObject("Mapleleaf_left");
                maple_left.transform.parent = markings.transform;
                maple_left.transform.position = markings.transform.position;
                Utilities.NewQuad(maple_left, cross_mat, maple);               
                maple_left.transform.localScale = new Vector3(0.165f, 0.165f, 0.165f);
                maple_left.transform.localPosition = new Vector3(-1.23f, 2.175f, 0.16f);
                maple_left.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 90f));                

                GameObject maple_right = new GameObject("Mapleleaf_right");
                maple_right.transform.parent = markings.transform;
                maple_right.transform.position = markings.transform.position;
                Utilities.NewQuad(maple_right, cross_mat, maple);                
                maple_right.transform.localScale = new Vector3(0.165f, 0.165f, 0.165f);
                maple_right.transform.localPosition = new Vector3(1.33f, 2.1f, 0.1f);
                maple_right.transform.localRotation = Quaternion.Euler(new Vector3(0f, 180f, 90f));

                GameObject maple_lower_rear = new GameObject("Mapleleaf_lower_rear");
                maple_lower_rear.transform.parent = prop.transform.Find("leo2_rig/hull");
                maple_lower_rear.transform.position = prop.transform.Find("leo2_rig/hull").position;
                Utilities.NewQuad(maple_lower_rear, cross_mat, mapleBlack);
                maple_lower_rear.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
                maple_lower_rear.transform.localPosition = new Vector3(0.31f, 0.33f, -3.92f);
                maple_lower_rear.transform.localRotation = Quaternion.Euler(new Vector3(0f, 270f, 120f));
                maple_lower_rear.GetComponent<MeshRenderer>().material.SetVector("_background", new Vector4(-1.3f, -1.2f, -1.3f, 0f));

                int platoonPosition;
                if (prop.name == "LEO2A1 static display") platoonPosition = 0;
                else if (prop.name == "LEO2A1 static display (1)") platoonPosition = 1;
                else platoonPosition = 2;

                MergedVehicleNumberControl[] components = markings.GetComponents<MergedVehicleNumberControl>();
                MergedVehicleNumberControl turret_numbers = new MergedVehicleNumberControl();
                MergedVehicleNumberControl cfr_numbers = new MergedVehicleNumberControl();
                foreach (var mvnc in components)
                {
                    if (mvnc.Type == VehicleDecalType.UnitNumber) { turret_numbers = mvnc; }
                    if (mvnc.Type == VehicleDecalType.LicensePlate) { cfr_numbers = mvnc; }
                }

                turret_numbers._allValues[0] = 1;
                turret_numbers._allValues[1] = 2;                
                turret_numbers._allValues[2] = platoonPosition + 6; // 6-9 in the texture will become letter codes
                turret_numbers.RefreshDecals();
                GameObject numbers = markings.transform.Find("numbers").gameObject;
                numbers.GetComponent<MeshRenderer>().material.mainTexture = callsigns;

                cfr_numbers._allValues[0] = 8; //CFR numbers for Leo2 hulls are 85-72XXX to 85-73XXX
                cfr_numbers._allValues[1] = 5;
                cfr_numbers._allValues[2] = 36; //hyphen
                cfr_numbers._allValues[3] = 7;
                if (cfr_numbers._allValues[4] > 3 || cfr_numbers._allValues[4] < 2) cfr_numbers._allValues[4] = 2;
                cfr_numbers.RefreshDecals();
                Material cfr_mat = hull.transform.Find("license number").GetComponent<MeshRenderer>().material;
                cfr_mat.SetVector("_fieldcolour", new Vector4(0.1f, 0.1f, 0.1f, 0f));
                cfr_mat.SetVector("_numeralcolours", new Vector4(0.1f, 0.1f, 0f, 0f));

                Transform number_decal = markings.Find("numbers");
                if (platoonPosition == 0) { number_decal.localPosition = new Vector3(1.16f, 2.06f, -2.11f); }
                else { number_decal.localPosition = new Vector3(1.1f, 2.06f, -2.11f); }
                number_decal.localRotation = Quaternion.Euler(new Vector3(270f, 90f, 0f));
                number_decal.localScale = new Vector3(0.8f, 0.8f, 0.8f);

                if (CanadianLeopardsClass.leo2_mlc.Value != 0)
                {
                    GameObject mlc_decal = new GameObject("MLC decal");
                    mlc_decal.transform.parent = hull;
                    mlc_decal.transform.position = hull.position;
                    if (CanadianLeopardsClass.leo2_mlc.Value == 1) { Utilities.NewQuad(mlc_decal, numbers.GetComponent<MeshRenderer>().material, mlc60); }
                    else { Utilities.NewQuad(mlc_decal, numbers.GetComponent<MeshRenderer>().material, mlc60Black); }
                    mlc_decal.transform.localPosition += new Vector3(1.41f, 0.385f, 3.58f);
                    mlc_decal.transform.localRotation = Quaternion.Euler(new Vector3(300f, 180f, 0f));
                    mlc_decal.transform.localScale = new Vector3(0.08f, 0.08f, 0.08f);
                }

                GameObject tactical = new GameObject("tactical unit symbol");
                tactical.transform.parent = hull;
                tactical.transform.position = prop.transform.Find("leo2_rig/hull").position;
                Utilities.NewQuad(tactical, numbers.GetComponent<MeshRenderer>().material, tac);
                tactical.transform.localPosition += new Vector3(-1.42f, 0.385f, 3.58f);
                tactical.transform.localRotation = Quaternion.Euler(new Vector3(300f, 180f, 0f));
                tactical.transform.localScale = new Vector3(0.08f, 0.08f, 0.07f);

                Utilities.Log("Leopard 2 menu prop converted!");
            }
        }
    }
}
