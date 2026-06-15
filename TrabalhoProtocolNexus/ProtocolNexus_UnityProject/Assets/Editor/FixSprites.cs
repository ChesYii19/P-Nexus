using UnityEngine;
using UnityEditor;

public class FixSprites
{
    [MenuItem("Protocol Nexus/1 - Corrigir Texturas (Rodar Primeiro!)")]
    public static void FixAllSprites()
    {
        string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { "Assets/Sprites" });
        int count = 0;
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                bool changed = false;
                
                // Transforma em Sprite 2D
                if (importer.textureType != TextureImporterType.Sprite)
                {
                    importer.textureType = TextureImporterType.Sprite;
                    changed = true;
                }
                
                // Modo Point (No Filter) para Pixel Art não ficar borrado
                if (importer.filterMode != FilterMode.Point)
                {
                    importer.filterMode = FilterMode.Point;
                    changed = true;
                }
                
                // Sem compressão para manter as cores puras
                if (importer.textureCompression != TextureImporterCompression.Uncompressed)
                {
                    importer.textureCompression = TextureImporterCompression.Uncompressed;
                    changed = true;
                }
                
                if (changed)
                {
                    importer.SaveAndReimport();
                    count++;
                }
            }
        }
        Debug.Log($"Sucesso! {count} texturas foram convertidas para Sprites 2D de Pixel Art!");
    }
}
