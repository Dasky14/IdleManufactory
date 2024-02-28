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
    public Building building;

    public GameObject GetBuildingGameObject()
    {
        return building.gameObject;
    }

    public void DemolishBuilding()
    {
        if (isOccupied)
        {
            isOccupied = false;
            building.OnDemolish();
            Destroy(GetBuildingGameObject());
        }
    }
}
