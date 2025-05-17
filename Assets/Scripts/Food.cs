using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    private PlayerHP playerHp;
    void Awake()
    {
        playerHp = GetComponent<PlayerHP>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerHP hp = collision.GetComponent<PlayerHP>();
            if (hp != null)
            {
                hp.Heal(1); // 위에서 만든 Heal 함수 사용
            }
            Destroy(gameObject);
        }
    }   
}