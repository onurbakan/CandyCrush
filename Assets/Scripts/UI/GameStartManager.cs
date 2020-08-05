using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStartManager : MonoBehaviour
{
    public GameObject startPanel;
    public GameObject levelPanel;
    public static int index;

    // Start is called before the first frame update
    ///void Awake()
    ///{
    ///    startPanel.SetActive(true);
    ///    levelPanel.SetActive(false);
    ///}

    void Start()
    {
        if (index == 1)
        {
            startPanel.SetActive(false);
            levelPanel.SetActive(true);
            //Debug.Log("index : "+ index);
        }
        else
        {
            startPanel.SetActive(true);
            levelPanel.SetActive(false);
            index = 1;
            //Debug.Log("index : " + index);
        }
    }

    public void PlayGame()
    {
        startPanel.SetActive(false);
        levelPanel.SetActive(true);
    }

    public void Home()
    {
        startPanel.SetActive(true);
        levelPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
