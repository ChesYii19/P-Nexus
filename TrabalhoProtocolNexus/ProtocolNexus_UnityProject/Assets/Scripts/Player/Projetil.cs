using UnityEngine;

/// <summary>
/// Controla o comportamento do projétil disparado por Tatsuya Yuki.
/// Move em direção configurada, destrói ao colidir com inimigos ou paredes.
/// Tag de inimigo: "Inimigo" | Tag de parede: "Parede" ou LayerMask.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class Projetil : MonoBehaviour
{
    // ─── Configurações ─────────────────────────────────────────────────
    [Header("Projétil")]
    [Tooltip("Velocidade do projétil")]
    public float velocidade = 14f;

    [Tooltip("Dano causado ao inimigo")]
    public int dano = 1;

    [Tooltip("Tempo máximo de vida antes de auto-destruir")]
    public float tempoDeVida = 3f;

    [Header("Efeito Visual")]
    [Tooltip("Prefab de efeito de impacto (opcional)")]
    public GameObject efeitoImpacto;

    // ─── Estado Interno ────────────────────────────────────────────────
    private Rigidbody2D rb;
    private Vector2 direcao = Vector2.right;

    // ──────────────────────────────────────────────────────────────────
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f; // Projétil não cai
        
        // FOOLPROOF PROJETIL
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            sr = gameObject.AddComponent<SpriteRenderer>();
#if UNITY_EDITOR
            Object[] assets = UnityEditor.AssetDatabase.LoadAllAssetsAtPath("Assets/Sprites/Weapons/pulse.png");
            if (assets != null && assets.Length > 0) sr.sprite = assets[0] as Sprite;
#endif
            sr.color = Color.cyan; // Deixa o tiro com uma cor legal
            
            transform.localScale = new Vector3(2f, 2f, 1f); // Aumenta o tamanho pra ficar visivel
        }
    }

    void Start()
    {
        // Auto-destruição por tempo
        Destroy(gameObject, tempoDeVida);
    }

    void FixedUpdate()
    {
        rb.velocity = direcao * velocidade;
    }

    // ─── API: Inicializa a direção do projétil ─────────────────────────
    /// <summary>Chamado por PlayerAttack ao instanciar o projétil.</summary>
    public void Inicializar(Vector2 direcaoDisparo)
    {
        direcao = direcaoDisparo.normalized;

        // Rotaciona visualmente o sprite se atirar para a esquerda
        if (direcao.x < 0f)
            transform.localScale = new Vector3(-1f, 1f, 1f);
    }

    // ─── Colisão ───────────────────────────────────────────────────────
    void OnTriggerEnter2D(Collider2D other)
    {
        // Ignorar colisão com o próprio jogador
        if (other.gameObject.name.Contains("Player") || other.CompareTag("Player")) return;

        // Colidiu com inimigo
        if (other.gameObject.name.Contains("Inimigo"))
        {
            EnemyHealth vida = other.GetComponent<EnemyHealth>();
            if (vida != null)
                vida.ReceberDano(dano);

            CriarEfeito();
            Destroy(gameObject);
        }
        else if (other.gameObject.name.Contains("Chao") || other.gameObject.name.Contains("Bloco"))
        {
            // Destrói ao bater na parede/chão
            CriarEfeito();
            Destroy(gameObject);
        }
    }

    // ─── Cria efeito visual de impacto ────────────────────────────────
    private void CriarEfeito()
    {
        if (efeitoImpacto != null)
        {
            GameObject efeito = Instantiate(efeitoImpacto, transform.position, Quaternion.identity);
            Destroy(efeito, 0.5f);
        }
    }
}

