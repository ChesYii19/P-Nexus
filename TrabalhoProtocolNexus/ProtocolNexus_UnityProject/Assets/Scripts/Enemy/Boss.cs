using UnityEngine;

/// <summary>
/// Boss do Nível 4: NEXUS-PRIME
/// Comportamento em 3 fases baseado na vida restante.
/// Fase 1 (100–60% HP): Patrulha e atira projéteis simples.
/// Fase 2 (60–30% HP): Velocidade aumenta, padrão de tiro duplo.
/// Fase 3 (30–0% HP): Dash, projéteis em leque, velocidade máxima.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class Boss : MonoBehaviour
{
    // ─── Configurações Gerais ─────────────────────────────────────────
    [Header("Boss — NEXUS-PRIME")]
    public int vidaMaxima = 20;

    [Header("Fase 1 (100-60%)")]
    public float velocidadeFase1 = 2f;
    public float cooldownTiroFase1 = 2f;

    [Header("Fase 2 (60-30%)")]
    public float velocidadeFase2 = 4f;
    public float cooldownTiroFase2 = 1f;

    [Header("Fase 3 (30-0%)")]
    public float velocidadeFase3 = 6f;
    public float cooldownTiroFase3 = 0.5f;
    public float cooldownDash = 3f;
    public float forcaDash = 15f;

    [Header("Projétil")]
    public GameObject prefabProjetil;
    public Transform[] pontosDisparo;

    [Header("UI — Barra de Vida do Boss")]
    public UnityEngine.UI.Slider barraVidaBoss;
    public TMPro.TextMeshProUGUI textoNomeBoss;

    // ─── Estado Interno ────────────────────────────────────────────────
    private int vidaAtual;
    private int faseAtual = 1;
    private float tempoCooldownTiro;
    private float tempoCooldownDash;
    private bool estaMorto;
    private bool fezTransicaoFase2;
    private bool fezTransicaoFase3;

    private Rigidbody2D rb;
    private Animator anim;
    private Transform jogador;

    private static readonly int AnimFase    = Animator.StringToHash("fase");
    private static readonly int AnimAtaque  = Animator.StringToHash("ataque");
    private static readonly int AnimDash    = Animator.StringToHash("dash");
    private static readonly int AnimMorto   = Animator.StringToHash("morto");
    private static readonly int AnimDano    = Animator.StringToHash("dano");

    // ──────────────────────────────────────────────────────────────────
    private SpriteAnimator spriteAnim;

    void Awake()
    {
        rb   = GetComponent<Rigidbody2D>();
        
        // FOOLPROOF ANIMATOR
        spriteAnim = GetComponent<SpriteAnimator>();
        if (spriteAnim == null)
        {
            spriteAnim = gameObject.AddComponent<SpriteAnimator>();
            spriteAnim.pathIdle = "Assets/Sprites/Enemies/Golem/Golem_1_idle.png";
            spriteAnim.pathRun = "Assets/Sprites/Enemies/Golem/Golem_1_walk.png";
            spriteAnim.pathJump = "Assets/Sprites/Enemies/Golem/Golem_1_attack.png"; // Usando Jump pro Attack por simplificacao de variaveis
            spriteAnim.pathFall = "Assets/Sprites/Enemies/Golem/Golem_1_die.png";
        }
    }

    void Start()
    {
        vidaAtual = vidaMaxima;

        GameObject pObj = GameObject.FindGameObjectWithTag("Player");
        if (pObj != null) jogador = pObj.transform;

        ConfigurarBarraVida();
        AudioManager.Instance?.PlayMusica("boss");
    }

    void Update()
    {
        if (estaMorto || jogador == null) return;

        tempoCooldownTiro -= Time.deltaTime;
        tempoCooldownDash -= Time.deltaTime;

        VerificarTransicaoFase();

        switch (faseAtual)
        {
            case 1: ComportamentoFase1(); break;
            case 2: ComportamentoFase2(); break;
            case 3: ComportamentoFase3(); break;
        }

        AtualizarBarraVida();
    }

    // ─── Fases ────────────────────────────────────────────────────────
    private void VerificarTransicaoFase()
    {
        float porcentagem = (float)vidaAtual / vidaMaxima;

        if (porcentagem <= 0.6f && !fezTransicaoFase2)
        {
            fezTransicaoFase2 = true;
            faseAtual = 2;
            // // anim.SetInteger...
            TremerTela();
        }

        if (porcentagem <= 0.3f && !fezTransicaoFase3)
        {
            fezTransicaoFase3 = true;
            faseAtual = 3;
            // // anim.SetInteger...
            TremerTela();
        }
    }

    private void ComportamentoFase1()
    {
        MoverEmDirecaoAoJogador(velocidadeFase1);

        if (tempoCooldownTiro <= 0f)
        {
            AtirarSimples();
            tempoCooldownTiro = cooldownTiroFase1;
        }
    }

    private void ComportamentoFase2()
    {
        MoverEmDirecaoAoJogador(velocidadeFase2);

        if (tempoCooldownTiro <= 0f)
        {
            AtirarDuplo();
            tempoCooldownTiro = cooldownTiroFase2;
        }
    }

    private void ComportamentoFase3()
    {
        MoverEmDirecaoAoJogador(velocidadeFase3);

        if (tempoCooldownTiro <= 0f)
        {
            AtirarLeque();
            tempoCooldownTiro = cooldownTiroFase3;
        }

        if (tempoCooldownDash <= 0f)
        {
            ExecutarDash();
            tempoCooldownDash = cooldownDash;
        }
    }

    // ─── Movimento ────────────────────────────────────────────────────
    private void MoverEmDirecaoAoJogador(float velocidade)
    {
        Vector2 direcao = ((Vector2)jogador.position - rb.position).normalized;
        rb.velocity = new Vector2(direcao.x * velocidade, rb.velocity.y);

        if (direcao.x != 0)
            transform.localScale = new Vector3(Mathf.Sign(direcao.x), 1f, 1f);
            
        if (spriteAnim != null && !estaMorto) spriteAnim.Play("Run");
    }

    // ─── Padrões de Ataque ────────────────────────────────────────────
    private void AtirarSimples()
    {
        if (pontosDisparo.Length == 0 || prefabProjetil == null) return;
        if (spriteAnim != null) spriteAnim.Play("Jump"); // Attack
        Vector2 dir = ((Vector2)jogador.position - (Vector2)pontosDisparo[0].position).normalized;
        CriarProjetil(pontosDisparo[0].position, dir);
    }

    private void AtirarDuplo()
    {
        if (prefabProjetil == null) return;
        if (spriteAnim != null) spriteAnim.Play("Jump"); // Attack
        foreach (Transform ponto in pontosDisparo)
        {
            if (ponto == null) continue;
            Vector2 dir = ((Vector2)jogador.position - (Vector2)ponto.position).normalized;
            CriarProjetil(ponto.position, dir);
        }
    }

    private void AtirarLeque()
    {
        if (prefabProjetil == null || pontosDisparo.Length == 0) return;
        if (spriteAnim != null) spriteAnim.Play("Jump"); // Attack
        
        if (pontosDisparo[0] == null) return;
        Vector2 baseDir = ((Vector2)jogador.position - (Vector2)pontosDisparo[0].position).normalized;
        float[] angulos = { -30f, -15f, 0f, 15f, 30f };

        foreach (float angulo in angulos)
        {
            float rad = angulo * Mathf.Deg2Rad;
            Vector2 dir = new Vector2(
                baseDir.x * Mathf.Cos(rad) - baseDir.y * Mathf.Sin(rad),
                baseDir.x * Mathf.Sin(rad) + baseDir.y * Mathf.Cos(rad)
            );
            CriarProjetil(pontosDisparo[0].position, dir);
        }
    }

    private void CriarProjetil(Vector3 posicao, Vector2 direcao)
    {
        GameObject proj = Instantiate(prefabProjetil, posicao, Quaternion.identity);
        Projetil compProj = proj.GetComponent<Projetil>();
        if (compProj != null)
            compProj.Inicializar(direcao);
    }

    private void ExecutarDash()
    {
        if (jogador == null) return;
        // // anim.SetTrigger...
        Vector2 dir = ((Vector2)jogador.position - rb.position).normalized;
        rb.AddForce(dir * forcaDash, ForceMode2D.Impulse);
        AudioManager.Instance?.PlaySFX("boss_dash");
    }

    // ─── Receber Dano ─────────────────────────────────────────────────
    public void ReceberDano(int quantidade)
    {
        if (estaMorto) return;

        vidaAtual -= quantidade;
        vidaAtual  = Mathf.Max(vidaAtual, 0);

        // // anim.SetTrigger...
        AudioManager.Instance?.PlaySFX("boss_dano");

        if (vidaAtual <= 0)
            Morrer();
    }

    // ─── Morte do Boss ────────────────────────────────────────────────
    private void Morrer()
    {
        estaMorto = true;
        if (spriteAnim != null) spriteAnim.Play("Fall"); // Die
        rb.velocity = Vector2.zero;
        GetComponent<Collider2D>().enabled = false;

        AudioManager.Instance?.PlaySFX("boss_morte");
        AudioManager.Instance?.PlayMusica("vitoria");

        // Notifica GameManager após animação de morte
        Invoke(nameof(FimDeJogo), 2f);
    }

    private void FimDeJogo()
    {
        GameManager.Instance?.VitoriaFinal();
    }

    // ─── HUD ──────────────────────────────────────────────────────────
    private void ConfigurarBarraVida()
    {
        if (barraVidaBoss != null)
        {
            barraVidaBoss.maxValue = vidaMaxima;
            barraVidaBoss.value    = vidaAtual;
            barraVidaBoss.gameObject.SetActive(true);
        }

        if (textoNomeBoss != null)
            textoNomeBoss.text = "NEXUS-PRIME";
    }

    private void AtualizarBarraVida()
    {
        if (barraVidaBoss != null)
            barraVidaBoss.value = vidaAtual;
    }

    private void TremerTela()
    {
        CameraFollow cam = Camera.main?.GetComponent<CameraFollow>();
        if (cam != null)
            cam.IniciarShake(0.4f, 0.3f);
    }

    // ─── Contato com Jogador ──────────────────────────────────────────
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerStatus status = collision.gameObject.GetComponent<PlayerStatus>();
            status?.ReceberDano(2); // Boss causa 2 de dano por contato
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, 5f);
    }
}

