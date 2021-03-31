using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnTower : MonoBehaviour
{
    public Collider border;
    Vector3 mySide;
    Vector3 mouseSide;
    float _fixedDeltaTime;
    public bool isTurningOK;
    bool isKeyDown;

    // Start is called before the first frame update
    void Start()
    {
        mySide = border.transform.InverseTransformPoint(transform.position);
        _fixedDeltaTime = Time.fixedDeltaTime;
        isTurningOK = true;
        isKeyDown = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isTurningOK)
        {
            
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
            {
                mouseSide = border.transform.InverseTransformPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition + new Vector3(0,0,3)));
                Physics.autoSimulation = false;
                isKeyDown = true;
            }
            if (isKeyDown)
            {
                if ((((mySide.x < 0) && (mouseSide.x < 0)) || ((mySide.x > 0) && (mouseSide.x > 0))) && isTurningOK)
                {   // same side of the border
                    if (Input.GetKey(KeyCode.A))
                    {
                        transform.eulerAngles += new Vector3(0, 0.1f, 0);
                    }
                    if (Input.GetKey(KeyCode.D))
                    {
                        transform.eulerAngles += new Vector3(0, -0.1f, 0);
                    }

                    if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
                    {
                        isKeyDown = false;
                        StartCoroutine(TurnDeltaTimeOn());
                    }
                }
            }
        }
    }

    IEnumerator TurnDeltaTimeOn()
    {
        isTurningOK = false;
        yield return new WaitForSeconds(0.05f);
        Physics.autoSimulation = true;
        isTurningOK = true;
    }

    IEnumerator WaitSomeTime(float sec)
    {
        isTurningOK = false;
        yield return new WaitForSeconds(sec);
        isTurningOK = true;
    }
}
