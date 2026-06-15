using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// HUDController — Controla todos os elementos de interface do Protocol Nexus.
/// Gerencia: nome do nível, painel de pausa, tela de Game Over e de Vitória.
/// Deve existir um único HUDController por cena de jogo.
/// </summary>
public class HUDController : MonoBehaviour
{
    // ─── Painéis ───────────────────────────────────────────────────────
    [Header("Painéis")]
    public GameObject painelPausa;
    public GameObject painelGameOver;
    public GameObject painelVitoria;

    // ─── Textos de HUD ─────────────────────────────────────────────────
    [Header("Textos HUD")]
    [Tooltip("Texto que exibe o nome do nível por alguns segundos")]
    public TextMeshProUGUI textoNomeNivel;

    [Tooltip("Duração em segundos que o nome do nível fica visível")]
    public float duracaoNomeNivel = 3f;

    [Header("Game Over / Vitória")]
    public TextMeshProUGUI textoPontuacaoFinal;

    // ──────────────────────────────────────────────────────────────────
    void OnEnable()
    {
        GameManager.OnPausaAlterada += MostrarPausa;
        PlayerStatus.OnJogadorMorreu += MostrarGameOver;
    }

    void OnDisable()
    {
        GameManager.OnPausaAlterada -= MostrarPausa;
        PlayerStatus.OnJogadorMorreu -= MostrarGameOver;
    }

    void Start()
    {
        painelPausa?.SetActive(false);
        painelGameOver?.SetActive(false);
        painelVitoria?.SetActive(false);
    }

    // ─── Nome do Nível ─────────────────────────────────────────────────
    /// <summary>Exibe o nome do nível por alguns segundos e some.</summary>
    public void ExibirNomeNivel(string nome)
    {
        if (textoNomeNivel == null) return;

        textoNomeNivel.text = nome;
        textoNomeNivel.gameObject.SetActive(true);
        Invoke(nameof(OcultarNomeNivel), duracaoNomeNivel);
    }

    private void OcultarNomeNivel()
    {
        textoNomeNivel?.gameObject.SetActive(false);
    }

    // ─── Pausa ─────────────────────────────────────────────────────────
    private void MostrarPausa(bool emPausa)
    {
        painelPausa?.SetActive(emPausa);
    }

    // ─── Game Over ─────────────────────────────────────────────────────
    private void MostrarGameOver()
    {
        painelGameOver?.SetActive(true);

        if (textoPontuacaoFinal != null)
        {
            int pontos = GameManager.Instance?.GetPontuacaoTotal() ?? 0;
            textoPontuacaoFinal.text = $"SCORE FINAL: {pontos:D6}";
        }
    }

    // ─── Vitória ───────────────────────────────────────────────────────
    public void MostrarVitoria()
    {
        painelVitoria?.SetActive(true);

        if (textoPontuacaoFinal != null)
        {
            int pontos = GameManager.Instance?.GetPontuacaoTotal() ?? 0;
            textoPontuacaoFinal.text = $"SCORE FINAL: {pontos:D6}";
        }
    }

    // ─── Botões dos Painéis ────────────────────────────────────────────
    public void BotaoReiniciar()
    {
        GameManager.Instance?.ReiniciarNivel();
    }

    public void BotaoMenu()
    {
        GameManager.Instance?.IrParaMenu();
    }

    public void BotaoRetomar()
    {
        GameManager.Instance?.AlternarPausa();
    }
}
