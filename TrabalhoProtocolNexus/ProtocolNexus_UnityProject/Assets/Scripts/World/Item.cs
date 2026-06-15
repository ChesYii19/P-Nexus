using UnityEngine;

/// <summary>
/// Item coletável do Protocol Nexus.
/// Tipos suportados: Moeda (pontos), Cura (restaura vida), PowerUp (futura expansão).
/// Ao ser coletado pelo jogador, executa a ação e se autodestrói.
/// Tag obrigatória: "Item".
/// </summary>
public class Item : MonoBehaviour
{
    // ─── Tipos de Item ─────────────────────────────────────────────────
    public enum TipoItem { Moeda, Cura, PowerUp }

    // ─── Configurações ─────────────────────────────────────────────────
    [Header("Item")]
    public TipoItem tipo = TipoItem.Moeda;

    [Tooltip("Valor do item: pontos (moeda) ou HP recuperado (cura)")]
    public int valor = 100;

    [Header("Animação Flutuante")]
    [Tooltip("Ativar efeito de flutuação")]
    public bool flutuar = true;

    [Tooltip("Altura da flutuação em unidades")]
    public float alturaFlutuacao = 0.3f;

    [Tooltip("Velocidade da flutuação")]
    public float velocidadeFlutuacao = 2f;

    [Header("Efeito ao Coletar")]
    [Tooltip("Prefab de partícula ao coletar (opcional)")]
    public GameObject efeitoColeta;

    // ─── Estado Interno ────────────────────────────────────────────────
    private Vector3 posicaoInicial;
    private float tempoInicial;

    // ──────────────────────────────────────────────────────────────────
    void Start()
    {
        posicaoInicial = transform.position;
        tempoInicial   = Time.time;
    }

    void Update()
    {
        if (!flutuar) return;

        // Movimento senoidal de flutuação
        float novoY = posicaoInicial.y +
            Mathf.Sin((Time.time - tempoInicial) * velocidadeFlutuacao) * alturaFlutuacao;

        transform.position = new Vector3(
            posicaoInicial.x,
            novoY,
            posicaoInicial.z
        );
    }

    // ─── API: Coleta pelo Jogador ──────────────────────────────────────
    /// <summary>
    /// Chamado pelo PlayerStatus ao entrar no trigger do item.
    /// </summary>
    public void Coletar(PlayerStatus jogador)
    {
        switch (tipo)
        {
            case TipoItem.Moeda:
                jogador.AdicionarPontos(valor);
                break;

            case TipoItem.Cura:
                jogador.Curar(valor);
                break;

            case TipoItem.PowerUp:
                // Extensível: adicionar lógica de power-up aqui
                jogador.AdicionarPontos(valor);
                break;
        }

        // Efeito visual
        if (efeitoColeta != null)
        {
            GameObject efeito = Instantiate(efeitoColeta, transform.position, Quaternion.identity);
            Destroy(efeito, 1f);
        }

        // Autodestruição
        Destroy(gameObject);
    }

    // ─── Coleta por Trigger ────────────────────────────────────────────
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStatus status = other.GetComponent<PlayerStatus>();
            if (status != null)
                Coletar(status);
        }
    }
}
