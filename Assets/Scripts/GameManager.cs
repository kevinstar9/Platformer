using System.Collections;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public PlayerMove playerMove;
    void Awake()
    {
        playerMove = FindObjectOfType<PlayerMove>();
    }
    public void VelocityZero()
    {
        playerMove.rigid.velocity = Vector2.zero;
    }
}
