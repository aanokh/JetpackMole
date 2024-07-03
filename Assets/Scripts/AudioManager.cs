using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Created by Alexander Anokhin

public class AudioManager : MonoBehaviour {

    // Config
    public AudioPlayer audioPlayer;
    public AudioClip startClip;
    public AudioClip digSound;
    public AudioClip breakSound;
    public AudioClip buySound;
    public AudioClip damageSound;
    public AudioClip flySound;
    public AudioClip loseSound;
    public AudioClip noteSound;
    public List<float> notePitch;

    // Cache
    private AudioSource jetpackSource;

    public void Start() {
        jetpackSource = GetComponent<AudioSource>();
        playSound(startClip);
    }

    public void playSound(AudioClip sound) {
        AudioPlayer a = Instantiate(audioPlayer, Camera.main.transform.position, Quaternion.identity, Camera.main.transform);
        if (sound == digSound) {
            a.PlayClip(sound, 1, 0.65f);
        } else {
            a.PlayClip(sound);
        }
    }

    public void playNote(int pitchIndex) {
        pitchIndex = Mathf.Min(pitchIndex, notePitch.Count - 1);
        AudioPlayer a = Instantiate(audioPlayer, Camera.main.transform.position, Quaternion.identity, Camera.main.transform);
        a.PlayClip(noteSound, notePitch[pitchIndex], 1);
    }

    public void startFlySound() {
        jetpackSource.Play();
    }

    public void stopFlySound() {
        jetpackSource.Stop();
    }
}
