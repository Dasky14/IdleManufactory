using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSpot : MonoBehaviour
{
    public int x;
    public int y;
    public bool isOccupied = false;

    public GameObject buildingContainer;
    public GameObject groundModel;

    public GameObject GetBuilding()
    {
        return buildingContainer.transform.GetChild(0).gameObject;
    }
}
