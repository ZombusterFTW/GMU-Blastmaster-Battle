using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class TileManager : MonoBehaviour
{

    private Tilemap tilemap;
    private TileBase[] allTiles;
    private BoundsInt bounds;

    // Start is called before the first frame update
    void OnEnable()
    {
        //Init all of the stuffs
        tilemap = GetComponent<Tilemap>();
        tilemap.CompressBounds();
        bounds = tilemap.cellBounds;
        allTiles = tilemap.GetTilesBlock(bounds);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public Vector3 GetTileCenterFromWorldPosPlayer(Vector3 pos)
    {
        //Take in the players target pos, check if there is a used cell at that pos
        if(tilemap.HasTile(tilemap.WorldToCell(pos)))
        {
            //If there is a used cell at that position, we get the center of that cell and return it.
            Vector3 temp = tilemap.GetCellCenterWorld(tilemap.WorldToCell(pos));
            return new Vector3(temp.x, 0, temp.z);   
        }
        //If there is no used cell we return null
        else return new Vector3();
    }


    public Vector3 GetTileCenterFromWorldPosBomb(Vector3 pos)
    {
        //Get cell center for bomb. doesn't have a check to ensure if there is a null tile.
        return tilemap.GetCellCenterWorld(tilemap.WorldToCell(pos)) + new Vector3(0,3f,0);
    }

    public bool IsTileBlocked(Vector3 pos)
    {
        //Return if there is a tile at that world position. If there isn't a indestructible block is there.
        if (!tilemap.HasTile(tilemap.WorldToCell(pos))) return true;
        else return false;  
    }



}
