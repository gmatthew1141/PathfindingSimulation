using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchAlgorithm : MonoBehaviour {

    public virtual float CalculateHeuristic(Vector2 currPoint, CellDetails endCell) {
        return 0;
    }

    public virtual void StartSearchAlgorithm(float x, float z, CellDetails startCell, CellDetails endCell, GameManager gameManager) {

    }
}
