using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class SpriteAutoSlicer
{
    [MenuItem("Protocol Nexus/4.5 - Fatiar Sprites (Sewers e Free City)")]
    public static void FatiarTudo()
    {
        // Player Sewers (64x77)
        SliceSpriteSheet("Assets/Sprites/Player/Soldier/player-idle.png", 64, 77);
        SliceSpriteSheet("Assets/Sprites/Player/Soldier/player-run.png", 64, 77);
        SliceSpriteSheet("Assets/Sprites/Player/Soldier/player-jump.png", 64, 77);
        SliceSpriteSheet("Assets/Sprites/Player/Soldier/player-fall.png", 64, 77);
        SliceSpriteSheet("Assets/Sprites/Player/Soldier/player-shoot.png", 64, 77);
        SliceSpriteSheet("Assets/Sprites/Player/Soldier/player-crouch.png", 64, 77);
        SliceSpriteSheet("Assets/Sprites/Player/Soldier/player-Hurt.png", 64, 77);

        // Free City Enemies 1 (48x48)
        SliceSpriteSheet("Assets/Sprites/Enemies/FreeCity/1/Idle.png", 48, 48);
        SliceSpriteSheet("Assets/Sprites/Enemies/FreeCity/1/Walk.png", 48, 48);
        SliceSpriteSheet("Assets/Sprites/Enemies/FreeCity/1/Attack.png", 48, 48);
        SliceSpriteSheet("Assets/Sprites/Enemies/FreeCity/1/Death.png", 48, 48);
        SliceSpriteSheet("Assets/Sprites/Enemies/FreeCity/1/Hurt.png", 48, 48);

        // Free City Enemies 2 (48x48)
        SliceSpriteSheet("Assets/Sprites/Enemies/FreeCity/2/Idle.png", 48, 48);
        SliceSpriteSheet("Assets/Sprites/Enemies/FreeCity/2/Walk.png", 48, 48);
        SliceSpriteSheet("Assets/Sprites/Enemies/FreeCity/2/Attack.png", 48, 48);
        SliceSpriteSheet("Assets/Sprites/Enemies/FreeCity/2/Death.png", 48, 48);
        SliceSpriteSheet("Assets/Sprites/Enemies/FreeCity/2/Hurt.png", 48, 48);

        // Free City Enemies 6 - Boss (48x48)
        SliceSpriteSheet("Assets/Sprites/Enemies/FreeCity/6/Idle.png", 48, 48);
        SliceSpriteSheet("Assets/Sprites/Enemies/FreeCity/6/Walk.png", 48, 48);
        SliceSpriteSheet("Assets/Sprites/Enemies/FreeCity/6/Attack.png", 48, 48);
        SliceSpriteSheet("Assets/Sprites/Enemies/FreeCity/6/Death.png", 48, 48);
        SliceSpriteSheet("Assets/Sprites/Enemies/FreeCity/6/Hurt.png", 48, 48);

        // Sewers Tileset (16x16)
        SliceSpriteSheet("Assets/Sprites/Environment/Sewers_Tileset/tileset.png", 16, 16);

        // Atualizar também os Backgrounds para garantir FilterMode Point (Pixel Art)
        ApplyPixelArtSettings("Assets/Sprites/Environment/Sewers_Tileset/back.png");
        ApplyPixelArtSettings("Assets/Sprites/Environment/Sewers_Tileset/middle.png");
        ApplyPixelArtSettings("Assets/Sprites/Environment/Sewers_Tileset/front.png");

        Debug.Log("✂️ Sprites fatiados com sucesso!");
    }

    private static void SliceSpriteSheet(string path, int cellWidth, int cellHeight, int ppu = 16)
    {
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null) return;
        
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Multiple;
        importer.filterMode = FilterMode.Point;
        importer.spritePixelsPerUnit = ppu;
        importer.textureCompression = TextureImporterCompression.Uncompressed;

        Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        if (tex == null) return;

        int cols = tex.width / cellWidth;
        int rows = tex.height / cellHeight;
        
        List<SpriteMetaData> metaDataList = new List<SpriteMetaData>();
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                SpriteMetaData smd = new SpriteMetaData();
                smd.name = $"{tex.name}_{x + (rows - 1 - y) * cols}";
                // Unity UI coordinates: Y is bottom to top!
                smd.rect = new Rect(x * cellWidth, y * cellHeight, cellWidth, cellHeight);
                smd.pivot = new Vector2(0.5f, 0.5f);
                smd.alignment = 9; // Custom pivot
                metaDataList.Add(smd);
            }
        }

        importer.spritesheet = metaDataList.ToArray();
        EditorUtility.SetDirty(importer);
        importer.SaveAndReimport();
    }

    private static void ApplyPixelArtSettings(string path)
    {
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.filterMode = FilterMode.Point;
            importer.spritePixelsPerUnit = 16;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            EditorUtility.SetDirty(importer);
            importer.SaveAndReimport();
        }
    }
}
