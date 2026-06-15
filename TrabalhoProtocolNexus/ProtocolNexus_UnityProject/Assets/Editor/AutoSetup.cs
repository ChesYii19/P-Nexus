using UnityEngine;
using UnityEditor;

public class AutoSetup
{
    [MenuItem("Protocol Nexus/Auto-Montar Nivel 1")]
    public static void SetupScene()
    {
        // 1. Configurar Camera para 2D
        Camera cam = Camera.main;
        if (cam == null) {
            GameObject camObj = new GameObject("Main Camera");
            camObj.tag = "MainCamera";
            cam = camObj.AddComponent<Camera>();
        }
        cam.orthographic = true;
        cam.orthographicSize = 5f;
        cam.backgroundColor = new Color(0.05f, 0.05f, 0.1f); // Fundo escuro
        
        // 2. Chão
        GameObject floor = new GameObject("Chao_Neon");
        SpriteRenderer floorSr = floor.AddComponent<SpriteRenderer>();
        Sprite floorSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Environment/Neon_District/Tiles/floor_concrete_clean_base.png");
        floorSr.sprite = floorSprite;
        floorSr.drawMode = SpriteDrawMode.Tiled;
        floorSr.size = new Vector2(30f, 1f); // Plataforma longa
        
        BoxCollider2D floorCol = floor.AddComponent<BoxCollider2D>();
        floorCol.size = new Vector2(30f, 1f);
        floor.transform.position = new Vector3(0, -3f, 0);

        // 3. Tatsuya (Jogador)
        GameObject player = new GameObject("Tatsuya");
        player.tag = "Player";
        SpriteRenderer playerSr = player.AddComponent<SpriteRenderer>();
        Sprite playerSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Player/MainCharacter(FreePack)/MainCharacter(FreePack)/Idle.png");
        playerSr.sprite = playerSprite;
        
        Rigidbody2D rb = player.AddComponent<Rigidbody2D>();
        rb.freezeRotation = true; // Não cair de cara
        
        BoxCollider2D playerCol = player.AddComponent<BoxCollider2D>();
        
        // Adicionando os scripts que criamos
        player.AddComponent<PlayerController>();
        player.AddComponent<PlayerStatus>();
        player.AddComponent<PlayerAttack>();
        
        player.transform.position = new Vector3(0, 0, 0);

        // A Câmera segue o jogador
        CameraFollow camScript = cam.gameObject.AddComponent<CameraFollow>();
        camScript.alvo = player.transform;

        // 4. Game Manager Base
        GameObject gm = new GameObject("GameManager");
        gm.AddComponent<GameManager>();

        Debug.Log("🎉 Auto-Setup concluído! Câmera, Chão e Jogador configurados. Clique no PLAY para testar.");
    }
}
