using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Events;

public class MouseRaycast : MonoBehaviour
{
    private bool mouseOver = false;
    [SerializeField] private LayerMask layer;

    public event EventHandler OnMouseLeaveCharacter;
    public event EventHandler<OnMouseOverCharacterParameters> OnMouseOverCharacter;
    public class OnMouseOverCharacterParameters
    {
        public BattleUnit unit;
    }

    private BattleUnit lastUnit;
    private void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1, layer);

        if (hit.collider != null)
        {
            if (!mouseOver)
            {
                lastUnit = hit.collider.GetComponent<BattleUnit>();
                OnMouseOverCharacter?.Invoke(this, new OnMouseOverCharacterParameters{unit = lastUnit});
                mouseOver = true;
            }
        }
        else
        {
            if (mouseOver)
            {
                OnMouseLeaveCharacter?.Invoke(this, EventArgs.Empty);
                lastUnit.GetSpriteRenderer().color = Color.white;
                mouseOver = false;
            }
        }
    }
}