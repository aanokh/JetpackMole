using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// Created by Alexander Anokhin

public class TilemapManager : MonoBehaviour {

    // Config
    public static TilemapManager main;
    public Tilemap goldBorderTilemap;
    public Tilemap eleniteBorderTilemap;
    public Tilemap stoneBorderTilemap;
    public Tilemap obsidianBorderTilemap;
    public TileBase borderTile;

    public int startingGeneratedDepth = 10;
    public int depthGenerationBuffer = 5;
    public int levelWidth = 14;

    public TileBase stoneTile;

    public TileBase goldTile;
    public TileBase goldOverlayTile;
    public int goldVeinChance = 10;
    public int goldPropogationChance = 40;

    public TileBase eleniteTile;
    public TileBase eleniteOverlayTile;
    public int eleniteVeinChance = 7;
    public int elenitePropogationChance = 70;

    public TileBase diamondTile;
    public TileBase diamondOverlayTile;
    public int diamondVeinChance = 5;
    public int diamondPropogationChance = 20;

    public TileBase obsidianTile;
    public TileBase obsidianOverlayTile;
    public int obsidianVeinChance = 5;
    public int obsidianPropogationChance = 20;

    public TileBase shop1Tile;
    public TileBase shop2Tile;
    public TileBase shop3Tile;

    public int caveChance = 1;
    public int cavePropogationChance = 95;

    public int oreDecay = 50;
    public int caveDecay = 85;

    public int genInterval = 100;

    // y (depth), x
    public List<List<string>> tileArray;
    

    // Cache
    private Tilemap tilemap;
    private int lastGeneratedDepth;
    private Player player;

    public void Awake() {
        main = this;
    }

    public void Start() {
        tilemap = GetComponent<Tilemap>();
        player = FindObjectOfType<Player>();
        tileArray = new List<List<string>>();
        lastGeneratedDepth = 0;
        GenerateVeins();
        /*
        for (int y = 0; y < tileArray.Count; y++) {
            string s = "";
            for (int x = 0; x < tileArray[y].Count; x++) {
                s += tileArray[y][x];
                s += " ";
            }
            Debug.Log(s);
        }*/
        lastGeneratedDepth = startingGeneratedDepth - 1;
    }

    public void Update() {
        //Debug.Log(Mathf.Abs(((int)player.transform.position.y) - (-lastGeneratedDepth)));

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

        // borders
        Vector3Int leftBorderPos = new Vector3Int(-1, y, 0);
        stoneBorderTilemap.SetTile(leftBorderPos, borderTile);
        eleniteBorderTilemap.SetTile(leftBorderPos, borderTile);
        goldBorderTilemap.SetTile(leftBorderPos, borderTile);
        obsidianBorderTilemap.SetTile(leftBorderPos, borderTile);
        tilemap.RefreshTile(leftBorderPos);
        stoneBorderTilemap.RefreshTile(leftBorderPos);
        eleniteBorderTilemap.RefreshTile(leftBorderPos);
        goldBorderTilemap.RefreshTile(leftBorderPos);
        obsidianBorderTilemap.RefreshTile(leftBorderPos);

        Vector3Int rightBorderPos = new Vector3Int(levelWidth, y, 0);
        stoneBorderTilemap.SetTile(rightBorderPos, borderTile);
        eleniteBorderTilemap.SetTile(rightBorderPos, borderTile);
        goldBorderTilemap.SetTile(rightBorderPos, borderTile);
        obsidianBorderTilemap.SetTile(rightBorderPos, borderTile);
        tilemap.RefreshTile(rightBorderPos);
        stoneBorderTilemap.RefreshTile(rightBorderPos);
        eleniteBorderTilemap.RefreshTile(rightBorderPos);
        goldBorderTilemap.RefreshTile(rightBorderPos);
        obsidianBorderTilemap.RefreshTile(rightBorderPos);
    }

