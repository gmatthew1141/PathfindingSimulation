using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar : SearchAlgorithm {

    public override float CalculateHeuristic(Vector2 currPoint, CellDetails endCell) {
        float output = Mathf.Sqrt((currPoint.x * endCell.point.x) + (currPoint.y * endCell.point.y));
        return output;
    }

    public override void StartSearchAlgorithm(float x, float z, CellDetails startCell, CellDetails endCell, GameManager gameManager) {
        // destination is not yet reached, set its initial value to false
        bool destinationReached = false;

        // create closed list and initialize it to false to indicate that no grid has been included yet.
        bool[,] closedList = new bool[(int)x, (int)z];

        // initialize the parameter of the starting cell
        startCell.heuristicCost = 0;
        startCell.movementCost = 0;
        startCell.totalCost = 0;
        startCell.parent = startCell;

        // create an open list that having information of (totalCost, (x, y))
        // where totalCost = movementCost + heuristicCost
        // and x, y is the position of the cell in the grid
        Queue<(double, CellDetails)> openListQueue = new Queue<(double, CellDetails)>();


        Queue<CellDetails> visitedOrder = new Queue<CellDetails>(); 

        // put starting cell in the list and set its total cost to 0f
        openListQueue.Enqueue((0f, startCell));

        CellDetails currCell = null;

        //Debug.Log("openList size: " + openList.Count + " with element: " + openList[0].Item1 + ", " + openList[0].Item2.point);

        while (openListQueue.Count != 0 && !destinationReached) {
            var pair = openListQueue.Dequeue();

            currCell = pair.Item2;

            closedList[(int)pair.Item2.point.x, (int)pair.Item2.point.y] = true;

            // north cell
            if (currCell.northCell != null) {
                CellDetails checkedCell = currCell.northCell.GetComponent<CellDetails>();
                if (checkedCell == endCell) {
                    checkedCell.parent = currCell;
                    destinationReached = true;

                    Debug.Log("Destination cell reached");
                    gameManager.EmulateVisitedPath(visitedOrder, destinationReached);
                    return;

                // if the cell is not yet closed and is not an obstacle
                } else if (closedList[(int)checkedCell.point.x, (int)checkedCell.point.y] == false && !checkedCell.isObstacleCell) {
                    // calculate the new cost
                    float newHeuristicCost = CalculateHeuristic(checkedCell.point, endCell);
                    float newTotalCost = checkedCell.movementCost + newHeuristicCost;

                    // if checkedCell has never been visited or the new cost is less than the old cost
                    // update the costs and parent cell
                    if (checkedCell.totalCost == float.MaxValue || checkedCell.totalCost > newTotalCost) {
                        //openList.Add((newTotalCost, checkedCell));
                        openListQueue.Enqueue((newTotalCost, checkedCell));

                        checkedCell.heuristicCost = newHeuristicCost;
                        checkedCell.totalCost = newTotalCost;
                        checkedCell.parent = currCell;
                        checkedCell.isVisited = true;               // mark the cell as visited

                        visitedOrder.Enqueue(checkedCell);         // add the visited cell to the stack if its not goal cell
                    }
                }
            }


            // east cell
            if (currCell.eastCell != null) {
                CellDetails checkedCell = currCell.eastCell.GetComponent<CellDetails>();

                if (checkedCell == endCell) {
                    checkedCell.parent = currCell;
                    destinationReached = true;

                    Debug.Log("Destination cell reached");
                    gameManager.EmulateVisitedPath(visitedOrder, destinationReached);
                    return;
                } else if (closedList[(int)checkedCell.point.x, (int)checkedCell.point.y] == false && !checkedCell.isObstacleCell) {
                    float newHeuristicCost = CalculateHeuristic(checkedCell.point, endCell);
                    float newTotalCost = checkedCell.movementCost + newHeuristicCost;

                    if (checkedCell.totalCost == float.MaxValue || checkedCell.totalCost > newTotalCost) {
                        //openList.Add((newTotalCost, checkedCell));
                        openListQueue.Enqueue((newTotalCost, checkedCell));

                        checkedCell.heuristicCost = newHeuristicCost;
                        checkedCell.totalCost = newTotalCost;
                        checkedCell.parent = currCell;
                        checkedCell.isVisited = true;

                        visitedOrder.Enqueue(checkedCell);
                    }
                }
            }

            // south cell
            if (currCell.southCell != null) {
                CellDetails checkedCell = currCell.southCell.GetComponent<CellDetails>();

                if (checkedCell == endCell) {
                    checkedCell.parent = currCell;
                    destinationReached = true;

                    Debug.Log("Destination cell reached");
                    gameManager.EmulateVisitedPath(visitedOrder, destinationReached);
                    return;
                } else if (closedList[(int)checkedCell.point.x, (int)checkedCell.point.y] == false && !checkedCell.isObstacleCell) {
                    float newHeuristicCost = CalculateHeuristic(checkedCell.point, endCell);
                    float newTotalCost = checkedCell.movementCost + newHeuristicCost;

                    if (checkedCell.totalCost == float.MaxValue || checkedCell.totalCost > newTotalCost) {
                        //openList.Add((newTotalCost, checkedCell));
                        openListQueue.Enqueue((newTotalCost, checkedCell));

                        checkedCell.heuristicCost = newHeuristicCost;
                        checkedCell.totalCost = newTotalCost;
                        checkedCell.parent = currCell;
                        checkedCell.isVisited = true;

                        visitedOrder.Enqueue(checkedCell);
                    }
                }
            }

            // west cell
            if (currCell.westCell != null) {
                CellDetails checkedCell = currCell.westCell.GetComponent<CellDetails>();

                if (checkedCell == endCell) {
                    checkedCell.parent = currCell;
                    destinationReached = true;

                    Debug.Log("Destination cell reached");
                    gameManager.EmulateVisitedPath(visitedOrder, destinationReached);
                    return;
                } else if (closedList[(int)checkedCell.point.x, (int)checkedCell.point.y] == false && !checkedCell.isObstacleCell) {
                    float newHeuristicCost = CalculateHeuristic(checkedCell.point, endCell);
                    float newTotalCost = checkedCell.movementCost + newHeuristicCost;

                    if (checkedCell.totalCost == float.MaxValue || checkedCell.totalCost > newTotalCost) {
                        // openList.Add((newTotalCost, checkedCell));
                        openListQueue.Enqueue((newTotalCost, checkedCell));

                        checkedCell.heuristicCost = newHeuristicCost;
                        checkedCell.totalCost = newTotalCost;
                        checkedCell.parent = currCell;
                        checkedCell.isVisited = true;

                        visitedOrder.Enqueue(checkedCell);
                    }
                }
            }

            // north east cell
            if (currCell.northEastCell != null) {
                CellDetails checkedCell = currCell.northEastCell.GetComponent<CellDetails>();

                if (checkedCell == endCell) {
                    checkedCell.parent = currCell;
                    destinationReached = true;

                    Debug.Log("Destination cell reached");
                    gameManager.EmulateVisitedPath(visitedOrder, destinationReached);
                    return;
                } else if (closedList[(int)checkedCell.point.x, (int)checkedCell.point.y] == false && !checkedCell.isObstacleCell) {
                    float newHeuristicCost = CalculateHeuristic(checkedCell.point, endCell);
                    float newTotalCost = checkedCell.movementCost + newHeuristicCost;

                    if (checkedCell.totalCost == float.MaxValue || checkedCell.totalCost > newTotalCost) {
                        //openList.Add((newTotalCost, checkedCell));
                        openListQueue.Enqueue((newTotalCost, checkedCell));

                        checkedCell.heuristicCost = newHeuristicCost;
                        checkedCell.totalCost = newTotalCost;
                        checkedCell.parent = currCell;
                        checkedCell.isVisited = true;

                        visitedOrder.Enqueue(checkedCell);
                    }
                }
            }

            // south east cell
            if (currCell.southEastCell != null) {
                CellDetails checkedCell = currCell.southEastCell.GetComponent<CellDetails>();

                if (checkedCell == endCell) {
                    checkedCell.parent = currCell;
                    destinationReached = true;

                    Debug.Log("Destination cell reached");
                    gameManager.EmulateVisitedPath(visitedOrder, destinationReached);
                    return;
                } else if (closedList[(int)checkedCell.point.x, (int)checkedCell.point.y] == false && !checkedCell.isObstacleCell) {
                    float newHeuristicCost = CalculateHeuristic(checkedCell.point, endCell);
                    float newTotalCost = checkedCell.movementCost + newHeuristicCost;

                    if (checkedCell.totalCost == float.MaxValue || checkedCell.totalCost > newTotalCost) {
                        //openList.Add((newTotalCost, checkedCell));
                        openListQueue.Enqueue((newTotalCost, checkedCell));

                        checkedCell.heuristicCost = newHeuristicCost;
                        checkedCell.totalCost = newTotalCost;
                        checkedCell.parent = currCell;
                        checkedCell.isVisited = true;

                        visitedOrder.Enqueue(checkedCell);
                    }
                }
            }

            // south west cell
            if (currCell.southWestCell != null) {
                CellDetails checkedCell = currCell.southWestCell.GetComponent<CellDetails>();

                if (checkedCell == endCell) {
                    checkedCell.parent = currCell;
                    destinationReached = true;

                    Debug.Log("Destination cell reached");
                    gameManager.EmulateVisitedPath(visitedOrder, destinationReached);
                    return;
                } else if (closedList[(int)checkedCell.point.x, (int)checkedCell.point.y] == false && !checkedCell.isObstacleCell) {
                    float newHeuristicCost = CalculateHeuristic(checkedCell.point, endCell);
                    float newTotalCost = checkedCell.movementCost + newHeuristicCost;

                    if (checkedCell.totalCost == float.MaxValue || checkedCell.totalCost > newTotalCost) {
                        //openList.Add((newTotalCost, checkedCell));
                        openListQueue.Enqueue((newTotalCost, checkedCell));

                        checkedCell.heuristicCost = newHeuristicCost;
                        checkedCell.totalCost = newTotalCost;
                        checkedCell.parent = currCell;
                        checkedCell.isVisited = true;

                        visitedOrder.Enqueue(checkedCell);
                    }
                }
            }

            // north west cell
            if (currCell.northWestCell != null) {
                CellDetails checkedCell = currCell.northWestCell.GetComponent<CellDetails>();

                if (checkedCell == endCell) {
                    checkedCell.parent = currCell;
                    destinationReached = true;

                    Debug.Log("Destination cell reached");
                    gameManager.EmulateVisitedPath(visitedOrder, destinationReached);
                    return;
                } else if (closedList[(int)checkedCell.point.x, (int)checkedCell.point.y] == false && !checkedCell.isObstacleCell) {
                    float newHeuristicCost = CalculateHeuristic(checkedCell.point, endCell);
                    float newTotalCost = checkedCell.movementCost + newHeuristicCost;

                    if (checkedCell.totalCost == float.MaxValue || checkedCell.totalCost > newTotalCost) {
                        //openList.Add((newTotalCost, checkedCell));
                        openListQueue.Enqueue((newTotalCost, checkedCell));

                        checkedCell.heuristicCost = newHeuristicCost;
                        checkedCell.totalCost = newTotalCost;
                        checkedCell.parent = currCell;
                        checkedCell.isVisited = true;

                        visitedOrder.Enqueue(checkedCell);
                    }
                }
            }

        }

        if (!destinationReached) {
            Debug.Log("Destination cannot be reached");
            Debug.Log("visitedCell count: " + visitedOrder.Count);
            gameManager.EmulateVisitedPath(visitedOrder, destinationReached);
        }
    }
}
