using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public float speed = 2f;
    public float minY = 2.5f;
    public float maxY = 10.5f;

    private bool movingUp = true;

    void Update()
    {
        Vector3 pos = transform.position;
        float targetY = movingUp ? maxY : minY;

        pos.y = Mathf.MoveTowards(pos.y, targetY, speed * Time.deltaTime);
        transform.position = pos;

        if (Mathf.Approximately(pos.y, maxY)) movingUp = false;
        else if (Mathf.Approximately(pos.y, minY)) movingUp = true;
    }
}
