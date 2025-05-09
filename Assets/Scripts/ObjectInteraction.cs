using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectInteraction : MonoBehaviour
{
    [SerializeField] 
    private LayerMask interactableLayer;
    private GameObject lastHoveredObject;

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, interactableLayer))
        {
            if (lastHoveredObject != hit.collider.gameObject)
            {
                if (lastHoveredObject != null)
                {
                    //Not implemented yet
                }

                lastHoveredObject = hit.collider.gameObject;
                Debug.Log("Hovering over: " + lastHoveredObject.name);
            }
        }
        else
        {
            if (lastHoveredObject != null)
            {
                //Not implemented yet
                lastHoveredObject = null;
            }
        }

        if (Input.GetMouseButtonDown(0)) // 0 is left click
        {
            if (lastHoveredObject != null)
            {
                OnMouseDown();
            }
        }
    }

    public void OnMouseDown()
    {
        GameHandler.Instance.AddClue();
        Destroy(lastHoveredObject);
        lastHoveredObject = null;
    }
}