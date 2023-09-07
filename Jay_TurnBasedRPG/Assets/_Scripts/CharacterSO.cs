using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

//This is a class to hold data, It is created on the asset menu (like when you create a script)
[CreateAssetMenu(fileName = "Character", menuName = "ScriptableObjects/CharacterSO")]
public class CharacterSO : ScriptableObject
{
    [Header("Battle Stats")]
    public string _name;
    public int _health;
    public int _damage;
    public int _ultMax;

    [Header("Attributes")]
    public bool _flip;
    public Vector3 _visualPosition;
    public Vector3 _visualSize;
    public Vector2 _colliderSize;
    public Vector2 _colliderOffset;

    [Header("Components")]
    public AnimatorOverrideController _animator;
    public Sprite _battleSprite;
}
