using UnityEngine;

/// <summary>
/// Gerencia o ataque do jogador Tatsuya Yuki.
/// Estilo Megaman: dispara projéteis em direção ao movimento do sprite.
/// Requer: PlayerController na mesma GameObject.
/// Configurar: pontoDisparo (Transform filho), prefabProjetil.
/// </summary>
public class PlayerAttack : MonoBehaviour
{
    // ─── Configurações de Ataque ──────────────────────────────────────
    [Header("Projétil")]
    [Tooltip("Prefab do projétil a instanciar")]
    public GameObject prefabProjetil;

    [Tooltip("Ponto de onde o projétil sai (filho do Player)")]
    public Transform pontoDisparo;

    [Tooltip("Cooldown entre disparos em segundos")]
    public float cooldownAtaque = 0.3f;

    [Header("Animação")]
    [Tooltip("Parâmetro bool no Animator para animação de ataque")]
    public string parametroAnimAtaque = "atacando";

    // ─── Estado Interno ───────────────────────────────────────────────
    private float tempoCooldown = 0f;
    private Animator anim;
    private int hashAtacando;

    // ──────────────────────────────────────────────────────────────────
    void Awake()
    {
        anim = GetComponent<Animator>();
        hashAtacando = Animator.StringToHash(parametroAnimAtaque);

        // FOOLPROOF WEAPON VISUAL
        if (transform.Find("WeaponVisual") == null)
        {
            GameObject weaponObj = new GameObject("WeaponVisual");
            weaponObj.transform.SetParent(this.transform);
            weaponObj.transform.localPosition = new Vector3(0.5f, 0f, 0f); // Ao lado do Tatsuya
            weaponObj.transform.localScale = Vector3.one;
            
            SpriteRenderer sr = weaponObj.AddComponent<SpriteRenderer>();
            sr.sortingOrder = 15; // Na frente do Tatsuya (order 10)
            
#if UNITY_EDITOR
            Object[] assets = UnityEditor.AssetDatabase.LoadAllAssetsAtPath("Assets/Sprites/Weapons/Sci-Fi machine gun.png");
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
    }

    void Update()
    {
        // Reduz cooldown por frame
        if (tempoCooldown > 0f)
            tempoCooldown -= Time.deltaTime;

        // Detecta botão de ataque (Mouse0 ou Z)
        if ((Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.Z)) && tempoCooldown <= 0f)
        {
            Atirar();
        }
    }

    // ─── Lógica de Disparo ────────────────────────────────────────────
    private void Atirar()
    {
        if (prefabProjetil == null || pontoDisparo == null)
        {
            Debug.LogWarning("[PlayerAttack] Prefab ou PontoDisparo não configurado!");
            return;
        }

        // Direção baseada na escala X do sprite (virar)
        float direcaoX = Mathf.Sign(transform.localScale.x);

        // Instancia o projétil
        GameObject proj = Instantiate(prefabProjetil, pontoDisparo.position, Quaternion.identity);
        proj.SetActive(true); // GATILHO CORRIGIDO: Ativa o tiro que estava invisível

        // Passa a direção para o projétil
        Projetil compProjetil = proj.GetComponent<Projetil>();
        
        // Toca o som do tiro
        AudioManager.Instance?.PlaySFX("tiro");

        if (compProjetil != null)
            compProjetil.Inicializar(new Vector2(direcaoX, 0f));

        // Aplica cooldown
        tempoCooldown = cooldownAtaque;

        // Dispara animação de ataque
        if (anim != null && anim.runtimeAnimatorController != null)
        {
            // try { anim.SetTrigger...
        }
    }
}
