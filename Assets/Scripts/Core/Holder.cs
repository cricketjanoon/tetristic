using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Holder : MonoBehaviour
{
    public Transform holderXform;
    public Shape heldShape = null;
    float scale = 0.5f;
    public bool canRelease = false;

    public void Catch(Shape shape)
    {
        if (!shape)
        {
            Debug.LogWarning("HOLDER WARNING! " + shape.name + " is invalid!");
            return;
        }

        if (!holderXform)
        {
            Debug.LogWarning("HOLDER WARNING! Missing Holder transform!");
            return;
        }

        if (heldShape)
        {
            Debug.LogWarning("HOLDER WARNING!  Release a shape before trying to hold.");
            return;
        }
        else
        {
            shape.transform.position = holderXform.position + shape.queueOffset;
            shape.transform.localScale = new Vector3(scale, scale, scale);
            heldShape = shape;
        }
    }

    public Shape Release()
    {
        heldShape.transform.localScale = Vector3.one;
        Shape shape = heldShape;
        heldShape = null;

        canRelease = false;

        return shape;
    }
}