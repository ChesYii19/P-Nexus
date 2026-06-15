using UnityEngine;

/// <summary>
/// EnemyProjectile — Projétil disparado pelos inimigos do Protocol Nexus.
/// Diferente do Projetil.cs (do jogador), este causa dano APENAS ao Player.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class EnemyProjectile : MonoBehaviour
{
    [Header("Projétil Inimigo")]
    public float velocidade   = 8f;
    public float tempoDeVida  = 3f;
    public GameObject efeitoImpacto;

    private Rigidbody2D rb;
    private Vector2 direcao;
    private int dano = 1;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        Destroy(gameObject, tempoDeVida);
    }

    void FixedUpdate()
    {
        rb.velocity = direcao * velocidade;
    }

    /// <summary>Inicializa direção e dano do projétil.</summary>
    public void Inicializar(Vector2 dir, int danoValor)
    {
        direcao = dir.normalized;
        dano    = danoValor;

        // Rotaciona o sprite de acordo com a direção
        float angulo = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angulo, Vector3.forward);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Colide com o jogador
        if (other.CompareTag("Player"))
        {
            PlayerStatus status = other.GetComponent<PlayerStatus>();
            status?.ReceberDano(dano);
            CriarEfeito();
            Destroy(gameObject);
            return;
        }

        // Colide com parede/chão
        if (!other.isTrigger && !other.CompareTag("Inimigo"))
        {
            CriarEfeito();
            Destroy(gameObject);
        }
    }

    private void CriarEfeito()
    {
        if (efeitoImpacto != null)
        {
            GameObject e = Instantiate(efeitoImpacto, transform.position, Quaternion.identity);
            Destroy(e, 0.5f);
        }
    }
}

