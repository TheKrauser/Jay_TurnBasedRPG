using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;


public class Cutscene : MonoBehaviour
{
    [SerializeField] private VideoPlayer vPlayer;

    [SerializeField] private string nextScene;

    private void Start()
    {
        vPlayer.loopPointReached += ChangeScene;
    }

    private void OnDestroy()
    {
        vPlayer.loopPointReached -= ChangeScene;
    }

    public void Skip()
    {
        SceneManager.LoadScene(nextScene);
    }

    private void ChangeScene(VideoPlayer source)
    {
        SceneManager.LoadScene(nextScene);
    }
}
