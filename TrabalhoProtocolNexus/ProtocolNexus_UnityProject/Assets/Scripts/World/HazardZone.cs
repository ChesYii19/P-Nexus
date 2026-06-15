using UnityEngine;

/// <summary>
/// HazardZone — Zona de dano do Protocol Nexus.
/// Causa dano contínuo ou instantâneo ao jogador.
/// Usos: espinhos, lasers, ácido, chão elétrico.
/// </summary>
public class HazardZone : MonoBehaviour
{
    // ─── Configurações ─────────────────────────────────────────────────
    [Header("Dano")]
    [Tooltip("Dano causado ao jogador")]
    public int dano = 1;

    [Tooltip("Se true, causa dano contínuo a cada intervalo")]
    public bool danoContinuo = false;

    [Tooltip("Intervalo entre danos contínuos (segundos)")]
    public float intervaloDano = 0.5f;

    [Header("Morte Instantânea")]
    [Tooltip("Se true, mata o jogador instantaneamente (ex: buracos)")]
    public bool morteInstantanea = false;

    // ─── Estado ────────────────────────────────────────────────────────
    private float temporizadorDano = 0f;
    private bool jogadorDentro     = false;
    private PlayerStatus statusJogador;

    // ──────────────────────────────────────────────────────────────────
    void Update()
    {
        if (!danoContinuo || !jogadorDentro || statusJogador == null) return;

        temporizadorDano -= Time.deltaTime;
        if (temporizadorDano <= 0f)
        {
            statusJogador.ReceberDano(dano);
            temporizadorDano = intervaloDano;
        }
    }

    // ─── Triggers ─────────────────────────────────────────────────────
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        statusJogador = other.GetComponent<PlayerStatus>();

        if (morteInstantanea)
        {
            // Causa dano máximo para garantir morte
            statusJogador?.ReceberDano(999);
            return;
        }

        // Dano imediato ao entrar
        statusJogador?.ReceberDano(dano);
        temporizadorDano = intervaloDano;
        jogadorDentro    = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jogadorDentro  = false;
            statusJogador  = null;
        }
    }
}
