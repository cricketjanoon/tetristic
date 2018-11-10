using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour
{
    public Shape[] shapes;
    public Transform[] queuedXForms = new Transform[3];

    Shape[] queuedShapes = new Shape[3];

    public float queueScale = 0.5f;

    public ParticlePlayer spawnFX;

    void Start()
    {
        InitQueue();
    }

    public Shape GetRandomShape()
    {
        int rand = UnityEngine.Random.Range(0, shapes.Length);
        if (shapes[rand])
            return shapes[rand];
        else
            return null;
    }

    public Shape SpawnShape()
    {
        Shape shape = null;
        //shape = Instantiate(GetRandomShape(), transform.position, Quaternion.identity);
        shape = GetQueuedShape();
        shape.transform.position = transform.position;
        //shape.transform.localScale = Vector3.one;

        StartCoroutine(GrowShape(shape, transform.position, 0.25f));

        if (spawnFX)
        {
            spawnFX.Play();
        }

        if (shape)
        {
            return shape;
        }
        else
        {
            return null;
        }
    }

    void InitQueue()
    {
        for (int i = 0; i < queuedShapes.Length; i++)
        {
            queuedShapes[i] = null;
        }

        FillQueue();
    }

    void FillQueue()
    {
        for (int i = 0; i < queuedShapes.Length; i++)
        {
            if (!queuedShapes[i])
            {
                queuedShapes[i] = Instantiate(GetRandomShape(), transform.position, Quaternion.identity) as Shape;
                queuedShapes[i].transform.position = queuedXForms[i].transform.position + queuedShapes[i].queueOffset;

                queuedShapes[i].transform.localScale = new Vector3(queueScale, queueScale, queueScale);
            }
        }
    }

    Shape GetQueuedShape()
    {
        Shape firstShape = null;
        if (queuedShapes[0])
        {
            firstShape = queuedShapes[0];
        }
        for (int i = 1; i < queuedShapes.Length; i++)
        {
            queuedShapes[i - 1] = queuedShapes[i];
            queuedShapes[i - 1].transform.position = queuedXForms[i - 1].position + queuedShapes[i].queueOffset;
        }

        queuedShapes[queuedShapes.Length - 1] = null;
        FillQueue();

        return firstShape;
    }

    IEnumerator GrowShape(Shape shape, Vector3 position, float growTime = 0.5f)
    {
        float size = 0f;
        growTime = Mathf.Clamp(growTime, 0.1f, 2f);
        float sizeDelta = 1 / growTime * Time.deltaTime;

        while (size < 1f)
        {
            shape.transform.localScale = new Vector3(size, size, size);
            size += sizeDelta;
            shape.transform.position = position;
            yield return null;
        }

        shape.transform.localScale = Vector3.one;
    }
}