using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour {

    public GameObject gridPrefab;
    public GameObject borderGridPrefab;
    public Transform gridParent;
    public Transform borderGridParent;
    public Vector3 startingCoord;

    public GameObject[,] grid;

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
    }

    // Generate a grid with x * y size
    public void GenerateGrid(float x, float z) {
        grid = new GameObject[(int) x, (int) z];

        float realX = x + 2;
        float realZ = z + 2;

        for (int i = 0; i < realX; i++) {
            if (i == 0 || i == realX - 1) {
                // draw left and right border
                for (int j = 0; j < realZ; j++) {
                    Instantiate(borderGridPrefab, startingCoord, gridPrefab.transform.rotation, borderGridParent);
                    startingCoord.z += 1;
                }
            } else {
                for (int j = 0; j < realZ; j++) {
                    if (j == 0 || j == realZ - 1) {
                        // draw top and bottom border
                        Instantiate(borderGridPrefab, startingCoord, gridPrefab.transform.rotation, borderGridParent);
                    } else {
                        var newGrid = Instantiate(gridPrefab, startingCoord, gridPrefab.transform.rotation, gridParent);
                        var details = newGrid.GetComponent<CellDetails>();
                        details.SetPoint(i - 1, j - 1);

                        // set the default cost value to each cell
                        details.heuristicCost = float.MaxValue;
                        details.movementCost = 1f;
                        details.totalCost = float.MaxValue;
                        grid[i - 1, j - 1] = newGrid;
                    }
                    startingCoord.z += 1;
                }
            }
            
            startingCoord.z -= realZ;
            startingCoord.x += 1;
        }

        GetNeighbourGrid();
    }

    // get all possible neighbour of all grids
    public void GetNeighbourGrid() {
        for (int i = 0; i < grid.GetLength(0); i++) {
            for (int j = 0; j < grid.GetLength(1); j++) {
                var currGrid = grid[i, j].GetComponent<CellDetails>();
                if (i == 0) {                                               // if x = 0
                    if (j == 0) {                                               // if x = 0 & z = 0
                        currGrid.northCell = grid[i, j + 1];
                        currGrid.northEastCell = grid[i + 1, j + 1];
                        currGrid.eastCell = grid[i + 1, j];

                    } else if (j == grid.GetLength(1) - 1) {                    // if x = 0 & z = grid.GetLength(1)
                        currGrid.southCell = grid[i, j - 1];
                        currGrid.southEastCell = grid[i + 1, j - 1];
                        currGrid.eastCell = grid[i + 1, j];
                    
                    } else {                                                   // if x = 0 & z is anything else
                        currGrid.northCell = grid[i, j + 1];
                        currGrid.northEastCell = grid[i + 1, j + 1];
                        currGrid.eastCell = grid[i + 1, j];
                        currGrid.southCell = grid[i, j - 1];
                        currGrid.southEastCell = grid[i + 1, j - 1];
                    }
                } else if (i == grid.GetLength(0) - 1) {
                    if (j == 0) {
                        currGrid.northCell = grid[i, j + 1];
                        currGrid.northWestCell = grid[i - 1, j + 1];
                        currGrid.westCell = grid[i - 1, j];

                    } else if (j == grid.GetLength(1) - 1) {
                        currGrid.southCell = grid[i, j - 1];
                        currGrid.southWestCell = grid[i - 1, j - 1];
                        currGrid.westCell = grid[i - 1, j];

                    } else {
                        currGrid.northCell = grid[i, j + 1];
                        currGrid.northWestCell = grid[i - 1, j + 1];
                        currGrid.southCell = grid[i, j - 1];
                        currGrid.southWestCell = grid[i - 1, j - 1];
                        currGrid.westCell = grid[i - 1, j];
                    }
                } else {
                    if (j == 0) {
                        currGrid.northCell = grid[i, j + 1];
                        currGrid.eastCell = grid[i + 1, j];
                        currGrid.westCell = grid[i - 1, j];
                        currGrid.northEastCell = grid[i + 1, j + 1];
                        currGrid.northWestCell = grid[i - 1, j + 1];

                    } else if (j == grid.GetLength(1) - 1) {
                        currGrid.southCell = grid[i, j - 1];
                        currGrid.southEastCell = grid[i + 1, j - 1];
                        currGrid.southWestCell = grid[i - 1, j - 1];
                        currGrid.eastCell = grid[i + 1, j];
                        currGrid.westCell = grid[i - 1, j];

                    } else {
                        currGrid.northCell = grid[i, j + 1];
                        currGrid.eastCell = grid[i + 1, j];
                        currGrid.westCell = grid[i - 1, j];
                        currGrid.northEastCell = grid[i + 1, j + 1];
                        currGrid.northWestCell = grid[i - 1, j + 1];
                        currGrid.southCell = grid[i, j - 1];
                        currGrid.southEastCell = grid[i + 1, j - 1];
                        currGrid.southWestCell = grid[i - 1, j - 1];
                    }
                }
            }
        }
    }
}
