/// Unity Modules - Scene Viewer
/// Created by: Nicolas Capelier
/// Contact: capelier.nicolas@gmail.com
/// Version: 1.0.0
/// Version release date (dd/mm/yyyy): 29/07/2022

using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace UMO.Tools.SceneViewer
{
    public class SceneViewerWindow : EditorWindow
    {
        static SceneViewerWindow sceneViewer;
        Vector2 scrollPosition = Vector2.zero;

        [MenuItem("Window/General/Scene Viewer", priority = 1)]
        public static void Init()
        {
            sceneViewer = GetWindow<SceneViewerWindow>("Scene Viewer");
            sceneViewer.RefreshScenes();
        }

        string[] scenesGUIDs;
        string currentFolderName;
        string[] items;

        bool refreshed = false;

        GUIStyle titleStyle;
        GUIStyle GetTitleStyle()
        {
            GUIStyle _style = new GUIStyle();

            _style.alignment = TextAnchor.MiddleCenter;
            _style.fontStyle = FontStyle.Bold;
            _style.fontSize = 24;
            _style.normal.textColor = Color.white;

            return _style;
        }

        GUIStyle buttonStyle;
        GUIStyle GetButtonStyle()
        {
            GUIStyle _style = new GUIStyle(GUI.skin.button);

            _style.fontStyle = FontStyle.Bold;
            _style.fontSize = 16;
            _style.normal.textColor = Color.white;

            return _style;
        }

        void RefreshScenes()
        {
            scenesGUIDs = AssetDatabase.FindAssets("t:Scene");
            refreshed = true;
        }

        private void OnGUI()
        {
            titleStyle = GetTitleStyle();
            buttonStyle = GetButtonStyle();

			if (GUILayout.Button("Refresh project scenes", buttonStyle))
			{
				RefreshScenes();
			}

			if (scenesGUIDs == null || (scenesGUIDs.Length == 0 && !refreshed))
                RefreshScenes();

            currentFolderName = "";

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            EditorGUILayout.Space(5f);

            foreach (string sceneGUID in scenesGUIDs)
            {
                items = AssetDatabase.GUIDToAssetPath(sceneGUID).Split('/');

                if (items[1] != "Scenes")
                    continue;

                if (currentFolderName != items[items.Length - 2])
                {
                    EditorGUILayout.Space(5f);
                    currentFolderName = items[items.Length - 2];
                    GUILayout.Label(currentFolderName, titleStyle);
                }

                GUILayout.BeginHorizontal();
                if (GUILayout.Button(items[items.Length - 1].Remove(items[items.Length - 1].Length - 6), buttonStyle))
                {
                    if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                    {
                        try
                        {
                            EditorSceneManager.OpenScene(AssetDatabase.GUIDToAssetPath(sceneGUID).ToString());
                        }
                        catch
                        {
                            Debug.LogError("Scene not found, try refreshing the scene viewer.");
                        }
                    }
                }

                if(GUILayout.Button("Add", buttonStyle, GUILayout.Width(50f)))
				{
                    EditorSceneManager.OpenScene(AssetDatabase.GUIDToAssetPath(sceneGUID).ToString(), OpenSceneMode.Additive);
				}
                GUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
        }
    }
}