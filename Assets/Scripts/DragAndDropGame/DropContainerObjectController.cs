using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropContainerObjectController : MonoBehaviour
{
    public DragAndDropObjectController dragAndDropObjectController;
    private PolygonCollider2D polygonCollider2D;
    public SpriteRenderer spriteRenderer;
    private void Start()
    {
        polygonCollider2D = GetComponent<PolygonCollider2D>();
    }

    private void Update()
    {
        if(polygonCollider2D==null)
        {
            return;
        }

        if (!Input.GetMouseButton(0))
        {
            polygonCollider2D.enabled = true;
        }
        else
        {
            polygonCollider2D.enabled = false;
        }


    }
}