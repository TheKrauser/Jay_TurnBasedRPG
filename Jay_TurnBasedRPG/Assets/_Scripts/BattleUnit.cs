using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUnit : MonoBehaviour
{
    [SerializeField] private CharacterSO character;

    private string nome;
    private int maxHealth;
    private int currentHealth;

    private Transform visuals;
    private BoxCollider2D box2D;
    private SpriteRenderer sprite;
    private Animator anim;

    private Sprite uiSprite;

    private void Awake()
    {
        visuals = transform.Find("Visuals");

        box2D = GetComponent<BoxCollider2D>();
        sprite = visuals.GetComponent<SpriteRenderer>();
        anim = visuals.GetComponent<Animator>();
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

        visuals.localPosition = character._visualPosition;
        visuals.localScale = character._visualSize;
        sprite.flipX = character._flip;
        box2D.size = character._colliderSize;
        box2D.offset = character._colliderOffset;

        anim.runtimeAnimatorController = character._animator;

        uiSprite = character._battleSprite;
    }

    public int GetMaxHealth()
    {
        return maxHealth;;
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
}
