using UnityEngine;

namespace IdleSpace.Audio
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioClip backgroundMusic;
        [SerializeField] private AudioClip clickClip;
        [SerializeField] private AudioClip buyClip;
        [SerializeField] private AudioClip prestigeClip;

        private void Start()
        {
            if (musicSource != null && backgroundMusic != null)
            {
                musicSource.clip = backgroundMusic;
                musicSource.loop = true;
                if (!musicSource.isPlaying)
                {
                    musicSource.Play();
                }
            }
        }

        public void PlayClick() => PlaySfx(clickClip);
        public void PlayBuy() => PlaySfx(buyClip);
        public void PlayPrestige() => PlaySfx(prestigeClip);

        public void PlaySfx(AudioClip clip)
        {
            if (sfxSource == null || clip == null)
            {
                return;
            }

            sfxSource.PlayOneShot(clip);
        }
    }
}
