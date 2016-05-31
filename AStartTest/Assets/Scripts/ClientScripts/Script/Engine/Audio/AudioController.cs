using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioController
{
    public static readonly AudioController Instance = new AudioController();

    private bool _musicEnabled = true;
    private bool _soundEnabled = true;
    private float _musicVolume = 1;
    private float _soundVolume = 1;

    private Dictionary<string, AudioClip> _cacheSound = new Dictionary<string, AudioClip>();

    public void DoPlayMusic(string music)
    {
    }

    public void DoPlaySound(string sound)
    {
    }

    public static void PlayMusic(string path)
    {

    }

    public static void PlaySound(string path)
    {
        
    }
}
