using UnityEngine;

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
            lastHoveredObject = hit.collider.gameObject;
        }
        else
        {
            lastHoveredObject = null;
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
        AudioSource audio = lastHoveredObject.transform.parent.GetComponent<AudioSource>();
        audio.Play();

        GameHandler.Instance.AddClue();
        Destroy(lastHoveredObject);
        lastHoveredObject = null;
    }
}