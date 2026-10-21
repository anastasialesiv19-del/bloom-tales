using System.Collections;
using Match3.Unity.Settings;
using UnityEngine;

namespace Match3.Unity.Audio
{
    public class GameAudioController : MonoBehaviour
    {
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioClip menuMusic;
        [SerializeField] private AudioClip gameplayMusic;
        [SerializeField, Min(0.1f)] private float musicFadeSeconds = 0.6f;
        [SerializeField] private AudioSource effectsSource;
        [SerializeField] private AudioClip victorySound;
        [SerializeField] private AudioClip defeatSound;
        [SerializeField] private AudioSource uiSource;
        [SerializeField] private AudioClip buttonClickSound;
        [SerializeField] private AudioClip matchSound;
        [SerializeField] private AudioClip specialActivationSound;

        private float musicVolume = 0.5f;
        private float musicMultiplier = 1f;
        private float targetMusicMultiplier = 1f;
        private float transitionMultiplier = 1f;
        private Coroutine musicTransition;
        private AudioClip requestedMusic;
        
        
        public static GameAudioController Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            var settings = new JsonSettingsStore().Load();
            SetMusicVolume(settings.musicVolume);
            
            SetEffectsVolume(settings.effectVolume);
        }
        
        private void Update()
        {
            musicMultiplier = Mathf.MoveTowards(musicMultiplier, targetMusicMultiplier, 1.5f * Time.unscaledDeltaTime);
            ApplyMusicVolume();
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        public void SetMusicVolume(float value)
        {
            musicVolume = Mathf.Clamp01(value);
            ApplyMusicVolume();
        }

        private void ApplyMusicVolume()
        {
            musicSource.volume = musicVolume * musicMultiplier * transitionMultiplier;
        }

        public void SetEffectsVolume(float value)
        {
            float volume = Mathf.Clamp01(value);
            effectsSource.volume = volume;
            uiSource.volume = volume;
        }
        
        public void SetMusicDucked(bool ducked)
        {
            targetMusicMultiplier = ducked ? 0.25f : 1f;
        }

        public void PlayMenuMusic()
        {
            SetMusicDucked(false);
            PlayMusic(menuMusic);
        }

        public void PlayGameplayMusic()
        {
            SetMusicDucked(false);
            PlayMusic(gameplayMusic);
        }

        private void PlayMusic(AudioClip clip)
        {
            if (clip == null) return;

            if (requestedMusic == clip && musicTransition != null) return;

            requestedMusic = clip;
            if (musicTransition != null)
            {
                StopCoroutine(musicTransition);
                musicTransition = null;
            }

            musicTransition = StartCoroutine(TransitionMusic(clip));
        }
        
        private IEnumerator TransitionMusic(AudioClip clip)
        {
            bool alreadyPlaying = musicSource.clip == clip && musicSource.isPlaying;
            if (!alreadyPlaying)
            {
                if (musicSource.isPlaying)
                    yield return FadeMusicTo(0f);

                transitionMultiplier = 0f;
                ApplyMusicVolume();

                musicSource.clip = clip;
                musicSource.loop = true;
                musicSource.Play();
            }
            yield return FadeMusicTo(1f);
            musicTransition = null;
        }
        
        private IEnumerator FadeMusicTo(float target)
        {
            float start = transitionMultiplier;
            float elapsed = 0f;
            float duration = Mathf.Max(0.1f, musicFadeSeconds);

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                transitionMultiplier = Mathf.Lerp(
                    start, target, elapsed / duration);

                ApplyMusicVolume();
                yield return null;
            }

            transitionMultiplier = target;
            ApplyMusicVolume();
        }
        
        public void PlayVictorySound()
        {
            PlayResultSound(victorySound);
        }

        public void PlayDefeatSound()
        {
            PlayResultSound(defeatSound);
        }

        private void PlayResultSound(AudioClip clip)
        {
            StopResultEffects();

            if (clip == null) return;
            
            effectsSource.clip = clip;
            effectsSource.Play();
        }

        public void PlayButtonClick()
        {
            if (buttonClickSound == null) return;
            
            uiSource.PlayOneShot(buttonClickSound);
        }
        
        public void StopResultEffects()
        {
            effectsSource.Stop();
        }

        public void PlayMatchSound()
        {
            if(matchSound == null) return;
            
            effectsSource.PlayOneShot(matchSound);
        }
        
        public void PlaySpecialActivationSound()
        {
            if(specialActivationSound == null) return;
            
            effectsSource.PlayOneShot(specialActivationSound);
        }
    }
}