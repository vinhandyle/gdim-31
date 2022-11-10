using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


/// <summary>
/// Defines all audio-related actions.
/// </summary>
public class AudioController : Singleton<AudioController>
{
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private List<AudioClip> playlist;
    [SerializeField] private List<AudioClip> sfxList;

    /// <summary>
    /// Change the volume of a specific audio channel.
    /// </summary>
    public void ChangeVolume(string channel, float volume)
    {
        mixer.SetFloat(channel, Mathf.Log10(volume + 0.0001f) * 20);
    }

    /// <summary>
    /// Get the volume of a specific audio channel.
    /// </summary>
    public float GetVolume(string channel)
    {
        float volume;
        mixer.GetFloat(channel, out volume);
        return Mathf.Pow(10, volume / 20);
    }

    /// <summary>
    /// Play the specificed track from the playlist. Loop by default.
    /// </summary>
    public void PlayTrack(int trackNum, bool loop = true)
    {
        musicSource.clip = playlist[trackNum];
        musicSource.loop = loop;
        musicSource.Stop();
        musicSource.Play();
    }

    /// <summary>
    /// Play the specified sfx from the list. Don't loop by default.
    /// </summary>
    public void PlayEffect(int effectNum, bool loop = false)
    {
        sfxSource.clip = sfxList[effectNum];
        sfxSource.loop = loop;
        if (!sfxSource.isPlaying)
        {
            sfxSource.Stop();
            sfxSource.Play();
        }
    }
}