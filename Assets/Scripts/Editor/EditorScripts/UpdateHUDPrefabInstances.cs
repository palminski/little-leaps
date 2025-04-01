using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Linq;
using Unity.VisualScripting;

public class UpdateHUDPrefabInstances : EditorWindow
{

    private GameObject prefabToUpdate;


    [MenuItem("Tools/Update All HUD instances")]
    public static void ShowWindow()
    {
        GetWindow<UpdateHUDPrefabInstances>("Update Specific Prefab");
    }

    private void OnGUI()
    {
        GUILayout.Label("Select Prefab To Update", EditorStyles.boldLabel);
        prefabToUpdate = (GameObject)EditorGUILayout.ObjectField("Prefab:", prefabToUpdate, typeof(GameObject), false);

        if (GUILayout.Button("Update Prefab Instances") && prefabToUpdate != null)
        {
            UpdateSpecificPrefab(prefabToUpdate);
        }
    }

    private static void UpdateSpecificPrefab(GameObject prefab)
    {
        string[] scenePaths = AssetDatabase.FindAssets("t:Scene").Select(AssetDatabase.GUIDToAssetPath).Where(path => path.StartsWith("Assets/")).ToArray();

        foreach (string path in scenePaths)
        {
            Scene scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
            bool sceneModified = false;
            GameObject[] allObjects = scene.GetRootGameObjects();
            foreach (GameObject obj in allObjects)
            {
                if (RevertPrefabInstances(obj, prefab))
                {
                    sceneModified = true;
                }
            }

            if (sceneModified)
            {
                EditorSceneManager.SaveScene(scene);
            }
        }
    }

    private static bool RevertPrefabInstances(GameObject obj, GameObject prefab)
    {
        bool modified = false;

        PrefabInstanceStatus prefabStatus = PrefabUtility.GetPrefabInstanceStatus(obj);
        if (prefabStatus == PrefabInstanceStatus.Connected)
        {
            GameObject prefabAsset = PrefabUtility.GetCorrespondingObjectFromSource(obj);

            if (prefabAsset == prefab)
            {
                PrefabUtility.RevertPrefabInstance(obj, InteractionMode.AutomatedAction);
                modified = true;
            }
        }

        foreach (Transform child in obj.transform)
        {
            if (RevertPrefabInstances(child.gameObject, prefab))
            {
                modified = true;
            }
            
        }

        return modified;
    }
}
