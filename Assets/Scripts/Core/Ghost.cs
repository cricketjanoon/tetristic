using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    private Shape ghostShape;
    private bool hitBottom = false;

    public Color color = new Color(1f, 1f, 1f, 2f);

    public void DrawGhost(Shape originalShape, Board gameBoard)
    {
        if (!ghostShape)
        {
            ghostShape = Instantiate(originalShape, originalShape.transform.position, originalShape.transform.rotation) as Shape;
            ghostShape.gameObject.name = "GhostShape";

            SpriteRenderer[] allRenders = ghostShape.GetComponentsInChildren<SpriteRenderer>();

            foreach (SpriteRenderer sprite in allRenders)
            {
                sprite.color = color;
            }
        }
        else
        {
            ghostShape.transform.position = originalShape.transform.position;
            ghostShape.transform.rotation = originalShape.transform.rotation;
            ghostShape.transform.localScale = Vector3.one;
        }

        hitBottom = false;

        while (!hitBottom)
        {
            ghostShape.MoveDown();
            if (!gameBoard.IsValidPosition(ghostShape))
            {
                ghostShape.MoveUp();
                hitBottom = true;
            }
        }
    }

    public void Reset()
    {
        Destroy(ghostShape.gameObject);
    }
}