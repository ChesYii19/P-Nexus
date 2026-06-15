using UnityEngine;

/// <summary>
/// Checkpoint — Ponto de retorno no nível do Protocol Nexus.
/// Ao tocar o checkpoint, salva a posição do jogador.
/// Em caso de morte (com continues), o jogador retorna ao último checkpoint ativo.
/// Visual: sprite inativo → ativo ao ser tocado.
/// Tag obrigatória: "Checkpoint".
/// </summary>
public class Checkpoint : MonoBehaviour
{
    // ─── Configurações ─────────────────────────────────────────────────
    [Header("Visual")]
    [Tooltip("Sprite quando o checkpoint está inativo")]
    public Sprite spriteInativo;

    [Tooltip("Sprite quando o checkpoint está ativo (tocado pelo jogador)")]
    public Sprite spriteAtivo;

    [Header("Efeitos")]
    [Tooltip("Prefab de partícula ao ativar o checkpoint (opcional)")]
    public GameObject efeitoAtivacao;

    // ─── Estado ────────────────────────────────────────────────────────
    private bool estaAtivo   = false;
    private SpriteRenderer spriteRenderer;

    // ─── Posição Estática (Salva para respawn) ─────────────────────────
    /// <summary>Posição do último checkpoint ativado. Usada pelo GameManager ao reiniciar.</summary>
    public static Vector3 UltimaPosicaoCheckpoint;

    // ─── Referência a todos os checkpoints da cena ─────────────────────
    private static Checkpoint checkpointAtivo;

    // ──────────────────────────────────────────────────────────────────
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Inicia com sprite inativo
        if (spriteInativo != null)
            spriteRenderer.sprite = spriteInativo;
    }

    // ─── Trigger de Ativação ──────────────────────────────────────────
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !estaAtivo)
            Ativar();
    }

    // ─── Ativação do Checkpoint ───────────────────────────────────────
    private void Ativar()
    {
        // Desativa o checkpoint anterior
        if (checkpointAtivo != null && checkpointAtivo != this)
            checkpointAtivo.Desativar();

        estaAtivo      = true;
        checkpointAtivo = this;

        // Salva posição de respawn
        UltimaPosicaoCheckpoint = transform.position;

        // Troca sprite
        if (spriteAtivo != null)
            spriteRenderer.sprite = spriteAtivo;

        // Partícula de ativação
        if (efeitoAtivacao != null)
        {
            GameObject efeito = Instantiate(efeitoAtivacao, transform.position, Quaternion.identity);
            Destroy(efeito, 2f);
        }

        // SFX
        AudioManager.Instance?.PlaySFX("checkpoint");
    }

    private void Desativar()
    {
        estaAtivo = false;
        if (spriteInativo != null)
            spriteRenderer.sprite = spriteInativo;
    }

    public bool EstaAtivo() => estaAtivo;
}