    private void InitializeTile(int x, int y) {
        Vector3Int pos = new Vector3Int(x, y, 0);
        string s = tileArray[-y][x];
        if (s == "SH1" || s == "SH2" || s == "SH3") {
            if (s == "SH1") {
                tilemap.SetTile(pos, shop1Tile);
            } else if (s == "SH2") {
                tilemap.SetTile(pos, shop2Tile);
            } else if (s == "SH3") {
                tilemap.SetTile(pos, shop3Tile);
            }
        } else if (s == "VD" || s == "D") {
            tilemap.SetTile(pos, diamondTile);
            stoneBorderTilemap.SetTile(pos, diamondOverlayTile);
            // gold and elenite overlap
            goldBorderTilemap.SetTile(pos, borderTile);
            eleniteBorderTilemap.SetTile(pos, borderTile);
            tilemap.RefreshTile(pos);
            stoneBorderTilemap.RefreshTile(pos);
            goldBorderTilemap.RefreshTile(pos);
            eleniteBorderTilemap.RefreshTile(pos);
        } else if (s == "VE" || s == "E") {
            tilemap.SetTile(pos, eleniteTile);
            eleniteBorderTilemap.SetTile(pos, eleniteOverlayTile);
            // gold overlaps
            goldBorderTilemap.SetTile(pos, borderTile);
            tilemap.RefreshTile(pos);
            eleniteBorderTilemap.RefreshTile(pos);
            goldBorderTilemap.RefreshTile(pos);
        } else if (s == "VG" || s == "G") {
            tilemap.SetTile(pos, goldTile);
            goldBorderTilemap.SetTile(pos, goldOverlayTile);
            // no overlaps
            tilemap.RefreshTile(pos);
            goldBorderTilemap.RefreshTile(pos);
        } else if (s == "VO" || s == "O") {
            tilemap.SetTile(pos, obsidianTile);
            obsidianBorderTilemap.SetTile(pos, obsidianOverlayTile);
            // gold elenite and diamond overlap
            goldBorderTilemap.SetTile(pos, borderTile);
            eleniteBorderTilemap.SetTile(pos, borderTile);
            stoneBorderTilemap.SetTile(pos, borderTile);
            tilemap.RefreshTile(pos);
            obsidianBorderTilemap.RefreshTile(pos);
            goldBorderTilemap.RefreshTile(pos);
            eleniteBorderTilemap.RefreshTile(pos);
            stoneBorderTilemap.RefreshTile(pos);
        } else if (s == "VA" || s == "A") {
            // all overlap
            stoneBorderTilemap.SetTile(pos, borderTile);
            eleniteBorderTilemap.SetTile(pos, borderTile);
            goldBorderTilemap.SetTile(pos, borderTile);
            obsidianBorderTilemap.SetTile(pos, borderTile);
            stoneBorderTilemap.RefreshTile(pos);
            eleniteBorderTilemap.RefreshTile(pos);
            goldBorderTilemap.RefreshTile(pos);
            obsidianBorderTilemap.RefreshTile(pos);
        } else {
            tilemap.SetTile(pos, stoneTile);
            // all overlap
            stoneBorderTilemap.SetTile(pos, borderTile);
            eleniteBorderTilemap.SetTile(pos, borderTile);
            goldBorderTilemap.SetTile(pos, borderTile);
            obsidianBorderTilemap.SetTile(pos, borderTile);
            tilemap.RefreshTile(pos);
            stoneBorderTilemap.RefreshTile(pos);
            eleniteBorderTilemap.RefreshTile(pos);
            goldBorderTilemap.RefreshTile(pos);
            obsidianBorderTilemap.RefreshTile(pos);
        }
    }

    private void GenerateVeins() {
        // generate air
        for (int y = lastGeneratedDepth; y < (genInterval + lastGeneratedDepth); y++) {
            if (tileArray.Count < y + 1) {
                tileArray.Add(new List<string>());
                for (int x = 0; x < levelWidth; x++) {
                    tileArray[y].Add("S");
                }
            }

            // does the row have cave?
            if (Random.value <= caveChance / 100f) {
                tileArray[y][Random.Range(0, levelWidth - 1)] = "VA";
            }
        }

        // fill out air
        for (int y = lastGeneratedDepth; y < (genInterval + lastGeneratedDepth); y++) {
            for (int x = 0; x < levelWidth; x++) {
                if (tileArray[y][x] == "VA") {
                    FillOutVein(x, y, "A", cavePropogationChance / 100f, caveDecay / 100f, 0, true);
                }
            }
        }

        // generate veins
        for (int y = lastGeneratedDepth; y < (genInterval + lastGeneratedDepth); y++) {
            // dont generate in air somehow?

            // does the row have a vein?
            if (Random.value <= diamondVeinChance / 100f) {
                // diamonds lowest priority
                tileArray[y][Random.Range(0, levelWidth - 1)] = "VD";
            }
            if (Random.value <= eleniteVeinChance / 100f) {
                // elenite lower priority
                tileArray[y][Random.Range(0, levelWidth - 1)] = "VE";
            }
            if (Random.value <= goldVeinChance / 100f) {
                // gold hgiher priority
                tileArray[y][Random.Range(0, levelWidth - 1)] = "VG";
            }
            if (Random.value <= obsidianVeinChance / 100f) {
                // obsidian highest priority
                tileArray[y][Random.Range(0, levelWidth - 1)] = "VO";
            }
        }

        // fill out veins
        for (int y = lastGeneratedDepth; y < (genInterval + lastGeneratedDepth); y++) {
            for (int x = 0; x < levelWidth; x++) {
                if (tileArray[y][x] == "VD") {
                    FillOutVein(x, y, "D", diamondPropogationChance / 100f, oreDecay / 100f, 0, false);
                } else if (tileArray[y][x] == "VE") {
                    FillOutVein(x, y, "E", elenitePropogationChance / 100f, oreDecay / 100f, 0, false);
                } else if (tileArray[y][x] == "VG") {
                    FillOutVein(x, y, "G", goldPropogationChance / 100f, oreDecay / 100f, 0, false);
                } else if (tileArray[y][x] == "VO") {
                    FillOutVein(x, y, "O", obsidianPropogationChance / 100f, oreDecay / 100f, 0, true);
                }
            }
        }

        // generate shop
        for (int y = lastGeneratedDepth + (genInterval - 5); y < (genInterval + lastGeneratedDepth); y++) {
            for (int x = 0; x < levelWidth; x++) {
                tileArray[y][x] = "A";
            }
        }

        tileArray[lastGeneratedDepth + (genInterval - 2)][2] = "SH1";
        tileArray[lastGeneratedDepth + (genInterval - 2)][5] = "SH2";
        tileArray[lastGeneratedDepth + (genInterval - 2)][8] = "SH3";

        for (int x = 0; x < levelWidth; x++) {
            tileArray[lastGeneratedDepth + (genInterval - 1)][x] = "S";
        }
    }

