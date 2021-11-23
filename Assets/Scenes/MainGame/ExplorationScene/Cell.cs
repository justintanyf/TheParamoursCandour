using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class Cell : ScriptableObject
{
    public TileBase groundTile;
    public TileBase objectTile;
}
