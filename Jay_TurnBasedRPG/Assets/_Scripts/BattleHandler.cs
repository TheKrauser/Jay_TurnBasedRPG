using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;
using DG.Tweening;

public class BattleHandler : MonoBehaviour
{
    public static BattleHandler Instance { get; private set; }

    [SerializeField] private List<BattleUnit> playerTeam = new List<BattleUnit>();
    [SerializeField] private List<BattleUnit> enemyTeam = new List<BattleUnit>();

    private MouseRaycast mouseRaycast;
    [SerializeField] private UI_BattleHandler ui_BattleHandler;

    private int turn = 0;
    private bool isPlayerTurn = true;
    public bool targetSelection = false;
    private BattleUnit currentTurnUnit;
    private BattleUnit targetUnit;

    public event EventHandler OnTargetSelected;
    public event EventHandler OnCharacterAttacked;
    public event EventHandler OnTurnChanged;

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
    }

    private void Start()
    {
        StartBattle();
    }

    public void StartBattle()
    {
        TurnManager();
    }

    public void TurnManager()
    {
        if (isPlayerTurn)
        {
            currentTurnUnit = playerTeam[turn];

            if (currentTurnUnit.GetDead())
                ChangeTurn();
            else
            {
                currentTurnUnit.SetCircle(Color.green);
                OnTargetSelected?.Invoke(this, EventArgs.Empty);
                OnTurnChanged?.Invoke(this, EventArgs.Empty);
            }

        }
        else
        {
            currentTurnUnit = enemyTeam[turn];
            StartCoroutine(EnemyAttack());
        }
    }

    public void ChangeTurn()
    {
        currentTurnUnit.SetCircle(Color.black);
        targetSelection = false;

        turn++;

        if (turn >= 3)
        {
            turn = 0;
            isPlayerTurn = !isPlayerTurn;
        }

        TurnManager();
    }

    public void TargetSelection(bool changeTo)
    {
        targetSelection = changeTo;
    }

    public void SelectTarget(BattleUnit target)
    {
        if (!playerTeam.Contains(target))
        {
            TargetSelection(false);
            targetUnit = target;
            OnTargetSelected?.Invoke(this, EventArgs.Empty);
            Attack();
        }
    }

    bool isNormalAttack = true;
    public void Attack()
    {
        var pos = currentTurnUnit.transform.position;
        currentTurnUnit.transform.DOMove(targetUnit.transform.position - new Vector3(2, 0, 0), 1f).OnComplete(() =>
        {
            if (isNormalAttack)
                currentTurnUnit.Attack(targetUnit);
            else
                currentTurnUnit.Ultimate(targetUnit);

            OnCharacterAttacked?.Invoke(this, EventArgs.Empty);

            StartCoroutine(ReturnPosition(pos, 1f));
        });
    }

    public void SetAttackType(bool isNormal)
    {
        isNormalAttack = isNormal;
    }

    private IEnumerator ReturnPosition(Vector3 pos, float delay)
    {
        yield return new WaitForSeconds(delay);

        currentTurnUnit.transform.DOMove(pos, 1f).OnComplete(() =>
        {
            ChangeTurn();
        });
    }

    int target = 0;
    public IEnumerator EnemyAttack()
    {
        foreach (BattleUnit unit in enemyTeam)
        {
            if (unit.GetDead())
            {
                continue;
            }

            unit.SetCircle(Color.green);

            do
            {
                target = UnityEngine.Random.Range(0, playerTeam.Count);
            } while (playerTeam[target].GetDead());

            var initialPos = unit.transform.position;

            unit.transform.DOMove(playerTeam[target].transform.position + new Vector3(2, 0, 0), 1f).OnComplete(() =>
            {
                unit.Attack(playerTeam[target]);
            });
            yield return new WaitForSeconds(2.5f);

            unit.transform.DOMove(initialPos, 1f);

            yield return new WaitForSeconds(1f);
            unit.SetCircle(Color.black);
        }

        isPlayerTurn = !isPlayerTurn;
        TurnManager();
    }

    public BattleUnit GetTarget()
    {
        return targetUnit;
    }

    public BattleUnit GetCurrentTurn()
    {
        return currentTurnUnit;
    }

    public MouseRaycast GetMouseRaycast()
    {
        return mouseRaycast;
    }
}
