using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileTest : MonoBehaviour
{
    void Start()
    {
        Tilemap tilemap = GetComponent<Tilemap>();

        BoundsInt bounds = tilemap.cellBounds;
        TileBase[] allTiles = tilemap.GetTilesBlock(bounds);

        for (int x = 0; x < bounds.size.x; x++)
        {
            for (int z = 0; z < bounds.size.y; z++)
            {
                TileBase tile = allTiles[x + z * bounds.size.x];
                if (tile != null)
                {
                    Debug.Log("x:" + x + " z:" + z + " tile:" + tile.name);
                }
                else
                {
                    Debug.Log("x:" + x + " z:" + z + " tile: (null)");
                }
            }
        }
    }
}
