using System;
using System.Collections.Generic;
using GHPC;
using GHPC.Infantry;
using UnityEngine;

namespace CanadianLeopards
{
    class Infantry
    {
        public static void Convert(InfantryUnit troop)
        {            
            troop.gameObject.AddComponent<CanLepConverted>();

            Texture2D canInf = Utilities.canInf;
            Texture2D canInf_nm = Utilities.canInf_nm;
            Texture2D canInf_sm = Utilities.canInf_sm;
            Texture2D bloodMask = Utilities.blood_mask;
            GameObject c1a1_prefab = Utilities.c1a1_prefab;
            GameObject dressing_prefab = Utilities.dressing_prefab;

            SkinnedMeshRenderer dress = troop.transform.Find("Troop Base/BLU_FAZ63_OLIVE/dress").GetComponent<SkinnedMeshRenderer>();
            SkinnedMeshRenderer accoutrements = troop.transform.Find("Troop Base/BLU_FAZ63_OLIVE/accoutrements").GetComponent<SkinnedMeshRenderer>();
            SkinnedMeshRenderer helmet = troop.transform.Find("Troop Base/BLU_FAZ63_OLIVE/helmet").GetComponent<SkinnedMeshRenderer>();
            SkinnedMeshRenderer webbing = troop.transform.Find("Troop Base/BLU_FAZ63_OLIVE/webbing").GetComponent<SkinnedMeshRenderer>();
            Material canInf_mat = dress.material;
            canInf_mat.SetTexture("_Albedo", canInf);
            canInf_mat.SetTexture("_Normal", canInf_nm);
            canInf_mat.SetTexture("_Smoothness", canInf_sm);
            canInf_mat.SetTexture("_regions", bloodMask);
            accoutrements.material = canInf_mat;
            helmet.material = canInf_mat;
            webbing.material = canInf_mat;

            AarVisual aarVis = troop.transform.Find("Troop Base").GetComponent<AarVisual>();
            aarVis.OriginalMaterials[dress] = new List<Material> { dress.material };
            aarVis.OriginalMaterials[accoutrements] = new List<Material> { accoutrements.material };
            aarVis.OriginalMaterials[helmet] = new List<Material> { helmet.material };
            aarVis.OriginalMaterials[webbing] = new List<Material> { webbing.material };

            GameObject default_rifle = troop.transform.Find("Troop Base/TRP_SKELETON/weaponmain/G3A3").gameObject;
            GameObject c1a1_rifle = GameObject.Instantiate(c1a1_prefab, default_rifle.transform);
            c1a1_rifle.transform.localPosition = new Vector3(0f, 0.05f, 0.22f);
            c1a1_rifle.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            c1a1_rifle.transform.localScale = new Vector3(0.51f, 0.51f, 0.51f);
            c1a1_rifle.transform.Find("default").GetComponent<MeshRenderer>().material.color = new Color(0.6604f, 0.6604f, 0.6604f, 1);

            AarVisual weapon_AAR = troop.transform.Find("Troop Base/TRP_SKELETON/weaponmain/G3A3").GetComponent<AarVisual>();
            weapon_AAR._renderers = new List<Renderer> { c1a1_rifle.transform.Find("default").GetComponent<MeshRenderer>() };
            weapon_AAR.OriginalMaterials = new Dictionary<Renderer, List<Material>>
            {
                [c1a1_rifle.transform.Find("default").GetComponent<MeshRenderer>()] = new List<Material>
                            { c1a1_rifle.transform.Find("default").GetComponent<MeshRenderer>().material }
            };

            troop.transform.Find("Troop Base/TRP_SKELETON/weaponmain/G3A3/G3A3/G3A3").gameObject.SetActive(false);
            troop.transform.Find("Troop Base/TRP_SKELETON/weaponmain/G3A3/G3A3 Rigidbody/WPN_G3A3/G3A3").gameObject.SetActive(false);

            int seed = DateTime.Now.Millisecond;
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

            Utilities.Log(troop.name + " converted to CF Infantry");
        }
    }
}
