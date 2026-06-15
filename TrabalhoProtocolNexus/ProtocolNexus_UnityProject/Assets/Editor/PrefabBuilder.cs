using UnityEngine;
using UnityEditor;
using System.IO;

public class PrefabBuilder : EditorWindow
{
    [MenuItem("Protocol Nexus/4 - Criar Peças de LEGO (Prefabs)")]
    public static void CriarPrefabs()
    {
        // Forçar fatiamento antes de criar
        SpriteAutoSlicer.FatiarTudo();

        // 1. Criar Tags necessárias para evitar o erro "Tag is not defined"
        CriarTagSeNaoExistir("Ground");
        CriarTagSeNaoExistir("Hazard");
        CriarTagSeNaoExistir("Saida");
        CriarTagSeNaoExistir("Coin");
        CriarTagSeNaoExistir("Enemy");

        if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
        {
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        }

        CriarPrefabChao();
        CriarPrefabEspinhos();
        CriarPrefabEscada();
        CriarPrefabPortal();
        CriarPrefabProjetil();
        CriarPrefabMoeda();
        CriarPrefabTatsuya();
        CriarPrefabDog();
        CriarPrefabSpider();
        CriarPrefabBoss();
        CriarPrefabHUD();
        CriarPrefabAudio();

        Debug.Log("✅ Peças de LEGO (Prefabs) criadas com sucesso na pasta Assets/Prefabs!");
    }

    private static void CriarPrefabChao()
    {
        GameObject chao = new GameObject("Bloco_Chao");
        chao.tag = "Ground";
        SpriteRenderer sr = chao.AddComponent<SpriteRenderer>();
        
        // Pegar uma sprite específica do tileset (ex: tileset_17)
        Object[] assets = AssetDatabase.LoadAllAssetsAtPath("Assets/Sprites/Environment/Sewers_Tileset/tileset.png");
        Sprite spriteChao = null;
        if (assets != null) {
            foreach (var a in assets) {
                if (a is Sprite && a.name == "tileset.png_17") {
                    spriteChao = (Sprite)a; break;
                }
            }
            if (spriteChao == null && assets.Length > 1 && assets[1] is Sprite) spriteChao = (Sprite)assets[1];
        }
        sr.sprite = spriteChao;
        sr.drawMode = SpriteDrawMode.Tiled;
        sr.size = new Vector2(5f, 2f);
        BoxCollider2D col = chao.AddComponent<BoxCollider2D>();
        col.size = new Vector2(5f, 2f);
        
        PrefabUtility.SaveAsPrefabAsset(chao, "Assets/Prefabs/Bloco_Chao.prefab");
        DestroyImmediate(chao);
    }

    private static void CriarPrefabEspinhos()
    {
        GameObject espinhos = new GameObject("Espinhos_Morte");
        espinhos.tag = "Hazard";
        SpriteRenderer sr = espinhos.AddComponent<SpriteRenderer>();
        // Reusando um tile do Sewers e pintando de vermelho pra indicar perigo, ou usando water.png
        Object[] assets = AssetDatabase.LoadAllAssetsAtPath("Assets/Sprites/Environment/Sewers_Tileset/water.png");
        if (assets != null && assets.Length > 0 && assets[0] is Sprite) sr.sprite = (Sprite)assets[0];
        
        sr.drawMode = SpriteDrawMode.Tiled;
        sr.size = new Vector2(5f, 1f);
        sr.color = new Color(0.8f, 0.2f, 0.8f); // Água tóxica roxa
        
        BoxCollider2D col = espinhos.AddComponent<BoxCollider2D>();
        col.size = new Vector2(5f, 1f);
        col.isTrigger = true;
        
        HazardZone hz = espinhos.AddComponent<HazardZone>();
        hz.dano = 1;
        hz.morteInstantanea = true;

        PrefabUtility.SaveAsPrefabAsset(espinhos, "Assets/Prefabs/Espinhos_Morte.prefab");
        DestroyImmediate(espinhos);
    }

    private static void CriarPrefabPortal()
    {
        GameObject portal = new GameObject("Portal_ProximaFase");
        portal.tag = "Saida";
        SpriteRenderer sr = portal.AddComponent<SpriteRenderer>();
        sr.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Environment/Neon_District/Props/door_open.png");
        
        BoxCollider2D pCol = portal.AddComponent<BoxCollider2D>();
        pCol.isTrigger = true;
        pCol.size = new Vector2(2f, 4f);
        portal.AddComponent<LevelManager>();

        PrefabUtility.SaveAsPrefabAsset(portal, "Assets/Prefabs/Portal_ProximaFase.prefab");
        DestroyImmediate(portal);
    }

