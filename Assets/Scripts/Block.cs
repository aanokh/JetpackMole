using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// Created by Alexander Anokhin

public class Block : MonoBehaviour {
    
    // Config
    public TileBase nextStageTile;
    public BlockProperty blockProperty;

    // Cache
    private float health;
    private Player player;

    public void Start() {
        health = blockProperty.stageHealth;
        player = FindObjectOfType<Player>();
    }

    public void TakeDamage(float dmg) {
        health -= dmg;
        if (health <= 0) {
            Die();
        }
    }

    private void Die() {
        if (nextStageTile != null) {
            TilemapManager.main.ChangeTile(transform.position, nextStageTile);
        } else {

            // death effects


            player.AddScore(blockProperty.breakScore, blockProperty.comboAffectsScore);
            player.AddMining(blockProperty.miningMultiplier);
            player.AddGold(blockProperty.gold);
            player.AddMult(blockProperty.scoreMultiplier);

            player.AddCombo(blockProperty);

            player.audioManager.playSound(player.audioManager.breakSound);

            TilemapManager.main.BreakTile(transform.position);
        }
    }
}
