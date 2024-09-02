using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// Created by Alexander Anokhin

public class TilemapManager : MonoBehaviour {

    // Config
    public static TilemapManager main;
    public TileBase borderTile;

    public int startingGeneratedDepth = 10;
    public int depthGenerationBuffer = 5;
    public int levelWidth = 14;

    public TileBase shop1Tile;
    public TileBase shop2Tile;
    public TileBase shop3Tile;

    public int caveChance = 1;
    public int cavePropogationChance = 95;

    public int oreDecay = 50;
    public int caveDecay = 85;

    public int genInterval = 100;

    public GameObject borderTilemapPrefab;
    public List<BlockProperty> tileTypes;
    public BlockProperty stoneBlock;

    // y (depth), x, bool: is spawner
    private List<List<Tuple<BlockProperty, bool>>> tileArray;


    // Cache
    private Dictionary<BlockProperty, Tilemap> borderTilemaps = new Dictionary<BlockProperty, Tilemap>(); // smaller order painted above (ex: 1 above 2)
    private Tilemap tilemap;
    private int lastGeneratedDepth;
    private Player player;

    public void Awake() {
        main = this;
    }

    public void Start() {
        tilemap = GetComponent<Tilemap>();
        player = FindObjectOfType<Player>();

        // generate border tilemaps (for each block type if they have border/overlay)
        for (int i = 0; i < tileTypes.Count; i++) {
            tileTypes[i].layerOrder = i + 1;
            if (tileTypes[i].overlayTile != null) {
                borderTilemaps.Add(tileTypes[i], Instantiate(borderTilemapPrefab, transform).GetComponent<Tilemap>());
                borderTilemaps[tileTypes[i]].GetComponent<TilemapRenderer>().sortingOrder = tileTypes[i].layerOrder * -1;
            }
        }

        tileArray = new List<List<Tuple<BlockProperty, bool>>>();
        lastGeneratedDepth = 0;
        GenerateVeins();
        lastGeneratedDepth = startingGeneratedDepth - 1;
    }

    public void Update() {
        if (Mathf.Abs(((int)player.transform.position.y) - (-lastGeneratedDepth)) < depthGenerationBuffer) {
            while (Mathf.Abs(((int)player.transform.position.y) - (-lastGeneratedDepth)) < depthGenerationBuffer) {
                GenerateRow();
            }
        }
    }

    private void GenerateRow() {
        lastGeneratedDepth++;

        if (lastGeneratedDepth % genInterval == 0) {
            GenerateVeins();
        }

        Vector3Int pos;

        int y = -lastGeneratedDepth;
        for (int x = 0; x < levelWidth; x++) {
            pos = new Vector3Int(x, y, 0);
            InitializeTile(x, y);
        }

        Vector3Int leftBorderPos = new Vector3Int(-1, y, 0);
        Vector3Int rightBorderPos = new Vector3Int(levelWidth, y, 0);

        // borders
        foreach (var pair in borderTilemaps) {
            pair.Value.SetTile(leftBorderPos, borderTile);
            pair.Value.RefreshTile(leftBorderPos);
            pair.Value.SetTile(rightBorderPos, borderTile);
            pair.Value.RefreshTile(rightBorderPos);
        }
    }

    private void InitializeTile(int x, int y) {
        Vector3Int pos = new Vector3Int(x, y, 0);

        BlockProperty block = tileArray[-y][x].Item1;
        int layerOrder;

        if (block == null) { // air
            layerOrder = int.MaxValue; // send to bottom
        } else {
            tilemap.SetTile(pos, block.tile);
            tilemap.RefreshTile(pos);
            layerOrder = block.layerOrder;

            if (borderTilemaps.ContainsKey(block)) {
                borderTilemaps[block].SetTile(pos, block.overlayTile);
            }
        }

        foreach (var pair in borderTilemaps) {
            // smaller order painted above (ex: 1 above 2)
            if (pair.Key.layerOrder < layerOrder) {
                pair.Value.SetTile(pos, borderTile);
                pair.Value.RefreshTile(pos);
            }
        }
    }