    private static void CriarPrefabMoeda()
    {
        GameObject moeda = new GameObject("Moeda_Coletavel");
        moeda.tag = "Coin";
        SpriteRenderer sr = moeda.AddComponent<SpriteRenderer>();
        sr.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Environment/Neon_District/Props/lamp_neon.png"); // placeholder
        sr.color = Color.yellow;
        
        BoxCollider2D col = moeda.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size = new Vector2(1f, 1f);
        
        Item item = moeda.AddComponent<Item>();
        item.valor = 100;
        item.tipo = Item.TipoItem.Moeda;

        PrefabUtility.SaveAsPrefabAsset(moeda, "Assets/Prefabs/Moeda_Coletavel.prefab");
        DestroyImmediate(moeda);
    }

    private static void CriarPrefabProjetil()
    {
        GameObject proj = new GameObject("Tiro_Tatsuya");
        SpriteRenderer sr = proj.AddComponent<SpriteRenderer>();
        sr.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Weapons/Warped_FX/Warped shooting fx files/Assets/Pixelart/Bolt/sprites/bolt1.png");
        
        BoxCollider2D col = proj.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size = new Vector2(0.5f, 0.2f);
        
        Rigidbody2D rb = proj.AddComponent<Rigidbody2D>();
        rb.isKinematic = true;
        
        proj.AddComponent<Projetil>();

        PrefabUtility.SaveAsPrefabAsset(proj, "Assets/Prefabs/Tiro_Tatsuya.prefab");
        DestroyImmediate(proj);
    }

    private static void CriarPrefabTatsuya()
    {
        GameObject player = new GameObject("Tatsuya_Completo");
        player.tag = "Player";
        SpriteRenderer playerSr = player.AddComponent<SpriteRenderer>();
        playerSr.sortingOrder = 5;
        
        SpriteAnimator anim = player.AddComponent<SpriteAnimator>();
        anim.states = new SpriteAnimState[] {
            new SpriteAnimState { stateName = "Idle", spriteSheetPath = "Assets/Sprites/Player/Soldier/player-idle.png", fps = 8, loop = true },
            new SpriteAnimState { stateName = "Run", spriteSheetPath = "Assets/Sprites/Player/Soldier/player-run.png", fps = 12, loop = true },
            new SpriteAnimState { stateName = "Jump", spriteSheetPath = "Assets/Sprites/Player/Soldier/player-jump.png", fps = 10, loop = false },
            new SpriteAnimState { stateName = "Fall", spriteSheetPath = "Assets/Sprites/Player/Soldier/player-fall.png", fps = 10, loop = true },
            new SpriteAnimState { stateName = "Attack", spriteSheetPath = "Assets/Sprites/Player/Soldier/player-shoot.png", fps = 12, loop = false },
            new SpriteAnimState { stateName = "Hurt", spriteSheetPath = "Assets/Sprites/Player/Soldier/player-Hurt.png", fps = 10, loop = false }
        };
        
        Rigidbody2D rb = player.AddComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        
        PlayerController pCtrl = player.AddComponent<PlayerController>();
        pCtrl.forcaPulo = 7f;
        
        BoxCollider2D playerCol = player.AddComponent<BoxCollider2D>();
        playerCol.size = new Vector2(0.8f, 1.2f); // Ajustado para o novo tamanho do soldier
        
        PlayerStatus pStatus = player.AddComponent<PlayerStatus>();
        PlayerAttack pAttack = player.AddComponent<PlayerAttack>();
        
        GameObject chaoCheck = new GameObject("VerificadorChao");
        chaoCheck.transform.SetParent(player.transform);
        chaoCheck.transform.localPosition = new Vector3(0, -0.6f, 0);
        pCtrl.verificadorChao = chaoCheck.transform;
        pCtrl.layerChao = 1;

        GameObject tiroCheck = new GameObject("PontoDisparo");
        tiroCheck.transform.SetParent(player.transform);
        tiroCheck.transform.localPosition = new Vector3(0.6f, 0, 0);
        pAttack.pontoDisparo = tiroCheck.transform;

        GameObject prefabTiro = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Tiro_Tatsuya.prefab");
        pAttack.prefabProjetil = prefabTiro;
        
        CameraFollow camScript = Camera.main?.gameObject.GetComponent<CameraFollow>();
        if (camScript == null) camScript = Camera.main?.gameObject.AddComponent<CameraFollow>();
        if (camScript != null) camScript.alvo = player.transform;

        PrefabUtility.SaveAsPrefabAsset(player, "Assets/Prefabs/Tatsuya_Completo.prefab");
        DestroyImmediate(player);
    }

