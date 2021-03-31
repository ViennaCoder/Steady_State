using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public Material selectionMaterial;
    public int isBlocked = 0;

    bool hasMouseEntered = false;
    bool isDragging = false;
    bool isPhase1 = false;
    bool isPhase2 = false;
    bool isPhase3 = false;
    int maxBlocked = 2;
    float _fixedDeltaTime;
    float timer;
    Plane hPlane;
    Transform shadow;
    Vector3 oldPos;
    Vector3 oldMousePos;
    bool isFirstDrag;
    GameMaster gameMasterScript;
    TurnTower turnTowerScript;
    Rigidbody rigidBody;
    Renderer currentRenderer;
    Winning winningScript;
    AudioSource bumping;

    // Start is called before the first frame update
    void Start()
    {
        GameObject _GameMaster = GameObject.Find("GameMaster");
        gameMasterScript = _GameMaster.GetComponent<GameMaster>();

        Transform _winning = transform.parent.transform.Find("Winning");
        winningScript = _winning.GetComponent<Winning>();

        bumping = GetComponent<AudioSource>();
        bumping.volume = gameMasterScript.MasterVolume;

        _fixedDeltaTime = Time.fixedDeltaTime;
        isPhase1 = true;
        isBlocked = 0;
        oldMousePos = Vector3.up;
        isFirstDrag = true;
        rigidBody = GetComponent<Rigidbody>();
        currentRenderer = GetComponent<Renderer>();

    }

    // Update is called once per frame
    void Update()
    {
        float distance = 0;

        if (hasMouseEntered)
        {
            if (Input.GetMouseButtonDown(0) && !isDragging)
            {   // left MB down
                if (transform.parent.name == gameMasterScript.CurrentPlayer && isBlocked == 0)
                {
                    Physics.autoSimulation = false;
                    isDragging = true;
                    GetComponent<Rigidbody>().isKinematic = true;
                    oldPos = transform.position;
                    GetComponent<Collider>().enabled = false;
                    hPlane = new Plane(Vector3.up, Vector3.up * transform.position.y);

                    turnTowerScript = transform.parent.GetComponent<TurnTower>();
                    turnTowerScript.isTurningOK = false;
                    winningScript.shallCheck = false;

                    timer = 0f;
                }
            }
        }
        if (isDragging)
        {
            if (isPhase1)
            {
                transform.position = Vector3.Lerp(transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition + new Vector3(0, 0, 1)), timer);
                timer += Time.deltaTime / ((timer + 1) * 5f);

                Bounds b = gameMasterScript.GetBoundingBox(transform.parent.name).bounds;
                b.size += new Vector3(1, 1, 1);
                if (!b.Contains(transform.position))
                {   // switch to phase 2
                    isPhase2 = true;
                    isPhase1 = false;
                    hPlane = new Plane(Vector3.forward, Vector3.forward * transform.position.z);
                    transform.parent = null;
                    Physics.autoSimulation = true;
                    turnTowerScript.isTurningOK = true;
                }
            }
            else if (isPhase2)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                // Plane.Raycast stores the distance from ray.origin to the hit point in this variable:
                // if the ray hits the plane...
                if (hPlane.Raycast(ray, out distance))
                {
                    transform.position = ray.GetPoint(distance);

                    isPhase3 = true;
                    isPhase2 = false;
                    shadow = Instantiate(gameObject).transform;
                    shadow.parent = transform;
                    shadow.GetComponent<Renderer>().material = selectionMaterial;
                    shadow.localPosition = new Vector3();
                    transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
                    shadow.rotation = transform.rotation;
                    shadow.localScale = new Vector3(1, 1, 1);
                    shadow.position += new Vector3(0, 0, -transform.position.z);
                    Destroy(shadow.GetComponent<Collider>());
                    Destroy(shadow.GetComponent<Rigidbody>());
                    Destroy(shadow.GetComponent<Block>());
                }
            }
            else if (isPhase3)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                // Plane.Raycast stores the distance from ray.origin to the hit point in this variable:
                RaycastHit hitInfo;

                // if the ray hits the plane...
                if (hPlane.Raycast(ray, out distance))
                {
                    transform.position = ray.GetPoint(distance);
                }
                shadow.position = new Vector3(shadow.position.x, transform.position.y, shadow.position.z);


                if (Physics.OverlapBox(shadow.position, shadow.lossyScale / 2.0f, shadow.rotation).Length == 0)
                {
                    shadow.GetComponent<Renderer>().material.color = Color.green + new Color(0, 0, 0, -0.5f);
                }
                else
                {
                    shadow.GetComponent<Renderer>().material.color = Color.red + new Color(0, 0, 0, -0.5f);
                }
                if (Input.GetKeyDown(KeyCode.W))
                {
                    shadow.position += new Vector3(0, 0, 0.2f);
                }
                if (Input.GetKeyDown(KeyCode.S))
                {
                    shadow.position += new Vector3(0, 0, -0.2f);
                }
            }
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            winningScript.shallCheck = true;
            GetComponent<Collider>().enabled = true;
            if (isPhase1)
            {
                transform.position = oldPos;
                turnTowerScript.isTurningOK = true;
            }
            else if (isPhase2)
            {
                isPhase2 = false;
                isBlocked = maxBlocked;
                gameMasterScript.SwitchPlayer();
            }
            else if (isPhase3)
            {
                isPhase3 = false;

                if (shadow.GetComponent<Renderer>().material.color != (Color.red + new Color(0, 0, 0, -0.5f)))
                {
                    transform.position = shadow.position;
                }
                else
                {
                    GetComponent<Collider>().enabled = false;
                }
                Destroy(shadow.gameObject);

                if (gameMasterScript.GetBoundingBox("Tower1").bounds.Contains(transform.position))
                {
                    transform.parent = gameMasterScript.Tower1.transform;
                }
                if (gameMasterScript.GetBoundingBox("Tower2").bounds.Contains(transform.position))
                {
                    transform.parent = gameMasterScript.Tower2.transform;
                }
                isBlocked = maxBlocked;
                gameMasterScript.SwitchPlayer();
            }
            isPhase1 = true;
            isDragging = false;
            isFirstDrag = true;
            hPlane = new Plane(Vector3.up, Vector3.up * transform.position.y);
            rigidBody.isKinematic = false;
            Physics.autoSimulation = true;
        }

    }

    private void LateUpdate()
    {
        if (rigidBody.velocity.magnitude < 0.02f)
        {
            rigidBody.velocity = new Vector3();
        }
    }

    private void OnMouseEnter()
    {
        hasMouseEntered = true;
    }

    private void OnMouseExit()
    {
        hasMouseEntered = false;
    }

    public void CheckMarkers()
    {
        foreach (Transform child in transform)
        {
            if (isBlocked > 0)
            {
                child.gameObject.SetActive(true);
            }
            else
            {
                child.gameObject.SetActive(false);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (gameMasterScript != null && gameMasterScript.runGame)
        {
            bumping.PlayOneShot(bumping.clip);
        }
    }
}
