using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBelt : Building
{
    [SerializeField]
    private GameObject basePlate;
    [SerializeField]
    private GameObject[] edges;

    public override void OnBuild()
    {
        UpdateEdges();
        UpdateNearby();
    }

    public override void Rotate(int amount)
    {
        if (basePlate != null)
        {
            int rotationCount = amount % 4;
            int currentRotation = (int)direction;
            int newDirection = (currentRotation + rotationCount) % 4;

            basePlate.transform.rotation = Quaternion.identity;
            basePlate.transform.Rotate(Vector3.up, 90 * newDirection);

            direction = (BuildingDirection)newDirection;

            UpdateEdges();
        }
    }

    public override void SetRotation(BuildingDirection direction)
    {
        throw new System.NotImplementedException();
    }

    public void UpdateEdges()
    {
        GridManager gridManager = GridManager.GetInstance();

        int edge = 0;
        (int x, int y)[] directions = new (int x, int y)[4] { (0, 1), (1, 0), (0, -1), (-1, 0) };

        foreach ((int x, int y) dir in directions)
        {
            Building building = gridManager.GetGridBuilding(gridPosX + dir.x, gridPosY + dir.y);
            if (building != null && building is ConveyorBelt)
            {
                if ((int)building.direction != (edge + 2) % 4  && (int)direction == edge)
                    edges[edge].SetActive(true);
                else if ((int)building.direction == (edge + 2) % 4  && (int)direction != edge)
                    edges[edge].SetActive(true);
                else
                    edges[edge].SetActive(false);
            }
            else
                edges[edge].SetActive(false);

            edge++;
        }
    }

    private void UpdateNearby()
    {
        GridManager gridManager = GridManager.GetInstance();

        (int x, int y)[] directions = new (int x, int y)[4] { (0, 1), (1, 0), (0, -1), (-1, 0) };

        foreach ((int x, int y) dir in directions)
        {
            Building building = gridManager.GetGridBuilding(gridPosX + dir.x, gridPosY + dir.y);
            if (building != null && building is ConveyorBelt)
            {
                Debug.Log("Found.");
                (building as ConveyorBelt).UpdateEdges();
            }
        }
    }
}
