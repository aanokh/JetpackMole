using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Created by Alexander Anokhin

public class AudioPlayer : MonoBehaviour {

    // Config

    // Cache
    private AudioSource audioSource;

    public void Awake() {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayClip(AudioClip c) {
        audioSource.clip = c;
        audioSource.Play();
        Destroy(this, 10);
    }

    public void PlayClip(AudioClip c, float pitch, float volume) {
        audioSource.clip = c;
        audioSource.pitch = pitch;
        audioSource.volume = volume;
        audioSource.Play();
        Destroy(this, 10);
    }
}
