using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// Created by Alexander Anokhin

[CreateAssetMenu(fileName = "New BlockProperty", menuName = "BlockProperty")]
public class BlockProperty : ScriptableObject {

    [Header("Block Settings")]
    public int stageHealth = 100;
    public int breakScore = 10;
    public int scoreMultiplier = 0;
    public int gold = 0;
    public int miningMultiplier = 0;
    public int powerFill = 0;
    public bool comboAffectsScore = false; // for emeralds, makes combo affect score
    public bool hasCombo = true;

    [Header("Sprite Settings")]
    public TileBase tile;
    public TileBase overlayTile; // if null, no border

    [Header("Generation Settings")]
    public int veinChance = 10;
    public int propogationChance = 40;
    public bool horizontalPropogation = false;

    [HideInInspector] public int layerOrder; // used internally in tile generator
}
