using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    private static SelectionManager instance;

    public static int hoverX = -1;
    public static int hoverY = -1;
    public static int selectionX = -1;
    public static int selectionY = -1;

    private GameObject lastHover;
    private GameObject selection;

    [SerializeField]
    private GameObject selectionPrefab;
    private float selectionObjectHeight = 0.1f;

    private GameObject selectionObject;

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

        hoverX = -1;
        hoverY = -1;
        selectionX = -1;
        selectionY = -1;
    }

    // Update is called once per frame
    void Update()
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

        if (lastHover != null)
        {
            GridSpot spot = lastHover.GetComponent<GridSpot>();
            if (spot != null)
            {
                hoverX = spot.x;
                hoverY = spot.y;
            }
        }
    }

    public static SelectionManager GetInstance()
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
        }
    }
}
