using UnityEngine;

/// <summary>
/// LevelManager — Gerencia a progressão dentro de um nível do Protocol Nexus.
/// Detecta quando o jogador chega à porta/trigger de saída e carrega o próximo nível.
/// Também inicia a música correta ao carregar a cena.
/// Coloque um único LevelManager por cena.
/// </summary>
public class LevelManager : MonoBehaviour
{
    // ─── Configurações ─────────────────────────────────────────────────
    [Header("Nível")]
    [Tooltip("Nome do nível (aparece na HUD)")]
    public string nomeNivel = "Nível 1 — Distrito Neon";

    [Tooltip("Música deste nível (nome no AudioManager)")]
    public string nomeMusica = "nivel1";

    [Tooltip("Segundos de delay antes de carregar o próximo nível")]
    public float delayProximoNivel = 1.5f;

    [Header("Efeito de Entrada")]
    [Tooltip("Animator do painel de fade (opcional)")]
    public Animator animadorFade;

    [Tooltip("Nome do trigger de fade-in no Animator")]
    public string triggerFadeIn = "fadeIn";

    [Tooltip("Nome do trigger de fade-out no Animator")]
    public string triggerFadeOut = "fadeOut";

    // ─── Estado ────────────────────────────────────────────────────────
    private bool nivelConcluido = false;

    // ──────────────────────────────────────────────────────────────────
    void Start()
    {
        // Toca a música do nível
        AudioManager.Instance?.PlayMusica(nomeMusica);

        // Fade-in de entrada
        animadorFade?.SetTrigger(triggerFadeIn);

        // Exibe nome do nível na HUD (se houver)
        HUDController hud = FindFirstObjectByType<HUDController>();
        hud?.ExibirNomeNivel(nomeNivel);
    }

    // ─── Trigger de Saída do Nível ────────────────────────────────────
    /// <summary>
    /// Coloque esta zona (Collider2D IsTrigger = true, Tag = "Saida") 
    /// na porta de saída do nível.
    /// </summary>
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !nivelConcluido)
            ConcluirNivel();
    }

    // ─── Conclusão do Nível ───────────────────────────────────────────
    /// <summary>Pode ser chamado externamente (ex: ao derrotar o boss).</summary>
    public void ConcluirNivel()
    {
        if (nivelConcluido) return;
        nivelConcluido = true;

        AudioManager.Instance?.PlaySFX("nivel_completo");

        // Fade-out antes de trocar de cena
        if (animadorFade != null)
        {
            animadorFade.SetTrigger(triggerFadeOut);
            Invoke(nameof(CarregarProximoNivel), delayProximoNivel);
        }
        else
        {
            Invoke(nameof(CarregarProximoNivel), delayProximoNivel);
        }
    }

    private void CarregarProximoNivel()
    {
        GameManager.Instance?.ProximoNivel();
    }
}
