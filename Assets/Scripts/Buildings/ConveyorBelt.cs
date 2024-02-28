using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBelt : Building
{
    [Header("References")]
    [SerializeField]
    private GameObject basePlate;
    [SerializeField]
    private GameObject itemLocation;
    public Vector3 GetItemLocation() { return itemLocation.transform.position; }
    [SerializeField]
    private GameObject[] edges;

    [Header("Status")]
    public List<GameObject> items;
    public bool canPassItem = false;


    public bool hasItem { get { return (items.Count > 0 && items[0] != null); } }


    // Private status variables
    private bool executingTick = false;
    private ConveyorBelt nextConveyorBelt;

    public override void OnBuild()
    {
        items = new List<GameObject>();

        FindStartingRotation();
        UpdateConnections();
        UpdateNearbyBelts();

        TickManager.GetInstance().onTick += OnTick;
    }

    public override void OnDemolish()
    {
        TickManager.GetInstance().onTick -= OnTick;
        UpdateNearbyBelts();
    }

    private void OnTick()
    {
        executingTick = false;
        if (hasItem)
            executingTick = true;
    }

    private void Update()
    {
        canPassItem = (nextConveyorBelt && !nextConveyorBelt.hasItem);

        if (!executingTick) return;

        if (hasItem && canPassItem)
        {
            TickManager tickManager = TickManager.GetInstance();

            if (tickManager.animateItemMovement)
                StartCoroutine(MoveItem(items[0], nextConveyorBelt.GetItemLocation(), (1f / tickManager.ticksPerSecond) / 2));
            else
                items[0].transform.position = nextConveyorBelt.GetItemLocation();

            nextConveyorBelt.items.Add(items[0]);
            items.RemoveAt(0);

            // Done with the tick
            executingTick = false;
        }
    }

    public override void Rotate(int amount)
    {
        if (basePlate != null)
        {
            RotateWithoutUpdate(amount);

            UpdateConnections();
            UpdateNearbyBelts();
        }
    }

    public void RotateWithoutUpdate(int amount)
    {
        if (basePlate != null)
        {
            int rotationCount = amount % 4;
            int currentRotation = (int)direction;
            int newDirection = (currentRotation + rotationCount) % 4;

            basePlate.transform.rotation = Quaternion.identity;
            basePlate.transform.Rotate(Vector3.up, 90 * newDirection);

            direction = (BuildingDirection)newDirection;
        }
    }

    public override void SetRotation(BuildingDirection direction)
    {
        if (basePlate != null)
        {
            SetRotationWithoutUpdate(direction);

            UpdateConnections();
            UpdateNearbyBelts();
        }
    }

    public void SetRotationWithoutUpdate(BuildingDirection direction)
    {
        if (basePlate != null)
        {
            basePlate.transform.rotation = Quaternion.identity;
            basePlate.transform.Rotate(Vector3.up, 90 * (int)direction);

            this.direction = direction;
        }
    }
    public override bool[] GetOutputDirections()
    {
        bool[] outputs = new bool[4];
        outputs[(int)direction] = true;
        return outputs;
    }

    public void UpdateConnections()
    {
        for (int i = 0; i < 4; i++)
        {
            Building building = GetBuildingInDirection((BuildingDirection)i);
            if (building != null && building is ConveyorBelt)
            {
                if ((int)building.direction != (i + 2) % 4  && (int)direction == i)
                    edges[i].SetActive(true);
                else if ((int)building.direction == (i + 2) % 4  && (int)direction != i)
                    edges[i].SetActive(true);
                else
                    edges[i].SetActive(false);
            }
            else if (building != null && GettingOutputFromDirection((BuildingDirection)i))
                edges[i].SetActive(true);
            else
                edges[i].SetActive(false);
        }

        Building frontBuilding = GetBuildingInDirection(direction);
        if (frontBuilding != null && frontBuilding is ConveyorBelt)
            nextConveyorBelt = frontBuilding as ConveyorBelt;
    }

    private void FindStartingRotation()
    {
        int newDirection = 0;
        for (int i = 0; i < 4; i++)
        {
            Building building = GetBuildingInDirection((BuildingDirection)i);
            if (building != null && building is ConveyorBelt)
            {
                RotateWithoutUpdate((newDirection + 2) % 4);

                ConveyorBelt previousBelt = building as ConveyorBelt;
                if (previousBelt.GetConnectionCount() <= 1)
                    previousBelt.SetRotationWithoutUpdate((BuildingDirection)((newDirection + 2) % 4));

                return;
            }
            newDirection++;
        }
    }

    public int GetConnectionCount()
    {
        int count = 0;
        foreach (GameObject edge in edges)
        {
            if (edge.activeSelf)
                count++;
        }
        return count;
    }
}
