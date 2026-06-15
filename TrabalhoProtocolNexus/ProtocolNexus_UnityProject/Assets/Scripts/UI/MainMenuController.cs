using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// MainMenuController — Controla a tela inicial do Protocol Nexus.
/// Gerencia: iniciar jogo, opções de volume, créditos e sair.
/// Attach na cena "MenuPrincipal".
/// </summary>
public class MainMenuController : MonoBehaviour
{
    // ─── Painéis ───────────────────────────────────────────────────────
    [Header("Painéis")]
    public GameObject painelPrincipal;
    public GameObject painelOpcoes;
    public GameObject painelCreditos;

    // ─── Opções de Volume ──────────────────────────────────────────────
    [Header("Sliders de Volume")]
    public Slider sliderMusica;
    public Slider sliderSFX;

    // ─── Animação de Entrada ───────────────────────────────────────────
    [Header("Animação de Logo")]
    [Tooltip("Animator do logo do jogo (opcional)")]
    public Animator animadorLogo;

    // ──────────────────────────────────────────────────────────────────
    void Start()
    {
        // Mostra apenas o painel principal
        MostrarPainel(painelPrincipal);

        // Toca música do menu
        AudioManager.Instance?.PlayMusica("menu");

        // Anima o logo
        animadorLogo?.SetTrigger("entrar");

        // Configura sliders com valores do AudioManager
        if (AudioManager.Instance != null)
        {
            if (sliderMusica != null)
            {
                sliderMusica.value = AudioManager.Instance.volumeMusica;
                sliderMusica.onValueChanged.AddListener(AudioManager.Instance.SetVolumeMusica);
            }

            if (sliderSFX != null)
            {
                sliderSFX.value = AudioManager.Instance.volumeSFX;
                sliderSFX.onValueChanged.AddListener(AudioManager.Instance.SetVolumeSFX);
            }
        }
    }

    // ─── Botões do Menu Principal ──────────────────────────────────────
    /// <summary>Inicia o jogo a partir do Nível 1.</summary>
    public void BotaoJogar()
    {
        AudioManager.Instance?.PlaySFX("menu_clique");
        GameManager.Instance?.IniciarJogo();
    }

    /// <summary>Abre o painel de opções.</summary>
    public void BotaoOpcoes()
    {
        AudioManager.Instance?.PlaySFX("menu_clique");
        MostrarPainel(painelOpcoes);
    }

    /// <summary>Abre o painel de créditos.</summary>
    public void BotaoCreditos()
    {
        AudioManager.Instance?.PlaySFX("menu_clique");
        MostrarPainel(painelCreditos);
    }

    /// <summary>Fecha o painel atual e volta ao principal.</summary>
    public void BotaoVoltar()
    {
        AudioManager.Instance?.PlaySFX("menu_clique");
        MostrarPainel(painelPrincipal);
    }

    /// <summary>Encerra a aplicação.</summary>
    public void BotaoSair()
    {
        AudioManager.Instance?.PlaySFX("menu_clique");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // ─── Helper ────────────────────────────────────────────────────────
    private void MostrarPainel(GameObject painel)
    {
        painelPrincipal?.SetActive(painel == painelPrincipal);
        painelOpcoes?.SetActive(painel == painelOpcoes);
        painelCreditos?.SetActive(painel == painelCreditos);
    }
}
