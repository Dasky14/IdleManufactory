using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ControlManager : MonoBehaviour
{
    private static ControlManager instance;

    public static int selectionX = -1;
    public static int selectionY = -1;
    private static bool _selectionActive = false;
    public static bool selectionActive { get { return _selectionActive; } }

    private GameObject lastHover;
    private GameObject selection;

    [SerializeField]
    private GameObject selectionPrefab;
    [SerializeField]
    private float selectionObjectHeight = 0.1f;

    private GameObject selectionObject;

    private enum ControlMode
    {
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

    private void HandleBuildMode()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        LayerMask mask = LayerMask.GetMask("ClickableGround");

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
        {
            if (Input.GetMouseButtonDown(0))
            {
                GridSpot spot = hit.collider.GetComponentInParent<GridSpot>();
                if (spot != null)
                {
                    if (selectionX == spot.x && selectionY == spot.y)
                    {
                        Debug.Log($"Unselected spot: {spot.x}, {spot.y}");
                        Unselect();
                    }
                    else
                    {
                        Debug.Log($"Selected spot: {spot.x}, {spot.y}");
                        selectionX = spot.x;
                        selectionY = spot.y;
                        SetSelection(spot.x, spot.y);
                    }
                }
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
                Unselect();
        }
    }

    private void HandleRotateMode()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        LayerMask mask = LayerMask.GetMask("ClickableGround");

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
        {
            GameObject hover = hit.collider.transform.parent.gameObject;
            if (lastHover != hover)
                lastHover = hover;

            if (Input.GetMouseButtonDown(0))
            {
                GridSpot spot = hit.collider.GetComponentInParent<GridSpot>();
                if (spot != null)
                {
                    spot.GetBuilding().GetComponent<Building>().Rotate(1);
                }
            }
        }
    }

    public static ControlManager GetInstance()
    {
        return instance;
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
            Debug.Log($"Unselected spot: {selectionX}, {selectionY}");
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
