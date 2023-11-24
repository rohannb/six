using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class colorTile : MonoBehaviour
{
    public Tilemap tilemap;
    public Tile red, black;
    private Tile tile;
    void Start()
    {
        tile = red;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)){
            Vector3Int pos = getMousePosition();
            Debug.Log(pos);
            bool isEmpty = checkIfTileEmpty(pos);
            if (isEmpty) {
                tilemap.SetTile(pos, tile);
                checkForVictory(pos, tile);
                // switchPlayer();
            }
        }
        if (Input.GetMouseButtonDown(1)){
            Vector3Int pos = getMousePosition();
            tilemap.SetTile(pos, null);
        }
    }

    Vector3Int getMousePosition(){
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int pos = tilemap.WorldToCell(mouseWorldPos);
        return new Vector3Int(pos.x, pos.y, 0);
    }

    void switchPlayer(){
        if(tile==red) tile = black;
        else tile = red;
    }

    bool checkIfTileEmpty(Vector3Int pos) {
        TileBase currentTile = tilemap.GetTile(pos);
        if (currentTile) return false;
        else return true;
    }

    void checkForVictory(Vector3Int pos, Tile tile){
        checkForLineVictory(pos, tile);
    }

    void checkForLineVictory(Vector3Int pos, Tile tile){
        checkLine1(pos, tile);
        checkLine2(pos, tile);
        checkLine3(pos, tile);
    }

    void checkLine1(Vector3Int pos, Tile tile){
        Vector3Int start = pos, previousTile = new Vector3Int(pos.x-1, pos.y, 0);
        TileBase previousTileType = tilemap.GetTile(previousTile);
        while(previousTileType == tile) {
            start = previousTile;
            previousTile = new Vector3Int(previousTile.x-1, previousTile.y, 0);
            previousTileType = tilemap.GetTile(previousTile);
        }
        Vector3Int[] tileSequence = new [] { start, new Vector3Int(start.x+1,start.y,0), new Vector3Int(start.x+2,start.y,0), new Vector3Int(start.x+3,start.y,0), new Vector3Int(start.x+4,start.y,0), new Vector3Int(start.x+5,start.y,0)};
        if(checkTiles(tileSequence, tile)) {
            Debug.Log(tile + " wins!");
            UnityEditor.EditorApplication.isPlaying = false;
        }
    }

    void checkLine2(Vector3Int pos, Tile tile){
        Vector3Int start = pos, previousTile;
        if (pos.y%2==0) previousTile = new Vector3Int(pos.x-1, pos.y+1, 0);
        else previousTile = new Vector3Int(pos.x, pos.y+1, 0);
        Vector3Int[] tileSequence;
        TileBase previousTileType = tilemap.GetTile(previousTile);
        while(previousTileType == tile) {
            start = previousTile;
            if (pos.y%2==0) previousTile = new Vector3Int(previousTile.x-1, previousTile.y+1, 0);
            else previousTile = new Vector3Int(previousTile.x, previousTile.y+1, 0);
            previousTileType = tilemap.GetTile(previousTile);
            Debug.Log("previousTileType: " + previousTileType);
        }
        Debug.Log("previousTile: "+ previousTile);
        Debug.Log("start: "+start);
        if (start.y%2==0) tileSequence = new [] { start, new Vector3Int(start.x,start.y-1,0), new Vector3Int(start.x+1,start.y-2,0), new Vector3Int(start.x+1,start.y-3,0), new Vector3Int(start.x+2,start.y-4,0), new Vector3Int(start.x+2,start.y-5,0)};
        else tileSequence = new [] { start, new Vector3Int(start.x+1,start.y-1,0), new Vector3Int(start.x+1,start.y-2,0), new Vector3Int(start.x+2,start.y-3,0), new Vector3Int(start.x+2,start.y-4,0), new Vector3Int(start.x+3,start.y-5,0)};
        Debug.Log("tileSequence: "+ String.Join(", ", tileSequence));
        if(checkTiles(tileSequence, tile)) {
            Debug.Log(tile + " wins!");
            UnityEditor.EditorApplication.isPlaying = false;
        }
    }

    void checkLine3(Vector3Int pos, Tile tile){

    }

    bool checkTiles(Vector3Int[] sequence, Tile tile){
        return Array.TrueForAll(sequence, element=>tilemap.GetTile(element)==tile);
    }
}
