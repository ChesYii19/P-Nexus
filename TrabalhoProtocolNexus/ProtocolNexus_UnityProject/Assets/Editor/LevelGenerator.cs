using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class LevelGenerator : EditorWindow
{
    [MenuItem("Protocol Nexus/6 - GERAR O JOGO COMPLETO (FINAL)")]
    public static void GerarJogoCompleto()
    {
        // 1. Garante que todos os prefabs (incluindo Escada, HUD e Audio) existam e estejam atualizados!
        PrefabBuilder.CriarPrefabs();

        GerarNivel("Nivel1", 1);
        GerarNivel("Nivel2", 2);
        GerarNivel("Nivel3", 3);
        GerarNivel("Nivel4", 4);
        
        // Adicionar cenas ao Build Settings para o portal funcionar
        EditorBuildSettingsScene[] buildScenes = new EditorBuildSettingsScene[4];
        buildScenes[0] = new EditorBuildSettingsScene("Assets/Scenes/Nivel1.unity", true);
        buildScenes[1] = new EditorBuildSettingsScene("Assets/Scenes/Nivel2.unity", true);
        buildScenes[2] = new EditorBuildSettingsScene("Assets/Scenes/Nivel3.unity", true);
        buildScenes[3] = new EditorBuildSettingsScene("Assets/Scenes/Nivel4.unity", true);
        EditorBuildSettings.scenes = buildScenes;
        
        Debug.Log("✅ JOGO COMPLETO GERADO COM SUCESSO! Abra a cena Nivel1 e dê o Play!");
    }

    private static void GerarNivel(string nomeCena, int numFase)
    {
        Scene cena = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        cena.name = nomeCena;

        // Câmera
        GameObject cam = new GameObject("Main Camera");
        cam.tag = "MainCamera";
        cam.transform.position = new Vector3(-10f, 0f, -10f); // Puxa a câmera para trás!
        Camera camera = cam.AddComponent<Camera>();
        camera.clearFlags = CameraClearFlags.SolidColor; // Tira o céu 3D
        camera.orthographic = true;
        camera.orthographicSize = 5f;
        camera.backgroundColor = new Color(0.1f, 0.1f, 0.2f); // Fundo Cyberpunk Escuro
        cam.AddComponent<AudioListener>();
        
        CameraFollow camFollow = cam.AddComponent<CameraFollow>();

        // Carrega os Prefabs
        GameObject prefabChao = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Bloco_Chao.prefab");
        GameObject prefabEspinhos = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Espinhos_Morte.prefab");
        GameObject prefabDog = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Inimigo_Cachorro.prefab");
        GameObject prefabSpider = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Inimigo_Aranha_Voadora.prefab");
        GameObject prefabBoss = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Inimigo_Boss_Nexus.prefab");
        GameObject prefabPortal = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Portal_ProximaFase.prefab");
        GameObject prefabMoeda = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Moeda_Coletavel.prefab");
        GameObject prefabEscada = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Escada_Subir.prefab");
        GameObject prefabTatsuya = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Tatsuya_Completo.prefab");
        GameObject prefabHUD = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/HUD_Canvas.prefab");
        GameObject prefabAudio = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Gerenciador_Audio.prefab");

        // Sistemas Base (HUD, Audio, Player)
        PrefabUtility.InstantiatePrefab(prefabHUD);
        PrefabUtility.InstantiatePrefab(prefabAudio);
        
        GameObject gmObj = new GameObject("GameManager_Global");
        GameManager gm = gmObj.AddComponent<GameManager>();
        gm.cenasNiveis = new string[] { "Nivel1", "Nivel2", "Nivel3", "Nivel4" };
        
        GameObject tatsuya = (GameObject)PrefabUtility.InstantiatePrefab(prefabTatsuya);
        tatsuya.transform.position = new Vector3(-10f, 0f, 0f);
        camFollow.alvo = tatsuya.transform;

        // --- BACKGROUND PARALLAX DO SEWERS ---
        float posX = -10f;
        
        GameObject bgBack = new GameObject("SewersBack");
        bgBack.transform.position = new Vector3(0f, 0f, 10f);
        bgBack.transform.localScale = new Vector3(8f, 8f, 1f);
        SpriteRenderer srBack = bgBack.AddComponent<SpriteRenderer>();
        srBack.sortingOrder = -20;
        srBack.drawMode = SpriteDrawMode.Tiled;
        srBack.size = new Vector2(30f, 2f); // Bem largo para cobrir a fase
#if UNITY_EDITOR
        Object[] assetsBack = UnityEditor.AssetDatabase.LoadAllAssetsAtPath("Assets/Sprites/Environment/Sewers_Tileset/back.png");
        if (assetsBack != null && assetsBack.Length > 0 && assetsBack[0] is Sprite) srBack.sprite = (Sprite)assetsBack[0];
#endif

        GameObject bgMiddle = new GameObject("SewersMiddle");
        bgMiddle.transform.position = new Vector3(0f, 0f, 5f);
        bgMiddle.transform.localScale = new Vector3(8f, 8f, 1f);
        SpriteRenderer srMid = bgMiddle.AddComponent<SpriteRenderer>();
        srMid.sortingOrder = -15;
        srMid.drawMode = SpriteDrawMode.Tiled;
        srMid.size = new Vector2(30f, 2f);
#if UNITY_EDITOR
        Object[] assetsMid = UnityEditor.AssetDatabase.LoadAllAssetsAtPath("Assets/Sprites/Environment/Sewers_Tileset/middle.png");
        if (assetsMid != null && assetsMid.Length > 0 && assetsMid[0] is Sprite) srMid.sprite = (Sprite)assetsMid[0];
#endif

        GameObject bgFront = new GameObject("SewersFront");
        bgFront.transform.position = new Vector3(0f, -2f, -5f); // Na frente do player
        bgFront.transform.localScale = new Vector3(8f, 8f, 1f);
        SpriteRenderer srFront = bgFront.AddComponent<SpriteRenderer>();
        srFront.sortingOrder = 20; // Na frente de tudo
        srFront.drawMode = SpriteDrawMode.Tiled;
        srFront.size = new Vector2(30f, 2f);
#if UNITY_EDITOR
        Object[] assetsFront = UnityEditor.AssetDatabase.LoadAllAssetsAtPath("Assets/Sprites/Environment/Sewers_Tileset/front.png");
        if (assetsFront != null && assetsFront.Length > 0 && assetsFront[0] is Sprite) srFront.sprite = (Sprite)assetsFront[0];
#endif

        // --- Level Design Procedural ---
        
        int blockCount = 6 + (numFase * 2); // Fases ficam cada vez maiores
        for (int i = 0; i < blockCount; i++)
        {
            // Chão Base
            GameObject chao = (GameObject)PrefabUtility.InstantiatePrefab(prefabChao);
            chao.transform.position = new Vector3(posX, -3f, 0f);
            
            // Decoração e Desafios (Semente variada pela fase)
            if (i > 0 && i < blockCount - 1)
            {
                int random = Random.Range(0, 5); // Mais variação
                
                // Em fases avançadas, mais inimigos
                if (random == 0 || (random == 1 && numFase > 1)) // Inimigo Terrestre (Free City 1)
                {
                    GameObject dog = (GameObject)PrefabUtility.InstantiatePrefab(prefabDog);
                    dog.transform.position = new Vector3(posX, -1.5f, 0f);
                }
                else if (random == 1 && numFase > 2) // Inimigo Voador (Free City 2)
                {
                    GameObject spider = (GameObject)PrefabUtility.InstantiatePrefab(prefabSpider);
                    spider.transform.position = new Vector3(posX, 2f, 0f);
                }
                else if (random == 2) // Fosso com Espinhos/Água
                {
                    chao.transform.position = new Vector3(posX, -10f, 0f); // Esconde o chão
                    GameObject espinhos = (GameObject)PrefabUtility.InstantiatePrefab(prefabEspinhos);
                    espinhos.transform.position = new Vector3(posX, -3f, 0f);
                    
                    // Plataforma flutuante acima
                    GameObject plat = (GameObject)PrefabUtility.InstantiatePrefab(prefabChao);
                    plat.transform.position = new Vector3(posX, 0f, 0f);
                    plat.transform.localScale = new Vector3(0.5f, 0.5f, 1f); // Plataforma menor
                }
                else if (random == 3) // Escada e Moeda
                {
                    GameObject escada = (GameObject)PrefabUtility.InstantiatePrefab(prefabEscada);
                    escada.transform.position = new Vector3(posX, 0f, 0f);
                    
                    GameObject moeda = (GameObject)PrefabUtility.InstantiatePrefab(prefabMoeda);
                    moeda.transform.position = new Vector3(posX, 3f, 0f);
                }
            }
            
            posX += 6f; // Avança pra direita
        }

        // Final da Fase (Portal ou Boss)
        if (numFase == 4)
        {
            // Fase do Boss
            GameObject chaoBoss = (GameObject)PrefabUtility.InstantiatePrefab(prefabChao);
            chaoBoss.transform.position = new Vector3(posX, -3f, 0f);
            chaoBoss.transform.localScale = new Vector3(2f, 1f, 1f); // Chão maior

            GameObject boss = (GameObject)PrefabUtility.InstantiatePrefab(prefabBoss);
            boss.transform.position = new Vector3(posX + 3f, -1.5f, 0f);
        }
        else
        {
            GameObject portal = (GameObject)PrefabUtility.InstantiatePrefab(prefabPortal);
            portal.transform.position = new Vector3(posX - 6f, -1f, 0f); // Coloca o portal no último bloco
        }

        // Salvar a cena
        if (!AssetDatabase.IsValidFolder("Assets/Scenes"))
        {
            AssetDatabase.CreateFolder("Assets", "Scenes");
        }
        EditorSceneManager.SaveScene(cena, "Assets/Scenes/" + nomeCena + ".unity");
    }
}
