using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;

public class TutorialBattle : MonoBehaviour
{
    [SerializeField] private Button atk, hl, ult, heal, buff, freeze, nextBattle;
    [SerializeField] private Button buttonAlly, buttonEnemy;
    [SerializeField] private BattleUnit cottama, coldalla, enemy;
    [SerializeField] private CanvasGroup buttons;

    [SerializeField] private Image pointerATK, pointerEnemy, pointerColdalla, pointerItem, pointerFreeze, pointerUlt;
    [SerializeField] private GameObject textBG;
    [SerializeField] private TextMeshProUGUI text, turnText;

    [SerializeField] private SpriteRenderer cottamaCircle, coldallaCircle, enemyCircle;

    [SerializeField] private Image turnColdalla, turnCottama, turnEnemy;

    //private float delay = 2f;

    private bool clickedAttack = false;
    private bool selectedEnemy = false;
    private bool coldallaAttacked = false;

    private bool selectedBuff = false;
    private bool buffedColdalla = false;

    private bool enemyAttacked = false;

    private bool clickedAttack2 = false;
    private bool selectedEnemy2 = false;
    private bool buffedColdallaAttacked = false;

    private bool clickedHeal = false;
    private bool healedColdalla = false;

    private bool enemyAttacked2 = false;

    private bool clickedFreeze = false;
    private bool frozeEnemy = false;

    private bool clickedUltimate = false;
    private bool coldallaUltimate = false;

    private bool mouseClicked = false;

