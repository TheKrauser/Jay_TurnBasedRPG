using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Events;

public class MouseRaycast : MonoBehaviour
{
    private bool mouseOver = false;
    [SerializeField] private LayerMask layer;

    public event EventHandler<OnMouseOverCharacterParameters> OnMouseOverCharacter;
    public class OnMouseOverCharacterParameters : MonoBehaviour
    {
        public BattleUnit unit;
    }

    private void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1, layer);

        if (hit.collider != null)
        {
            if (!mouseOver)
            {
                OnMouseOverCharacter?.Invoke(this, new OnMouseOverCharacterParameters{unit = hit.collider.GetComponent<BattleUnit>()});
                mouseOver = true;
            }
        }
        else
        {
            if (mouseOver)
                mouseOver = false;
        }
    }
}