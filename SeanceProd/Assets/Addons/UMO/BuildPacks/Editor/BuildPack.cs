/// Unity Modules - Build Packs
/// Created by: Nicolas Capelier
/// Contact: capelier.nicolas@gmail.com
/// Version: 1.0.0
/// Version release date (dd/mm/yyyy): 29/07/2022

using UnityEditor;
using UnityEngine;

namespace UMO.Tools.BuildPacks
{
    [CreateAssetMenu(fileName = "new Build Pack", menuName ="Build Pack", order = 201)]
    public class BuildPack : ScriptableObject
    {
        [System.Serializable]
        public struct SceneToBuild
        {
            public SceneAsset sceneAsset;
            public bool enabled;
        }

        public SceneToBuild[] scenesToBuild;

        public void SetToBuildSettings()
        {
            int enabledCount = 0;

            for(int i = 0; i < scenesToBuild.Length; i++)
            {
                if(scenesToBuild[i].enabled)
                {
                    enabledCount++;
                }
            }

            EditorBuildSettingsScene[] newScenes = new EditorBuildSettingsScene[enabledCount];

            int newIndex = 0;

            for (int i = 0; i < scenesToBuild.Length; i++)
            {
                if(scenesToBuild[i].enabled)
                {
                    string scenePath = AssetDatabase.GetAssetPath(scenesToBuild[i].sceneAsset);
                    newScenes[newIndex] = new EditorBuildSettingsScene(scenePath, scenesToBuild[i].enabled);
                    newIndex++;
                }
            }

            EditorBuildSettings.scenes = newScenes;
        }

        public void SetEnabling(bool enabling)
        {
            for (int i = 0; i < scenesToBuild.Length; i++)
            {
                scenesToBuild[i].enabled = enabling;
            }
        }
    }
}