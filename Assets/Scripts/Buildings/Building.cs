using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    [HideInInspector]
    public int gridPosX;
    [HideInInspector]
    public int gridPosY;


    public bool acceptsItems = false;
    public bool producesItems = false;


    public void SetPosition(int x, int y)
    {
        gridPosX = x;
        gridPosY = y;
    }
}