    private void Start()
    {
        StartCoroutine(Starting());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            mouseClicked = true;
        }
    }

    public void ClickATK()
    {
        if (!clickedAttack)
        {
            clickedAttack = true;
            atk.interactable = false;
            pointerATK.enabled = false;
            pointerEnemy.enabled = true;
            buttonEnemy.interactable = true;
        }
        else if (!clickedAttack2)
        {
            clickedAttack2 = true;
            atk.interactable = false;
            pointerATK.enabled = false;
            pointerEnemy.enabled = true;
            buttonEnemy.interactable = true;
        }
    }

    public void ClickHeal()
    {
        if (!clickedHeal)
        {
            clickedHeal = true;
            pointerATK.enabled = false;
            hl.interactable = false;
            pointerColdalla.enabled = true;
            buttonAlly.interactable = true;
        }
    }

    public void ClickItem()
    {
        if (!selectedBuff)
        {
            selectedBuff = true;
            buff.interactable = false;
            pointerItem.enabled = false;
            pointerColdalla.enabled = true;
            buttonAlly.interactable = true;
        }
    }

    public void ClickEnemy()
    {
        if (!selectedEnemy)
        {
            selectedEnemy = true;
            buttonEnemy.interactable = false;
            pointerEnemy.enabled = false;
            StartCoroutine(Attack(coldalla, enemy, false));
        }
        else if (!selectedEnemy2)
        {
            selectedEnemy2 = true;
            buttonEnemy.interactable = false;
            pointerEnemy.enabled = false;
            StartCoroutine(Attack(coldalla, enemy, false));
        }
        else if (!frozeEnemy)
        {
            frozeEnemy = true;
            pointerEnemy.enabled = false;
            enemy.Freeze();
            StartCoroutine(Turn());
        }
        else if (!coldallaUltimate)
        {
            coldallaUltimate = true;
            pointerEnemy.enabled = false;
            buttonEnemy.interactable = false;
            StartCoroutine(Attack(coldalla, enemy, true));
            StartCoroutine(FinishBattle());
        }
    }

    public void ClickColdalla()
    {
        if (!buffedColdalla)
        {
            buffedColdalla = true;
            buttonAlly.interactable = false;
            coldalla.BuffAttack();
            pointerColdalla.enabled = false;
            StartCoroutine(EnemyTurn());
        }
        else if (!healedColdalla)
        {
            buttonAlly.interactable = false;
            pointerColdalla.enabled = false;
            StartCoroutine(Heal(coldalla));
        }
    }

    public void ClickUlt()
    {
        if (!clickedUltimate)
        {
            clickedUltimate = true;
            pointerUlt.enabled = false;
            ult.interactable = false;
            pointerEnemy.enabled = true;
            buttonEnemy.interactable = true;
        }
    }

    public void ClickFreeze()
    {
        if (!clickedFreeze)
        {
            clickedFreeze = true;
            pointerFreeze.enabled = false;
            pointerEnemy.enabled = true;
            freeze.interactable = false;
            buttonEnemy.interactable = true;
        }
    }

    public void NextBattle()
    {
        SceneManager.LoadScene("BattleScene");
    }

    private void DisableTurn()
    {
        turnColdalla.enabled = false;
        turnCottama.enabled = false;
        turnEnemy.enabled = false;
    } 

    public IEnumerator Starting()
    {
        textBG.SetActive(true);
        text.text = "Let's teach you the basics of the <color=red>BATTLE SYSTEM!</color>";
        //yield return new WaitForSeconds(delay);
        yield return new WaitUntil(() => mouseClicked);
        mouseClicked = false;

        text.text = "Your main goal is to defeat all the <color=red>enemies</color> on the right side of the screen.";
        //yield return new WaitForSeconds(delay);
        yield return new WaitUntil(() => mouseClicked);
        mouseClicked = false;

        turnColdalla.enabled = true;
        coldallaCircle.color = Color.green;
        enemyCircle.color = Color.black;
        text.text = "It's Coldalla's turn now, you can click on the <color=red>ATK</color> button to <color=red>attack</color> the enemy.";
        //yield return new WaitForSeconds(delay);
        yield return new WaitUntil(() => mouseClicked);
        mouseClicked = false;

        textBG.SetActive(false);
        pointerATK.enabled = true;
        atk.interactable = true;
    }

    public IEnumerator EnemyTurn()
    {
        mouseClicked = false;
        textBG.SetActive(true);
        text.text = "When all your allies have made an action, It's the enemy's turn, he will attack one of your team characters.";
        //yield return new WaitForSeconds(delay);
        yield return new WaitUntil(() => mouseClicked);
        mouseClicked = false;
        textBG.SetActive(false);
        DisableTurn();
        turnEnemy.enabled = true;
        enemyCircle.color = Color.green;
        cottamaCircle.color = Color.black;
        StartCoroutine(Attack(enemy, coldalla, false));
    }

    public IEnumerator Attack(BattleUnit attacker, BattleUnit target, bool isUlt)
    {
        var initial = attacker.transform.position;
        if (attacker != enemy)
        {
            attacker.transform.DOMove(target.transform.position - new Vector3(2.5f, 0, 0), 1f).OnComplete(() =>
            {
                if (!isUlt)
                {
                    attacker.Attack(target);
                }
                else
                {
                    attacker.Ultimate(target);
                }
            });
        }
        else
        {
            attacker.transform.DOMove(target.transform.position + new Vector3(2.5f, 0, 0), 1f).OnComplete(() =>
            {
                attacker.Attack(target);
            });
        }

        yield return new WaitForSeconds(2f);
        attacker.transform.DOMove(initial, 1f);
        yield return new WaitForSeconds(2f);

        if (!coldallaAttacked)
        {
            mouseClicked = false;
            coldallaAttacked = true;
            textBG.SetActive(true);
            DisableTurn();
            turnCottama.enabled = true;
            cottamaCircle.color = Color.green;
            coldallaCircle.color = Color.black;
            text.text = "Cottama is just a <color=green>HEALER</color> and your team is full life, but you can use one of the <color=blue>ITEMS</color> in the bottom right corner instead.";
            //yield return new WaitForSeconds(delay);
            yield return new WaitUntil(() => mouseClicked);
            mouseClicked = false;
            text.text = "Try using the <color=red>GREEN LEAF</color>, it will <color=red>buff</color> your ally next attack in <color=red>+50%</color>, click and choose to buff Coldalla.";
            //yield return new WaitForSeconds(delay);
            yield return new WaitUntil(() => mouseClicked);
            mouseClicked = false;
            textBG.SetActive(false);
            pointerItem.enabled = true;
            buff.interactable = true;
        }
        else if (!enemyAttacked)
        {
            enemyAttacked = true;
            yield return new WaitForSeconds(2f);
            mouseClicked = false;
            textBG.SetActive(true);
            DisableTurn();
            turnColdalla.enabled = true;
            coldallaCircle.color = Color.green;
            enemyCircle.color = Color.black;
            text.text = "It's your turn again, you can attack the enemy with Coldalla, since he is <color=red>buffed</color>, he will do <color=red>more damage</color>.";
            //yield return new WaitForSeconds(delay);
            yield return new WaitUntil(() => mouseClicked);
            mouseClicked = false;
            pointerATK.enabled = true;
            atk.interactable = true;
            textBG.SetActive(false);
        }
        else if (!buffedColdallaAttacked)
        {
            mouseClicked = false;
            buffedColdallaAttacked = true;
            textBG.SetActive(true);
            DisableTurn();
            turnCottama.enabled = true;
            cottamaCircle.color = Color.green;
            coldallaCircle.color = Color.black;
            text.text = "Coldalla received some damage from the enemy, but Cottama is focused on <color=green>HEALING</color>, click on the <color=green>HEAL</color> button and select Coldalla to <color=green>heal</color> him.";
            //yield return new WaitForSeconds(delay);
            yield return new WaitUntil(() => mouseClicked);
            mouseClicked = false;
            textBG.SetActive(false);
            pointerATK.enabled = true;
            atk.interactable = false;
            hl.gameObject.SetActive(true);
            hl.interactable = true;
        }
        else if (!enemyAttacked2)
        {
            enemyAttacked2 = true;
            yield return new WaitForSeconds(2f);
            mouseClicked = false;
            textBG.SetActive(true);
            DisableTurn();
            turnColdalla.enabled = true;
            coldallaCircle.color = Color.green;
            enemyCircle.color = Color.black;
            text.text = "Your other two items are a <color=green>HEALING POTION</color> that heals <color=green>60 HP</color> and some <color=blue>ICE CUBES</color> that <color=blue>freezes</color> the enemy for <color=blue>one turn</color>.";
            //yield return new WaitForSeconds(delay);
            yield return new WaitUntil(() => mouseClicked);
            mouseClicked = false;
            text.text = "You can <color=red>attack</color> with Coldalla and <color=blue>freeze</color> the enemy with Cottama, your enemy will have no chance of doing anything.";
            //yield return new WaitForSeconds(delay);
            yield return new WaitUntil(() => mouseClicked);
            mouseClicked = false;
            text.text = "But be careful, the items have a limit of 2 uses per fight. Let's skip Coldalla's turn and try the <color=blue>ICE CUBES!</color>";
            //yield return new WaitForSeconds(delay);
            yield return new WaitUntil(() => mouseClicked);
            mouseClicked = false;
            DisableTurn();
            turnCottama.enabled = true;
            cottamaCircle.color = Color.green;
            coldallaCircle.color = Color.black;
            textBG.SetActive(false);
            pointerFreeze.enabled = true;
            freeze.interactable = true;
        }
    }

    public IEnumerator Heal(BattleUnit target)
    {
        cottama.Heal(target);
        yield return new WaitForSeconds(2f);

        if (!healedColdalla)
        {
            healedColdalla = true;
            DisableTurn();
            turnEnemy.enabled = true;
            enemyCircle.color = Color.green;
            cottamaCircle.color = Color.black;
            StartCoroutine(Attack(enemy, cottama, false));
        }
    }

    public IEnumerator Turn()
    {
        yield return new WaitForSeconds(2f);
        mouseClicked = false;
        textBG.SetActive(true);
        DisableTurn();
        turnEnemy.enabled = true;
        enemyCircle.color = Color.green;
        cottamaCircle.color = Color.black;
        text.text = "Your enemy was <color=blue>frozen</color> and could not perform his action, so, It's your turn again!";
        //yield return new WaitForSeconds(delay);
        yield return new WaitUntil(() => mouseClicked);
        mouseClicked = false;
        DisableTurn();
        turnColdalla.enabled = true;
        coldallaCircle.color = Color.green;
        enemyCircle.color = Color.black;
        enemy.RemoveFreeze();
        text.text = "Let's finish this battle with Coldalla's <color=red>Ultimate</color>, an attack that deals <color=red>2.5x</color> damage, when your blue bar is filled, the <color=red>ultimate</color> button will appear. Lets try!";
        //yield return new WaitForSeconds(delay + 2);
        yield return new WaitUntil(() => mouseClicked);
        mouseClicked = false;
        textBG.SetActive(false);
        hl.gameObject.SetActive(false);
        pointerUlt.enabled = true;
        ult.gameObject.SetActive(true);
        ult.interactable = true;
    }

    public IEnumerator FinishBattle()
    {
        yield return new WaitForSeconds(2f);
        mouseClicked = false;
        textBG.SetActive(true);
        text.text = "We finished the tutorial for the battle system, now you can click on the <color=yellow>Next Battle</color> button to start the next battle.";
        //yield return new WaitForSeconds(delay);
        yield return new WaitUntil(() => mouseClicked);
        mouseClicked = false;
        text.text = "Thanks for your time and comprehension.\n<color=yellow>Good luck!</color>";
        //yield return new WaitForSeconds(delay);
        yield return new WaitUntil(() => mouseClicked);
        mouseClicked = false;
        nextBattle.gameObject.SetActive(true);
    }
}
