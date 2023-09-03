using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

public class BattleHandler : MonoBehaviour
{
    public static BattleHandler Instance { get; private set; }

    [SerializeField] private List<BattleUnit> playerTeam = new List<BattleUnit>();
    [SerializeField] private List<BattleUnit> enemyTeam = new List<BattleUnit>();

    private MouseRaycast mouseRaycast;
    [SerializeField] private UI_BattleHandler ui_BattleHandler;

    public event EventHandler OnMouveOverCharacter;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        mouseRaycast = GetComponent<MouseRaycast>();

        StartBattle();
    }

    public void StartBattle()
    {

    }

    public MouseRaycast GetMouseRaycast()
    {
        return mouseRaycast;
    }
}
