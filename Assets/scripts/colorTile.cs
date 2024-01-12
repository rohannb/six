using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using System.Dynamic;

public class colorTile : MonoBehaviour
{
    Tilemap tilemap;
    public Tile red, black;
    private Tile tile;
    private List<Vector3Int> paintedTiles;
    void Start()
    {
        tilemap = GetComponent<Tilemap>();
        tile = red;
        paintedTiles = new List<Vector3Int>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)){
            Vector3Int pos = getMousePosition();
            bool isEmpty = checkIfTileEmpty(pos);
            if (isEmpty) {
                tilemap.SetTile(pos, tile);
                paintedTiles.Add(pos);
                // if (paintedTiles.Count >= 11) // TODO
                checkForVictory(pos, tile); 
                // switchPlayer();
            }
        }
        if (Input.GetMouseButtonDown(1)){
            Vector3Int pos = getMousePosition();
            tilemap.SetTile(pos, null);
        }
        if (Input.GetMouseButtonDown(2)) {
            clearTiles(paintedTiles.ToArray());
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
        checkForCircleVictory(pos, tile);
        checkForTriangleVictory(pos, tile);
    }

    void checkForTriangleVictory(Vector3Int pos, Tile tile) {
        checkTriangleMidVictory(pos, tile);
        checkTriangleEdgeVictory(pos, tile);
    }

    void checkTriangleMidVictory(Vector3Int pos, Tile tile) {
        Vector3Int[] neighbours = getNeighbourTiles(pos);
        if (checkTiles(new[] { neighbours[0], neighbours[1], neighbours[2], neighbours[3] }, tile)) checkTriangleMidVictory1(new[] { neighbours[0], neighbours[1], neighbours[2], neighbours[3] }, tile);
        if (checkTiles(new[] { neighbours[1], neighbours[2], neighbours[3], neighbours[4] }, tile)) checkTriangleMidVictory1(new[] { neighbours[1], neighbours[2], neighbours[3], neighbours[4] }, tile);
        if (checkTiles(new[] { neighbours[2], neighbours[3], neighbours[4], neighbours[5] }, tile)) checkTriangleMidVictory1(new[] { neighbours[2], neighbours[3], neighbours[4], neighbours[5] }, tile);
        if (checkTiles(new[] { neighbours[3], neighbours[4], neighbours[5], neighbours[0] }, tile)) checkTriangleMidVictory1(new[] { neighbours[3], neighbours[4], neighbours[5], neighbours[0] }, tile);
        if (checkTiles(new[] { neighbours[4], neighbours[5], neighbours[0], neighbours[1] }, tile)) checkTriangleMidVictory1(new[] { neighbours[4], neighbours[5], neighbours[0], neighbours[1] }, tile);
        if (checkTiles(new[] { neighbours[5], neighbours[0], neighbours[1], neighbours[2] }, tile)) checkTriangleMidVictory1(new[] { neighbours[5], neighbours[0], neighbours[1], neighbours[2] }, tile);
    }

    void checkTriangleEdgeVictory(Vector3Int pos, Tile tile) {
        Vector3Int[] neighbours = getNeighbourTiles(pos);
        if (checkTiles(new[] { neighbours[0], neighbours[1] }, tile)) checkTriangleMidVictory(neighbours[0], tile);
        if (checkTiles(new[] { neighbours[1], neighbours[2] }, tile)) checkTriangleMidVictory(neighbours[1], tile);
        if (checkTiles(new[] { neighbours[2], neighbours[3] }, tile)) checkTriangleMidVictory(neighbours[2], tile);
        if (checkTiles(new[] { neighbours[3], neighbours[4] }, tile)) checkTriangleMidVictory(neighbours[3], tile);
        if (checkTiles(new[] { neighbours[4], neighbours[5] }, tile)) checkTriangleMidVictory(neighbours[4], tile);
        if (checkTiles(new[] { neighbours[5], neighbours[0] }, tile)) checkTriangleMidVictory(neighbours[5], tile);
    }

    void checkForLineVictory(Vector3Int pos, Tile tile){
        checkLine1(pos, tile);
        checkLine2(pos, tile);
        checkLine3(pos, tile);
    }

    void checkForCircleVictory(Vector3Int pos, Tile tile){
        Vector3Int[] neighbours = getNeighbourTiles(pos);
        if (checkTiles(new []{neighbours[0], neighbours[2]}, tile))  checkCircle1(new []{neighbours[0], pos, neighbours[2]}, tile);
        if (checkTiles(new []{neighbours[1], neighbours[3]}, tile))  checkCircle1(new []{neighbours[1], pos, neighbours[3]}, tile);
        if (checkTiles(new []{neighbours[2], neighbours[4]}, tile))  checkCircle1(new []{neighbours[2], pos, neighbours[4]}, tile);
        if (checkTiles(new []{neighbours[3], neighbours[5]}, tile))  checkCircle1(new []{neighbours[3], pos, neighbours[5]}, tile);
        if (checkTiles(new []{neighbours[4], neighbours[0]}, tile))  checkCircle1(new []{neighbours[4], pos, neighbours[0]}, tile);
        if (checkTiles(new []{neighbours[5], neighbours[1]}, tile))  checkCircle1(new []{neighbours[5], pos, neighbours[1]}, tile);
    }

    void checkTriangleMidVictory1(Vector3Int[] sequence, Tile tile)
    {
        Vector3Int finalTile = new Vector3Int();
        if (sequence[0].y == sequence[1].y) {
            finalTile.x = sequence[1].x > sequence[0].x ? sequence[1].x + 1 : sequence[1].x - 1;
            finalTile.y = sequence[0].y;
        } 
        else if (sequence[2].y == sequence[3].y) {
            finalTile.x = sequence[2].x > sequence[3].x ? sequence[2].x + 1 : sequence[2].x - 1;
            finalTile.y = sequence[2].y;
        } 
        else {
            finalTile.y = sequence[1].y > sequence[0].y ? sequence[1].y + 1 : sequence[1].y - 1;
            finalTile.x = sequence[0].x == sequence[1].x ? sequence[2].x : sequence[1].x;
        }
        if (checkTiles(new[] { finalTile }, tile)) declareVictory("triangle", tile);
    }

    void checkCircle1(Vector3Int[] sequence, Tile tile){
        if (sequence.Length != 3 )  Debug.Log("sequence.Length: " + sequence.Length);
        Vector3Int[] remainingElements = Array.Empty<Vector3Int>();
        if ((sequence[0].y!=sequence[1].y) && (sequence[1].y!=sequence[2].y) && (sequence[2].y!=sequence[0].y)){
            if (sequence[1].y%2==0){
                if ((sequence[1].x>sequence[0].x) && (sequence[1].x>sequence[2].x)) {
                    remainingElements = new [] { new Vector3Int(sequence[0].x-1, sequence[0].y, 0), new Vector3Int (sequence[1].x-2, sequence[1].y, 0), new Vector3Int (sequence[2].x-1, sequence[2].y, 0)};
                } else {
                    remainingElements = new [] { new Vector3Int(sequence[0].x+1, sequence[0].y, 0), new Vector3Int (sequence[1].x+2, sequence[1].y, 0), new Vector3Int (sequence[2].x+1, sequence[2].y, 0)};
                }
            } else {
                if ((sequence[1].x<sequence[0].x) && (sequence[1].x<sequence[2].x)) {
                    remainingElements = new [] { new Vector3Int(sequence[0].x+1, sequence[0].y, 0), new Vector3Int (sequence[1].x+2, sequence[1].y, 0), new Vector3Int (sequence[2].x+1, sequence[2].y, 0)};
                } else {
                    remainingElements = new [] { new Vector3Int(sequence[0].x-1, sequence[0].y, 0), new Vector3Int (sequence[1].x-2, sequence[1].y, 0), new Vector3Int (sequence[2].x-1, sequence[2].y, 0)};
                }
            }
        } else {
            Vector3Int[] twoTilesOnSameY = Array.Empty<Vector3Int>();
            Vector3Int soloYTile;
            if (sequence[0].y == sequence[1].y) {
                twoTilesOnSameY = new [] { sequence[0], sequence[1] };
                soloYTile = sequence[2];
            } else {
                twoTilesOnSameY = new [] { sequence[1], sequence[2] };
                soloYTile = sequence[0];
            }
            Vector3Int[] remainingTwoTilesOnSameY = twoTilesOnSameY[0].y > soloYTile.y ? new [] { new Vector3Int(twoTilesOnSameY[0].x, twoTilesOnSameY[0].y-2, 0), new Vector3Int(twoTilesOnSameY[1].x, twoTilesOnSameY[1].y-2, 0) } : new [] { new Vector3Int(twoTilesOnSameY[0].x, twoTilesOnSameY[0].y+2, 0), new Vector3Int(twoTilesOnSameY[1].x, twoTilesOnSameY[1].y+2, 0) };
            Vector3Int remainingSoloYTile = twoTilesOnSameY[0].x > soloYTile.x || twoTilesOnSameY[1].x > soloYTile.x ? new Vector3Int (soloYTile.x+2, soloYTile.y, 0) : new Vector3Int (soloYTile.x-2, soloYTile.y, 0);
            remainingElements = new [] { remainingSoloYTile, remainingTwoTilesOnSameY [0], remainingTwoTilesOnSameY[1] };
        }

        //victory check
        if ((remainingElements.Length == 3) && (checkTiles(remainingElements, tile))) declareVictory("circle", tile);
    }

    dynamic getNeighbourTiles(Vector3Int pos) {
        var list = new List<Vector3Int>();
        if (pos.y%2==0) {
            list.Add(new Vector3Int(pos.x-1, pos.y+1, 0));
            list.Add(new Vector3Int(pos.x, pos.y+1, 0));
            list.Add(new Vector3Int(pos.x+1, pos.y, 0));
            list.Add(new Vector3Int(pos.x, pos.y-1, 0));
            list.Add(new Vector3Int(pos.x-1, pos.y-1, 0));
            list.Add(new Vector3Int(pos.x-1, pos.y, 0));
        } else {
            list.Add(new Vector3Int(pos.x, pos.y+1, 0));
            list.Add(new Vector3Int(pos.x+1, pos.y+1, 0));
            list.Add(new Vector3Int(pos.x+1, pos.y, 0));
            list.Add(new Vector3Int(pos.x+1, pos.y-1, 0));
            list.Add(new Vector3Int(pos.x, pos.y-1, 0));
            list.Add(new Vector3Int(pos.x-1, pos.y, 0));
        }
        return list.ToArray();
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
        if (checkTiles(tileSequence, tile)) declareVictory("line", tile);
    }

    void checkLine2(Vector3Int pos, Tile tile){
        Vector3Int start = pos, previousTile=pos;
        Vector3Int[] tileSequence;
        TileBase previousTileType = tilemap.GetTile(previousTile);
        while(previousTileType == tile) {
            start = previousTile;
            if (previousTile.y%2==0) previousTile = new Vector3Int(previousTile.x-1, previousTile.y+1, 0);
            else previousTile = new Vector3Int(previousTile.x, previousTile.y+1, 0);
            previousTileType = tilemap.GetTile(previousTile);
        }
        if (start.y%2==0) tileSequence = new [] { start, new Vector3Int(start.x,start.y-1,0), new Vector3Int(start.x+1,start.y-2,0), new Vector3Int(start.x+1,start.y-3,0), new Vector3Int(start.x+2,start.y-4,0), new Vector3Int(start.x+2,start.y-5,0)};
        else tileSequence = new [] { start, new Vector3Int(start.x+1,start.y-1,0), new Vector3Int(start.x+1,start.y-2,0), new Vector3Int(start.x+2,start.y-3,0), new Vector3Int(start.x+2,start.y-4,0), new Vector3Int(start.x+3,start.y-5,0)};
        if (checkTiles(tileSequence, tile)) declareVictory("line", tile);
    }

    void checkLine3(Vector3Int pos, Tile tile){
        Vector3Int start = pos, previousTile=pos;
        Vector3Int[] tileSequence;
        TileBase previousTileType = tilemap.GetTile(previousTile);
        while(previousTileType == tile) {
            start = previousTile;
            if (previousTile.y%2==0) previousTile = new Vector3Int(previousTile.x, previousTile.y+1, 0);
            else previousTile = new Vector3Int(previousTile.x+1, previousTile.y+1, 0);
            previousTileType = tilemap.GetTile(previousTile);
        }
        if (start.y%2==0) tileSequence = new [] { start, new Vector3Int(start.x-1,start.y-1,0), new Vector3Int(start.x-1,start.y-2,0), new Vector3Int(start.x-2,start.y-3,0), new Vector3Int(start.x-2,start.y-4,0), new Vector3Int(start.x-3,start.y-5,0)};
        else tileSequence = new [] { start, new Vector3Int(start.x,start.y-1,0), new Vector3Int(start.x-1,start.y-2,0), new Vector3Int(start.x-1,start.y-3,0), new Vector3Int(start.x-2,start.y-4,0), new Vector3Int(start.x-2,start.y-5,0)};
        if (checkTiles(tileSequence, tile)) declareVictory("line", tile);
    }

    bool checkTiles(Vector3Int[] sequence, Tile tile){
        return Array.TrueForAll(sequence, element=>tilemap.GetTile(element)==tile);
    }

    void clearTiles(Vector3Int[] tileArray) {
        Array.ForEach(tileArray, currentTile=>{
            tilemap.SetTile(currentTile, null);
        });
    }

    void declareVictory(string type, Tile tile) {
        Debug.Log(tile + " wins by " + type + " victory!");
        UnityEditor.EditorApplication.isPlaying = false;
    }
}