    private static void CriarPrefabDog()
    {
        GameObject enemy = new GameObject("Inimigo_Cachorro");
        enemy.tag = "Enemy";
        SpriteRenderer enemySr = enemy.AddComponent<SpriteRenderer>();
        
        SpriteAnimator anim = enemy.AddComponent<SpriteAnimator>();
        anim.states = new SpriteAnimState[] {
            new SpriteAnimState { stateName = "Idle", spriteSheetPath = "Assets/Sprites/Enemies/FreeCity/1/Idle.png", fps = 8, loop = true },
            new SpriteAnimState { stateName = "Walk", spriteSheetPath = "Assets/Sprites/Enemies/FreeCity/1/Walk.png", fps = 10, loop = true },
            new SpriteAnimState { stateName = "Attack", spriteSheetPath = "Assets/Sprites/Enemies/FreeCity/1/Attack.png", fps = 12, loop = false },
            new SpriteAnimState { stateName = "Hurt", spriteSheetPath = "Assets/Sprites/Enemies/FreeCity/1/Hurt.png", fps = 10, loop = false },
            new SpriteAnimState { stateName = "Death", spriteSheetPath = "Assets/Sprites/Enemies/FreeCity/1/Death.png", fps = 10, loop = false }
        };
        
        BoxCollider2D col = enemy.AddComponent<BoxCollider2D>();
        col.size = new Vector2(1f, 1f);
        
        Rigidbody2D rb = enemy.AddComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        
        enemy.AddComponent<EnemyController>().tipo = EnemyController.TipoInimigo.Terrestre;

        PrefabUtility.SaveAsPrefabAsset(enemy, "Assets/Prefabs/Inimigo_Cachorro.prefab");
        DestroyImmediate(enemy);
    }

    private static void CriarPrefabSpider()
    {
        GameObject enemy = new GameObject("Inimigo_Aranha_Voadora");
        enemy.tag = "Enemy";
        SpriteRenderer enemySr = enemy.AddComponent<SpriteRenderer>();
        
        SpriteAnimator anim = enemy.AddComponent<SpriteAnimator>();
        anim.states = new SpriteAnimState[] {
            new SpriteAnimState { stateName = "Idle", spriteSheetPath = "Assets/Sprites/Enemies/FreeCity/2/Idle.png", fps = 8, loop = true },
            new SpriteAnimState { stateName = "Walk", spriteSheetPath = "Assets/Sprites/Enemies/FreeCity/2/Walk.png", fps = 10, loop = true },
            new SpriteAnimState { stateName = "Attack", spriteSheetPath = "Assets/Sprites/Enemies/FreeCity/2/Attack.png", fps = 12, loop = false },
            new SpriteAnimState { stateName = "Hurt", spriteSheetPath = "Assets/Sprites/Enemies/FreeCity/2/Hurt.png", fps = 10, loop = false },
            new SpriteAnimState { stateName = "Death", spriteSheetPath = "Assets/Sprites/Enemies/FreeCity/2/Death.png", fps = 10, loop = false }
        };
        
        BoxCollider2D col = enemy.AddComponent<BoxCollider2D>();
        col.size = new Vector2(1f, 1f);
        
        Rigidbody2D rb = enemy.AddComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        rb.gravityScale = 0; // Voa
        
        enemy.AddComponent<EnemyController>().tipo = EnemyController.TipoInimigo.Voador;

        PrefabUtility.SaveAsPrefabAsset(enemy, "Assets/Prefabs/Inimigo_Aranha_Voadora.prefab");
        DestroyImmediate(enemy);
    }

