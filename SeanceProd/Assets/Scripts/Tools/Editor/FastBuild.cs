/// Author: Nicolas Capelier
/// Last modified by: Nicolas Capelier

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Diagnostics;

namespace Seance.Tools
{
    public class FastBuild
    {
        [MenuItem("Testing/Fast Build")]
        public static void BuildGame()
        {
            string path = EditorUtility.SaveFolderPanel("Choose Build Location", "", "");
            string[] levels = new string[] { "Assets/Scenes/ProtoWeek/Main.unity" };

            BuildPipeline.BuildPlayer(levels, path + "/Seance - FastBuild.exe", BuildTarget.StandaloneWindows, BuildOptions.None);

            Process proc;

            for (int i = 0; i < 2; i++)
            {
                proc = new Process();
                proc.StartInfo.FileName = path + "/Seance - FastBuild.exe";
                proc.Start();
			}

            EditorApplication.EnterPlaymode();
        }
    }
}
