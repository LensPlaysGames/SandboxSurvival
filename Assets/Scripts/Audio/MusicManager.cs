using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;
    public DataDontDestroyOnLoad dataDontDestroyOnLoad;

    public Sound[] tracks;

    void Start()
    {
        if (instance != null) { UnityEngine.Debug.LogError("Multiple Audio Managers in Scene!"); }
        else { instance = this; DontDestroyOnLoad(instance); }

        if (dataDontDestroyOnLoad != null) { UnityEngine.Debug.LogError("Multiple Audio Managers in Scene!"); }
        else { dataDontDestroyOnLoad = GameObject.Find("DataDontDestroyOnLoad").GetComponent<DataDontDestroyOnLoad>(); }

        foreach (Sound s in tracks)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;

            s.source.loop = s.loop;
        }

        if (dataDontDestroyOnLoad.playingMusic == false)
        {
            PlayMusic("track01");
            dataDontDestroyOnLoad.playingMusic = true;
        }

    }

    public void PlayMusic(string name)
    {
        Sound s = Array.Find(tracks, sound => sound.name == name);
        if (s == null) { UnityEngine.Debug.LogWarning("Sound of name " + name + " was not found."); }
        s.source.Play();
    }
}
