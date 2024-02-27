using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ControlManager : MonoBehaviour
{
    private static ControlManager instance;
    public static ControlManager GetInstance()
    {
        return instance;
    }

    public static int selectionX = -1;
    public static int selectionY = -1;
    private static bool _selectionActive = false;
    public static bool selectionActive { get { return _selectionActive; } }

    private GameObject selection;
    private GameObject buildingPrefab;

    [SerializeField]
    private GameObject selectionPrefab;
    [SerializeField]
    private float selectionObjectHeight = 0.1f;

    private GameObject selectionObject;

    public enum ControlMode
    {
        Select,
        Build,
        Rotate,
        Demolish
    }
    private ControlMode controlMode;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("Too many selection managers, deleting...", gameObject);
            Destroy(this);
        }

        selectionX = -1;
        selectionY = -1;
    }

    // Update is called once per frame
    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        switch (controlMode)
        {
            case ControlMode.Select:
                HandleSelectMode();
                break;
            case ControlMode.Build:
                HandleBuildMode();
                break;
            case ControlMode.Rotate:
                HandleRotateMode();
                break;
            case ControlMode.Demolish:
                //HandleDemolishMode();
                break;
        }
    }

    private void HandleSelectMode()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            LayerMask mask = LayerMask.GetMask("ClickableGround");
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
            {
                GridSpot spot = hit.collider.GetComponentInParent<GridSpot>();
                if (spot != null)
                {
                    if (selectionX == spot.x && selectionY == spot.y)
                    {
                        Unselect();
                    }
                    else
                    {
                        selectionX = spot.x;
                        selectionY = spot.y;
                        SetSelection(spot.x, spot.y);
                    }
                }
            }
            else
            {
                Unselect();
            }
        }
    }

    private void HandleBuildMode()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (buildingPrefab == null)
            {
                Debug.LogWarning("No building prefab set!", gameObject);
                return;
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            LayerMask mask = LayerMask.GetMask("ClickableGround");

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
            {
                GridSpot spot = hit.collider.GetComponentInParent<GridSpot>();
                if (spot != null && !spot.isOccupied)
                {
                    GridManager gridManager = GridManager.GetInstance();
                    gridManager.CreateBuilding(buildingPrefab, spot.x, spot.y);
                }
            }
        }
    }

    private void HandleRotateMode()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            LayerMask mask = LayerMask.GetMask("ClickableGround");

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
            {
                GridSpot spot = hit.collider.GetComponentInParent<GridSpot>();
                if (spot != null && spot.isOccupied)
                {
                    spot.GetBuilding().GetComponent<Building>().Rotate(1);
                }
            }
        }
    }

    public void SetControlMode(ControlMode mode)
    {
        controlMode = mode;
        if (mode != ControlMode.Select)
            Unselect();
    }

    public void SetBuildingPrefab(GameObject prefab)
    {
        buildingPrefab = prefab;
    }

    private void SetSelection(int x, int y)
    {
        selection = GridManager.GetInstance().GetGridObject(x, y);
        Vector3 pos = selection.transform.position;
        pos.y = selectionObjectHeight;

        if (selectionObject == null)
            selectionObject = Instantiate(selectionPrefab, pos, Quaternion.identity);
        else
            selectionObject.transform.position = pos;

        _selectionActive = true;
    }

    private void Unselect()
    {
        if (selection != null)
        {
            selectionX = -1;
            selectionY = -1;
            Vector3 pos = selection.transform.position;
            pos.y = 0;
            selection.transform.position = pos;
            selection = null;

            if (selectionObject != null)
                Destroy(selectionObject);
            _selectionActive = false;
        }
    }
}
