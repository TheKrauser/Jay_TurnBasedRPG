using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup menu, options, stage;

    public void Play()
    {
        ManageView(menu, false);
        ManageView(stage, true);
    }

    public void SelectStage()
    {
        SceneManager.LoadScene("Cutscene");
    }

    public void Back()
    {
        ManageView(stage, false);
        ManageView(menu, true);
    }

    public void Options()
    {
        ManageView(menu, false);
        ManageView(options, true);
    }

    public void OptionsOK()
    {
        ManageView(options, false);
        ManageView(menu, true);
    }

    public void Quit()
    {
        Application.Quit();
    }

    private void ManageView(CanvasGroup group, bool enable)
    {
        if (enable)
        {
            group.alpha = 1;
            group.interactable = true;
            group.blocksRaycasts = true;
        }
        else
        {
            group.alpha = 0;
            group.interactable = false;
            group.blocksRaycasts = false;
        }
    }
}
