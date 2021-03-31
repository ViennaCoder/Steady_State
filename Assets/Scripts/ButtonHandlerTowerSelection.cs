using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonHandlerTowerSelection : MonoBehaviour
{
    public Image Player1;
    public Image Player2;

    GameMaster gameMasterScript;

    private void Start()
    {
        GameObject _GameMaster = GameObject.Find("GameMaster");
        gameMasterScript = _GameMaster.GetComponent<GameMaster>();

        gameMasterScript.GetHeaps(Application.dataPath + "/Prefabs/Resources");
        gameMasterScript.Player1Selection = -1;
        gameMasterScript.Player2Selection = -1;

        gameMasterScript.currentHeapIndex = 0;
        gameMasterScript.ShowHeap();

        SetPlayerDisplay();
    }

    void SetPlayerDisplay()
    {
        if (gameMasterScript.Player1Selection == -1)
        {
            Player1.color = new Color(1, 0.8235294f, 0.02352941f, 1);
            Player1.gameObject.SetActive(true);
        }
        else if (gameMasterScript.Player2Selection == -1)
        {
            Player1.gameObject.SetActive(false);
            Player2.color = new Color(1, 0.8235294f, 0.02352941f, 1);
            Player2.gameObject.SetActive(true);
        }
    }

    public void NextPlayer()
    {
        if (gameMasterScript.Player1Selection == -1)
        {
            gameMasterScript.Player1Selection = gameMasterScript.currentHeapIndex;
            gameMasterScript.currentHeapIndex = 0;
            gameMasterScript.ShowHeap();
            SetPlayerDisplay();
        }
        else if (gameMasterScript.Player2Selection == -1)
        {
            gameMasterScript.Player2Selection = gameMasterScript.currentHeapIndex;
            gameMasterScript.currentHeapIndex = 0;
            LoadScene(2);
        }
    }

    public void NextHeap()
    {
        gameMasterScript.currentHeapIndex++;
        if (gameMasterScript.currentHeapIndex >= gameMasterScript.HeapSize())
        {
            gameMasterScript.currentHeapIndex = 0;
        }
        gameMasterScript.ShowHeap();
    }

    public void PreviousHeap()
    {
        gameMasterScript.currentHeapIndex--;
        if (gameMasterScript.currentHeapIndex < 0)
        {
            gameMasterScript.currentHeapIndex = gameMasterScript.HeapSize() - 1;
        }
        gameMasterScript.ShowHeap();
    }

    public void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
