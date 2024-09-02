using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class BorderOverlayTile : RuleTile<BorderOverlayTile.Neighbor> {
    public List<TileBase> stone = new List<TileBase>();
    public List<TileBase> gold = new List<TileBase>();
    public List<TileBase> elenite = new List<TileBase>();
    public List<TileBase> obsidian = new List<TileBase>();
    public List<TileBase> emerald = new List<TileBase>();
    public class Neighbor : RuleTile.TilingRule.Neighbor {
        public const int Stone = 3;
        public const int Gold = 4;
        public const int Elenite = 5;
        public const int Obsidian = 6;
        public const int Emerald = 7;
    }
    public override bool RuleMatch(int neighbor, TileBase tile) {
        switch (neighbor) {
            case Neighbor.Stone: return stone.Contains(tile);
            case Neighbor.Gold: return gold.Contains(tile);
            case Neighbor.Elenite: return elenite.Contains(tile);
            case Neighbor.Obsidian: return obsidian.Contains(tile);
            case Neighbor.Emerald: return emerald.Contains(tile);
        }
        return base.RuleMatch(neighbor, tile);
    }
}