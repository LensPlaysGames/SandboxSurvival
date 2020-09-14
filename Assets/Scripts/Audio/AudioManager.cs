using System;
using UnityEngine;
using UnityEngine.Audio;

namespace U_Grow
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager instance;

        public Sound[] sounds;

        void Start()
        {
            if (instance != null) { UnityEngine.Debug.LogError("Multiple Audio Managers in Scene!"); Destroy(this.gameObject); }
            else { instance = this; GameReferences.audioManager = instance; }

            AudioMixer mixer = Resources.Load<AudioMixer>("Audio/AudioMixer");
            foreach (Sound s in sounds)
            {
                s.source = gameObject.AddComponent<AudioSource>();
                s.source.clip = s.clip;

                s.source.volume = s.volume;
                s.source.pitch = s.pitch;

                s.source.loop = s.loop;

                s.source.outputAudioMixerGroup = mixer.FindMatchingGroups("Sfx")[0];
            }
        }

        public void PlaySound(string name)
        {
            Sound s = Array.Find(sounds, sound => sound.name == name);
            if (s == null) { UnityEngine.Debug.LogWarning("Sound of name " + name + " was not found."); }
            s.source.Play();
        }
    }
}