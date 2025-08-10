using UnityEditor;
using UnityEngine;

namespace Fraktal.Framework.Utilities
{
    public static class AssetUtility
    {
        public static T GetAssetOfType<T>() where T : ScriptableObject
        {
            string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                return AssetDatabase.LoadAssetAtPath<T>(path);
            }
            return null;
        }
    }

}