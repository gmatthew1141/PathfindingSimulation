using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellDetails : MonoBehaviour {

    public bool isStartCell = false;
    public bool isEndCell = false;
    public bool isObstacleCell = false;
    public bool isPath = false;
    public bool isVisited = false;
    
    public float heuristicCost;
    public float movementCost;
    public float totalCost;
    public Vector2 point;
    public CellDetails parent;

    public GameObject northCell = null;
    public GameObject northEastCell = null;
    public GameObject eastCell = null;
    public GameObject southEastCell = null;
    public GameObject southCell = null;
    public GameObject southWestCell = null;
    public GameObject westCell = null;
    public GameObject northWestCell = null;

    public void SetPoint(int x, int y) {
        point = new Vector2(x, y);
    }
}
