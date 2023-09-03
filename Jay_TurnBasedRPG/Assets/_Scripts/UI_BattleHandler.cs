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

    private void Start()
    {
        BattleHandler.Instance.GetMouseRaycast().OnMouseOverCharacter += MouseRaycast_OnMouseOverCharacter;
    }

    private void OnDestroy()
    {
        BattleHandler.Instance.GetMouseRaycast().OnMouseOverCharacter -= MouseRaycast_OnMouseOverCharacter;
    }

    private void MouseRaycast_OnMouseOverCharacter(object sender, MouseRaycast.OnMouseOverCharacterParameters e)
    {
        FillInformations(e.unit);
    }

    public void FillInformations(BattleUnit unit)
    {
        informations.SetActive(true);

        infoPortrait.sprite = unit.GetUISprite();
        infoName.text = unit.GetName();
        infoHealthText.text = $"{unit.GetCurrentHealth()}/{unit.GetMaxHealth()}";
        infoHealthBar.fillAmount = (float)unit.GetMaxHealth() / unit.GetCurrentHealth();
    }

    public void HideInformations()
    {
        informations.SetActive(false);
    }
}
