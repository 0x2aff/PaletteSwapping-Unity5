using System;
using System.Collections.Generic;

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class ColorPalette : ScriptableObject {

    public Texture2D Source;
    public List<Color> Palette = new List<Color>();
    public List<Color> NewPalette = new List<Color>();
    public Texture2D CachedTexture;

#if UNITY_EDITOR
    [MenuItem("Assets/Create/Color Palette")]
    public static void CreateColorPalette()
    {
        var activeObject = Selection.activeObject as Texture2D;
        if (activeObject != null)
        {
            var selectedTexture = activeObject;
            var selectedPath = AssetDatabase.GetAssetPath(selectedTexture);
            selectedPath = selectedPath.Replace(".png", "-ColorPalette.asset");

            var newPalette = CustomAssetUtil.CreateAsset<ColorPalette>(selectedPath);
            newPalette.Source = selectedTexture;
            newPalette.ResetPalette();

            Debug.Log("Creating a Color Palette " + selectedPath);
        }
        else
        {
            Debug.Log("Can't create a Color Palette");
        }
    }
#endif

    private static List<Color> BuildPalette(Texture2D texture)
    {
        var palette = new List<Color>();
        var colors = texture.GetPixels();

        foreach (var color in colors)
        {
            if (palette.Contains(color)) continue;
            if (Math.Abs(color.a - 1) <= 0)
                palette.Add(color);
        }

        return palette;
    }

    public void ResetPalette()
    {
        Palette = BuildPalette(Source);
        NewPalette = new List<Color>(Palette);
    }

    public Color GetColor(Color color)
    {
        for (var i = 0; i < Palette.Count; i++)
        {
            var tempColor = Palette[i];
            if (Mathf.Approximately(color.r, tempColor.r) &&
                Mathf.Approximately(color.g, tempColor.g) &&
                Mathf.Approximately(color.b, tempColor.b) &&
                Mathf.Approximately(color.a, tempColor.a))
            {
                return NewPalette[i];
            }
        }

        return color;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ColorPalette))]
public class ColorPaletteEditor : Editor
{
    public ColorPalette ColorPalette;

    private void OnEnable()
    {
        ColorPalette = target as ColorPalette;
    }

    public override void OnInspectorGUI()
    {
        GUILayout.Label("Source Texture");
        ColorPalette.Source = EditorGUILayout.ObjectField(ColorPalette.Source, typeof(Texture2D), false) as Texture2D;

        EditorGUILayout.BeginHorizontal();

        GUILayout.Label("Current Color");
        GUILayout.Label("New Color");

        EditorGUILayout.EndHorizontal();

        for (var i = 0; i < ColorPalette.Palette.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.ColorField(ColorPalette.Palette[i]);
            ColorPalette.NewPalette[i] = EditorGUILayout.ColorField(ColorPalette.NewPalette[i]);

            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Revert Palette"))
        {
            ColorPalette.ResetPalette();
        }

        EditorUtility.SetDirty(ColorPalette);
    }
}
#endif