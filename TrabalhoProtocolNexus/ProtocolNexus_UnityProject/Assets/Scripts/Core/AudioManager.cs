using UnityEngine;

/// <summary>
/// Gerencia todos os efeitos sonoros e músicas do jogo.
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Músicas")]
    public AudioClip musicaBoss;
    private AudioSource bgmSource;

    [Header("Efeitos")]
    public AudioClip somPulo;
    public AudioClip somTiro;
    public AudioClip somColeta;
    public AudioClip somDano;
    private AudioSource sfxSource;
    
    public float volumeMusica { get { return bgmSource.volume; } }
    public float volumeSFX { get { return sfxSource.volume; } }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            ConfigurarSources();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void ConfigurarSources()
    {
        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.loop = true;
        bgmSource.volume = 0.5f;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.volume = 0.8f;

        // FOOLPROOF AUDIO LOADING
        if (somTiro == null)
        {
#if UNITY_EDITOR
            somTiro = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sprites/Weapons/Warped_FX/Warped shooting fx files/warped-shooting-fx.wav");
#endif
        }
    }

    public void PlayMusica(AudioClip clip)
    {
        if (clip != null && bgmSource.clip != clip)
        {
            bgmSource.clip = clip;
            bgmSource.Play();
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    public void PlaySFX(string nome)
    {
        nome = nome.ToLower();
        if (nome.Contains("pulo") || nome.Contains("jump")) PlaySFX(somPulo);
        else if (nome.Contains("tiro") || nome.Contains("shoot") || nome.Contains("laser")) PlaySFX(somTiro);
        else if (nome.Contains("dano") || nome.Contains("hit") || nome.Contains("morte")) PlaySFX(somDano);
        else if (nome.Contains("moeda") || nome.Contains("coin") || nome.Contains("coleta")) PlaySFX(somColeta);
    }

    public void PlayMusica(string nome)
    {
        // Se pedir musica de boss, toca o boss.
        if (nome.ToLower().Contains("boss"))
        {
            PlayMusica(musicaBoss);
        }
    }

    public void SetVolumeMusica(float vol)
    {
        if (bgmSource != null) bgmSource.volume = vol;
    }

    public void SetVolumeSFX(float vol)
    {
        if (sfxSource != null) sfxSource.volume = vol;
    }

    public void TocarMusicaBoss()
    {
        PlayMusica(musicaBoss);
    }

    public void TocarPulo() { PlaySFX(somPulo); }
    public void TocarTiro() { PlaySFX(somTiro); }
    public void TocarColeta() { PlaySFX(somColeta); }
    public void TocarDano() { PlaySFX(somDano); }
}