    private void GenerateVeins() {
        // generate air
        for (int y = lastGeneratedDepth; y < (genInterval + lastGeneratedDepth); y++) {
            
            // fill with stone
            if (tileArray.Count < y + 1) {
                tileArray.Add(new List<Tuple<BlockProperty, bool>>());
                for (int x = 0; x < levelWidth; x++) {
                    tileArray[y].Add(Tuple.Create(stoneBlock, false));
                }
            }

            // does the row have cave?
            if (UnityEngine.Random.value <= caveChance / 100f) {
                tileArray[y][UnityEngine.Random.Range(0, levelWidth - 1)] = Tuple.Create<BlockProperty, bool>(null, true);
            }
        }

        // fill out air
        for (int y = lastGeneratedDepth; y < (genInterval + lastGeneratedDepth); y++) {
            for (int x = 0; x < levelWidth; x++) {
                if (tileArray[y][x].Item2 == true) {
                    FillOutVein(x, y, null, cavePropogationChance / 100f, caveDecay / 100f, 0, true);
                }
            }
        }

        // generate veins
        for (int y = lastGeneratedDepth; y < (genInterval + lastGeneratedDepth); y++) {

            // if want to not generate in air, change this
            List<int> availableSpots = new List<int>();
            for (int i = 0; i < levelWidth; i++) {
                availableSpots.Add(i);
            }

            // does the row have a vein? create spawners for every tile type (keep track of empty spots to not overlap) (stone has 0 chance)
            foreach (BlockProperty block in tileTypes) {
                if (UnityEngine.Random.value <= block.veinChance / 100f) {
                    if (availableSpots.Count > 0) {
                        int spotIndex = UnityEngine.Random.Range(0, availableSpots.Count - 1);
                        int x = availableSpots[spotIndex];
                        availableSpots.RemoveAt(spotIndex);
                        tileArray[y][x] = Tuple.Create(block, true);
                    }
                }
            }
        }

        // fill out veins (priority goes left to right right now)
        for (int y = lastGeneratedDepth; y < (genInterval + lastGeneratedDepth); y++) {
            for (int x = 0; x < levelWidth; x++) {
                if (tileArray[y][x].Item2 == true) {
                    BlockProperty block = tileArray[y][x].Item1;
                    FillOutVein(x, y, block, block.propogationChance / 100f, oreDecay / 100f, 0, block.horizontalPropogation);
                }
            }
        }

        // generate shop
        for (int y = lastGeneratedDepth + (genInterval - 5); y < (genInterval + lastGeneratedDepth); y++) {
            for (int x = 0; x < levelWidth; x++) {
                tileArray[y][x] = Tuple.Create<BlockProperty, bool>(null, false);
            }
        }

        //tileArray[lastGeneratedDepth + (genInterval - 2)][2] = "SH1";
        //tileArray[lastGeneratedDepth + (genInterval - 2)][5] = "SH2";
        //tileArray[lastGeneratedDepth + (genInterval - 2)][8] = "SH3";

        for (int x = 0; x < levelWidth; x++) {
            tileArray[lastGeneratedDepth + (genInterval - 1)][x] = Tuple.Create(stoneBlock, false);
        }
    }

    private void FillOutVein(int x, int y, BlockProperty block, float propogation, float decay, int iteration, bool horizontal) {
        if (propogation <= 0) {
            return;
        }
        float propChance = propogation * decay;
        int sidesGenerated = 0;
        float verticalCoefficient = 1f;
        if (horizontal) {
            verticalCoefficient = 0.5f;
        }
        // above
        if (y - 1 >= 0) {
            if (UnityEngine.Random.value <= (propogation * verticalCoefficient)) {
                tileArray[y - 1][x] = Tuple.Create(block, false);
                FillOutVein(x, y - 1, block, propChance, decay, iteration + 1, horizontal);
                sidesGenerated++;
            }
        }
        // below
        if (y + 1 < tileArray.Count) {
            if (UnityEngine.Random.value <= (propogation * verticalCoefficient)) {
                tileArray[y + 1][x] = Tuple.Create(block, false);
                FillOutVein(x, y + 1, block, propChance, decay, iteration + 1, horizontal);
                sidesGenerated++;
            }
        }
        // left
        if (x - 1 >= 0) {
            if (UnityEngine.Random.value <= propogation) {
                tileArray[y][x - 1] = Tuple.Create(block, false);
                FillOutVein(x - 1, y, block, propChance, decay, iteration + 1, horizontal);
                sidesGenerated++;
            }
        }
        // right
        if (x + 1 < levelWidth) {
            if (UnityEngine.Random.value <= propogation) {
                tileArray[y][x + 1] = Tuple.Create(block, false);
                FillOutVein(x + 1, y, block, propChance, decay, iteration + 1, horizontal);
                sidesGenerated++;
            }
        }

        if (iteration == 0 && sidesGenerated < 2) {
            // prevent single vein on first iteration
            FillOutVein(x, y, block, propogation, decay, 0, horizontal);
        }
    }

    public void ChangeTile(Vector3 pos, TileBase tile) {
        tilemap.SetTile(tilemap.WorldToCell(pos), tile);
        tilemap.RefreshTile(tilemap.WorldToCell(pos));
    }

    // for mining
    public void BreakTile(Vector3 pos) {

        Vector3Int posInt = tilemap.WorldToCell(pos);

        foreach (var pair in borderTilemaps) {
            pair.Value.SetTile(posInt, borderTile);
            pair.Value.RefreshTile(posInt);
        }

        tilemap.SetTile(tilemap.WorldToCell(posInt), null);
        tilemap.RefreshTile(tilemap.WorldToCell(posInt));
    }
}
