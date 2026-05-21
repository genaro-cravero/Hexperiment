using Player;
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager Instance { get; private set; }

    [SerializeField] private AudioSource _soundFXObject;
    [SerializeField] private Transform _soundContainer;

    [Header("Music")]
    [SerializeField, Range(0, 1)] private float _musicVolume = 0.75f;
    [SerializeField] private AudioSource _bgMusicSource;
    [SerializeField] private AudioSource _waveMusicSource;

    private IObjectPool<AudioSource> _audioSourcePool;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        _audioSourcePool = new ObjectPool<AudioSource>(
            createFunc: () => Instantiate(_soundFXObject, _soundContainer),
            actionOnGet: audioSource => audioSource.gameObject.SetActive(true),
            actionOnRelease: audioSource => audioSource.gameObject.SetActive(false),
            actionOnDestroy: audioSource => Destroy(audioSource.gameObject),
            maxSize: 20
        );

    }

    private IEnumerator ReleaseAudio(AudioSource audioSource)
    {
        if (audioSource && audioSource.clip.length > 0)
        {
            float clipLength = audioSource.clip.length;
            yield return new WaitForSeconds(clipLength);
        }

        _audioSourcePool.Release(audioSource);
    }

    public void PlaySound(AudioClip clip, Vector3 position, float volume = 1f)
    {
        var audioSource = _audioSourcePool.Get();
        audioSource.transform.position = position;

        audioSource.clip = clip;
        audioSource.volume = volume;

        audioSource.Play();

        StartCoroutine(ReleaseAudio(audioSource));
    }

    public void PlaySound(AudioClip[] clips, Vector3 position, float volume = 1f)
    {
        var clip = clips[Random.Range(0, clips.Length)];

        var audioSource = _audioSourcePool.Get();
        audioSource.transform.position = position;

        audioSource.clip = clip;
        audioSource.volume = volume;

        audioSource.Play();

        StartCoroutine(ReleaseAudio(audioSource));
    }

    public void FadeSongs(bool toWave = true)
    {
        StartCoroutine(FadeSongsCoroutine(toWave));
    }

    private IEnumerator FadeSongsCoroutine(bool toWave)
    {
        float fadeDuration = toWave ? 0.65f : 3f;
        float elapsedTime = 0f;

        AudioSource fadingOutSource = toWave ? _bgMusicSource : _waveMusicSource;
        AudioSource fadingInSource = toWave ? _waveMusicSource : _bgMusicSource;

        fadingInSource.volume = 0f;
        fadingInSource.Play();

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / fadeDuration;
            fadingOutSource.volume = Mathf.Lerp(_musicVolume, 0f, t);
            fadingInSource.volume = Mathf.Lerp(0f, _musicVolume, t);
            yield return null;
        }

        fadingInSource.volume = _musicVolume;
        fadingOutSource.Stop();
    }

}
