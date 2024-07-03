using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Created by Alexander Anokhin

public class Firewall : MonoBehaviour {

    // Config
    public float speed = 1;
    public BoxCollider2D triggerCollider;
    public BoxCollider2D physicsCollider;

    // Cache
    private int speedMultiplier = 100;
    private Rigidbody2D rbody;
    private Player player;

    public void Start() {
        rbody = GetComponent<Rigidbody2D>();
        player = FindObjectOfType<Player>();
    }

    public void Update() {
        if (player.transform.position.y < 0) {
            speedMultiplier = 100 + (int)Mathf.Floor(((int) Mathf.Abs(player.transform.position.y) * 0.5f));
        } else {
            speedMultiplier = 100;
        }
        Vector2 vel = new Vector2(0f, -(speed * (speedMultiplier / 100f)));
        rbody.velocity = vel;

        if (triggerCollider.IsTouching(player.boxCollider)) {
            player.TakeDamage();
        }

        if (player.transform.position.y > (transform.position.y + 10)) {
            player.Die();
        }
    }
}
