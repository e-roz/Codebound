using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;

namespace Codebound.EditorTools
{
    public static class FontAssetBuilder
    {
        const string FontsFolder = "Assets/_Project/Fonts";
        public const string UiFontAssetPath = FontsFolder + "/Silkscreen_UI.asset";
        public const string CodeFontAssetPath = FontsFolder + "/JetBrainsMono_Code.asset";

        const string Characters =
            " !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~" +
            "ÑñÁÉÍÓÚáéíóú";

        [MenuItem("Codebound/Build Font Assets")]
        public static void BuildAll()
        {
            Ensure(FontsFolder + "/Silkscreen-Regular.ttf", UiFontAssetPath);
            Ensure(FontsFolder + "/JetBrainsMono-Regular.ttf", CodeFontAssetPath);
            SetTmpDefault(AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(UiFontAssetPath));
            AssetDatabase.SaveAssets();
        }

        public static TMP_FontAsset LoadUiFont()
        {
            BuildAll();
            return AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(UiFontAssetPath);
        }

        static void Ensure(string fontPath, string assetPath)
        {
            if (AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(assetPath) != null) return;

            var font = AssetDatabase.LoadAssetAtPath<Font>(fontPath);
            if (font == null)
            {
                Debug.LogError("FontAssetBuilder: font file not found at " + fontPath);
                return;
            }

            var asset = TMP_FontAsset.CreateFontAsset(font, 64, 5, GlyphRenderMode.SDFAA, 1024, 1024,
                AtlasPopulationMode.Dynamic, true);
            AssetDatabase.CreateAsset(asset, assetPath);
            AssetDatabase.AddObjectToAsset(asset.material, asset);
            foreach (var texture in asset.atlasTextures)
                AssetDatabase.AddObjectToAsset(texture, asset);

            asset.TryAddCharacters(Characters, out _);
            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssets();
        }

        static void SetTmpDefault(TMP_FontAsset asset)
        {
            if (asset == null || TMP_Settings.instance == null) return;
            var so = new SerializedObject(TMP_Settings.instance);
            var property = so.FindProperty("m_defaultFontAsset");
            if (property == null) return;
            property.objectReferenceValue = asset;
            so.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
