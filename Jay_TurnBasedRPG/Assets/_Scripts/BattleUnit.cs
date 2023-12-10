using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUnit : MonoBehaviour
{
    [SerializeField] private CharacterSO character;

    private string nome;
    private int maxHealth;
    private int currentHealth;
    private int maxUlt;
    private int currentUlt;
    private int damage;

    private bool hasUltimate;
    private bool isFreezed;
    private bool attackBuffed;

    private bool isDead = false;

    private Transform visuals;
    private BoxCollider2D box2D;
    private SpriteRenderer sprite;
    private SpriteRenderer circle;
    private Animator anim;

    private Sprite uiSprite;
    private Image healthBar;
    private Image manaBar;
    //private Image freezeIndicator;
    //private Image attackBuffIndicator;

    private Color currentColor = Color.white;

    [SerializeField] private bool isTutorial;

    private GameObject particleBuff, particleFreeze;

    private void Awake()
    {
        visuals = transform.Find("Visuals");

        box2D = GetComponent<BoxCollider2D>();
        sprite = visuals.GetComponent<SpriteRenderer>();
        anim = visuals.GetComponent<Animator>();

        //Searches for the specific gameObject on the childrens of this object
        healthBar = transform.Find("UI/Canvas/Health/Bar").GetComponent<Image>();
        manaBar = transform.Find("UI/Canvas/Mana/Bar").GetComponent<Image>();
        //freezeIndicator = transform.Find("UI/Canvas/Freeze").GetComponent<Image>();
        //attackBuffIndicator = transform.Find("UI/Canvas/Buff").GetComponent<Image>();
        particleBuff = transform.Find("Buff").gameObject;
        particleFreeze = transform.Find("Freeze").gameObject;
        circle = transform.Find("Circle").GetComponent<SpriteRenderer>();

        //freezeIndicator.enabled = false;
        //attackBuffIndicator.enabled = false;
        particleFreeze.gameObject.SetActive(false);
        particleBuff.gameObject.SetActive(false);
    }

    private void Start()
    {
        Setup();
    }

    //Fill this character variables getting from the ScriptableObject that has been
    //passed to it on the Inspector
    private void Setup()
    {
        nome = character._name;
        maxHealth = character._health;
        currentHealth = maxHealth;
        damage = character._damage;
        maxUlt = character._ultMax;
        currentUlt = 0;
        hasUltimate = character._hasUltimate;
        isFreezed = false;
        attackBuffed = false;

        visuals.localPosition = character._visualPosition;
        visuals.localScale = character._visualSize;
        sprite.flipX = character._flip;
        box2D.size = character._colliderSize;
        box2D.offset = character._colliderOffset;

        anim.runtimeAnimatorController = character._animator;

        uiSprite = character._battleSprite;
        healthBar.fillAmount = (float)currentHealth / maxHealth;
        manaBar.fillAmount = (float)currentUlt / maxUlt;
    }

    //Functions to get the values from the variables without referencing them
    public int GetMaxHealth()
    {
        return maxHealth;
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public Sprite GetUISprite()
    {
        return uiSprite;
    }

    public string GetName()
    {
        return nome;
    }

    public SpriteRenderer GetSpriteRenderer()
    {
        return sprite;
    }

    public float GetUltimate()
    {
        return (float)currentUlt / maxUlt;
    }

    public bool GetFreezed()
    {
        return isFreezed;
    }

    public void BuffAttack()
    {
        if (!attackBuffed)
        {
            AudioManager.Instance.PlaySound("Buff");
            attackBuffed = true;
            //currentColor = Color.red;
            //attackBuffIndicator.enabled = true;
            particleBuff.gameObject.SetActive(true);
        }
    }

    public bool GetBuffed()
    {
        return attackBuffed;
    }

    public Color GetCurrentColor()
    {
        return currentColor;
    }

    //Attacks the target
    public void Attack(BattleUnit target)
    {
        anim.SetTrigger("Attack");
        if (attackBuffed)
        {
            StartCoroutine(PlaySound("Buffed"));
            target.Damage((int)(damage * 1.5f));
            attackBuffed = false;
            particleBuff.gameObject.SetActive(false);
            //currentColor = Color.white;
            //attackBuffIndicator.enabled = false;
        }
        else
        {
            StartCoroutine(PlaySound("Damage"));
            target.Damage(damage);
        }

        StartCoroutine(ParticleManager.Instance.InstantiateParticle("P3", target.transform, 3f, false, 2f, true));
        //Gain ult when attack
        if (hasUltimate)
        {
            GainUlt();
        }
    }

    public void Heal(BattleUnit target)
    {
        anim.SetTrigger("Attack");
        target.RestoreHealth(damage);
    }

    //Same function as the normal attack, but dealing x2.5 the normal damage
    public void Ultimate(BattleUnit target)
    {
        StartCoroutine(PlaySound("Ultimate"));
        anim.SetTrigger("Attack");
        if (!attackBuffed)
        {
            target.Damage((int)(damage * 2.5f));
        }
        else
        {
            target.Damage((int)(damage * 1.5f * 2.5f));
            attackBuffed = false;
            particleBuff.gameObject.SetActive(false);
        }
        StartCoroutine(ParticleManager.Instance.InstantiateParticle("P2", target.transform, 3f, false, 3f, true));
        //Sets the ultimate back to zero
        currentUlt = 0;
        manaBar.fillAmount = 0;
    }

    public IEnumerator PlaySound(string n)
    {
        yield return new WaitForSeconds(0.35f);
        AudioManager.Instance.PlaySound(n);
    }

    public void Freeze()
    {
        if (!isFreezed)
        {
            AudioManager.Instance.PlaySound("Freeze");
            sprite.color = Color.cyan;
            isFreezed = true;
            particleFreeze.gameObject.SetActive(true);
            currentColor = Color.cyan;
            //freezeIndicator.enabled = true;
        }
    }

    public void RemoveFreeze()
    {
        isFreezed = false;
        particleFreeze.gameObject.SetActive(false);
        currentColor = Color.white;
        sprite.color = currentColor;
        //freezeIndicator.enabled = false;
    }

    //Gain ult after attack
    private void GainUlt()
    {
        //Gains the ult equivalent to the base damage x2.5
        currentUlt += (int)(damage * 2.5f);
        manaBar.fillAmount = (float)currentUlt / maxUlt;

        if (currentUlt >= maxUlt)
            currentUlt = maxUlt;
    }

    //Takes damage
    public void Damage(int damage)
    {
        currentHealth -= damage;
        StartCoroutine(Blink());
        StartCoroutine(Hurt());

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            isDead = true;
            //Disable the character on the scene instead of destroying it
            //It is better for now cause its not necessary to change the team arrays on BattleHandler script
            StartCoroutine(Dead());
            //Destroy(gameObject);
        }
    }

    private IEnumerator Hurt()
    {
        yield return new WaitForSeconds(0.35f);
        anim.SetTrigger("Hurt");
    }

    public IEnumerator Dead()
    {
        yield return new WaitForSeconds(0.35f);
        gameObject.SetActive(false);
    }

    public void RestoreHealth(int value)
    {
        AudioManager.Instance.PlaySound("Heal");
        currentHealth += value;
        StartCoroutine(Blink());
        StartCoroutine(ParticleManager.Instance.InstantiateParticle("P1", transform, 3f, false, 1f, true));

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    public bool IsDead()
    {
        return isDead;
    }

    public int GetDamage()
    {
        return damage;
    }

    public void SetCircleColor(Color color)
    {
        circle.color = color;
    }

    //Coroutine to change the sprite color when takes hit
    private IEnumerator Blink()
    {
        yield return new WaitForSeconds(0.35f);
        //sprite.color = color;
        healthBar.fillAmount = (float)currentHealth / maxHealth;
        yield return new WaitForSeconds(0.2f);
        //sprite.color = GetCurrentColor();
    }

    //Function called when a click occurs on the gameObject 2D collider
    private void OnMouseDown()
    {
        if (!isTutorial)
        {
            //Only allows to get the target if is time to select
            if (BattleHandler.Instance.isSelectingTarget)
            {
                //Pass this script as the target selected
                BattleHandler.Instance.SelectTarget(this);
            }
            else if (BattleHandler.Instance.isUsingItem)
            {
                BattleHandler.Instance.SelectItemTarget(this);
            }
        }
    }
}
