using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class UI_BattleHandler : MonoBehaviour
{
    [SerializeField] private GameObject informations;
    [SerializeField] private Image infoPortrait;
    [SerializeField] private TextMeshProUGUI infoName;
    [SerializeField] private Image infoHealthBar;
    [SerializeField] private TextMeshProUGUI infoHealthText;

    [SerializeField] private Button attack;
    [SerializeField] private Button ultimate;
    [SerializeField] private TextMeshProUGUI ultimateText;
    [SerializeField] private Button bag;

    [SerializeField] private CanvasGroup groupButtons;

    private BattleHandler battleHandler;

    private void Start()
    {
        battleHandler = BattleHandler.Instance;

        //Subscribes to all the events on BattleHandler script
        battleHandler.GetMouseRaycast().OnMouseOverCharacter += MouseRaycast_OnMouseOverCharacter;
        battleHandler.GetMouseRaycast().OnMouseLeaveCharacter += MouseRaycast_MouseLeaveCharacter;
        battleHandler.OnCharacterAttacked += BattleHandler_OnCharacterAttacked;
        battleHandler.OnTargetSelected += BattleHandler_OnTargetSelected;
        battleHandler.OnTurnChanged += BattleHandler_OnTurnChanged;
    }

    private void BattleHandler_OnTurnChanged(object sender, EventArgs e)
    {
        UpdateUltimateText();
    }

    //Disable the UI buttons when a target is selected, to prevent clicks when the character is attacking
    private void BattleHandler_OnTargetSelected(object sender, EventArgs e)
    {
        groupButtons.interactable = !groupButtons.interactable;
    }

    //Show the information of the target after attacked
    private void BattleHandler_OnCharacterAttacked(object sender, EventArgs e)
    {
        FillInformations(battleHandler.GetTarget());
    }

    //Unsubscribes to all the events when the script is destroyed
    //Necessary to prevent problems
    private void OnDestroy()
    {
        battleHandler.GetMouseRaycast().OnMouseOverCharacter -= MouseRaycast_OnMouseOverCharacter;
        battleHandler.GetMouseRaycast().OnMouseLeaveCharacter -= MouseRaycast_MouseLeaveCharacter;
        battleHandler.OnCharacterAttacked -= BattleHandler_OnCharacterAttacked;
        battleHandler.OnTargetSelected -= BattleHandler_OnTargetSelected;
        battleHandler.OnTurnChanged -= BattleHandler_OnTurnChanged;
    }

    private void MouseRaycast_OnMouseOverCharacter(object sender, MouseRaycast.OnMouseOverCharacterParameters e)
    {
        FillInformations(e.unit);
        e.unit.GetSpriteRenderer().color = Color.green;
    }

    private void MouseRaycast_MouseLeaveCharacter(object sender, MouseRaycast.OnMouseOverCharacterParameters e)
    {
        HideInformations();
        e.unit.GetSpriteRenderer().color = Color.white;
    }

    //Show the informations on screen
    public void FillInformations(BattleUnit unit)
    {
        informations.SetActive(true);

        infoPortrait.sprite = unit.GetUISprite();
        infoName.text = unit.GetName();
        infoHealthText.text = $"{unit.GetCurrentHealth()}/{unit.GetMaxHealth()}";
        infoHealthBar.fillAmount = (float)unit.GetCurrentHealth() / unit.GetMaxHealth();
    }

    public void HideInformations()
    {
        informations.SetActive(false);
    }

    public void UpdateUltimateText()
    {
        var ult = BattleHandler.Instance.GetCurrentTurn().GetUltimate() * 100;
        ultimateText.text = ult.ToString("F0") + "%";
    }


    public void AttackButton()
    {
        //Go to the selecting target state and pass the attack type as normal (cause its the normal attack)
        battleHandler.IsSelectingTarget(true);
        battleHandler.SetAttackType(true);
    }

    public void UltimateButton()
    {
        //Gets the current unit turn and then gets the ultimate value, if its 1, then its all charged
        if (battleHandler.GetCurrentTurn().GetUltimate() == 1)
        {
            //Go to the selecting target state and pass the attack type as not normal (ultimate)
            battleHandler.IsSelectingTarget(true);
            battleHandler.SetAttackType(false);
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
