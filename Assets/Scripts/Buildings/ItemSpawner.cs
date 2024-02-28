using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemSpawner : Building
{
    [Header("References")]
    [SerializeField]
    private GameObject visualsObject;
    [SerializeField]
    private GameObject itemLocation;
    public Vector3 GetItemLocation() { return itemLocation.transform.position; }

    [Header("Config")]
    public GameObject itemToSpawn;
    public int spawnInterval = 5;

    // Private status variables
    private int spawnTimer;
    private bool canSpawn = true;

    public override void OnBuild()
    {
        UpdateNearbyBelts();
        TickManager.GetInstance().onTick += OnTick;

        spawnTimer = spawnInterval;
    }

    public override void OnDemolish()
    {
        UpdateNearbyBelts();
        TickManager.GetInstance().onTick -= OnTick;
    }

    private void OnTick()
    {
        if (spawnTimer > 0)
        {
            spawnTimer--;
        }
        else if (!canSpawn)
        {
            canSpawn = true;
        }
    }
    
    void Update()
    {
        if (!canSpawn) return;

        Building building = GetBuildingInDirection(direction);
        if (building != null && building is ConveyorBelt)
        {
            ConveyorBelt belt = building as ConveyorBelt;
            if (!belt.hasItem)
            {
                GameObject item = Instantiate(itemToSpawn, itemLocation.transform.position, Quaternion.identity);
                belt.items.Add(item);
                canSpawn = false;
                spawnTimer = spawnInterval;

                TickManager tickManager = TickManager.GetInstance();
                if (tickManager.animateItemMovement)
                    StartCoroutine(MoveItem(item, belt.GetItemLocation(), (1f / tickManager.ticksPerSecond) / 2));
                else
                    item.transform.position = belt.GetItemLocation();
            }
        }
    }

    public override void Rotate(int amount)
    {
        if (visualsObject != null)
        {
            int rotationCount = amount % 4;
            int currentRotation = (int)direction;
            int newDirection = (currentRotation + rotationCount) % 4;

            visualsObject.transform.rotation = Quaternion.identity;
            visualsObject.transform.Rotate(Vector3.up, 90 * newDirection);

            direction = (BuildingDirection)newDirection;

            UpdateNearbyBelts();
        }
    }

    public override void SetRotation(BuildingDirection direction)
    {
        if (visualsObject != null)
        {
            visualsObject.transform.rotation = Quaternion.identity;
            visualsObject.transform.Rotate(Vector3.up, 90 * (int)direction);

            this.direction = direction;
        }
    }

    public override bool[] GetOutputDirections()
    {
        bool[] outputs = new bool[4];
        outputs[(int)direction] = true;
        return outputs;
    }

}
