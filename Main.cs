using System.Collections;
using System.Linq;
using GHPC.State;
using GHPC.Vehicle;
using MelonLoader;
using NoCamoM60;
using PactIncreasedLethality;
using Thermals;
using UnityEngine;

[assembly: MelonInfo(typeof(NoCamoM60Mod), "No Camo M60s", "1.0.0", "ATLAS")]
[assembly: MelonGame("Radian Simulations LLC", "GHPC")]

namespace NoCamoM60
{
    public class NoCamoM60Mod : MelonMod
    {
        public static Vehicle[] vics;
        static Material no_camo_mat;

        public IEnumerator GetVics(GameState _)
        {
            vics = GameObject.FindObjectsByType<Vehicle>(FindObjectsSortMode.None);

            yield break;
        }

        public static IEnumerator Convert(GameState _)
        {
            foreach (Vehicle vic in vics)
            {
                GameObject vic_go = vic.gameObject;

                if (vic == null) continue;
                if (vic.GetComponent<AlreadyConverted>() != null) continue;
                if (!vic.FriendlyName.Contains("M60A")) continue;

                vic_go.AddComponent<AlreadyConverted>();

                MeshRenderer hull_mesh = vic.transform.Find("M60_mesh/M60_hull").GetComponent<MeshRenderer>();
                hull_mesh.materials[0].SetFloat("_CamoAmount", 0f);

                /*
                Material[] new_materials = hull_mesh.materials;

                if (no_camo_mat == null)
                {
                    Material mat = new_materials[0];
                    no_camo_mat = new Material(Shader.Find("GHPC/VehicleShader"));

                    no_camo_mat.SetTexture("_Albedo", mat.GetTexture("_Albedo"));
                    no_camo_mat.SetTexture("_Smoothness", mat.GetTexture("_Smoothness"));
                    no_camo_mat.SetTexture("_Normal", mat.GetTexture("_Normal"));
                    no_camo_mat.SetTexture("_Occlusion", mat.GetTexture("_Occlusion"));
                    no_camo_mat.SetTexture("_PaintMask", mat.GetTexture("_PaintMask"));
                    no_camo_mat.SetTexture("_scorchnormal", mat.GetTexture("_scorchnormal"));
                }

                new_materials[0] = no_camo_mat;
                hull_mesh.materials = new_materials;
                */

                SkinnedMeshRenderer turret_mesh = vic.transform.Find("M60_mesh/M60 1").GetComponent<SkinnedMeshRenderer>();
                //turret_mesh.materials = new_materials;
                turret_mesh.materials[0].SetFloat("_CamoAmount", 0f);


                if (vic.FriendlyName == "M60A3 TTS")
                {
                    SkinnedMeshRenderer gun_mesh = vic.transform.Find("M60A3_mesh/m60a3_parts").GetComponent<SkinnedMeshRenderer>();
                    //gun_mesh.materials = new_materials;
                    gun_mesh.materials[0].SetFloat("_CamoAmount", 0f);
                }

                vic.transform.Find("M60_mesh").gameObject.GetComponent<HeatSource>().FetchSwapableMats();
            }

            yield break;
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            if (Util.menu_screens.Contains(sceneName)) return;

            StateController.RunOrDefer(GameState.GameReady, new GameStateEventHandler(GetVics), GameStatePriority.Medium);
            StateController.RunOrDefer(GameState.GameReady, new GameStateEventHandler(Convert), GameStatePriority.Medium);
        }
    }
}

