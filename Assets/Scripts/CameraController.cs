using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Created by Alexander Anokhin

public class CameraController : MonoBehaviour {

    // Config
    public float yDiff = 4;

    // Cache
    private Player player;

    public void Start() {
        player = FindObjectOfType<Player>();
    }

    public void Update() {
        float diff = Mathf.Abs(player.transform.position.y - transform.position.y);
        if (diff > yDiff) {
            if (player.transform.position.y > transform.position.y) {
                transform.position = new Vector3(transform.position.x, transform.position.y + (diff - yDiff), transform.position.z);
            } else if (player.transform.position.y < transform.position.y) {
                transform.position = new Vector3(transform.position.x, transform.position.y - (diff - yDiff), transform.position.z);
            }
        }
    }
}
