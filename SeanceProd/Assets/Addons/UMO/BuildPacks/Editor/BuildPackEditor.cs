/// Unity Modules - Build Packs
/// Created by: Nicolas Capelier
/// Contact: capelier.nicolas@gmail.com
/// Version: 1.0.0
/// Version release date (dd/mm/yyyy): 29/07/2022

using UnityEngine;
using UnityEditor;

namespace UMO.Tools.BuildPacks
{
    [CustomEditor(typeof(BuildPack))]
    public class BuildPackEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            BuildPack buildPack = (BuildPack)target;

            GUILayout.Space(10f);

            GUILayout.BeginHorizontal();

            if(GUILayout.Button("Enable all scenes"))
            {
                buildPack.SetEnabling(true);
            }

            if (GUILayout.Button("Disable all scenes"))
            {
                buildPack.SetEnabling(false);
            }

            GUILayout.EndHorizontal();

            GUILayout.Label("");

            if (GUILayout.Button("Place this pack in build settings"))
            {
                buildPack.SetToBuildSettings();
            }
        }
    }
}