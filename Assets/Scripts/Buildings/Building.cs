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

    [HideInInspector]
    public int gridPosX;
    [HideInInspector]
    public int gridPosY;


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

    public abstract void OnBuild();
    public abstract void Rotate(int amount);
    public abstract void SetRotation(BuildingDirection direction);
}
