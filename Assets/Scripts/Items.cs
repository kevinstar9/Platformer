using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Items : MonoBehaviour
{
    public GameObject coal;
    private PlayerMove playerMove;
    void Awake()
    {

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerMove player = collision.GetComponent<PlayerMove>();
        if (player != null)
        {
            player.hasFireAttack = true;
            Destroy(gameObject);
        }
    }
}
