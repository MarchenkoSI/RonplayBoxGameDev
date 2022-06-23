using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityMediaCore;

namespace RonplayBoxGameDev
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioManager : SingletonBehaviour<AudioManager>
    {
        public static AudioManager instance;

        public AudioClip clickSound;
        public AudioClip selectSound;

        private void Start()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void PlayClickSound()
        {
            _audio_source.PlayOneShot(clickSound);
        }

        public void PlaySelectSound()
        {
            _audio_source.PlayOneShot(selectSound);
        }

        private AudioSource _audio_source { get { return GetComponent<AudioSource>(); } }
    }
}


