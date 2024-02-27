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
        UpdateNearby();

        TickManager.GetInstance().onTick += OnTick;
    }

    public override void OnDemolish()
    {
        TickManager.GetInstance().onTick -= OnTick;
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
                StartCoroutine(MoveItem(items[0], nextConveyorBelt.itemLocation.transform.position, (1f / tickManager.ticksPerSecond) / 2));
            else
                items[0].transform.position = nextConveyorBelt.itemLocation.transform.position;

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
            UpdateNearby();
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
            UpdateNearby();
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

    public void UpdateConnections()
    {
        GridManager gridManager = GridManager.GetInstance();

        int edge = 0;
        (int x, int y)[] directions = new (int x, int y)[4] { (0, 1), (1, 0), (0, -1), (-1, 0) };

        foreach ((int x, int y) dir in directions)
        {
            Building building = gridManager.GetGridBuilding(gridPosX + dir.x, gridPosY + dir.y);
            if (building != null && building is ConveyorBelt)
            {
                if ((int)building.direction != (edge + 2) % 4  && (int)direction == edge)
                    edges[edge].SetActive(true);
                else if ((int)building.direction == (edge + 2) % 4  && (int)direction != edge)
                    edges[edge].SetActive(true);
                else
                    edges[edge].SetActive(false);
            }
            else
                edges[edge].SetActive(false);

            edge++;
        }

        (int x, int y) front = directions[(int)direction];
        Building frontBuilding = gridManager.GetGridBuilding(gridPosX + front.x, gridPosY + front.y);
        if (frontBuilding != null && frontBuilding is ConveyorBelt)
        {
            nextConveyorBelt = frontBuilding as ConveyorBelt;
        }
    }

    private void UpdateNearby()
    {
        GridManager gridManager = GridManager.GetInstance();

        (int x, int y)[] directions = new (int x, int y)[4] { (0, 1), (1, 0), (0, -1), (-1, 0) };

        foreach ((int x, int y) dir in directions)
        {
            Building building = gridManager.GetGridBuilding(gridPosX + dir.x, gridPosY + dir.y);
            if (building != null && building is ConveyorBelt)
                (building as ConveyorBelt).UpdateConnections();
        }
    }

    private void FindStartingRotation()
    {
        GridManager gridManager = GridManager.GetInstance();

        (int x, int y)[] directions = new (int x, int y)[4] { (0, 1), (1, 0), (0, -1), (-1, 0) };

        int newDirection = 0;
        foreach ((int x, int y) dir in directions)
        {
            Building building = gridManager.GetGridBuilding(gridPosX + dir.x, gridPosY + dir.y);
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

    private IEnumerator MoveItem(GameObject item, Vector3 target, float overTime)
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
}
