using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public Sound[] sounds;

    void Start()
    {
        if (instance != null) { UnityEngine.Debug.LogError("Multiple Audio Managers in Scene!"); }
        else { instance = this; }

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;

            s.source.loop = s.loop;
        }
    }

    public void PlaySound(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null) { UnityEngine.Debug.LogWarning("Sound of name " + name + " was not found."); }
        s.source.Play();
    }
}
