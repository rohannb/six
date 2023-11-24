using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
 
public class GridController : MonoBehaviour
{
    public Grid grid;
    [SerializeField] private Tilemap interactiveMap = null;
    [SerializeField] private Tile hoverTile = null;
 
 
    private Vector3Int previousMousePos = new Vector3Int();
 
    // Start is called before the first frame update
    void Start()
    {
        // grid = gameObject.GetComponent<Grid>();
    }
 
    // Update is called once per frame
    void Update()
    {
        // Mouse over -> highlight tile
        Vector3Int mousePos = GetMousePosition();
        if (mousePos != previousMousePos)
        {
            Vector3Int newPos = new Vector3Int(mousePos.x, mousePos.y, 0);
            Vector3Int oldPos = new Vector3Int(previousMousePos.x, previousMousePos.y, 0);
            Debug.Log(newPos);
            interactiveMap.SetTile(oldPos, null); // Remove old hoverTile
            interactiveMap.SetTile(newPos, hoverTile);
            previousMousePos = mousePos;
        }
    }
 
    Vector3Int GetMousePosition()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return grid.WorldToCell(mouseWorldPos);
    }
}