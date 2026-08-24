using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgMusic;
    [SerializeField] private AudioSource soundEffectforUI;

    [Header("Global Music & Sounds")]
    [SerializeField] private AudioClip music;
    [SerializeField, Range(0f, 2f)] private float musicVolume = 0.5f;

    [SerializeField] private AudioClip movement;
    [SerializeField, Range(0f, 2f)] private float movementVolume = 1f;

    [SerializeField] private AudioClip headbutt;
    [SerializeField, Range(0f, 2f)] private float headbuttVolume = 1f;

    [SerializeField] private AudioClip boardtilting;
    [SerializeField, Range(0f, 2f)] private float boardtiltingVolume = 1f;

    [SerializeField] private AudioClip enemySpawn;
    [SerializeField, Range(0f, 2f)] private float enemySpawnVolume = 1f;

    [SerializeField] private AudioClip TakingDamage;
    [SerializeField, Range(0f, 2f)] private float TakingDamageVolume = 1f;

    [SerializeField] private AudioClip EnemyDie;
    [SerializeField, Range(0f, 2f)] private float EnemyDieVolume = 1f;

    [SerializeField] private AudioClip GameOver;
    [SerializeField, Range(0f, 2f)] private float GameOverVolume = 1f;


    [Header("Upgrades & UI")]
    [SerializeField] private AudioClip UpgradeButton;
    [SerializeField, Range(0f, 2f)] private float UpgradeButtonVolume = 1f;

    [SerializeField] private AudioClip HoverSound;
    [SerializeField, Range(0f, 2f)] private float HoverSoundVolume = 1f;
    
    [SerializeField] private AudioClip Buttons;
    [SerializeField, Range(0f, 2f)] private float ButtonsVolume = 1f;

    [SerializeField] public AudioClip waveTransition;
    [SerializeField, Range(0f, 2f)] public float waveTransitionVolume = 1f;

    [SerializeField] private AudioClip Abilities;
    [SerializeField, Range(0f, 2f)] private float AbilitiesVolume = 1f;

    [SerializeField] private AudioClip GoldenWind;
    [SerializeField, Range(0f, 2f)] private float GoldenWindVolume = 1f;


    [Header("Enemies")]
    [SerializeField] private AudioClip HandPoke;
    [SerializeField, Range(0f, 2f)] private float HandPokeVolume = 1f;

    [SerializeField] private AudioClip HandShoot;
    [SerializeField, Range(0f, 2f)] private float HandShootVolume = 1f;

    [SerializeField] private AudioClip HandLazer;
    [SerializeField, Range(0f, 2f)] private float HandLazerVolume = 1f;

    [SerializeField] private AudioClip explosion;
    [SerializeField, Range(0f, 2f)] private float explosionVolume = 1f;

    [SerializeField] private AudioClip blocktoss;
    [SerializeField, Range(0f, 2f)] private float blocktossVolume = 1f;

    [SerializeField] private AudioClip blockPlace;
    [SerializeField, Range(0f, 2f)] private float blockPlaceVolume = 1f;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (music != null) playMusic(music, musicVolume);
    }


    public void playMusic(AudioClip clip, float volume = 1f)
    {
        bgMusic.clip = clip;
        bgMusic.volume = volume;
        bgMusic.loop = true;
        bgMusic.Play();
    }

    public void playUISound(AudioClip clip, float volume = 1f)
    {
        if (clip != null) soundEffectforUI.PlayOneShot(clip, volume);
    }

    public void PlaySFX(AudioClip clip, Vector3 position, float volume = 1f)
    {
        if (clip != null) AudioSource.PlayClipAtPoint(clip, position, volume);
    }

    public void PlayTrimmedAudio(AudioClip clip, Vector3 position, float duration, float volume = 1f)
    {
        if (clip != null) StartCoroutine(TrimmedAudio(clip, position, duration, volume));
    }

    IEnumerator TrimmedAudio(AudioClip clip, Vector3 position, float duration, float volume = 1f)
    {
        var tempSound = new GameObject("TempSound");
        tempSound.transform.position = position;

        var audioSource = tempSound.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.spatialBlend = 1f; 

        audioSource.Play();
        yield return new WaitForSeconds(duration);

        Destroy(tempSound);
    }


    public void PlayMovementSound(Vector3 position) => PlaySFX(movement, position, movementVolume);
    public void PlayHeadbuttSound(Vector3 position) => PlaySFX(headbutt, position, headbuttVolume);
    public void PlayBoardTiltingSound(Vector3 position) => PlaySFX(boardtilting, position, boardtiltingVolume);
    public void PlayTakingDamageSound(Vector3 position) => PlaySFX(TakingDamage, position, TakingDamageVolume);

    public void PlayEnemySpawnSound(Vector3 position) => PlaySFX(enemySpawn, position, enemySpawnVolume);
    public void PlayEnemyDieSound(Vector3 position) => PlaySFX(EnemyDie, position, EnemyDieVolume);
    public void PlayGameOverSound() => playUISound(GameOver, GameOverVolume); 

    public void PlayUpgradeButtonSound() => playUISound(UpgradeButton, UpgradeButtonVolume);
    public void PlayHoverSound() => playUISound(HoverSound, HoverSoundVolume);

    public void PlayGenericButtonSound() => playUISound(Buttons, ButtonsVolume);
    public void PlayAbilitiesSound(Vector3 position, float duration) => PlayTrimmedAudio(Abilities, position, duration, AbilitiesVolume);
    public void PlayGoldenWindSound(Vector3 position) => PlaySFX(GoldenWind, position, GoldenWindVolume);
    public void PlayWaveTransitionSound() => playUISound(waveTransition, waveTransitionVolume);
    public void PlayHandPokeSound(Vector3 position) => PlaySFX(HandPoke, position, HandPokeVolume);
    public void PlayHandShootSound(Vector3 position) => PlaySFX(HandShoot, position, HandShootVolume);
    public void PlayHandLazerSound(Vector3 position) => PlaySFX(HandLazer, position, HandLazerVolume);
    public void PlayExplosionSound(Vector3 position) => PlaySFX(explosion, position, explosionVolume);
    public void PlayBlockTossSound(Vector3 position) => PlaySFX(blocktoss, position, blocktossVolume);
    public void PlayBlockPlaceSound(Vector3 position) => PlaySFX(blockPlace, position, blockPlaceVolume);
}