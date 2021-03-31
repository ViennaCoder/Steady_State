using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class GameMaster : MonoBehaviour
{
    public GameObject cubePrefab;
    public bool runGame;

    public Image Player1;
    public Image Player2;
    public Slider GlobalEvent;
    public Image GlobalEventIcon;
    public GameObject UIWinning;


    public Collider border;
    public string CurrentPlayer = "Tower1";
    public int isGameOver;      // 0..no,1..player1 lost, 2..player2 lost

    public int currentHeapIndex { get; set; }
    public GameObject Tower1;
    public GameObject Tower2;

    static float sMasterVolume = 0.3f;
    public float MasterVolume
    {
        get { return sMasterVolume; }
        set { sMasterVolume = value; }
    }

    static string sSongName = "14_OptimisticLoneliness";
    public string SongName
    {
        get { return sSongName; }
        set { sSongName = value; }
    }

    static int sPlayer1Selection;
    static int sPlayer2Selection;
    public int Player1Selection
    {
        get { return sPlayer1Selection; }
        set { sPlayer1Selection = value; }
    }
    public int Player2Selection
    {
        get { return sPlayer2Selection; }
        set { sPlayer2Selection = value; }
    }
    GameObject currentHeapObject;
    GameObject[] Heaps;
    Vector3 HeapPosition;

    Transform Floor1;
    Transform Floor2;

    Collider Box1;
    Collider Box2;

    string[] GEImageNames = new string[] { "block_falling", "decrease", "pngwing_earthquake.com", "pngwing_wind.com" };
    Sprite[] GEImages;
    int GEIndex;

    int playerMoves;
    int maxRounds2GE;

    Vector3[] sizeOptions = new Vector3[]
    {
        new Vector3( 0.5f, 0.2f, 0.5f), new Vector3( 0.5f, 0.2f, 0.2f),
        new Vector3( 0.2f, 0.2f, 1f), new Vector3( 0.2f, 0.2f, 0.5f), new Vector3( 0.2f, 0.2f, 0.2f)
    };

    public AudioSource backgroundSong;

    // Start is called before the first frame update
    void Start()
    {
        if (runGame)
        {
            CurrentPlayer = "Start";
            isGameOver = 0;

            // create towers
            GameObject _Border = GameObject.Find("Border");

            GetHeaps(Application.dataPath + "/Prefabs/Resources");
            HeapPosition = new Vector3(0, 0, 0);
            Tower1 = Instantiate<GameObject>(Heaps[sPlayer1Selection], HeapPosition, new Quaternion(0, 0, 0, 1));
            Tower1.name = "Tower1";
            Tower1.GetComponent<TurnTower>().border = _Border.GetComponent<Collider>();

            HeapPosition = new Vector3(3.11f, 0, 0f);
            Tower2 = Instantiate<GameObject>(Heaps[sPlayer2Selection], HeapPosition, new Quaternion(0, 0, 0, 1));
            Tower2.name = "Tower2";
            Tower2.GetComponent<TurnTower>().border = _Border.GetComponent<Collider>();

            Floor1 = Tower1.transform.Find("Floor");
            Box1 = Floor1.transform.Find("BoundingBox").GetComponent<Collider>();
            Floor2 = Tower2.transform.Find("Floor");
            Box2 = Floor2.transform.Find("BoundingBox").GetComponent<Collider>();

            GEIndex = -1;   // undefined
            GEImages = new Sprite[GEImageNames.Length];

            for (int i = 0; i < GEImageNames.Length; i++)
            {
                GEImages[i] = Resources.Load<Sprite>(GEImageNames[i]);
            }

            maxRounds2GE = 2;

            if (SongName != "none")
            {
                backgroundSong = GetComponent<AudioSource>();
                backgroundSong.volume = MasterVolume;
                backgroundSong.clip = Resources.Load<AudioClip>(SongName);
                backgroundSong.Play();
            }
            SwitchPlayer();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isGameOver != 0)
        {
            Transform winningText = UIWinning.transform.Find("Text");
            UIWinning.SetActive(true);
            if (isGameOver == 1)
            {
                winningText.GetComponent<Text>().text = "Player 2 has won";
            }
            else if (isGameOver == 2)
            {
                winningText.GetComponent<Text>().text = "Player 1 has won";
            }
        }
    }

    public void SwitchPlayer()
    {
        Block childScript;

        switch (CurrentPlayer)
        {
            case "Start":
                playerMoves = 0;
                SetNewGE();
                CurrentPlayer = "Tower1";
                Player1.color = new Color(1, 0.8235294f, 0.02352941f, 1);
                Player2.color = Color.white;
                break;
            case "Tower1":
                foreach (Transform child in Tower1.transform)
                {   // reduce blocked by 1 for all children
                    if (child.name.StartsWith("Cube"))
                    {
                        childScript = child.GetComponent<Block>();
                        if (childScript.isBlocked > 0)
                        {
                            childScript.isBlocked--;
                        }
                    }
                }
                CurrentPlayer = "Tower2";
                Player2.color = new Color(1, 0.8235294f, 0.02352941f, 1);
                Player1.color = Color.white;
                playerMoves++;
                break;
            case "Tower2":
                foreach (Transform child in Tower2.transform)
                {   // reduce blocked by 1 for all children
                    if (child.name.StartsWith("Cube"))
                    {
                        childScript = child.GetComponent<Block>();
                        if (childScript.isBlocked > 0)
                        {
                            childScript.isBlocked--;
                        }
                    }
                }
                CurrentPlayer = "Tower1";
                Player1.color = new Color(1, 0.8235294f, 0.02352941f, 1);
                Player2.color = Color.white;
                playerMoves++;
                break;
        }
        #region adjust blocking
        foreach (Transform child in Tower1.transform)
        {
            if (child.name.StartsWith("Cube"))
            {
                childScript = child.GetComponent<Block>();
                childScript.CheckMarkers();
            }
        }
        foreach (Transform child in Tower2.transform)
        {
            if (child.name.StartsWith("Cube"))
            {
                childScript = child.GetComponent<Block>();
                childScript.CheckMarkers();
            }
        }
        #endregion
        if (playerMoves >= maxRounds2GE * 2)
        {
            switch (GEIndex)
            {
                case 0:
                    FallingStone();
                    break;
                case 1:
                    ChangeBlockSize();
                    break;
                case 2:
                    Earthquake();
                    break;
                case 3:
                    BlowWind();
                    break;
            }
            playerMoves = 0;
            SetNewGE();
        }

        // show GlobalEvent slider
        GlobalEvent.value = playerMoves / (maxRounds2GE * 2.0f);
    }

    public Collider GetBoundingBox(string name)
    {
        if (name == "Tower1")
            return Box1;
        return Box2;
    }

    void SetNewGE()
    {
        GEIndex = Random.Range(0, GEImages.Length);
        GlobalEventIcon.sprite = GEImages[GEIndex];
    }

    void ChangeBlockSize()
    {
        Vector3 newSize = sizeOptions[Random.Range(0, sizeOptions.Length)];
        int indexChoosenCube = Random.Range(0, NumberCubeChildren(Tower1));
        Transform choosenCube = GetChildCube(Tower1, indexChoosenCube);
        if (choosenCube != null)
        {
            choosenCube.localScale = newSize;
        }

        newSize = sizeOptions[Random.Range(0, sizeOptions.Length)];
        indexChoosenCube = Random.Range(0, NumberCubeChildren(Tower2));
        choosenCube = GetChildCube(Tower2, indexChoosenCube);
        if (choosenCube != null)
        {
            choosenCube.localScale = newSize;
        }
    }

    int NumberCubeChildren(GameObject go)
    {
        int counter = 0;

        foreach (Transform child in go.transform)
        {
            if (child.name.StartsWith("Cube"))
            {
                counter++;
            }
        }
        return counter;
    }

    Transform GetChildCube(GameObject go, int index)
    {
        int counter = 0;

        foreach (Transform child in go.transform)
        {
            if (child.name.StartsWith("Cube"))
            {
                if (counter == index)
                {
                    return child;
                }
                counter++;
            }
        }
        return null;
    }

    void Earthquake()
    {
        Floor1.transform.GetComponent<Rigidbody>().AddForceAtPosition(Vector3.up * 1f,
            Floor1.transform.position + new Vector3(Random.Range(0.5f, 1.5f), 0, Random.Range(0.5f, 1.5f)),
            ForceMode.Impulse);
        Floor2.transform.GetComponent<Rigidbody>().AddForceAtPosition(Vector3.up * 1f,
            Floor2.transform.position + new Vector3(Random.Range(0.5f, 1.5f), 0, Random.Range(0.5f, 1.5f)),
            ForceMode.Impulse);
        StartCoroutine(DelayedHit(0.5f));
    }

    IEnumerator DelayedHit(float sec)
    {
        yield return new WaitForSeconds(sec);
        Floor1.transform.GetComponent<Rigidbody>().AddForceAtPosition(Vector3.up * 1f,
            Floor1.transform.position + new Vector3(Random.Range(0.5f, 1.5f), 0, Random.Range(0.5f, 1.5f)),
            ForceMode.Impulse);
        Floor2.transform.GetComponent<Rigidbody>().AddForceAtPosition(Vector3.up * 1f,
            Floor2.transform.position + new Vector3(Random.Range(0.5f, 1.5f), 0, Random.Range(0.5f, 1.5f)),
            ForceMode.Impulse);
    }

    void BlowWind()
    {
        Tower1.transform.Find("Wind").GetComponent<ParticleSystem>().Play();
        Tower2.transform.Find("Wind").GetComponent<ParticleSystem>().Play();
    }

    void FallingStone()
    {
        float xMax = Floor1.transform.localScale.x / 2;
        float xMin = -xMax;
        float zMax = Floor1.transform.localScale.z / 2;
        float zMin = -zMax;
        GameObject newCube = Instantiate(cubePrefab, new Vector3(0, 0, 0), Quaternion.identity, Tower1.transform);
        newCube.transform.localPosition = new Vector3(Random.Range(xMin, xMax), 3f, Random.Range(zMin, zMax));

        xMax = Floor2.transform.localScale.x / 2;
        xMin = -xMax;
        zMax = Floor2.transform.localScale.z / 2;
        zMin = -zMax;
        newCube = Instantiate(cubePrefab, new Vector3(0, 0, 0), Quaternion.identity, Tower2.transform);
        newCube.transform.localPosition = new Vector3(Random.Range(xMin, xMax), 3f, Random.Range(zMin, zMax));
    }

    public void GetHeaps(string path)
    {
        string tmpResName;
        string[] fileEntries = Directory.GetFiles(path, "Heap*.prefab");
        List<string> ResNames = new List<string>();

        foreach (string fileName in fileEntries)
        {
            tmpResName = fileName.Replace(path + "\\", "");
            ResNames.Add(tmpResName.Substring(0, tmpResName.IndexOf('.')));
        }

        Heaps = new GameObject[ResNames.Count];

        for (int i = 0; i < ResNames.Count; i++)
        {
            Heaps[i] = Resources.Load<GameObject>(ResNames[i]);
        }
    }

    public void ShowHeap()
    {
        HeapPosition = new Vector3(0, -1, 4f);

        DestroyImmediate(currentHeapObject);

        currentHeapObject = Instantiate<GameObject>(Heaps[currentHeapIndex], HeapPosition, new Quaternion(0, 0, 0, 1));
        currentHeapObject.GetComponent<TurnTower>().enabled = false;
    }

    public int HeapSize()
    {
        return Heaps.Length;
    }


}
