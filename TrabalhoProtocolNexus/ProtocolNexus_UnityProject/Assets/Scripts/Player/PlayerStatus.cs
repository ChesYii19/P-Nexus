using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Gerencia o status do jogador Tatsuya Yuki:
/// - Vidas (HP), pontuação e exibição no HUD via TextMeshPro
/// - Coleta de itens (tag "Item")
/// - Recebimento de dano com invencibilidade temporária
/// - Morte e comunicação com GameManager
/// </summary>
public class PlayerStatus : MonoBehaviour
{
    // ─── Configurações de Status ──────────────────────────────────────
    [Header("Vida")]
    [Tooltip("Vidas máximas do jogador")]
    public int vidasMaximas = 5;

    [Tooltip("Duração da invencibilidade após tomar dano (segundos)")]
    public float tempoInvencibilidade = 1.5f;

    // ─── Referências de HUD ───────────────────────────────────────────
    [Header("HUD — TextMeshPro")]
    [Tooltip("Texto que exibe as vidas: ex: 'VIDA: 5'")]
    public TextMeshProUGUI textoVidas;

    [Tooltip("Texto que exibe a pontuação: ex: 'SCORE: 0'")]
    public TextMeshProUGUI textoPontuacao;

    [Header("Barra de Vida (opcional)")]
    [Tooltip("Slider de barra de vida visual")]
    public Slider barraDeVida;

    // ─── Estado Interno ───────────────────────────────────────────────
    private int vidasAtuais;

    private float temporizadorInvencibilidade;
    private bool estaInvencivel;
    private PlayerController controller;
    private SpriteRenderer spriteRenderer;

    // ─── Evento Estático ──────────────────────────────────────────────
    /// <summary>Disparado quando o jogador morre. GameManager escuta.</summary>
    public static System.Action OnJogadorMorreu;

    // ──────────────────────────────────────────────────────────────────
    void Awake()
    {
        controller    = GetComponent<PlayerController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        vidasAtuais = vidasMaximas;
        AtualizarHUD();
    }

    void Update()
    {
        // Conta regressiva da invencibilidade
        if (estaInvencivel)
        {
            temporizadorInvencibilidade -= Time.deltaTime;

            // Pisca o sprite durante invencibilidade
            spriteRenderer.enabled = Mathf.Sin(temporizadorInvencibilidade * 20f) > 0f;

            if (temporizadorInvencibilidade <= 0f)
            {
                estaInvencivel         = false;
                spriteRenderer.enabled = true;
            }
        }
    }

    // ─── API Pública: Dano ────────────────────────────────────────────
    /// <summary>Aplica dano ao jogador. Respeitando invencibilidade.</summary>
    public void ReceberDano(int quantidade = 1)
    {
        if (estaInvencivel) return;

        vidasAtuais -= quantidade;
        vidasAtuais  = Mathf.Max(vidasAtuais, 0);

        // Inicia invencibilidade temporária
        estaInvencivel              = true;
        temporizadorInvencibilidade = tempoInvencibilidade;

        AtualizarHUD();
        AudioManager.Instance?.PlaySFX("dano");

        if (vidasAtuais <= 0)
            Morrer();
    }

    // ─── API Pública: Coletar Item ────────────────────────────────────
    /// <summary>Adiciona pontos à pontuação do jogador.</summary>
    public void AdicionarPontos(int quantidade)
    {
        GameManager.Instance?.AdicionarPontuacao(quantidade);
        AtualizarHUD();
        AudioManager.Instance?.PlaySFX("coletar");
    }

    /// <summary>Cura o jogador (não ultrapassa o máximo).</summary>
    public void Curar(int quantidade)
    {
        vidasAtuais = Mathf.Min(vidasAtuais + quantidade, vidasMaximas);
        AtualizarHUD();
    }

    // ─── Getters ──────────────────────────────────────────────────────
    public int GetVidas()     => vidasAtuais;
    public int GetPontuacao() => GameManager.Instance?.GetPontuacaoTotal() ?? 0;

    // ─── Lógica Interna ───────────────────────────────────────────────
    private void Morrer()
    {
        controller?.SetMorto(true);
        AudioManager.Instance?.PlaySFX("morte");

        // Notifica GameManager após breve delay
        Invoke(nameof(NotificarMorte), 1.2f);
    }

    private void NotificarMorte()
    {
        OnJogadorMorreu?.Invoke();
    }

    private void AtualizarHUD()
    {
        if (textoVidas != null)
        {
            textoVidas.text = $"VIDA: {vidasAtuais}";
            textoVidas.gameObject.SetActive(false); // Esconde o texto antigo
        }

        if (textoPontuacao != null)
            textoPontuacao.text = $"SCORE: {GameManager.Instance?.GetPontuacaoTotal() ?? 0:D6}";

        if (barraDeVida != null)
        {
            barraDeVida.maxValue = vidasMaximas;
            barraDeVida.value    = vidasAtuais;
        }
        
        // FOOLPROOF HEARTS UI
        GameObject hudCanvas = GameObject.Find("HUD_Canvas");
        if (hudCanvas == null) hudCanvas = GameObject.FindObjectOfType<Canvas>()?.gameObject;
        if (hudCanvas != null)
        {
            Transform panel = hudCanvas.transform.Find("HeartsPanel");
            if (panel == null)
            {
                GameObject p = new GameObject("HeartsPanel");
                panel = p.transform;
                panel.SetParent(hudCanvas.transform, false);
                RectTransform rt = p.AddComponent<RectTransform>();
                rt.anchorMin = new Vector2(0, 1);
                rt.anchorMax = new Vector2(0, 1);
                rt.pivot = new Vector2(0, 1);
                rt.anchoredPosition = new Vector2(20, -20);
            }
            
            // Limpa antigos
            foreach (Transform child in panel) Destroy(child.gameObject);
            
            // Cria novos coracoes
            for (int i = 0; i < vidasAtuais; i++)
            {
                GameObject heart = new GameObject("Heart_" + i);
                heart.transform.SetParent(panel, false);
                RectTransform rt = heart.AddComponent<RectTransform>();
                rt.anchoredPosition = new Vector2(i * 60, 0);
                rt.sizeDelta = new Vector2(50, 50);
                Image img = heart.AddComponent<Image>();
#if UNITY_EDITOR
                Object[] assets = UnityEditor.AssetDatabase.LoadAllAssetsAtPath("Assets/Sprites/UI/coracao.png");
                foreach (var asset in assets)
                {
                    if (asset is Sprite s)
                    {
                        img.sprite = s;
                        break;
                    }
                }
#endif
            }
        }
    }

    // ─── Coleta Automática por Trigger ────────────────────────────────
    void OnTriggerEnter2D(Collider2D other)
    {
        // Ao inves de comparar a Tag "Item" (que requer que a tag exista no projeto),
        // basta ver se o objeto possui o script "Item". Muito mais seguro.
        Item item = other.GetComponent<Item>();
        if (item != null)
        {
            item.Coletar(this);
        }
    }
}
