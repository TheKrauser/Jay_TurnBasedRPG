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

    private bool isDead = false;

    private Transform visuals;
    private BoxCollider2D box2D;
    private SpriteRenderer sprite;
    private SpriteRenderer circle;
    private Animator anim;

    private Sprite uiSprite;
    private Image healthBar;

    private void Awake()
    {
        visuals = transform.Find("Visuals");

        box2D = GetComponent<BoxCollider2D>();
        sprite = visuals.GetComponent<SpriteRenderer>();
        anim = visuals.GetComponent<Animator>();

        //Searches for the specific gameObject on the childrens of this object
        healthBar = transform.Find("HealthBar/Canvas/Health/Bar").GetComponent<Image>();
        circle = transform.Find("Circle").GetComponent<SpriteRenderer>();
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

        visuals.localPosition = character._visualPosition;
        visuals.localScale = character._visualSize;
        sprite.flipX = character._flip;
        box2D.size = character._colliderSize;
        box2D.offset = character._colliderOffset;

        anim.runtimeAnimatorController = character._animator;

        uiSprite = character._battleSprite;
        healthBar.fillAmount = (float)currentHealth / maxHealth;
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

    //Attacks the target
    public void Attack(BattleUnit target)
    {
        anim.SetTrigger("Attack");
        target.Damage(damage);
        //Gain ult when attack
        GainUlt();
    }

    //Same function as the normal attack, but dealing x2.5 the normal damage
    public void Ultimate(BattleUnit target)
    {
        anim.SetTrigger("Attack");
        target.Damage((int)(damage * 2.5f));
        //Sets the ultimate back to zero
        currentUlt = 0;
    }

    //Gain ult after attack
    private void GainUlt()
    {
        //Gains the ult equivalent to the base damage x2.5
        currentUlt += (int)(damage * 2.5f);

        if (currentUlt >= maxUlt)
            currentUlt = maxUlt;
    }

    //Takes damage
    public void Damage(int damage)
    {
        currentHealth -= damage;
        StartCoroutine(BlinkDamage());

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            isDead = true;
            //Disable the character on the scene instead of destroying it
            //It is better for now cause its not necessary to change the team arrays on BattleHandler script
            gameObject.SetActive(false);
            //Destroy(gameObject);
        }
    }

    public bool IsDead()
    {
        return isDead;
    }

    public void SetCircleColor(Color color)
    {
        circle.color = color;
    }

    //Coroutine to change the sprite color when takes hit
    private IEnumerator BlinkDamage()
    {
        yield return new WaitForSeconds(0.35f);
        sprite.color = Color.red;
        healthBar.fillAmount = (float)currentHealth / maxHealth;
        yield return new WaitForSeconds(0.2f);
        sprite.color = Color.white;
    }

    //Function called when a click occurs on the gameObject 2D collider
    private void OnMouseDown()
    {
        //Only allows to get the target if is time to select
        if (BattleHandler.Instance.isSelectingTarget)
        {
            //Pass this script as the target selected
            BattleHandler.Instance.SelectTarget(this);
        }
    }
}
