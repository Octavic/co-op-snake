using System.Collections;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    private AudioSource Audio;

    // Use this for initialization
    void Start()
    {
        this.Audio = this.GetComponent<AudioSource>();
    }

    public void Play()
    {
        this.Audio.Play();
    }

    public void Stop()
    {
        this.Audio.Stop();
    }

    public void Play(AudioClip clip)
    {
        this.Audio.clip = clip;
        this.Play();
    }
}
