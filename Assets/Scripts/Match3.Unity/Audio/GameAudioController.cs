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
        [SerializeField] private AudioClip specialSpawnSound;

        public static GameAudioController Instance { get; private set; }
        
        private float _musicVolume = 0.5f;
        private float _musicMultiplier = 1f;
        private float _targetMusicMultiplier = 1f;
        private float _transitionMultiplier = 1f;
        private Coroutine _musicTransition;
        private AudioClip _requestedMusic;

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
            _musicMultiplier = Mathf.MoveTowards(_musicMultiplier, _targetMusicMultiplier, 1.5f * Time.unscaledDeltaTime);
            ApplyMusicVolume();
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        public void SetMusicVolume(float value)
        {
            _musicVolume = Mathf.Clamp01(value);
            ApplyMusicVolume();
        }
        
        public void SetEffectsVolume(float value)
        {
            var volume = Mathf.Clamp01(value);
            effectsSource.volume = volume;
            uiSource.volume = volume;
        }
        
        public void SetMusicDucked(bool ducked)
        {
            _targetMusicMultiplier = ducked ? 0.25f : 1f;
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
        
        public void PlayVictorySound()
        {
            PlayResultSound(victorySound);
        }

        public void PlayDefeatSound()
        {
            PlayResultSound(defeatSound);
        }
        
        public void PlayButtonClick()
        {
            if (buttonClickSound == null)
            {
                return;
            }
            uiSource.PlayOneShot(buttonClickSound);
        }
        
        public void StopResultEffects()
        {
            effectsSource.Stop();
        }

        public void PlayMatchSound()
        {
            if (matchSound == null)
            {
                return;
            }
            effectsSource.PlayOneShot(matchSound);
        }
        
        public void PlaySpecialActivationSound()
        {
            if (specialActivationSound == null)
            {
                return;
            }
            effectsSource.PlayOneShot(specialActivationSound);
        }
        
        public void PlaySpecialSpawnSound()
        {
            if (specialSpawnSound == null)
            {
                return;
            }
            effectsSource.PlayOneShot(specialSpawnSound);
        }
        
        private void ApplyMusicVolume()
        {
            musicSource.volume = _musicVolume * _musicMultiplier * _transitionMultiplier;
        }
        
        private void PlayMusic(AudioClip clip)
        {
            if (clip == null)
            {
                return;
            }
            if (_requestedMusic == clip && _musicTransition != null)
            {
                return;
            }
            _requestedMusic = clip;
            
            if (_musicTransition != null)
            {
                StopCoroutine(_musicTransition);
                _musicTransition = null;
            }
            _musicTransition = StartCoroutine(TransitionMusic(clip));
        }
        
        private IEnumerator TransitionMusic(AudioClip clip)
        {
            var alreadyPlaying = musicSource.clip == clip && musicSource.isPlaying;
            if (!alreadyPlaying)
            {
                if (musicSource.isPlaying)
                {
                    yield return FadeMusicTo(0f);
                }

                _transitionMultiplier = 0f;
                ApplyMusicVolume();
                
                musicSource.clip = clip;
                musicSource.loop = true;
                musicSource.Play();
            }
            yield return FadeMusicTo(1f);
            _musicTransition = null;
        }
        
        private IEnumerator FadeMusicTo(float target)
        {
            var start = _transitionMultiplier;
            var elapsed = 0f;
            var duration = Mathf.Max(0.1f, musicFadeSeconds);

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                _transitionMultiplier = Mathf.Lerp(start, target, elapsed / duration);

                ApplyMusicVolume();
                yield return null;
            }
            _transitionMultiplier = target;
            ApplyMusicVolume();
        }
        
        private void PlayResultSound(AudioClip clip)
        {
            StopResultEffects();

            if (clip == null)
            {
                return;
            }
            effectsSource.clip = clip;
            effectsSource.Play();
        }
    }
}