    private static void CriarPrefabBoss()
    {
        GameObject boss = new GameObject("Inimigo_Boss_Nexus");
        boss.tag = "Enemy";
        SpriteRenderer bossSr = boss.AddComponent<SpriteRenderer>();
        boss.transform.localScale = new Vector3(2f, 2f, 1f); // Boss maior
        
        SpriteAnimator anim = boss.AddComponent<SpriteAnimator>();
        anim.states = new SpriteAnimState[] {
            new SpriteAnimState { stateName = "Idle", spriteSheetPath = "Assets/Sprites/Enemies/FreeCity/6/Idle.png", fps = 8, loop = true },
            new SpriteAnimState { stateName = "Walk", spriteSheetPath = "Assets/Sprites/Enemies/FreeCity/6/Walk.png", fps = 10, loop = true },
            new SpriteAnimState { stateName = "Attack", spriteSheetPath = "Assets/Sprites/Enemies/FreeCity/6/Attack.png", fps = 12, loop = false },
            new SpriteAnimState { stateName = "Hurt", spriteSheetPath = "Assets/Sprites/Enemies/FreeCity/6/Hurt.png", fps = 10, loop = false },
            new SpriteAnimState { stateName = "Death", spriteSheetPath = "Assets/Sprites/Enemies/FreeCity/6/Death.png", fps = 10, loop = false }
        };
        
        boss.AddComponent<Rigidbody2D>().freezeRotation = true;
        boss.AddComponent<BoxCollider2D>().size = new Vector2(1.5f, 1.5f);
        boss.AddComponent<Boss>();

        PrefabUtility.SaveAsPrefabAsset(boss, "Assets/Prefabs/Inimigo_Boss_Nexus.prefab");
        DestroyImmediate(boss);
    }

    private static void CriarPrefabEscada()
    {
        GameObject escada = new GameObject("Escada_Subir");
        SpriteRenderer sr = escada.AddComponent<SpriteRenderer>();
        sr.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Environment/Neon_District/Props/lamp_neon.png");
        sr.drawMode = SpriteDrawMode.Tiled;
        sr.size = new Vector2(1f, 5f);
        
        escada.AddComponent<Escada>();
        BoxCollider2D col = escada.GetComponent<BoxCollider2D>();
        col.size = new Vector2(1f, 5f);
        
        PrefabUtility.SaveAsPrefabAsset(escada, "Assets/Prefabs/Escada_Subir.prefab");
        DestroyImmediate(escada);
    }

    private static void CriarPrefabHUD()
    {
        GameObject hud = new GameObject("HUD_Canvas");
        Canvas canvas = hud.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        hud.AddComponent<UnityEngine.UI.CanvasScaler>().uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
        hud.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        
        GameObject texto = new GameObject("StatusText");
        texto.transform.SetParent(hud.transform);
        UnityEngine.UI.Text txt = texto.AddComponent<UnityEngine.UI.Text>();
        txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        txt.fontSize = 24;
        txt.color = Color.white;
        RectTransform rt = texto.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 1);
        rt.anchorMax = new Vector2(0, 1);
        rt.pivot = new Vector2(0, 1);
        rt.anchoredPosition = new Vector2(20, -20);
        rt.sizeDelta = new Vector2(400, 100);
        
        PrefabUtility.SaveAsPrefabAsset(hud, "Assets/Prefabs/HUD_Canvas.prefab");
        DestroyImmediate(hud);
    }

    private static void CriarPrefabAudio()
    {
        GameObject audio = new GameObject("Gerenciador_Audio");
        AudioManager am = audio.AddComponent<AudioManager>();
        am.musicaBoss = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/BOSS-alert.wav");
        
        PrefabUtility.SaveAsPrefabAsset(audio, "Assets/Prefabs/Gerenciador_Audio.prefab");
        DestroyImmediate(audio);
    }

    private static void CriarTagSeNaoExistir(string tag)
    {
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty tagsProp = tagManager.FindProperty("tags");

        for (int i = 0; i < tagsProp.arraySize; i++)
        {
            SerializedProperty t = tagsProp.GetArrayElementAtIndex(i);
            if (t.stringValue.Equals(tag)) return; // Já existe
        }

        tagsProp.InsertArrayElementAtIndex(tagsProp.arraySize);
        SerializedProperty newTag = tagsProp.GetArrayElementAtIndex(tagsProp.arraySize - 1);
        newTag.stringValue = tag;
        tagManager.ApplyModifiedProperties();
    }
}
