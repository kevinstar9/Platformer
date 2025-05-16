using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            PlayerHP playerHP = collision.GetComponentInParent<PlayerHP>();
            if(playerHP != null)
            {
                Debug.Log("Trigger hit: " + collision.name);
                playerHP.TakeDamage(1, transform);
            }
        }  
    }  
}
