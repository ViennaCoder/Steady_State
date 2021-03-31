using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Winning : MonoBehaviour
{
    public bool shallCheck;

    GameMaster gameMasterScript;
    float timer = 0.0f;
    float waitTime = 1.0f;

    void Start()
    {
        shallCheck = true;
        GameObject _GameMaster = GameObject.Find("GameMaster");
        gameMasterScript = _GameMaster.GetComponent<GameMaster>();
    }

    // Update is called once per frame
    void Update()
    {
        if (shallCheck && (Physics.OverlapBox(transform.position, transform.lossyScale / 2).Length == 0) && (gameMasterScript.isGameOver == 0))
        {   // no collission any more -> lost game
            timer += Time.deltaTime;

            if (timer > waitTime)
            {
                GetComponent<Renderer>().material.color = new Color(1, 0, 0, 0.2627f);
                if (transform.parent.name == "Tower1")
                {
                    gameMasterScript.isGameOver = 1;
                }
                else if (transform.parent.name == "Tower2")
                {
                    gameMasterScript.isGameOver = 2;
                }
            }
        }
        else
        {
            timer = 0.0f;
        }
    }

}
