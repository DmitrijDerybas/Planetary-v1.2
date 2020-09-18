using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Plays sounds on free audio sources.
public class AudioController : Controller
{
    [Header("Sound")]
    public AudioClip[] sfx;
    public AudioSource[] sfxSources;

    public override void Init(GlobalController controller)
    {
        base.Init(controller);

        isInitialized = true;
    }

    //Plays specified (soundName) sound.
    public void PlaySound(string soundName, bool loop = false)
    {
        for (int i = 0; i < sfxSources.Length; i++)
        {
            if (!sfxSources[i].isPlaying)
            {
                sfxSources[i].loop = loop;
                sfxSources[i].clip = GetSound(soundName);
                sfxSources[i].Play();
                return;
            }
        }
    }

    //Stops specified sound.
    public bool StopSound(string sound)
    {
        for (int i = 0; i < sfxSources.Length; i++)
        {
            if (sfxSources[i].clip != null && sfxSources[i].isPlaying && sfxSources[i].clip.name == sound)
            {
                sfxSources[i].Stop();
                sfxSources[i].loop = false;
                sfxSources[i].clip = null;
                return true;
            }
        }

        return false;
    }

    //Returns an audio clip from a sound array by name.
    public AudioClip GetSound(string soundName)
    {
        for (int i = 0; i < sfx.Length; i++)
        {
            if (sfx[i].name == soundName)
                return sfx[i];
        }

        return null;
    }

}
