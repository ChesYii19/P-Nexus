using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// GameManager — Singleton que gerencia o estado global do Protocol Nexus.
/// Controla: cenas, game over, vitória, pausar jogo e persistência de dados.
/// Única instância em toda a execução (DontDestroyOnLoad).
/// </summary>
public class GameManager : MonoBehaviour
{
    // ─── Singleton ────────────────────────────────────────────────────
    public static GameManager Instance { get; private set; }

    // ─── Nomes das Cenas (configure no Inspector) ─────────────────────
    [Header("Cenas do Jogo")]
    [Tooltip("Nome da cena do Menu Principal")]
    public string cenaMenuPrincipal = "MenuPrincipal";

    [Tooltip("Nomes das cenas dos níveis em ordem")]
    public string[] cenasNiveis = { "Nivel1", "Nivel2", "Nivel3", "Nivel4" };

    [Tooltip("Nome da cena de Game Over")]
    public string cenaGameOver = "GameOver";

    [Tooltip("Nome da cena de Vitória Final")]
    public string cenaVitoria = "Vitoria";

    // ─── Estado do Jogo ───────────────────────────────────────────────
    [Header("Estado")]
    [Tooltip("Número de continues restantes")]
    public int continuesRestantes = 3;

    private int nivelAtualIndex = 0;
    private bool jogoEmPausa   = false;
    private int pontuacaoTotal = 0;

    // ─── Evento ───────────────────────────────────────────────────────
    public static System.Action<bool> OnPausaAlterada;

    // ──────────────────────────────────────────────────────────────────
    void Awake()
    {
        // Garante apenas uma instância do GameManager
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        // Escuta morte do jogador
        PlayerStatus.OnJogadorMorreu += TratarMorteJogador;
        SceneManager.sceneLoaded     += OnCenaCarregada;
    }

    void OnDisable()
    {
        PlayerStatus.OnJogadorMorreu -= TratarMorteJogador;
        SceneManager.sceneLoaded     -= OnCenaCarregada;
    }

    void Update()
    {
        // Pausa/Resume com ESC
        if (Input.GetKeyDown(KeyCode.Escape))
            AlternarPausa();
    }

    // ─── Progressão de Nível ──────────────────────────────────────────
    /// <summary>Carrega o próximo nível da lista. Chamado pelo LevelManager.</summary>
    public void ProximoNivel()
    {
        nivelAtualIndex++;

        if (nivelAtualIndex >= cenasNiveis.Length)
        {
            VitoriaFinal();
            return;
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene(cenasNiveis[nivelAtualIndex]);
    }

    /// <summary>Reinicia o nível atual.</summary>
    public void ReiniciarNivel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(cenasNiveis[nivelAtualIndex]);
        // Reposiciona o jogador no último checkpoint, se houver
        if (Checkpoint.UltimaPosicaoCheckpoint != Vector3.zero)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                player.transform.position = Checkpoint.UltimaPosicaoCheckpoint;
            }
        }
    }

    /// <summary>Vai para o menu principal.</summary>
    public void IrParaMenu()
    {
        Time.timeScale = 1f;
        nivelAtualIndex = 0;
        SceneManager.LoadScene(cenaMenuPrincipal);
    }

    /// <summary>Inicia o jogo do nível 1.</summary>
    public void IniciarJogo()
    {
        nivelAtualIndex = 0;
        pontuacaoTotal  = 0;
        Time.timeScale  = 1f;
        SceneManager.LoadScene(cenasNiveis[0]);
    }

    // ─── Game Over ────────────────────────────────────────────────────
    private void TratarMorteJogador()
    {
        if (continuesRestantes > 0)
        {
            continuesRestantes--;
            Invoke(nameof(ReiniciarNivel), 2f);
        }
        else
        {
            Invoke(nameof(CarregarGameOver), 2f);
        }
    }

    private void CarregarGameOver()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(cenaGameOver);
    }

    // ─── Vitória ──────────────────────────────────────────────────────
    /// <summary>Chamado pelo Boss ao ser derrotado.</summary>
    public void VitoriaFinal()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(cenaVitoria);
    }

    // ─── Pausa ────────────────────────────────────────────────────────
    public void AlternarPausa()
    {
        jogoEmPausa    = !jogoEmPausa;
        Time.timeScale = jogoEmPausa ? 0f : 1f;
        OnPausaAlterada?.Invoke(jogoEmPausa);
    }

    public bool JogoEmPausa() => jogoEmPausa;

    // ─── Pontuação ────────────────────────────────────────────────────
    public void AdicionarPontuacao(int pontos) => pontuacaoTotal += pontos;
    public int  GetPontuacaoTotal()            => pontuacaoTotal;

    // ─── Callback de Cena ─────────────────────────────────────────────
    private void OnCenaCarregada(Scene cena, LoadSceneMode modo)
    {
        // Reinicia Time.timeScale caso tenha ficado pausado
        Time.timeScale = 1f;
        jogoEmPausa = false;
    }
}
