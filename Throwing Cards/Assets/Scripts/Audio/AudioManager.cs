﻿using UnityEngine.Audio; 
using UnityEngine;
using System;
using System.Diagnostics;

public class AudioManager : MonoBehaviour
{
    // -- for singleton pattern 
    public static AudioManager instance = null;

    public Sound[] sounds; 

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        foreach (Sound s in sounds)
        {
            s.source = this.gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.loop = s.loop; 
        }
    }

    void Start()
    {
        //Play("MainTheme"); 
    }

    public void Play(string name)
    {


        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.source.volume = s.volume;
        s.source.pitch = s.pitch;


        if (s==null)
        {
            UnityEngine.Debug.LogError("Audio clip not found: " + name); 
            return; 
        }

        if(!s.source.isPlaying)
        {
            s.source.Play();
            print("Playing: " + name);
        }

    }

    public void Stop(string sound)
    {
        Sound s = Array.Find(sounds, item => item.name == sound);
        if (s == null)
        {
            UnityEngine.Debug.LogError("Sound: " + name + " not found!");
            return;
        }

        s.source.Stop(); 
    }

}
