using UnityEngine;

/// <summary>
/// AntiGravityZone - Define uma área onde a gravidade do jogador é anulada.
/// Útil para seções de plataforma ou quebra-cabeças que exigem flutuação.
/// Requer um Collider2D com 'Is Trigger' ativado.
/// </summary>
public class AntiGravityZone : MonoBehaviour
{
    [Tooltip("A gravidade a ser aplicada ao jogador dentro desta zona. 0 para gravidade zero.")]
    public float novaGravidade = 0f;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody2D playerRb = other.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                playerRb.gravityScale = novaGravidade;
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody2D playerRb = other.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                // Retorna à gravidade padrão do jogo ao sair da zona
                playerRb.gravityScale = 1f; 
            }
        }
    }
}
