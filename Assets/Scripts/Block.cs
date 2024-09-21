using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// Created by Alexander Anokhin

public class Block : MonoBehaviour {
    
    // Config
    public TileBase nextStageTile;
    public BlockProperty blockProperty;
    public GameObject primaryBreakEffectPrefab;
    public GameObject secondaryBreakEffectPrefab;

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
            ParticleSystem primaryBreakEffect = Instantiate(primaryBreakEffectPrefab, transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
            ParticleSystem secondaryBreakEffect = Instantiate(secondaryBreakEffectPrefab, transform.position, Quaternion.identity).GetComponent<ParticleSystem>();

            ParticleSystem.MainModule primaryMain = primaryBreakEffect.main;
            primaryMain.startColor = blockProperty.primaryParticleColor;
            ParticleSystem.MainModule secondaryMain = secondaryBreakEffect.main;
            secondaryMain.startColor = blockProperty.secondaryParticleColor;

            primaryBreakEffect.Play();
            secondaryBreakEffect.Play();

            Destroy(primaryBreakEffect.gameObject, 10);
            Destroy(secondaryBreakEffect.gameObject, 10);

            player.AddScore(blockProperty.breakScore, blockProperty.comboAffectsScore);
            player.AddMining(blockProperty.miningMultiplier);
            player.AddGold(blockProperty.gold);
            player.AddMult(blockProperty.scoreMultiplier);

            player.AddCombo(blockProperty);

            player.audioManager.playSound(player.audioManager.breakSound);

            TilemapManager.main.BreakTile(transform.position);

            //object deletes itself when tile is removed in tilemap manager
        }
    }
}
