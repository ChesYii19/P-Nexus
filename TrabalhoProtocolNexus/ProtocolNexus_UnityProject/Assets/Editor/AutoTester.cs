using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;

public class AutoTester : MonoBehaviour
{
    [MenuItem("Protocol Nexus/7 - TESTAR O JOGO (AUTOMÁTICO)")]
    public static void IniciarTeste()
    {
        Debug.Log("o. Iniciando Auto-Teste da IA...");
        EditorPrefs.SetBool("AutoTestRunning", true);
        EditorApplication.isPlaying = true;
    }

    [InitializeOnLoadMethod]
    static void SetupPlayModeListener()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            if (EditorPrefs.GetBool("AutoTestRunning", false))
            {
                // Injeta o robô de teste no jogo
                GameObject tester = new GameObject("IA_AutoTester");
                tester.AddComponent<AutoTesterBot>();
            }
        }
        else if (state == PlayModeStateChange.ExitingPlayMode)
        {
            EditorPrefs.SetBool("AutoTestRunning", false);
        }
    }
}

public class AutoTesterBot : MonoBehaviour
{
    private GameObject tatsuya;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator anim;
    private string logPath = "C:/Users/Sparr/.gemini/antigravity/scratch/TestResults.txt";
    private string relatorio = "=== RELATÓRIO DO AUTO-TESTE ===\n\n";

    void Start()
    {
        StartCoroutine(ExecutarTeste());
    }

    IEnumerator ExecutarTeste()
    {
        relatorio += "[0.0s] Procurando Tatsuya no mapa...\n";
        yield return new WaitForSeconds(0.5f);
        
        tatsuya = GameObject.FindWithTag("Player");
        if (tatsuya == null)
        {
            relatorio += "ERRO CRÍTICO: Tatsuya não foi encontrado na cena!\n";
            FinalizarTeste();
            yield break;
        }

        rb = tatsuya.GetComponent<Rigidbody2D>();
        sr = tatsuya.GetComponent<SpriteRenderer>();
        anim = tatsuya.GetComponent<Animator>();

        relatorio += $"[0.5s] Tatsuya encontrado na posição {tatsuya.transform.position}\n";
        relatorio += $"[0.5s] Sprite visível? {(sr.sprite != null ? sr.sprite.name : "NÃO (INVISÍVEL)")}\n";
        relatorio += $"[0.5s] Animator Controller ativo? {(anim.runtimeAnimatorController != null ? "SIM" : "NÃO")}\n";
        
        // Testa a gravidade
        yield return new WaitForSeconds(1f);
        relatorio += $"[1.5s] Após gravidade, posição Y atual: {tatsuya.transform.position.y}\n";

        // Força movimento para a direita
        relatorio += "[1.5s] Aguardando 1 segundo para ver se o PlayerController o move para a direita (input forçado)...\n";
        // rb.velocity = new Vector2(5f, rb.velocity.y); // Removido para deixar o PlayerController agir
        
        yield return new WaitForSeconds(1f);
        relatorio += $"[2.5s] Posição após 1s movendo: {tatsuya.transform.position.x}\n";

        if (tatsuya.transform.position.x > -9.5f)
        {
            relatorio += "SUCESSO: Tatsuya moveu horizontalmente com sucesso!\n";
        }
        else
        {
            relatorio += "FALHA: Tatsuya não saiu do lugar.\n";
        }

        FinalizarTeste();
    }

    void FinalizarTeste()
    {
        File.WriteAllText(logPath, relatorio);
        Debug.Log("o. Teste finalizado. Resultados salvos.");
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
