using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Building : MonoBehaviour
{
    public enum BuildingDirection
    {
        North,
        East,
        South,
        West,
        None
    }

    [Header("Coordinates")]
    [HideInInspector]
    public int gridPosX;
    [HideInInspector]
    public int gridPosY;

    [Header("Base Building Options")]
    public BuildingDirection direction = BuildingDirection.None;
    public bool hasItemInput = false;
    public bool hasItemOutput = false;
    public bool isProduction = false;
    public bool isStorage = false;

    public void SetPosition(int x, int y)
    {
        gridPosX = x;
        gridPosY = y;
    }

    protected void UpdateNearbyBelts()
    {
        for (int i = 0; i < 4; i++)
        {
            Building building = GetBuildingInDirection((BuildingDirection)i);
            if (building != null && building is ConveyorBelt)
                (building as ConveyorBelt).UpdateConnections();
        }
    }

    /// <summary>
    /// Get the next building in a direction
    /// </summary>
    /// <param name="dir"></param>
    protected Building GetBuildingInDirection(BuildingDirection dir)
    {
        GridManager gridManager = GridManager.GetInstance();
        (int x, int y)[] directions = new (int x, int y)[4] { (0, 1), (1, 0), (0, -1), (-1, 0) };
        (int x, int y) direction = directions[(int)dir];
        return gridManager.GetGridBuilding(gridPosX + direction.x, gridPosY + direction.y);
    }

    protected bool GettingOutputFromDirection(BuildingDirection dir)
    {
        Building building = GetBuildingInDirection(dir);
        bool[] buildingOutputs = building.GetOutputDirections();
        return buildingOutputs[((int)dir + 2) % 4];
    }

    protected IEnumerator MoveItem(GameObject item, Vector3 target, float overTime)
    {
        Vector3 source = item.transform.position;

        float startTime = Time.time;
        while (Time.time < startTime + overTime)
        {
            item.transform.position = Vector3.Lerp(source, target, (Time.time - startTime) / overTime);
            yield return null;
        }
        item.transform.position = target;
    }

    public abstract void OnBuild();
    public abstract void OnDemolish();
    public abstract void Rotate(int amount);
    public abstract void SetRotation(BuildingDirection direction);
    public abstract bool[] GetOutputDirections();
}
