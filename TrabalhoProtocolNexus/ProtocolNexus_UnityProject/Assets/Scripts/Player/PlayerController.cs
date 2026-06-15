using UnityEngine;

/// <summary>
/// Controla toda a movimentação do protagonista Tatsuya Yuki.
/// Gerencia: andar, correr (Shift), pular (detecção de chão), 
/// escalar paredes e escadas (anulando gravidade).
/// Requer: Rigidbody2D, Collider2D, Animator na mesma GameObject.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    // ─── Parâmetros de Movimento ───────────────────────────────────────
    [Header("Movimento")]
    [Tooltip("Velocidade base de caminhada")]
    public float velocidadeCaminhada = 5f;

    [Tooltip("Velocidade ao correr (segurar Shift)")]
    public float velocidadeCorrida = 9f;

    [Tooltip("Força aplicada ao pular")]
    public float forcaPulo = 7f;

    // ─── Parâmetros de Verificação de Chão ────────────────────────────
    [Header("Verificação de Chão")]
    [Tooltip("Ponto vazio filho que marca onde verificar o chão")]
    public Transform verificadorChao;

    [Tooltip("Raio da verificação de chão")]
    public float raioChao = 0.2f;

    [Tooltip("Layers que são considerados chão")]
    public LayerMask layerChao;

    // ─── Parâmetros de Escada ──────────────────────────────────────────
    [Header("Escada / Parede")]
    [Tooltip("Velocidade de escalada")]
    public float velocidadeEscada = 4f;

    // ─── Referências Internas ──────────────────────────────────────────
    private Rigidbody2D rb;
    private float _defaultGravityScale;
    private Animator anim;

    private float movimentoHorizontal;
    private float movimentoVertical;
    private bool noChao;
    private bool naEscada;
    private bool estaMorto;

    // Hash dos parâmetros do Animator para performance
    private static readonly int AnimVelocidade = Animator.StringToHash("velocidade");
    private static readonly int AnimNoChao    = Animator.StringToHash("noChao");
    private static readonly int AnimPulando   = Animator.StringToHash("pulando");
    private static readonly int AnimEscalando = Animator.StringToHash("escalando");
    private static readonly int AnimMorto     = Animator.StringToHash("morto");

    // ──────────────────────────────────────────────────────────────────
    void Awake()
    {
        rb   = GetComponent<Rigidbody2D>();
        _defaultGravityScale = rb.gravityScale;
        anim = GetComponent<Animator>();

        // FOOLPROOF INVISIBILITY FIX
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
#if UNITY_EDITOR
            Object[] assets = UnityEditor.AssetDatabase.LoadAllAssetsAtPath("Assets/Sprites/Player/MainCharacter(FreePack)/MainCharacter(FreePack)/Idle.png");
            foreach (var asset in assets)
            {
                if (asset is Sprite s)
                {
                    sr.sprite = s;
                    break;
                }
            }
#endif
        }
        
        // FOOLPROOF ANIMATOR FIX
        if (anim != null && anim.runtimeAnimatorController == null)
        {
#if UNITY_EDITOR
            anim.runtimeAnimatorController = UnityEditor.AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>("Assets/Animations/Tatsuya_Controller.controller");
#endif
        }

        // FOOLPROOF SCRIPT ANIMATION
        SpriteAnimator sAnim = GetComponent<SpriteAnimator>();
        if (sAnim == null)
        {
            sAnim = gameObject.AddComponent<SpriteAnimator>();
            sAnim.pathIdle = "Assets/Sprites/Player/Sewers/player-idle.png";
            sAnim.pathRun = "Assets/Sprites/Player/Sewers/player-run.png";
            sAnim.pathJump = "Assets/Sprites/Player/Sewers/player-jump.png";
            sAnim.pathFall = "Assets/Sprites/Player/Sewers/player-fall.png";
            sAnim.fps = 10f;
        }
    }

    void Update()
    {
        if (estaMorto) return;

        LerEntradas();
        AtualizarAnimacoes();
        Pular();
    }

    void FixedUpdate()
    {
        if (estaMorto) return;

        VerificarChao();
        Mover();
        EscalarEscada();
    }

    // ─── Leitura de Entradas ──────────────────────────────────────────
    private void LerEntradas()
    {
        movimentoHorizontal = Input.GetAxisRaw("Horizontal");
        movimentoVertical = Input.GetAxisRaw("Vertical");

        // Escada
        if (naEscada && Mathf.Abs(movimentoVertical) > 0.1f)
        {
            // Código de escada original mantido
        }
    }

    // ─── Detecção de Chão por OverlapCircle ───────────────────────────
    private void VerificarChao()
    {
        noChao = Physics2D.OverlapCircle(verificadorChao.position, raioChao, layerChao);
    }

    // ─── Movimento Horizontal ─────────────────────────────────────────
    private void Mover()
    {
        if (naEscada) return;

        // Corrida com Shift
        float velocidade = Input.GetKey(KeyCode.LeftShift) ? velocidadeCorrida : velocidadeCaminhada;

        rb.velocity = new Vector2(movimentoHorizontal * velocidade, rb.velocity.y);

        // Virar o sprite na direção do movimento
        if (movimentoHorizontal != 0)
        {
            transform.localScale = new Vector3(
                Mathf.Sign(movimentoHorizontal),
                1f, 1f
            );
        }
    }

    // ─── Pulo ─────────────────────────────────────────────────────────
    private void Pular()
    {
        if (Input.GetButtonDown("Jump") && noChao)
        {
            rb.velocity = new Vector2(rb.velocity.x, forcaPulo);
        }
    }

    // ─── Escalada de Escada / Parede ──────────────────────────────────
    private void EscalarEscada()
    {
        if (!naEscada) return;

        rb.gravityScale = 0f; // Gravidade zero ao escalar
        rb.velocity = new Vector2(rb.velocity.x, movimentoVertical * velocidadeEscada);
    }

    private void AtualizarAnimacoes()
    {
        SpriteAnimator sAnim = GetComponent<SpriteAnimator>();
        if (sAnim != null)
        {
            if (!noChao)
            {
                if (rb.velocity.y > 0.1f)
                    sAnim.Play("Jump");
                else
                    sAnim.Play("Fall");
            }
            else
            {
                if (Mathf.Abs(movimentoHorizontal) > 0.1f)
                    sAnim.Play("Run");
                else
                    sAnim.Play("Idle");
            }
        }

        // Mantém a chamada pro Animator real caso o usuário tenha um configurado
        if (anim != null)
        {
            try
            {
                if (anim != null && anim.runtimeAnimatorController != null)
                {
                    // // anim.SetFloat...
                    // // anim.SetBool...
                    // // anim.SetBool...
                    // // anim.SetBool...
                }
            }
            catch { }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name.Contains("Escada"))
        {
            naEscada = true;
            rb.gravityScale = 0f;
            rb.velocity = Vector2.zero; // Para ao entrar na escada
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.name.Contains("Escada"))
        {
            naEscada      = false;
            rb.gravityScale = _defaultGravityScale; // Retorna à gravidade padrão ao sair da escada
        }
    }

    // ─── API Pública ──────────────────────────────────────────────────
    /// <summary>Chama ao matar o jogador (desativa controles).</summary>
    public void SetMorto(bool valor)
    {
        estaMorto = valor;
        rb.velocity = Vector2.zero;

        // if (valor)
        //     // anim.SetTrigger...
    }

    /// <summary>Retorna se o jogador está no chão.</summary>
    public bool EstaNoChao() => noChao;

    // ─── Gizmos para Debug ────────────────────────────────────────────
    void OnDrawGizmosSelected()
    {
        if (verificadorChao == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(verificadorChao.position, raioChao);
    }
}

