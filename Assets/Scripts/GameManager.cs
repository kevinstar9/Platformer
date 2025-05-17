using System.Collections;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public PlayerMove playerMove;
    public GameObject[] Stages;
    public int stageIndex;
    void Awake()
    {
        playerMove = FindObjectOfType<PlayerMove>();
    }
    public void VelocityZero()
    {
        playerMove.rigid.velocity = Vector2.zero;
    }
    void PlayerReposition()
    {
        VelocityZero();
        playerMove.transform.position = new Vector3(0, 1, 0);
    }
    public void NextStage()
    {
        //change Stage
        if (stageIndex < Stages.Length - 1)
        {
            Stages[stageIndex].SetActive(false);
            stageIndex++;
            Stages[stageIndex].SetActive(true);
            PlayerReposition();
        }
        else //GameClear
        {
            //Player Control Lock
            Time.timeScale = 0;
        }
    }   
}
