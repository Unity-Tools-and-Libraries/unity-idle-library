using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class Debug
{
    [MenuItem("GameObject/UI/Idle/Debug Console")]
    public static void Menu()
    {
        var selected = Selection.activeObject;
        var canvas = GameObject.FindObjectOfType<Canvas>();
        if(!canvas)
        {
            throw new System.Exception("Need to instantiate a canvas first!");
        }
        var consolePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Editor/Prefabs/UI/Debug Console.prefab");
        PrefabUtility.InstantiatePrefab(consolePrefab, canvas.transform);
    }

    [MenuItem("GameObject/UI/Idle/Debug Console", validate = true)]
    public static bool MenuValidation()
    {
        var selected = Selection.activeObject;
        if(!selected || typeof(GameObject) != selected.GetType())
        {
            return false;
        }

        var selectedGameObject = (GameObject)selected;
        var canvas = selectedGameObject.GetComponentInParent<Canvas>();

        return canvas;
    }
}
