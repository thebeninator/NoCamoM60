using System.Collections;
using System.Linq;
using GHPC.State;
using GHPC.Vehicle;
using MelonLoader;
using M60CamoPicker;
using Thermals;
using UnityEngine;
using System.Collections.Generic;

[assembly: MelonInfo(typeof(M60CamoPickerMod), "M60 Camo Picker", "1.0.0", "ATLAS")]
[assembly: MelonGame("Radian Simulations LLC", "GHPC")]

namespace M60CamoPicker
{
    public class M60CamoPickerMod : MelonMod
    {
        static Dictionary<string, Texture> camo_textures = new Dictionary<string, Texture>();
        static bool done = false;

        static MelonPreferences_Entry<string> camo_m60a3_tts;
        static MelonPreferences_Entry<string> camo_m60a3;
        static MelonPreferences_Entry<string> camo_m60a1_late;
        static MelonPreferences_Entry<string> camo_m60a1_early;
        static MelonPreferences_Entry<string> camo_m60a1_aos;
        static MelonPreferences_Entry<string> camo_m60a1;

        static GameObject m60a3_tts;
        static GameObject m60a3;
        static GameObject m60a1_late;
        static GameObject m60a1_early;
        static GameObject m60a1_aos;
        static GameObject m60a1;
  
        private static void OverrideCamo(GameObject v_go, string camo, string turret_path, string hull_path, string gun_path) {
            turret_path = "M60_meshes/" + turret_path;
            hull_path = "M60_meshes/" + hull_path;
            gun_path = "M60_meshes/" + gun_path;

            SkinnedMeshRenderer smr_turret = v_go.transform.Find(turret_path).GetComponent<SkinnedMeshRenderer>();
            SkinnedMeshRenderer smr_gun = v_go.transform.Find(gun_path).GetComponent<SkinnedMeshRenderer>();
            SkinnedMeshRenderer smr_rgear = v_go.transform.Find("M60_meshes/running_gear").GetComponent<SkinnedMeshRenderer>();

            MeshRenderer mr_hull = v_go.transform.Find(hull_path).GetComponent<MeshRenderer>();


            if (camo.ToUpper() != "NONE")
            {
                smr_turret.material.SetTexture("_CamoLayer", camo_textures[camo]);
                smr_gun.material.SetTexture("_CamoLayer", camo_textures[camo]);
                smr_rgear.material.SetTexture("_CamoLayer", camo_textures[camo]);
                mr_hull.material.SetTexture("_CamoLayer", camo_textures[camo]);
            }
            else {
                smr_turret.material.SetFloat("_CamoAmount", 0f);
                smr_gun.material.SetFloat("_CamoAmount", 0f);
                mr_hull.material.SetFloat("_CamoAmount", 0f);
                smr_rgear.material.SetFloat("_CamoAmount", 0f);
            }
        }  


        public override void OnInitializeMelon()
        {
            MelonPreferences_Category cfg = MelonPreferences.CreateCategory("M60CamoPicker");

            camo_m60a3_tts = cfg.CreateEntry<string>("M60A3 TTS", "NONE");
            camo_m60a3_tts.Description = "NONE, MERDC, DUALTEX, MASSTER";
            camo_m60a3 = cfg.CreateEntry<string>("M60A3", "MERDC");
            camo_m60a1_late = cfg.CreateEntry<string>("M60A1 RISE (P) Late", "MERDC");
            camo_m60a1_early = cfg.CreateEntry<string>("M60A1 RISE (P) Early", "DUALTEX");
            camo_m60a1_aos = cfg.CreateEntry<string>("M60A1 AOS", "MASSTER");
            camo_m60a1 = cfg.CreateEntry<string>("M60A1", "MASSTER");
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            if (done) return;
            if (sceneName == "LOADER_INITIAL") return;

            Vehicle[] vics = Resources.FindObjectsOfTypeAll<Vehicle>();

            int c = 0; 

            foreach (Vehicle v in vics) {
                if (v.gameObject.name == "M60A1 AOS") {
                    camo_textures["MASSTER"] = v.transform.Find("M60_meshes/turret_mid").GetComponent<SkinnedMeshRenderer>().material.GetTexture("_CamoLayer");
                    m60a1_aos = v.gameObject;
                    c++;
                }

                if (v.gameObject.name == "M60A1 RISE Passive Early")
                {
                    camo_textures["DUALTEX"] = v.transform.Find("M60_meshes/turret_mid").GetComponent<SkinnedMeshRenderer>().material.GetTexture("_CamoLayer");
                    m60a1_early = v.gameObject;
                    c++;
                }

                if (v.gameObject.name == "M60A3 TTS")
                {
                    camo_textures["MERDC"] = v.transform.Find("M60_meshes/turret_late").GetComponent<SkinnedMeshRenderer>().material.GetTexture("_CamoLayer");
                    m60a3_tts = v.gameObject;
                    c++;
                }

                if (v.gameObject.name == "M60A1 RISE Passive Late")
                {
                    m60a1_late = v.gameObject;
                    c++;
                }

                if (v.gameObject.name == "M60A3")
                {
                    m60a3 = v.gameObject;
                    c++;
                }

                if (v.gameObject.name == "M60A1")
                {
                    m60a1 = v.gameObject;
                    c++;
                }

                if (c == 6) { break; }
            }

            OverrideCamo(m60a1, camo_m60a1.Value, "turret_early", "hull_mid", "gun_early");
            OverrideCamo(m60a1_aos, camo_m60a1_aos.Value, "turret_mid", "hull_mid", "gun_early");
            OverrideCamo(m60a1_early, camo_m60a1_early.Value, "turret_mid", "hull_late", "gun_early");
            OverrideCamo(m60a1_late, camo_m60a1_late.Value, "turret_late", "hull_late", "gun_mid");
            OverrideCamo(m60a3, camo_m60a3.Value, "turret_late", "hull_late", "gun_mid");
            OverrideCamo(m60a3_tts, camo_m60a3_tts.Value, "turret_late", "hull_late", "gun_mid");

            done = true;
        }
    }
}

