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
    [SerializeField] private TextMeshProUGUI infoUltimateText;
    [SerializeField] private TextMeshProUGUI infoDamageText;

    [SerializeField] private TextMeshProUGUI textTurn;
    [SerializeField] private TextMeshProUGUI textAct;
    [SerializeField] private TextMeshProUGUI textHeal, textRevive, textFreeze;

    [SerializeField] private Button attack;
    [SerializeField] private Button ultimate;
    [SerializeField] private Button bag;
    [SerializeField] private Button heal;

    [SerializeField] private CanvasGroup pause;

    private int healAmount = 2, reviveAmount = 2, freezeAmount = 2;

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
        UpdateTurnText();
        Ultimate();
        //UpdateUltimateText();
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
        e.unit.GetSpriteRenderer().color = e.unit.GetCurrentColor();
    }

    //Show the informations on screen
    public void FillInformations(BattleUnit unit)
    {
        informations.SetActive(true);

        infoPortrait.sprite = unit.GetUISprite();
        infoName.text = unit.GetName();
        infoDamageText.text = $"Damage: {unit.GetDamage()}";
        infoHealthText.text = $"{unit.GetCurrentHealth()}/{unit.GetMaxHealth()}";
        infoHealthBar.fillAmount = (float)unit.GetCurrentHealth() / unit.GetMaxHealth();
    }

    public void HideInformations()
    {
        informations.SetActive(false);
    }

    /*public void UpdateUltimateText()
    {
        var ult = battleHandler.GetCurrentTurn().GetUltimate() * 100;
        infoUltimateText.text = ult.ToString("F0") + "%";
    }*/

    public void UpdateTurnText()
    {
        textTurn.text = $"{battleHandler.GetCurrentTurn().GetName()} Turn";

        if (battleHandler.GetCurrentTurn().GetName() == "Cottama")
        {
            heal.gameObject.SetActive(true);
            attack.gameObject.SetActive(false);
        }
        else
        {
            attack.gameObject.SetActive(true);
            heal.gameObject.SetActive(false);
        }
    }

    public int GetItemAmount(string itemName)
    {
        if (itemName == "Heal")
        {
            return healAmount;
        }
        else if (itemName == "Revive")
        {
            return reviveAmount;
        }
        else
        {
            return freezeAmount;
        }
    }

    public void UseItem(string itemName)
    {
        if (itemName == "Heal")
        {
            healAmount--;

            if (healAmount < 0)
            {
                healAmount = 0;
            }

            textHeal.text = healAmount.ToString();
        }
        else if (itemName == "Revive")
        {
            reviveAmount--;

            if (reviveAmount < 0)
            {
                reviveAmount = 0;
            }

            textRevive.text = reviveAmount.ToString();
        }
        else
        {
            freezeAmount--;

            if (freezeAmount < 0)
            {
                freezeAmount = 0;
            }

            textFreeze.text = freezeAmount.ToString();
        }
    }

    public void Ultimate()
    {
        if (battleHandler.GetCurrentTurn().GetUltimate() == 1)
        {
            ultimate.gameObject.SetActive(true);
        }
        else
        {
            ultimate.gameObject.SetActive(false);
        }
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

    public void HealButton()
    {
        battleHandler.IsSelectingTarget(false);
        battleHandler.SelectedItem(1);
        battleHandler.IsUsingItem(true, true);
    }

    public void ReviveButton()
    {
        battleHandler.IsSelectingTarget(false);
        battleHandler.SelectedItem(2);
        battleHandler.IsUsingItem(true, true);
    }

    public void FreezeButton()
    {
        battleHandler.IsSelectingTarget(false);
        battleHandler.SelectedItem(3);
        battleHandler.IsUsingItem(true, false);
    }

    public void Pause(bool open)
    {
        if (open)
        {
            Time.timeScale = 0;

            pause.alpha = 1;
            pause.interactable = true;
            pause.blocksRaycasts = true;
        }
        else
        {
            Time.timeScale = 1;

            pause.alpha = 0;
            pause.interactable = false;
            pause.blocksRaycasts = false;
        }
    }

    public void Menu()
    {
        SceneManager.LoadScene("Menu");
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