    private void FillOutVein(int x, int y, string s, float p, float d, int i, bool horizontal) {
        float dp = d * p;
        int sidesGenerated = 0;
        float verticalCoefficient = 1f;
        if (horizontal) {
            verticalCoefficient = 0.5f;
        }
        // above
        if (y - 1 >= 0) {
            if (Random.value <= (p * verticalCoefficient)) {
                tileArray[y - 1][x] = s;
                FillOutVein(x, y - 1, s, dp, d, i+1, horizontal);
                sidesGenerated++;
            }
        }
        // below
        if (y + 1 < tileArray.Count) {
            if (Random.value <= (p * verticalCoefficient)) {
                tileArray[y + 1][x] = s;
                FillOutVein(x, y + 1, s, dp, d, i+1, horizontal);
                sidesGenerated++;
            }
        }
        // left
        if (x - 1 >= 0) {
            if (Random.value <= p) {
                tileArray[y][x - 1] = s;
                FillOutVein(x - 1, y, s, dp, d, i+1, horizontal);
                sidesGenerated++;
            }
        }
        // right
        if (x + 1 < levelWidth) {
            if (Random.value <= p) {
                tileArray[y][x + 1] = s;
                FillOutVein(x + 1, y, s, dp, d, i+1, horizontal);
                sidesGenerated++;
            }
        }

        if (i == 0 && sidesGenerated < 2) {
            // prevent single vein on first iteration
            FillOutVein(x, y, s, p, d, 0, horizontal);
        }
    }

    public void ChangeTile(Vector3 pos, TileBase tile) {
        tilemap.SetTile(tilemap.WorldToCell(pos), tile);
        tilemap.RefreshTile(tilemap.WorldToCell(pos));
    }

    public void UpdateOverlayTile(Vector3 pos, BorderType type) {

        Vector3Int posInt = tilemap.WorldToCell(pos);

        if (type == BorderType.ObsidianBorder) {
            // no overlaps
            obsidianBorderTilemap.SetTile(posInt, borderTile);
            obsidianBorderTilemap.RefreshTile(posInt);
        } else if (type == BorderType.StoneBorder) {
            // gold overlaps
            obsidianBorderTilemap.SetTile(posInt, borderTile);
            stoneBorderTilemap.SetTile(posInt, borderTile);

            obsidianBorderTilemap.RefreshTile(posInt);
            stoneBorderTilemap.RefreshTile(posInt);
        } else if (type == BorderType.EleniteBorder) {
            // gold and elenite overlap
            obsidianBorderTilemap.SetTile(posInt, borderTile);
            stoneBorderTilemap.SetTile(posInt, borderTile);
            eleniteBorderTilemap.SetTile(posInt, borderTile);

            obsidianBorderTilemap.RefreshTile(posInt);
            stoneBorderTilemap.RefreshTile(posInt);
            eleniteBorderTilemap.RefreshTile(posInt);
        } else if (type == BorderType.GoldBorder) {
            // gold elenite and diamond overlap
            obsidianBorderTilemap.SetTile(posInt, borderTile);
            stoneBorderTilemap.SetTile(posInt, borderTile);
            eleniteBorderTilemap.SetTile(posInt, borderTile);
            goldBorderTilemap.SetTile(posInt, borderTile);

            obsidianBorderTilemap.RefreshTile(posInt);
            stoneBorderTilemap.RefreshTile(posInt);
            eleniteBorderTilemap.RefreshTile(posInt);
            goldBorderTilemap.RefreshTile(posInt);
        }
    }
}
