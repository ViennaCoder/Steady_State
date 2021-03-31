using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonHandler : MonoBehaviour
{
    public GameObject StartMenu;
    public GameObject InfoScreen;
    public GameObject OptionsScreen;
    public GameObject CreditScreen;

    GameMaster gameMasterScript;
    List<string> Songs = new List<string>();

    int selectedSong;
    Dropdown ddMusicName;

    private void Start()
    {
        GameObject _GameMaster = GameObject.Find("GameMaster");
        gameMasterScript = _GameMaster.GetComponent<GameMaster>();

        Transform transMusicName = OptionsScreen.transform.Find("MusicName");
        ddMusicName = transMusicName.GetComponent<Dropdown>();
        ddMusicName.options.Clear();
        ddMusicName.RefreshShownValue();        // new items are available

        StartMenu.SetActive(true);
        InfoScreen.SetActive(false);
        OptionsScreen.SetActive(false);
        CreditScreen.SetActive(false);
    }

    public void LoadScene(int sceneIndex)
    {
        if (sceneIndex < 2 && gameMasterScript.backgroundSong != null)
        {
            gameMasterScript.backgroundSong.Stop();
        }
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

    public void Info()
    {
        StartMenu.SetActive(false);
        InfoScreen.SetActive(true);
    }

    public void InfoScreenBack()
    {
        StartMenu.SetActive(true);
        InfoScreen.SetActive(false);
    }

    public void Options()
    {
        selectedSong = 0;

        StartMenu.SetActive(false);
        OptionsScreen.SetActive(true);
        GetMusic(Application.dataPath + "/Music/Resources");

        // fill dropdown with song names
        foreach (string song in Songs)
        {
            ddMusicName.options.Add(new Dropdown.OptionData(song));
        }
        ddMusicName.RefreshShownValue();        // new items are available

        // find old song index
        if (!string.IsNullOrEmpty(gameMasterScript.SongName))
        {
            selectedSong = ddMusicName.options.FindIndex((i) => { return i.text.Equals(gameMasterScript.SongName); });
        }

        ddMusicName.value = selectedSong;       // show last selected song
    }

    public void OptionsScreenBack()
    {
        gameMasterScript.SongName = ddMusicName.options[ddMusicName.value].text;
        ddMusicName.options.Clear();            // clear list for next use
        ddMusicName.RefreshShownValue();        // new items are available

        gameMasterScript.MasterVolume = OptionsScreen.transform.Find("MasterVolume").GetComponent<Slider>().value;
        StartMenu.SetActive(true);
        OptionsScreen.SetActive(false);
    }

    public void GetMusic(string path)
    {
        string tmpResName;
        string[] fileEntries = Directory.GetFiles(path, "*.mp3");

        Songs.Clear();
        Songs.Add("none");
        foreach (string fileName in fileEntries)
        {
            tmpResName = fileName.Replace(path + "\\", "");
            Songs.Add(tmpResName.Substring(0, tmpResName.IndexOf('.')));
        }
    }

    public void Credits()
    {
        StartMenu.SetActive(false);
        CreditScreen.SetActive(true);
    }

    public void CreditsScreenBack()
    {
        StartMenu.SetActive(true);
        CreditScreen.SetActive(false);
    }
}
