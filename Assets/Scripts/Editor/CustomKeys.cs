using UnityEditor;
using UnityEngine;

public class CustomKeys : Editor
{
    public class CustomShortcuts : ScriptableObject
    {
        [MenuItem("Edit/Custom Hide Selected #A")] // %h ±Ì æ Ctrl + H
        private static void HideSelectedObjects()
        {
            foreach (var obj in Selection.gameObjects)
            {
                obj.SetActive(!obj.activeSelf);
            }
        }
    }

}