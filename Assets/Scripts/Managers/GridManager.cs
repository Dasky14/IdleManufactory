using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    private static GridManager instance;
    public static GridManager GetInstance()
    {
        return instance;
    }

    // grid size variables
    public int xSize = 5;
    public int ySize = 5;

    public GameObject tilePrefab;
    private List<GameObject> _gridObjects;



    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("Too many gridmanagers, deleting...", gameObject);
            Destroy(this);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _gridObjects = new List<GameObject>();
        for (int i = 0; i < ySize; i++)
        {
            for (int j = 0; j < xSize; j++)
            {
                GameObject go = Instantiate(tilePrefab, transform);
                go.transform.localPosition = new Vector3(j, 0, i);
                go.name = $"Tile_{j}_{i}";

                GridSpot spotScript = go.GetComponent<GridSpot>();
                spotScript.x = j;
                spotScript.y = i;

                _gridObjects.Add(go);
            }
        }
    }

    public GameObject GetGridObject(int x, int y)
    {
        if (x < 0 || x >= xSize || y < 0 || y >= ySize)
        {
            return null;
        }
        return _gridObjects[y * xSize + x];
    }

    public Building GetGridBuilding(int x, int y)
    {
        if (x < 0 || x >= xSize || y < 0 || y >= ySize)
        {
            return null;
        }

        GameObject go = GetGridObject(x, y);
        if (go != null)
        {
            GridSpot spot = go.GetComponent<GridSpot>();
            if (spot.isOccupied)
            {
                return spot.GetBuilding().GetComponent<Building>();
            }
        }
        return null;
    }

    public void CreateBuilding(GameObject building, int x, int y)
    {
        if (x < 0 || x >= xSize || y < 0 || y >= ySize)
        {
            Debug.LogWarning("Spot is out of bounds!", gameObject);
            return;
        }

        GridSpot spot = GetGridObject(x, y).GetComponent<GridSpot>();
        if (spot.isOccupied)
        {
            Debug.LogWarning("Spot is already occupied!", gameObject);
            return;
        }
        GameObject go = Instantiate(building, spot.buildingContainer.transform);
        Building buildingComponent = go.GetComponent<Building>();
        if (buildingComponent != null)
        {
            buildingComponent.SetPosition(x, y);
            spot.isOccupied = true;
        }
        else
        {
            Debug.LogWarning($"Prefab does not have a Building component! Name: {go.name}");
            Destroy(go);
        }

        buildingComponent.OnBuild();
    }
}
