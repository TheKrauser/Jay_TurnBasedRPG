using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class BattleHandler : MonoBehaviour
{
    //Singleton
    public static BattleHandler Instance { get; private set; }

    [SerializeField] private List<BattleUnit> playerTeam = new List<BattleUnit>();
    [SerializeField] private List<BattleUnit> enemyTeam = new List<BattleUnit>();

    private int turn = 0;
    private bool isPlayerTurn = true;
    public bool isSelectingTarget = false;
    public bool isUsingItem = false;
    public bool itemForAlly = false;
    int selectedItem = 0;

    private BattleUnit currentTurnUnit;
    private BattleUnit targetUnit;

    //Components
    private MouseRaycast mouseRaycast;

    //Events
    public event EventHandler OnTargetSelected;
    public event EventHandler OnCharacterAttacked;
    public event EventHandler OnTurnChanged;

    [SerializeField] private UI_BattleHandler uiBattle;

    [SerializeField] private GameObject nextBattle;

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
        StartCoroutine(Waiting());
    }

    public void StartBattle()
    {
        TurnManager();
    }

    private IEnumerator Waiting()
    {
        yield return new WaitForSeconds(1.5f);
        StartBattle();
    }

    //Called to handle who is the next character to make an action
    public void TurnManager()
    {

        if (isPlayerTurn)
        {
            //If the current unit to attack is dead, proceed to the next unit
            if (playerTeam[turn].IsDead())
            {
                ChangeTurn();
            }
            else
            {
                currentTurnUnit = playerTeam[turn];
                //If the character is not dead, call the events and set the circle color to green
                currentTurnUnit.SetCircleColor(Color.green);

                OnTargetSelected?.Invoke(this, EventArgs.Empty);
                OnTurnChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        else
        {
            StartCoroutine(EnemyAttack());
        }
    }

    private int enemyTeamDeathCount = 0;

    //Handles the next unit that will attack
    public void ChangeTurn()
    {
        //Set the circle to black on the unit that attacked
        currentTurnUnit.SetCircleColor(Color.black);

        //Character already attacked, so change the bool to false cause no one is selecting a target
        isSelectingTarget = false;
        isUsingItem = false;

        turn++;
        //Turn >= 3 cause theres 3 fixed units in each team, when its above it
        //go back to 0 to restart the count and reverse the player turn
        if (turn >= 3)
        {
            turn = 0;
            isPlayerTurn = !isPlayerTurn;
        }

        enemyTeamDeathCount = 0;
        foreach (BattleUnit unit in enemyTeam)
        {
            if (unit.IsDead())
            {
                enemyTeamDeathCount++;
            }
        }

        if (SceneManager.GetActiveScene().buildIndex == 3)
        {
            if (enemyTeamDeathCount >= 1)
            {
                nextBattle.SetActive(true);
                return;
            }
        }
        else
        {
            if (enemyTeamDeathCount >= 3)
            {
                nextBattle.SetActive(true);
                return;
            }
        }

        TurnManager();
    }

    public void IsSelectingTarget(bool changeTo)
    {
        isSelectingTarget = changeTo;
        isUsingItem = !changeTo;
    }

    public void IsUsingItem(bool changeTo, bool allyItem)
    {
        isUsingItem = changeTo;

        itemForAlly = allyItem;
    }

    //Pass the target that the player clicked to the current target unit
    public void SelectTarget(BattleUnit target)
    {
        if (turn == 2)
        {
            if (!enemyTeam.Contains(target))
            {
                IsSelectingTarget(false);
                targetUnit = target;
                OnTargetSelected?.Invoke(this, EventArgs.Empty);
                currentTurnUnit.Heal(target);
                StartCoroutine(DelayChangeTurn());
            }
        }
        else
        {
            //If clicked target is not on the players team, do the following script
            if (!playerTeam.Contains(target))
            {
                IsSelectingTarget(false);
                targetUnit = target;
                OnTargetSelected?.Invoke(this, EventArgs.Empty);
                Attack();
            }
        }
    }

    public void SelectedItem(int value)
    {
        selectedItem = value;
    }

    public void SelectItemTarget(BattleUnit target)
    {
        if (itemForAlly)
        {
            if (playerTeam.Contains(target))
            {
                if (selectedItem == 1)
                {
                    if (uiBattle.GetItemAmount("Heal") > 0)
                    {
                        IsUsingItem(false, false);
                        uiBattle.UseItem("Heal");
                        target.RestoreHealth(60);
                        OnTargetSelected?.Invoke(this, EventArgs.Empty);
                        ChangeTurn();
                    }
                }
                else if (selectedItem == 2)
                {
                    if (uiBattle.GetItemAmount("Revive") > 0)
                    {
                        IsUsingItem(false, false);
                        uiBattle.UseItem("Revive");
                        target.BuffAttack();
                        OnTargetSelected?.Invoke(this, EventArgs.Empty);
                        ChangeTurn();
                    }
                }
            }
        }
        else
        {
            if (enemyTeam.Contains(target))
            {
                if (uiBattle.GetItemAmount("Freeze") > 0)
                {
                    uiBattle.UseItem("Freeze");
                    target.Freeze();
                    IsUsingItem(false, false);
                    OnTargetSelected?.Invoke(this, EventArgs.Empty);
                    ChangeTurn();
                }
            }
        }
    }

    SpriteRenderer sprite;

    //Bool to handle if the current attack player will make is the normal one
    //If not, then its the ultimate attack
    bool isNormalAttack = true;
    public void Attack()
    {
        sprite = currentTurnUnit.GetComponentInChildren<SpriteRenderer>();
        sprite.sortingOrder = 9;

        //Registers the initial position before start to walking towards the target
        var initialPosition = currentTurnUnit.transform.position;
        AudioManager.Instance.PlaySound("Movement");
        currentTurnUnit.transform.DOMove(targetUnit.transform.position - new Vector3(2, 0, 0), 1f).OnComplete(() =>
        {
            //Do this lines of code when the character reach its location (in front of the target)
            if (isNormalAttack)
            {
                currentTurnUnit.Attack(targetUnit);
            }
            else
            {
                currentTurnUnit.Ultimate(targetUnit);
            }

            //Calls the event saying someone was attacked
            OnCharacterAttacked?.Invoke(this, EventArgs.Empty);
            //Returns to the initial position using a Coroutine
            StartCoroutine(ReturnToInitialPosition(initialPosition, 1.2f));
        });
    }

    //To call from other scripts and set the chosen attack
    public void SetAttackType(bool isNormal)
    {
        isNormalAttack = isNormal;
    }

    private IEnumerator ReturnToInitialPosition(Vector3 pos, float delay)
    {
        yield return new WaitForSeconds(delay);

        AudioManager.Instance.PlaySound("Movement");
        currentTurnUnit.transform.DOMove(pos, 1f).OnComplete(() =>
        {
            sprite.sortingOrder = 0;
            //When back to the initial position, go to the next characters turn
            ChangeTurn();
        });
    }

    private IEnumerator DelayChangeTurn()
    {
        yield return new WaitForSeconds(1f);

        ChangeTurn();
    }

    //Stores the index of target the enemy has chose to attack
    int target = 0;
    private int playerTeamDeathCount = 0;
    public IEnumerator EnemyAttack()
    {
        //Cycle for each unit in the enemy team and Attack
        foreach (BattleUnit unit in enemyTeam)
        {
            //If the unit is dead, proceed to the next loop and dont do anything
            if (unit.IsDead())
            {
                /*enemyDeathCount++;

                if (SceneManager.GetActiveScene().buildIndex == 3)
                {
                    if (enemyDeathCount >= 1)
                    {
                        nextBattle.SetActive(true);
                    }
                }
                else
                {
                    if (enemyDeathCount >= 3)
                    {
                        nextBattle.SetActive(true);
                    }
                }*/

                continue;
            }

            unit.SetCircleColor(Color.green);

            if (unit.GetFreezed())
            {
                unit.SetCircleColor(Color.black);
                continue;
            }

            playerTeamDeathCount = 0;
            foreach (BattleUnit playerUnit in playerTeam)
            {
                if (playerUnit.IsDead())
                    playerTeamDeathCount++;
            }

            if (playerTeamDeathCount >= 3)
                yield break;

            //Chose a random number between 0 and 2, if the target index is dead, do this again till its not
            do
            {
                target = UnityEngine.Random.Range(0, playerTeam.Count);
            } while (playerTeam[target].IsDead());

            //Registers the initial position
            var initialPos = unit.transform.position;
            sprite = unit.GetComponentInChildren<SpriteRenderer>();
            sprite.sortingOrder = 9;

            //Go to the target and attack
            AudioManager.Instance.PlaySound("Movement");
            unit.transform.DOMove(playerTeam[target].transform.position + new Vector3(2, 0, 0), 1f).OnComplete(() =>
            {
                unit.Attack(playerTeam[target]);
            });

            //WaitForSeconds are just to make a delay between each part of the script
            yield return new WaitForSeconds(2.5f);
            //Return to the initial position
            unit.transform.DOMove(initialPos, 1f);
            AudioManager.Instance.PlaySound("Movement");
            yield return new WaitForSeconds(1f);
            unit.SetCircleColor(Color.black);
            sprite.sortingOrder = 0;
        }

        //When all the enemies attacked, simply toggle the isPlayerTurn to true
        isPlayerTurn = !isPlayerTurn;
        playerTeamDeathCount = 0;

        yield return new WaitForSeconds(2f);
        foreach (BattleUnit unit in enemyTeam)
        {
            unit.RemoveFreeze();
        }

        TurnManager();
    }

    //Functions to get a component from this script
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

    public void NextBattle(string scene)
    {
        SceneManager.LoadScene(scene);
    }
}
