using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    #region Public Variables
    public Transform mainCamera;
    public float x = 36f;                      // default value
    public float z = 22f;                      // default value
    public float visitedDelay = 0.01f;
    public float pathDelay = 0.3f;
    public float warningTimer;

    public CellDetails currHoveredCell;
    public CellDetails currStartCell;
    public CellDetails currEndCell;
    public CellDetails oldStartCell;
    public CellDetails oldEndCell;
    public List<CellDetails> obstacleCell;
    #endregion

    #region Cell Colors
    [Header("Color")]
    public Color hoverCellColor = Color.gray;
    public Color startCellColor = Color.black;
    public Color obstacleCellColor = Color.black;
    public Color endCellColor = Color.green;
    public Color pathTakenCellColor = Color.red;
    public Color visitedCellColor = Color.yellow;
    public Color defaultCellColor = Color.white;
    #endregion

    #region Private variables
    private RaycastHit hit;
    private GridGenerator gridGenerator;
    private bool setStartCell = true;
    private bool setEndCell = false;
    private bool setObstacleCell = false;
    private bool canHover = true;
    private bool isAutoplayOn = false;
    private bool newObstacle = false;
    private bool obstacleRemoved = false;
    public bool hasRun = false;

    private AStar astar;
    private Queue<CellDetails> visitedOrder;        // might no longer need this
    #endregion

    #region UIs
    [Header("UIs")]
    public Button setStartCellBtn;
    public Button setEndCellBtn;
    public Button setObstacleCellBtn;
    public Button playButton;

    public GameObject descriptionsPanel;
    public GameObject warningPanel;
    #endregion

    private void Awake() {
        gridGenerator = FindObjectOfType<GridGenerator>();
        obstacleCell = new List<CellDetails>();
        visitedOrder = new Queue<CellDetails>();
        astar = gameObject.AddComponent<AStar>();
    }

    // Start is called before the first frame update
    void Start() {
        gridGenerator.GenerateGrid(x, z);
        mainCamera.position = new Vector3((x - 3)/ 2, mainCamera.position.y, (z + 2) / 2);
    }

    // Update is called once per frame
    void Update() {
        HoverOnGrid();

        if (setStartCell) {
            setStartCellBtn.Select();
        }

        if (setEndCell) {
            setEndCellBtn.Select();
        }

        if (setObstacleCell) {
            setObstacleCellBtn.Select();
        }

        if (isAutoplayOn) {
            playButton.Select();
            OnAutoPlay();
        }
    }

    private void OnAutoPlay() {
        // if player haven't set any starting/goal cell, return
        if (currStartCell == null || currEndCell == null) {
            return;
        }

        // if the curresnt startCell/endCell is different from the old startCell/endCell, rerun the algorithm
        if (currStartCell != oldStartCell || currEndCell != oldEndCell) {
            foreach (var curr in gridGenerator.grid) {
                CellDetails currCellDetail = curr.GetComponent<CellDetails>();
                if (!currCellDetail.isStartCell && !currCellDetail.isEndCell && !currCellDetail.isObstacleCell) {
                    ResetGridDetails(currCellDetail);
                }
            }

            canHover = false;
            astar.StartSearchAlgorithm(x, z, currStartCell, currEndCell, this);

            oldStartCell = currStartCell;
            oldEndCell = currEndCell;
        } else {
            if (!hasRun && currStartCell != null && currEndCell != null) {
                hasRun = true;
                canHover = false;
                astar.StartSearchAlgorithm(x, z, currStartCell, currEndCell, this);
            }
        }

    }

    public void HoverOnGrid() {
        // raycast hit from camera to grid
        // if its clickable grid, change grid color
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit) && canHover) {
            CellDetails grid = hit.transform.GetComponent<CellDetails>();

            // if mouse pointing outside of interactable grid, change the last hovered grid color to default color
            if (grid == null) {
                if (currHoveredCell != null) {
                    if (currHoveredCell.isEndCell) {
                        currHoveredCell.GetComponent<MeshRenderer>().material.color = endCellColor;
                        return;
                    }

                    if (currHoveredCell.isStartCell) {
                        currHoveredCell.GetComponent<MeshRenderer>().material.color = startCellColor;
                        return;
                    }

                    if (currHoveredCell.isObstacleCell) {
                        currHoveredCell.GetComponent<MeshRenderer>().material.color = obstacleCellColor;
                        return;
                    }

                    if (currHoveredCell.isPath) {
                        currHoveredCell.GetComponent<MeshRenderer>().material.color = pathTakenCellColor;
                        return;
                    }

                    if (currHoveredCell.isVisited) {
                        currHoveredCell.GetComponent<MeshRenderer>().material.color = visitedCellColor;
                        return;
                    }

                    currHoveredCell.GetComponent<MeshRenderer>().material.color = defaultCellColor;
                    return;
                }
            }

            // if the mouse is hovering on top of a cell
            if (grid != currHoveredCell) {
                hit.transform.GetComponent<MeshRenderer>().material.color = hoverCellColor;
            }

            // if the mouse is moved to a different cell, return the previous cell color to its original color
            if (currHoveredCell != null && currHoveredCell != grid) {
                if (currHoveredCell.isEndCell) {
                    currHoveredCell.GetComponent<MeshRenderer>().material.color = endCellColor;
                } else if (currHoveredCell.isStartCell) {
                    currHoveredCell.GetComponent<MeshRenderer>().material.color = startCellColor;
                } else if (currHoveredCell.isObstacleCell) {
                    currHoveredCell.GetComponent<MeshRenderer>().material.color = obstacleCellColor;
                } else if (currHoveredCell.isPath) {
                    currHoveredCell.GetComponent<MeshRenderer>().material.color = pathTakenCellColor;
                } else if (currHoveredCell.isVisited) {
                    currHoveredCell.GetComponent<MeshRenderer>().material.color = visitedCellColor;
                } else {
                    currHoveredCell.GetComponent<MeshRenderer>().material.color = defaultCellColor;
                }
            }

            currHoveredCell = grid;

            OnClick();

        } else {
            if (currHoveredCell != null) {
                if (currHoveredCell.isEndCell) {
                    currHoveredCell.GetComponent<MeshRenderer>().material.color = endCellColor;
                } else if (currHoveredCell.isStartCell) {
                    currHoveredCell.GetComponent<MeshRenderer>().material.color = startCellColor;
                } else if (currHoveredCell.isObstacleCell) {
                    currHoveredCell.GetComponent<MeshRenderer>().material.color = obstacleCellColor;
                } else if (currHoveredCell.isPath) {
                    currHoveredCell.GetComponent<MeshRenderer>().material.color = pathTakenCellColor;
                } else if (currHoveredCell.isVisited) {
                    currHoveredCell.GetComponent<MeshRenderer>().material.color = visitedCellColor;
                } else {
                    currHoveredCell.GetComponent<MeshRenderer>().material.color = defaultCellColor;
                }
            }
        }
    }

    public void OnClick() {
        if (setStartCell) {
            if (Input.GetMouseButtonDown(0)) {
                if (currStartCell == null) {
                    currHoveredCell.isStartCell = true;
                    currHoveredCell.GetComponent<MeshRenderer>().material.color = startCellColor;
                    currStartCell = currHoveredCell;
                    oldStartCell = currStartCell;
                } else {
                    oldStartCell = currStartCell;
                    ResetGridDetails(currStartCell);
                    if (currHoveredCell != currStartCell) {
                        ResetGridDetails(currStartCell);

                        currHoveredCell.isStartCell = true;
                        currHoveredCell.GetComponent<MeshRenderer>().material.color = startCellColor;
                        currStartCell = currHoveredCell;
                    }
                }
            }
        }

        if (setEndCell) {
            if (Input.GetMouseButtonDown(0)) {
                if (currEndCell == null) {
                    currHoveredCell.isEndCell = true;
                    currHoveredCell.GetComponent<MeshRenderer>().material.color = endCellColor;
                    currEndCell = currHoveredCell;
                    oldEndCell = currEndCell;
                } else {
                    oldEndCell = currEndCell;
                    ResetGridDetails(currEndCell);
                    if (currHoveredCell != currEndCell) {
                        ResetGridDetails(currEndCell); 

                        currHoveredCell.isEndCell = true;
                        currHoveredCell.GetComponent<MeshRenderer>().material.color = endCellColor;
                        currEndCell = currHoveredCell;
                    }
                }
            }
        }

        if (setObstacleCell) {
            if (Input.GetMouseButton(0)) {
                ResetGridDetails(currHoveredCell);

                if (currHoveredCell == currStartCell) {
                    currStartCell = null;
                }

                if (currHoveredCell == currEndCell) {
                    currEndCell = null;
                }

                if (!currHoveredCell.isObstacleCell) {
                    newObstacle = true;
                    currHoveredCell.isObstacleCell = true;
                    currHoveredCell.GetComponent<MeshRenderer>().material.color = obstacleCellColor;
                }
            }

            if (Input.GetMouseButtonUp(0)) {
                if (isAutoplayOn && newObstacle && currStartCell != null && currEndCell != null) {
                    Debug.Log("autoplay on and detecting new obstacle");
                    // if the autoplay is on, start the search algorithm
                    // need to check if the user added a new obstacle or not. How?
                    newObstacle = false;

                    foreach (var curr in gridGenerator.grid) {
                        CellDetails currCellDetail = curr.GetComponent<CellDetails>();
                        if (!currCellDetail.isStartCell && !currCellDetail.isEndCell && !currCellDetail.isObstacleCell) {
                            ResetGridDetails(currCellDetail);
                        }
                    }

                    canHover = false;
                    astar.StartSearchAlgorithm(x, z, currStartCell, currEndCell, this);
                }
            }
        }

        if (Input.GetMouseButton(1)) {
            if (currHoveredCell.isObstacleCell || currHoveredCell.isEndCell || currHoveredCell.isStartCell) {
                if (currHoveredCell.isObstacleCell) {
                    obstacleRemoved = true;
                }

                ResetGridDetails(currHoveredCell);
                currHoveredCell.GetComponent<MeshRenderer>().material.color = defaultCellColor;

                if (currHoveredCell.isStartCell) {
                    currStartCell = null;
                }

                if (currHoveredCell.isEndCell) {
                    currEndCell = null;
                }

            }
        }

        if (Input.GetMouseButtonUp(1)) {
            Debug.Log("isAutoplayOn: " + isAutoplayOn + " obstacleremoved: " + obstacleRemoved);
            if (isAutoplayOn && obstacleRemoved && currStartCell != null && currEndCell != null) {
                obstacleRemoved = false;

                foreach (var curr in gridGenerator.grid) {
                    CellDetails currCellDetail = curr.GetComponent<CellDetails>();
                    if (!currCellDetail.isStartCell && !currCellDetail.isEndCell && !currCellDetail.isObstacleCell) {
                        ResetGridDetails(currCellDetail);
                    }
                }

                canHover = false;
                astar.StartSearchAlgorithm(x, z, currStartCell, currEndCell, this);
                Debug.Log("search algorithm run after removing an obstacle");
            }
        }
    }

    #region Button functions

    public void SetStartGrid() {
        setStartCell = true;
        setEndCell = false;
        setObstacleCell = false;
    }

    public void SetGoalGrid() {
        setStartCell = false;
        setEndCell = true;
        setObstacleCell = false;
    }

    public void SetObstacleGrid() {
        setStartCell = false;
        setEndCell = false;
        setObstacleCell = true;
    }

    public void ClearGrid() {
        currStartCell = null;
        currEndCell = null;

        foreach (var cell in gridGenerator.grid) {
            ResetGridDetails(cell.GetComponent<CellDetails>());
        }
    }

    public void StartPathfinding() {
        if (isAutoplayOn) {
            isAutoplayOn = false;
            hasRun = false;
        } else {
            isAutoplayOn = true;
            if (currStartCell != null && currEndCell != null) {
                Debug.Log("Play button pressed");
                currHoveredCell = null;
                canHover = false;
                //AStarSearch();

                foreach (var curr in gridGenerator.grid) {
                    CellDetails currCellDetail = curr.GetComponent<CellDetails>();
                    if (!currCellDetail.isStartCell && !currCellDetail.isEndCell && !currCellDetail.isObstacleCell) {
                        ResetGridDetails(currCellDetail);
                    }
                }

                astar.StartSearchAlgorithm(x, z, currStartCell, currEndCell, this);
                hasRun = true;
            }
        }
    }

    public void ShowDescriptionPanel() {
        if (descriptionsPanel.activeSelf) {
            descriptionsPanel.SetActive(false);
            canHover = true;
        } else {
            descriptionsPanel.SetActive(true);
            canHover = false;
        }
    }

    public void QuitApp() {
        Application.Quit();
    }

    #endregion

    // reset the details of 'cell'
    private void ResetGridDetails(CellDetails cell) {
        cell.parent = null;
        cell.isStartCell = false;
        cell.isEndCell = false;
        cell.isObstacleCell = false;
        cell.isVisited = false;
        cell.isPath = false;
        cell.heuristicCost = float.MaxValue;
        cell.totalCost = float.MaxValue;
        cell.GetComponent<MeshRenderer>().material.color = defaultCellColor;
    }

    private void TracePath(float delay) {
        CellDetails currCell = currEndCell;
        Stack<CellDetails> path = new Stack<CellDetails>();
        
        // trace back to the start cell
        while (currCell.parent != currCell) {
            currCell.isPath = true;
            path.Push(currCell);
            currCell = currCell.parent;
        }
        currCell.isPath = true;
        path.Push(currCell);

        // emulate the best path
        EmulatePathTaken(path, delay);

    }

    private void EmulatePathTaken(Stack<CellDetails> path, float delay) {
        float hoverDelay = (path.Count * pathDelay) + delay;
        while (path.Count != 0) {
            var curr = path.Pop();
            if (!curr.isStartCell && !curr.isEndCell) {
                // set the color of each cells in a different interval
                StartCoroutine(SetColor(curr, pathTakenCellColor, delay));
                delay += pathDelay;
            }
        }

        // enable player to play with the grid again after the animation finished
        StartCoroutine(EnableHover(hoverDelay));
    }

    public void EmulateVisitedPath(Queue<CellDetails> path, bool foundDestination) {
        float delay = 0;
        var hoverDelay = path.Count * visitedDelay;

        while (path.Count != 0) {
            var curr = path.Dequeue();
            if (!curr.isStartCell && !curr.isEndCell) {
                //curr.GetComponent<MeshRenderer>().material.color = pathTakenColor;
                StartCoroutine(SetColor(curr, visitedCellColor, delay));
                delay += visitedDelay;
            }
        }

        // if we found path to the destination, draw the path, otherwise enable player to play with the grid
        if (foundDestination) {
            Debug.Log("Destination found, tracing path");
            TracePath(hoverDelay);
        } else {
            StartCoroutine(EnableHover(hoverDelay));
            // notify the player that pathfinding cannot get to the end cell
            StartCoroutine(ShowWarning(hoverDelay));
        }
    }

    #region Enumerators
    private IEnumerator ShowWarning(float delay) {
        yield return new WaitForSeconds(delay);
        warningPanel.SetActive(true);
        StartCoroutine(PanelTimer(warningPanel));
    }

    private IEnumerator SetColor(CellDetails cell, Color color, float delay) {
        yield return new WaitForSeconds(delay);
        cell.GetComponent<MeshRenderer>().material.color = color;
    }

    private IEnumerator EnableHover(float delay) {
        yield return new WaitForSeconds(delay);
        canHover = true;
    }

    private IEnumerator PanelTimer(GameObject panel) {
        yield return new WaitForSeconds(warningTimer);
        panel.SetActive(false);
    }
    #endregion

    #region old A* Algorithm, ToDo: Delete later
    // calculate the heuristic cost
    // h = sqrt((x1 * x2) + (y1 * y2))
    private float CalculateHeuristic(Vector2 currPoint) {
        float output = Mathf.Sqrt((currPoint.x * currEndCell.point.x) + (currPoint.y * currEndCell.point.y));
        return output;
    }

    // We cannot have the same start and end grid position
    // start and end grid will not be one with obstacle grid
    private void AStarSearch() { 

        // destination is not yet reached, set its initial value to false
        bool destinationReached = false;

        // create closed list and initialize it to false to indicate that no grid has been included yet.
        bool[,] closedList = new bool[(int) x, (int) z];

        // initialize the parameter of the starting cell
        currStartCell.heuristicCost = 0;
        currStartCell.movementCost = 0;
        currStartCell.totalCost = 0;
        currStartCell.parent = currStartCell;

        // create an open list that having information of (totalCost, (x, y))
        // where totalCost = movementCost + heuristicCost
        // and x, y is the position of the cell in the grid
        //List<(double, CellDetails)> openList = new List<(double, CellDetails)>();   // ToDo swap List with Queue
        Queue<(double, CellDetails)> openListQueue = new Queue<(double, CellDetails)>();

        // put starting cell in the list and set its total cost to 0f
        //openList.Add((0f, startCell));
        openListQueue.Enqueue((0f, currStartCell));

        CellDetails currCell = null;

        //Debug.Log("openList size: " + openList.Count + " with element: " + openList[0].Item1 + ", " + openList[0].Item2.point);

        //while (openList.Count != 0 && !destinationReached) {
        while (openListQueue.Count != 0 && !destinationReached) {
            //var pair = openList[0]; 
            var pair = openListQueue.Dequeue();

            currCell = pair.Item2;

            //openList.RemoveAt(0);

            closedList[(int) pair.Item2.point.x, (int) pair.Item2.point.y] = true;

            // north cell
            if (currCell.northCell != null) {
                CellDetails checkedCell = currCell.northCell.GetComponent<CellDetails>();
                if (checkedCell == currEndCell) {
                    checkedCell.parent = currCell;
                    destinationReached = true;

                    Debug.Log("Destination cell reached");
                    EmulateVisitedPath(visitedOrder, destinationReached);
                    return;

                    // if the cell is not yet closed and is not an obstacle
                } else if (closedList[(int)checkedCell.point.x, (int)checkedCell.point.y] == false && !checkedCell.isObstacleCell) {
                    // calculate the new cost
                    float newHeuristicCost = CalculateHeuristic(checkedCell.point);
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

                if (checkedCell == currEndCell) {
                    checkedCell.parent = currCell;
                    destinationReached = true;

                    Debug.Log("Destination cell reached");
                    EmulateVisitedPath(visitedOrder, destinationReached);
                    return;
                } else if (closedList[(int)checkedCell.point.x, (int)checkedCell.point.y] == false && !checkedCell.isObstacleCell) {
                    float newHeuristicCost = CalculateHeuristic(checkedCell.point);
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

                if (checkedCell == currEndCell) {
                    checkedCell.parent = currCell;
                    destinationReached = true;

                    Debug.Log("Destination cell reached");
                    EmulateVisitedPath(visitedOrder, destinationReached);
                    return;
                } else if (closedList[(int)checkedCell.point.x, (int)checkedCell.point.y] == false && !checkedCell.isObstacleCell) {
                    float newHeuristicCost = CalculateHeuristic(checkedCell.point);
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

                if (checkedCell == currEndCell) {
                    checkedCell.parent = currCell;
                    destinationReached = true;

                    Debug.Log("Destination cell reached");
                    EmulateVisitedPath(visitedOrder, destinationReached);
                    return;
                } else if (closedList[(int)checkedCell.point.x, (int)checkedCell.point.y] == false && !checkedCell.isObstacleCell) {
                    float newHeuristicCost = CalculateHeuristic(checkedCell.point);
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

                if (checkedCell == currEndCell) {
                    checkedCell.parent = currCell;
                    destinationReached = true;

                    Debug.Log("Destination cell reached");
                    EmulateVisitedPath(visitedOrder, destinationReached);
                    return;
                } else if (closedList[(int)checkedCell.point.x, (int)checkedCell.point.y] == false && !checkedCell.isObstacleCell) {
                    float newHeuristicCost = CalculateHeuristic(checkedCell.point);
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

                if (checkedCell == currEndCell) {
                    checkedCell.parent = currCell;
                    destinationReached = true;

                    Debug.Log("Destination cell reached");
                    EmulateVisitedPath(visitedOrder, destinationReached);
                    return;
                } else if (closedList[(int)checkedCell.point.x, (int)checkedCell.point.y] == false && !checkedCell.isObstacleCell) {
                    float newHeuristicCost = CalculateHeuristic(checkedCell.point);
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

                if (checkedCell == currEndCell) {
                    checkedCell.parent = currCell;
                    destinationReached = true;

                    Debug.Log("Destination cell reached");
                    EmulateVisitedPath(visitedOrder, destinationReached);
                    return;
                } else if (closedList[(int)checkedCell.point.x, (int)checkedCell.point.y] == false && !checkedCell.isObstacleCell) {
                    float newHeuristicCost = CalculateHeuristic(checkedCell.point);
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

                if (checkedCell == currEndCell) {
                    checkedCell.parent = currCell;
                    destinationReached = true;

                    Debug.Log("Destination cell reached");
                    EmulateVisitedPath(visitedOrder, destinationReached);
                    return;
                } else if (closedList[(int)checkedCell.point.x, (int)checkedCell.point.y] == false && !checkedCell.isObstacleCell) {
                    float newHeuristicCost = CalculateHeuristic(checkedCell.point);
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
            EmulateVisitedPath(visitedOrder, destinationReached);
        }
    }
    #endregion
}
