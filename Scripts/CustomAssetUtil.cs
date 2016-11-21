using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR
public class CustomAssetUtil {

    public static T CreateAsset<T>(string path) where T : ScriptableObject
    {
        var asset = ScriptableObject.CreateInstance<T>();
        var newPath = AssetDatabase.GenerateUniqueAssetPath(path);

        AssetDatabase.CreateAsset(asset, newPath);
        AssetDatabase.SaveAssets();

        return asset;
    }
}
#endif
