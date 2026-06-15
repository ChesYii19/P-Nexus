using UnityEngine;

/// <summary>
/// EnemyShooter — Inimigo que atira projéteis em direção ao jogador.
/// Complementa o EnemyController (patrulha/perseguição).
/// Detecta o jogador dentro do raio de ataque e dispara com cooldown.
/// Tag obrigatória: "Inimigo".
/// </summary>
[RequireComponent(typeof(EnemyController))]
public class EnemyShooter : MonoBehaviour
{
    // ─── Configurações ─────────────────────────────────────────────────
    [Header("Ataque Ranged")]
    [Tooltip("Prefab do projétil do inimigo")]
    public GameObject prefabProjetil;

    [Tooltip("Ponto de disparo (filho do inimigo)")]
    public Transform pontoDisparo;

    [Tooltip("Distância máxima para atirar")]
    public float distanciaAtaque = 6f;

    [Tooltip("Tempo entre disparos")]
    public float cooldownTiro = 2f;

    [Tooltip("Dano do projétil do inimigo")]
    public int danoProjetil = 1;

    // ─── Estado Interno ────────────────────────────────────────────────
    private float tempoCooldown = 0f;
    private Transform jogador;
    private Animator anim;

    private static readonly int AnimAtirando = Animator.StringToHash("atirando");

    // ──────────────────────────────────────────────────────────────────
    void Start()
    {
        anim = GetComponent<Animator>();
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) jogador = p.transform;
    }

    void Update()
    {
        if (jogador == null) return;

        tempoCooldown -= Time.deltaTime;

        float distancia = Vector2.Distance(transform.position, jogador.position);

        if (distancia <= distanciaAtaque && tempoCooldown <= 0f)
        {
            Atirar();
            tempoCooldown = cooldownTiro;
        }
    }

    // ─── Disparo ───────────────────────────────────────────────────────
    private void Atirar()
    {
        if (prefabProjetil == null || pontoDisparo == null) return;

        // anim?.SetTrigger(AnimAtirando);
        AudioManager.Instance?.PlaySFX("inimigo_tiro");

        Vector2 direcao = ((Vector2)jogador.position - (Vector2)pontoDisparo.position).normalized;

        GameObject proj = Instantiate(prefabProjetil, pontoDisparo.position, Quaternion.identity);

        // Configura projétil para atingir apenas o jogador
        EnemyProjectile compProj = proj.GetComponent<EnemyProjectile>();
        if (compProj != null)
        {
            compProj.Inicializar(direcao, danoProjetil);
        }
        else
        {
            // Fallback: usa Projetil normal mas invertido
            Projetil compProjetilPadrao = proj.GetComponent<Projetil>();
            compProjetilPadrao?.Inicializar(direcao);
        }
    }

    // ─── Gizmo ────────────────────────────────────────────────────────
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.5f, 0f);
        Gizmos.DrawWireSphere(transform.position, distanciaAtaque);
    }
}

