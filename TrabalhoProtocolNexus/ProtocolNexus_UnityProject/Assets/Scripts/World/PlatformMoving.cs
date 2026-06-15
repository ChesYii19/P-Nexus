using UnityEngine;

/// <summary>
/// PlatformMoving — Plataforma móvel para os níveis do Protocol Nexus.
/// Move-se entre dois pontos (A e B) com velocidade configurável.
/// Transporta o jogador junto ao se mover (parent temporário).
/// </summary>
public class PlatformMoving : MonoBehaviour
{
    // ─── Configurações de Movimento ────────────────────────────────────
    [Header("Pontos de Movimento")]
    [Tooltip("Ponto A — posição inicial da plataforma")]
    public Transform pontoA;

    [Tooltip("Ponto B — destino da plataforma")]
    public Transform pontoB;

    [Header("Velocidade")]
    [Tooltip("Velocidade de deslocamento entre os pontos")]
    public float velocidade = 2f;

    [Header("Pausa nos Extremos")]
    [Tooltip("Tempo de pausa ao chegar em cada extremo (segundos)")]
    public float tempoPausa = 0.5f;

    // ─── Estado Interno ────────────────────────────────────────────────
    private Vector3 destino;
    private bool pausada   = false;
    private float temporizadorPausa = 0f;

    // ──────────────────────────────────────────────────────────────────
    void Start()
    {
        if (pontoA == null || pontoB == null)
        {
            Debug.LogError("[PlatformMoving] Configure pontoA e pontoB no Inspector!");
            enabled = false;
            return;
        }

        // Inicia indo para o ponto B
        destino = pontoB.position;
    }

    void Update()
    {
        if (pausada)
        {
            temporizadorPausa -= Time.deltaTime;
            if (temporizadorPausa <= 0f)
                pausada = false;
            return;
        }

        Mover();
    }

    // ─── Movimento ────────────────────────────────────────────────────
    private void Mover()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            destino,
            velocidade * Time.deltaTime
        );

        // Chegou ao destino → inverte direção e pausa
        if (Vector3.Distance(transform.position, destino) < 0.05f)
        {
            destino             = (destino == pontoB.position) ? pontoA.position : pontoB.position;
            pausada             = true;
            temporizadorPausa   = tempoPausa;
        }
    }

    // ─── Transportar o Jogador ────────────────────────────────────────
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            collision.transform.SetParent(transform);
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            collision.transform.SetParent(null);
    }

    // ─── Gizmos ───────────────────────────────────────────────────────
    void OnDrawGizmos()
    {
        if (pontoA == null || pontoB == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(pontoA.position, pontoB.position);
        Gizmos.DrawWireSphere(pontoA.position, 0.2f);
        Gizmos.DrawWireSphere(pontoB.position, 0.2f);
    }
}
