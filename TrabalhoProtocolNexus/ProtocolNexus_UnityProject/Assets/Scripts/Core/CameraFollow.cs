using UnityEngine;

/// <summary>
/// CameraFollow — Câmera suave que segue o jogador Tatsuya Yuki.
/// Recursos: Lerp suave, limites de mapa (bounds), efeito de screen shake.
/// Adicione este script na câmera principal da cena.
/// </summary>
public class CameraFollow : MonoBehaviour
{
    // ─── Alvo ─────────────────────────────────────────────────────────
    [Header("Alvo")]
    [Tooltip("Transform do jogador para seguir")]
    public Transform alvo;

    [Tooltip("Deslocamento relativo ao alvo (offset)")]
    public Vector3 offset = new Vector3(0f, 1f, -10f);

    // ─── Suavização ───────────────────────────────────────────────────
    [Header("Suavização")]
    [Tooltip("Velocidade do Lerp (menor = mais suave)")]
    [Range(1f, 20f)]
    public float velocidadeSeguimento = 5f;

    // ─── Limites do Mapa ──────────────────────────────────────────────
    [Header("Limites do Mapa (Bounds)")]
    [Tooltip("Ativar limitação da câmera ao mapa")]
    public bool usarLimites = false;

    public float limiteEsquerda  = -50f;
    public float limiteDireita   =  500f;
    public float limiteInferior  =  -5f;
    public float limiteSuperior  =  20f;

    // ─── Screen Shake ─────────────────────────────────────────────────
    [Header("Screen Shake")]
    private float shakeDuracao   = 0f;
    private float shakeMagnitude = 0f;
    private Vector3 posicaoOriginal;

    // ─── Cache ────────────────────────────────────────────────────────
    private Camera cam;

    // ──────────────────────────────────────────────────────────────────
    void Awake()
    {
        cam = GetComponent<Camera>();
        usarLimites = false; // FORÇADO PARA SOBRESCREVER DADOS SALVOS NA CENA

        // Tenta encontrar o jogador automaticamente se não configurado
        if (alvo == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) alvo = p.transform;
        }
    }

    void LateUpdate()
    {
        if (alvo == null) return;

        // Screen Shake tem prioridade
        if (shakeDuracao > 0f)
        {
            AplicarShake();
            shakeDuracao -= Time.deltaTime;
            return;
        }
        else
        {
            transform.localPosition = posicaoOriginal;
        }

        SeguirAlvo();
    }

    // ─── Seguimento Suave ─────────────────────────────────────────────
    private void SeguirAlvo()
    {
        Vector3 posicaoDesejada = alvo.position + offset;

        // Aplica limites do mapa
        if (usarLimites)
        {
            float metadeAltura  = cam.orthographicSize;
            float metadeLargura = cam.orthographicSize * cam.aspect;

            posicaoDesejada.x = Mathf.Clamp(posicaoDesejada.x,
                limiteEsquerda + metadeLargura,
                limiteDireita  - metadeLargura);

            posicaoDesejada.y = Mathf.Clamp(posicaoDesejada.y,
                limiteInferior + metadeAltura,
                limiteSuperior - metadeAltura);
        }

        // Mantém o Z fixo do offset
        posicaoDesejada.z = offset.z;

        // Lerp suave
        transform.position = Vector3.Lerp(
            transform.position,
            posicaoDesejada,
            velocidadeSeguimento * Time.deltaTime
        );

        posicaoOriginal = transform.position;
    }

    // ─── Screen Shake API ─────────────────────────────────────────────
    /// <summary>Inicia um efeito de tremor de câmera.</summary>
    /// <param name="duracao">Duração em segundos</param>
    /// <param name="magnitude">Intensidade do tremor</param>
    public void IniciarShake(float duracao, float magnitude)
    {
        shakeDuracao   = duracao;
        shakeMagnitude = magnitude;
    }

    private void AplicarShake()
    {
        float x = Random.Range(-1f, 1f) * shakeMagnitude;
        float y = Random.Range(-1f, 1f) * shakeMagnitude;

        transform.localPosition = new Vector3(
            posicaoOriginal.x + x,
            posicaoOriginal.y + y,
            posicaoOriginal.z
        );
    }

    // ─── Gizmos de Debug ──────────────────────────────────────────────
    void OnDrawGizmosSelected()
    {
        if (!usarLimites) return;

        Gizmos.color = Color.cyan;
        Vector3 centro = new Vector3(
            (limiteEsquerda + limiteDireita) / 2f,
            (limiteInferior + limiteSuperior) / 2f, 0f
        );
        Vector3 tamanho = new Vector3(
            limiteDireita - limiteEsquerda,
            limiteSuperior - limiteInferior, 0f
        );
        Gizmos.DrawWireCube(centro, tamanho);
    }
}
