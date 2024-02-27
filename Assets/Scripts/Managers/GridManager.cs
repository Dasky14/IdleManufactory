using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    private static GridManager instance;

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

    public static GridManager GetInstance()
    {
        return instance;
    }

    public GameObject GetGridObject(int x, int y)
    {
        return _gridObjects[y * xSize + x];
    }

    public void CreateBuilding(GameObject building, int x, int y)
    {
        GridSpot spot = GetGridObject(x, y).GetComponent<GridSpot>();
        GameObject go = Instantiate(building, spot.buildingContainer.transform);
        go.GetComponent<Building>().SetPosition(x, y);
        
    }
}
