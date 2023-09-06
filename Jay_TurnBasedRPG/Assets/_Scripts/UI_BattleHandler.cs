using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System;
using UnityEngine.Events;

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

    private void Start()
    {
        BattleHandler.Instance.GetMouseRaycast().OnMouseOverCharacter += MouseRaycast_OnMouseOverCharacter;
        BattleHandler.Instance.GetMouseRaycast().OnMouseLeaveCharacter += MouseRaycast_MouseLeaveCharacter;
        BattleHandler.Instance.OnCharacterAttacked += BattleHandler_OnCharacterAttacked;
        BattleHandler.Instance.OnTargetSelected += BattleHandler_OnTargetSelected;
        BattleHandler.Instance.OnTurnChanged += BattleHandler_OnTurnChanged;
    }

    private void BattleHandler_OnTurnChanged(object sender, EventArgs e)
    {
        UpdateUltimateText();
    }

    private void BattleHandler_OnTargetSelected(object sender, EventArgs e)
    {
        groupButtons.interactable = !groupButtons.interactable;
    }

    private void BattleHandler_OnCharacterAttacked(object sender, EventArgs e)
    {
        FillInformations(BattleHandler.Instance.GetTarget());
    }

    private void OnDestroy()
    {
        BattleHandler.Instance.GetMouseRaycast().OnMouseOverCharacter -= MouseRaycast_OnMouseOverCharacter;
        BattleHandler.Instance.GetMouseRaycast().OnMouseLeaveCharacter -= MouseRaycast_MouseLeaveCharacter;
        BattleHandler.Instance.OnCharacterAttacked -= BattleHandler_OnCharacterAttacked;
        BattleHandler.Instance.OnTargetSelected -= BattleHandler_OnTargetSelected;
        BattleHandler.Instance.OnTurnChanged -= BattleHandler_OnTurnChanged;
    }

    private void MouseRaycast_OnMouseOverCharacter(object sender, MouseRaycast.OnMouseOverCharacterParameters e)
    {
        FillInformations(e.unit);
        e.unit.GetSpriteRenderer().color = Color.green;
    }

    private void MouseRaycast_MouseLeaveCharacter(object sender, EventArgs e)
    {
        HideInformations();
    }

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
        var ult = BattleHandler.Instance.GetCurrentTurn().GetUlt() * 100;
        ultimateText.text = ult.ToString("F0") + "%";
    }

    public void AttackButton()
    {
        BattleHandler.Instance.TargetSelection(true);
        BattleHandler.Instance.SetAttackType(true);
    }

    public void UltimateButton()
    {
        if (BattleHandler.Instance.GetCurrentTurn().GetUlt() == 1)
        {
            BattleHandler.Instance.TargetSelection(true);
            BattleHandler.Instance.SetAttackType(false);
        }
    }
}
