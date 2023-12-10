using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;


public class Cutscene : MonoBehaviour
{
    [SerializeField] private VideoPlayer vPlayer;

    private void Start()
    {
        vPlayer.loopPointReached += ChangeScene;
    }

    public void Skip()
    {
        SceneManager.LoadScene("TutorialScene");
    }

    private void ChangeScene(VideoPlayer source)
    {
        SceneManager.LoadScene("TutorialScene");
    }
}
