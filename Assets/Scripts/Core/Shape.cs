using UnityEngine;

public class Shape : MonoBehaviour
{
    public bool canRotate = true;
    public Vector3 queueOffset;

    private GameObject[] glowSquareFX;
    public string glowSquareTag;

    private void Start()
    {
        //InvokeRepeating("MoveDown", 0f, 0.5f);
        if (glowSquareTag != null && glowSquareTag != "")
        {
            glowSquareFX = GameObject.FindGameObjectsWithTag(glowSquareTag);
        }
    }

    private void Move(Vector3 direction)
    {
        transform.position += direction;
    }

    public void MoveLeft()
    {
        Move(new Vector3(-1, 0, 0));
    }

    public void MoveRight()
    {
        Move(new Vector3(1, 0, 0));
    }

    public void MoveDown()
    {
        Move(new Vector3(0, -1, 0));
    }

    public void MoveUp()
    {
        Move(new Vector3(0, 1, 0));
    }

    //Rotating Methods
    public void RotateRight()
    {
        if (canRotate)
            transform.Rotate(new Vector3(0, 0, -90));
    }

    public void RotateLeft()
    {
        if (canRotate)
            transform.Rotate(new Vector3(0, 0, 90));
    }

    public void Rotate(bool clockwise)
    {
        if (clockwise)
            RotateRight();
        else
            RotateLeft();
    }

    public void LandShapeFX()
    {
        int i = 0;
        foreach (Transform child in gameObject.transform)
        {
            if (glowSquareFX[i])
            {
                glowSquareFX[i].transform.position = new Vector3(child.position.x, child.position.y, -5);
                ParticlePlayer particlePlayer = glowSquareFX[i].GetComponent<ParticlePlayer>();
                if (particlePlayer)
                    particlePlayer.Play();

                i++;
            }
        }
    }
}