using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;
    public DataDontDestroyOnLoad dataDontDestroyOnLoad;

    public AudioMixer mixer;

    public Sound[] tracks;

    void Start()
    {
        if (instance != null) { UnityEngine.Debug.LogError("Multiple Music Managers in Scene! Destroying: " + this.name); Destroy(this); }
        else { instance = this; DontDestroyOnLoad(instance); }

        if (dataDontDestroyOnLoad != null) { UnityEngine.Debug.LogError("Multiple DataDontDestroyOnLoad in Scene!"); }
        else { dataDontDestroyOnLoad = GameObject.Find("DataDontDestroyOnLoad").GetComponent<DataDontDestroyOnLoad>(); }

        mixer = Resources.Load<AudioMixer>("Audio/AudioMixer");

        foreach (Sound s in tracks)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;

            s.source.loop = s.loop;

            s.source.outputAudioMixerGroup = mixer.FindMatchingGroups("Music")[0];
        }

        UpdateMixerVolumes();

        if (dataDontDestroyOnLoad.playingMusic == false)
        {
            PlayRandomMusic();
            dataDontDestroyOnLoad.playingMusic = true;
        }

    }

    public void UpdateMixerVolumes()
    {
        mixer.SetFloat("Master Volume", PlayerPrefs.GetFloat("Master Volume", 1f));
        mixer.SetFloat("Music Volume", PlayerPrefs.GetFloat("Music Volume", 1f));
        mixer.SetFloat("Sfx Volume", PlayerPrefs.GetFloat("Sfx Volume", 1f));
    }

    public void PlayRandomMusic()
    {
        int randTrack = UnityEngine.Random.Range(0, tracks.Length);
        Sound s = tracks[randTrack];
        s.source.Play();
    }

    public void PlayMusic(string name)
    {
        Sound s = Array.Find(tracks, sound => sound.name == name);
        if (s == null) { UnityEngine.Debug.LogWarning("Sound of name " + name + " was not found."); }
        s.source.Play();
    }
}
