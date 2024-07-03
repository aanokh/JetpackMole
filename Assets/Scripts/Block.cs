using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// Created by Alexander Anokhin

public class Block : MonoBehaviour {
    
    // Config
    public int maxStageHealth = 100;
    public int breakScore = 10;
    public int scoreMultiplier = 0;
    public int gold = 0;
    public int miningMultiplier = 0;
    public TileBase nextStageTile;
    public bool hasOverlay;
    public BorderType borderType;
    public BlockType blockType;

    // Cache
    private float health;
    private Player player;

    public void Start() {
        health = maxStageHealth;
        player = FindObjectOfType<Player>();
    }

    public void Update() {

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

            player.AddCombo(blockType);
            float comboMult = player.GetComboMult();

            player.AddScore(breakScore);
            player.AddMining(miningMultiplier);
            player.AddGold(gold);
            player.AddMult(scoreMultiplier);

            player.audioManager.playSound(player.audioManager.breakSound);

            if (hasOverlay) {
               TilemapManager.main.UpdateOverlayTile(transform.position, borderType);
            }
            TilemapManager.main.ChangeTile(transform.position, null);
        }
    }
}
