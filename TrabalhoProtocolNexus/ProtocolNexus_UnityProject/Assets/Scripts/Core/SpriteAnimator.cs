using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SpriteAnimState
{
    public string stateName;
    public string spriteSheetPath;
    public bool loop = true;
    public float fps = 10f;
}

/// <summary>
/// Máquina de Estados de Animação guiada por Código.
/// Carrega múltiplas SpriteSheets via paths (no editor) e guarda em cache.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class SpriteAnimator : MonoBehaviour
{
    public SpriteAnimState[] states;
    
    private SpriteRenderer sr;
    private Dictionary<string, Sprite[]> animations = new Dictionary<string, Sprite[]>();
    private Dictionary<string, SpriteAnimState> stateConfig = new Dictionary<string, SpriteAnimState>();
    
    private Sprite[] currentFrames;
    private float timer;
    private int index;
    private string currentState = "";
    private bool currentLoop = true;
    private float currentFps = 10f;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();

#if UNITY_EDITOR
        if (states != null)
        {
            foreach (var s in states)
            {
                CarregarAnimacao(s.stateName, s.spriteSheetPath);
                stateConfig[s.stateName] = s;
            }
        }
#endif
    }

    void Start()
    {
        if (states != null && states.Length > 0)
            Play(states[0].stateName);
    }

#if UNITY_EDITOR
    private void CarregarAnimacao(string stateName, string path)
    {
        if (string.IsNullOrEmpty(path)) return;

        Object[] assets = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(path);
        List<Sprite> spriteList = new List<Sprite>();
        
        if (assets != null)
        {
            foreach (var a in assets)
            {
                if (a is Sprite s) spriteList.Add(s);
            }
        }
        
        if (spriteList.Count > 0)
        {
            spriteList.Sort((a, b) => {
                int numA = 0; int.TryParse(a.name.Substring(a.name.LastIndexOf('_') + 1), out numA);
                int numB = 0; int.TryParse(b.name.Substring(b.name.LastIndexOf('_') + 1), out numB);
                return numA.CompareTo(numB);
            });
            animations[stateName] = spriteList.ToArray();
        }
    }
#endif

    public void Play(string stateName)
    {
        if (currentState == stateName) return; // Já está tocando
        if (!animations.ContainsKey(stateName)) return; // Não existe

        currentState = stateName;
        currentFrames = animations[stateName];
        
        if (stateConfig.ContainsKey(stateName))
        {
            currentLoop = stateConfig[stateName].loop;
            currentFps = stateConfig[stateName].fps;
        }

        index = 0;
        timer = 0f;
        
        if (currentFrames.Length > 0)
            sr.sprite = currentFrames[0];
    }

    void Update()
    {
        if (currentFrames == null || currentFrames.Length == 0) return;

        timer += Time.deltaTime;

        if (timer >= 1f / currentFps)
        {
            timer -= 1f / currentFps;
            index++;
            
            if (index >= currentFrames.Length)
            {
                if (currentLoop)
                    index = 0;
                else
                    index = currentFrames.Length - 1; // Para no último frame
            }
            
            sr.sprite = currentFrames[index];
        }
    }
}
