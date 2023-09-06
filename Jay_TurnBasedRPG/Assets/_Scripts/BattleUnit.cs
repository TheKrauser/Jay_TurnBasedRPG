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
        healthBar = transform.Find("HealthBar/Canvas/Health/Bar").GetComponent<Image>();
        circle = transform.Find("Circle").GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        Setup();
    }

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

    public int GetMaxHealth()
    {
        return maxHealth; ;
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

    public void Attack(BattleUnit target)
    {
        anim.SetTrigger("Attack");
        target.Damage(damage);
        GainUlt();
    }

    public void Ultimate(BattleUnit target)
    {
        anim.SetTrigger("Attack");
        target.Damage((int)(damage * 2.5f));
        currentUlt = 0;
    }

    private void GainUlt()
    {
        currentUlt += (int)(damage * 2.5f);

        if (currentUlt >= maxUlt)
            currentUlt = maxUlt;

    }

    public float GetUlt()
    {
        return (float)currentUlt / maxUlt;
    }

    public void Damage(int damage)
    {
        currentHealth -= damage;
        StartCoroutine(BlinkDamage());

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            isDead = true;
            gameObject.SetActive(false);
            //Destroy(gameObject);
        }
    }

    public bool GetDead()
    {
        return isDead;
    }

    public void SetCircle(Color color)
    {
        circle.color = color;
    }

    private IEnumerator BlinkDamage()
    {
        yield return new WaitForSeconds(0.35f);
        sprite.color = Color.red;
        healthBar.fillAmount = (float)currentHealth / maxHealth;
        yield return new WaitForSeconds(0.2f);
        sprite.color = Color.white;
    }

    private void OnMouseDown()
    {
        if (BattleHandler.Instance.targetSelection)
        {
            BattleHandler.Instance.SelectTarget(this);
        }
    }
}
