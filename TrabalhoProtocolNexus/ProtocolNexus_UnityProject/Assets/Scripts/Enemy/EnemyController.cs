using UnityEngine;

/// <summary>
/// IA de inimigo básico do Protocol Nexus.
/// Comportamentos: Patrulha por pontos, detecta o jogador e persegue.
/// Causa dano ao contato. Tag obrigatória: "Inimigo".
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class EnemyController : MonoBehaviour
{
    // ─── Tipo de Inimigo ──────────────────────────────────────────────
    public enum TipoInimigo { Terrestre, Voador }

    [Header("Tipo")]
    public TipoInimigo tipo = TipoInimigo.Terrestre;

    // ─── Parâmetros de Patrulha ───────────────────────────────────────
    [Header("Patrulha")]
    [Tooltip("Pontos de patrulha no mundo (mínimo 2)")]
    public Transform[] pontosPatrulha;

    [Tooltip("Velocidade de patrulha")]
    public float velocidadePatrulha = 2f;

    [Tooltip("Distância mínima para trocar de ponto")]
    public float distanciaTrocaPonto = 0.2f;

    // ─── Parâmetros de Perseguição ────────────────────────────────────
    [Header("Perseguição")]
    [Tooltip("Distância de visão para detectar o jogador")]
    public float raioDeteccao = 5f;

    [Tooltip("Velocidade de perseguição")]
    public float velocidadePerseguicao = 4f;

    // ─── Parâmetros de Ataque ─────────────────────────────────────────
    [Header("Ataque por Contato")]
    [Tooltip("Dano causado ao colidir com o jogador")]
    public int danoContato = 1;

    // ─── Referências ──────────────────────────────────────────────────
    private Rigidbody2D rb;
    private Animator anim;
    private Transform jogador;

    private int indicePatrulha = 0;
    private bool estaPerceguindo = false;

    // Hash Animator
    private static readonly int AnimVelocidade    = Animator.StringToHash("velocidade");
    private static readonly int AnimPerseguindo   = Animator.StringToHash("perseguindo");
    private static readonly int AnimAtacando      = Animator.StringToHash("atacando");

    // ──────────────────────────────────────────────────────────────────
    void Awake()
    {
        rb   = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        if (GetComponent<EnemyHealth>() == null)
        {
            EnemyHealth hp = gameObject.AddComponent<EnemyHealth>();
            hp.vidaMaxima = 3; // Forçando a vida do inimigo
        }

        if (tipo == TipoInimigo.Terrestre)
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        else
            rb.gravityScale = 0f; // Voador sem gravidade

        // FOOLPROOF SCRIPT ANIMATION
        if (GetComponent<SpriteAnimator>() == null)
        {
            SpriteAnimator sAnim = gameObject.AddComponent<SpriteAnimator>();
            if (tipo == TipoInimigo.Terrestre)
                sAnim.pathIdle = "Assets/Sprites/Enemies/Dog/Dog/Dog.png";
            else
                sAnim.pathIdle = "Assets/Sprites/Enemies/Spider/Spider/Spider.png";
            sAnim.fps = 8f;
        }
    }

    void Start()
    {
        // Busca o jogador pela tag
        GameObject obj = GameObject.FindGameObjectWithTag("Player");
        if (obj != null)
            jogador = obj.transform;
    }

    void Update()
    {
        if (jogador == null) return;

        float distancia = Vector2.Distance(transform.position, jogador.position);
        estaPerceguindo = distancia <= raioDeteccao;

        // // if (anim != null && anim.runtimeAnimatorController != null) { try { anim.SetBool...
    }

    void FixedUpdate()
    {
        if (jogador == null) return;

        if (estaPerceguindo)
            Perseguir();
        else
            Patrulhar();
    }

    // ─── Patrulha entre pontos ────────────────────────────────────────
    private void Patrulhar()
    {
        if (pontosPatrulha == null || pontosPatrulha.Length == 0) return;

        Transform alvo = pontosPatrulha[indicePatrulha];
        if (alvo == null) return;
        Vector2 direcao = ((Vector2)alvo.position - rb.position).normalized;

        rb.velocity = new Vector2(direcao.x * velocidadePatrulha, tipo == TipoInimigo.Voador ? direcao.y * velocidadePatrulha : rb.velocity.y);

        // Vira o sprite
        if (direcao.x != 0)
            transform.localScale = new Vector3(Mathf.Sign(direcao.x), 1f, 1f);

        // if (anim != null && anim.runtimeAnimatorController != null) { try { anim.SetFloat...

        // Checa se chegou no ponto
        if (Vector2.Distance(transform.position, alvo.position) <= distanciaTrocaPonto)
            indicePatrulha = (indicePatrulha + 1) % pontosPatrulha.Length;
    }

    // ─── Perseguição do Jogador ───────────────────────────────────────
    private void Perseguir()
    {
        Vector2 direcao = ((Vector2)jogador.position - rb.position).normalized;

        if (tipo == TipoInimigo.Voador)
            rb.velocity = direcao * velocidadePerseguicao;
        else
            rb.velocity = new Vector2(direcao.x * velocidadePerseguicao, rb.velocity.y);

        // Vira o sprite
        if (direcao.x != 0)
            transform.localScale = new Vector3(Mathf.Sign(direcao.x), 1f, 1f);

        // if (anim != null && anim.runtimeAnimatorController != null) { try { anim.SetFloat...
    }

    // ─── Dano ao Jogador por Contato ──────────────────────────────────
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // // if (anim != null && anim.runtimeAnimatorController != null) { try { anim.SetTrigger...
            PlayerStatus status = collision.gameObject.GetComponent<PlayerStatus>();
            status?.ReceberDano(danoContato);
        }
    }

    // ─── Gizmos de Debug ──────────────────────────────────────────────
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, raioDeteccao);
    }
}

