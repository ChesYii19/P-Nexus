using UnityEngine;

/// <summary>
/// Permite que o jogador escale quando estiver encostado neste objeto.
/// Requer: Collider2D com IsTrigger=true e a Tag "Escada".
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class Escada : MonoBehaviour
{
    void Awake()
    {
        GetComponent<BoxCollider2D>().isTrigger = true;
    }
}
