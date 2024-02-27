using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateBuildingAtSelection : MonoBehaviour
{
    public GameObject buildingPrefab;

    public void CreateBuilding()
    {
        if (buildingPrefab != null)
        {
            GridManager gridManager = GridManager.GetInstance();
            gridManager.CreateBuilding(buildingPrefab, ControlManager.selectionX, ControlManager.selectionY);
        }
        else
        {
            Debug.LogWarning("No building prefab set!", gameObject);
        }
    }
}
