using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.IO;

public class AutoSlicer : EditorWindow
{
    [MenuItem("Protocol Nexus/Fatiar Sprites do Cyberpunk e Boss")]
    public static void SliceCyberpunkSprites()
    {
        string[] pastasList = {
            "Assets/Sprites/Player/Sewers",
            "Assets/Sprites/Enemies/Golem"
        };

        foreach (string pastaAlvo in pastasList)
        {
            string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { pastaAlvo });
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (path.Contains("Weapon") || path.Contains("bullet")) continue;
                
                SliceSprite(path);
            }
        }
        
        Debug.Log("Todas as sprites do Cyberpunk e Boss foram fatiadas com sucesso!");
    }

    private static void SliceSprite(string path)
    {
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer != null)
        {
            // Força modo múltiplo e legível
            importer.isReadable = true;
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Multiple;
            importer.filterMode = FilterMode.Point; // Pixel art
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            
            // Reimporta para poder ler a textura
            importer.SaveAndReimport();

            Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (tex == null) return;

            // Gera os retângulos automaticamente
            Rect[] rects = InternalSpriteUtility.GenerateAutomaticSpriteRectangles(tex, 0, 0);
            
            SpriteMetaData[] metaData = new SpriteMetaData[rects.Length];
            for (int i = 0; i < rects.Length; i++)
            {
                metaData[i] = new SpriteMetaData
                {
                    rect = rects[i],
                    alignment = 9, // Custom pivot
                    pivot = new Vector2(0.5f, 0.5f), // Center
                    name = tex.name + "_" + i
                };
            }
            
            importer.spritesheet = metaData;
            importer.isReadable = false; // Fecha leitura pra otimizar
            importer.SaveAndReimport();
            Debug.Log($"Fatiado: {path} em {rects.Length} quadros.");
        }
    }
}
