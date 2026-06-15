using UnityEngine;

/// <summary>
/// Gerencia a vida dos inimigos do Protocol Nexus.
/// Recebe dano, exibe feedback visual, morre e dá drop de item.
/// Tag obrigatória na GameObject: "Inimigo".
/// </summary>
public class EnemyHealth : MonoBehaviour
{
    // ─── Configurações ─────────────────────────────────────────────────
    [Header("Vida")]
    [Tooltip("Pontos de vida do inimigo")]
    public int vidaMaxima = 3;

    [Header("Drop ao Morrer")]
    [Tooltip("Prefab de item que o inimigo solta ao morrer (opcional)")]
    public GameObject prefabDrop;

    [Tooltip("Chance de drop (0 a 1). 1 = sempre dropa")]
    [Range(0f, 1f)]
    public float chanceDrop = 0.5f;

    [Tooltip("Pontos concedidos ao jogador ao matar este inimigo")]
    public int pontosPorMorte = 100;

    [Header("Feedback Visual")]
    [Tooltip("Cor flash ao tomar dano")]
    public Color corDano = Color.red;

    [Tooltip("Duração do flash de dano")]
    public float duracaoFlash = 0.15f;

    [Header("Efeito de Morte")]
    [Tooltip("Prefab de explosão/partícula ao morrer (opcional)")]
    public GameObject efeitoMorte;

    // ─── Estado Interno ────────────────────────────────────────────────
    private int vidaAtual;
    private bool estaMorto;
    private SpriteRenderer spriteRenderer;
    private Color corOriginal;
    private Animator anim;

    private static readonly int AnimMorto = Animator.StringToHash("morto");
    private static readonly int AnimDano  = Animator.StringToHash("dano");

    // ──────────────────────────────────────────────────────────────────
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim           = GetComponent<Animator>();

        if (spriteRenderer != null)
            corOriginal = spriteRenderer.color;
    }

    void Start()
    {
        vidaAtual = vidaMaxima;
    }

    // ─── API Pública: Receber Dano ────────────────────────────────────
    /// <summary>Aplica dano ao inimigo. Chamado por Projetil.</summary>
    public void ReceberDano(int quantidade)
    {
        if (estaMorto) return;

        vidaAtual -= quantidade;
        vidaAtual  = Mathf.Max(vidaAtual, 0);

        // Flash visual de dano
        if (spriteRenderer != null)
        {
            spriteRenderer.color = corDano;
            Invoke(nameof(ResetarCor), duracaoFlash);
        }

        // Animação de dano
        // if (anim != null && anim.runtimeAnimatorController != null) anim.SetTrigger...

        // SFX
        AudioManager.Instance?.PlaySFX("inimigo_dano");

        if (vidaAtual <= 0)
            Morrer();
    }

    // ─── Morte ────────────────────────────────────────────────────────
    private void Morrer()
    {
        if (estaMorto) return;
        estaMorto = true;

        // Desativa colisão para não colidir mais
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        // Animação de morte
        // if (anim != null && anim.runtimeAnimatorController != null) anim.SetTrigger...

        // Concede pontos ao jogador
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            GameManager.Instance?.AdicionarPontuacao(pontosPorMorte);
        }

        // Drop aleatório de item
        if (prefabDrop != null && Random.value <= chanceDrop)
            Instantiate(prefabDrop, transform.position, Quaternion.identity);

        // Efeito visual de morte
        if (efeitoMorte != null)
        {
            GameObject efeito = Instantiate(efeitoMorte, transform.position, Quaternion.identity);
            Destroy(efeito, 1f);
        }

        // SFX de morte
        AudioManager.Instance?.PlaySFX("inimigo_morte");

        // FOOLPROOF: Zera velocidade e desliga animação em loop
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.velocity = Vector2.zero;
        
        SpriteAnimator sAnim = GetComponent<SpriteAnimator>();
        if (sAnim != null) sAnim.enabled = false;

        // Destroi quase instantaneamente pois a animação manual não suporta morte ainda
        Destroy(gameObject, 0.1f);
    }

    // ─── Helpers ──────────────────────────────────────────────────────
    private void ResetarCor()
    {
        if (spriteRenderer != null)
            spriteRenderer.color = corOriginal;
    }

    public float GetPorcentagemVida() => (float)vidaAtual / vidaMaxima;
    public bool  EstaVivo()           => !estaMorto;
}
