using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Events;

public class MouseRaycast : MonoBehaviour
{
    private bool mouseOver = false;

    [SerializeField] private LayerMask layer; //The only layer that will check for raycast (is set on the inspector)

    public event EventHandler<OnMouseOverCharacterParameters> OnMouseLeaveCharacter;
    //An event that receives a custom class that I can put any variable to pass on the EventArgs
    public event EventHandler<OnMouseOverCharacterParameters> OnMouseOverCharacter;
    public class OnMouseOverCharacterParameters
    {
        public BattleUnit unit;
    }

    private BattleUnit unitTheMouseIsHover;

    private void Update()
    {
        //Raycast that detects objects on the given layer and always follow the mouse pointer
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1, layer);

        //If has a 2D collider
        if (hit.collider != null)
        {
            //If mouse is not already inside anything
            if (!mouseOver)
            {
                unitTheMouseIsHover = hit.collider.GetComponent<BattleUnit>();
                OnMouseOverCharacter?.Invoke(this, new OnMouseOverCharacterParameters{unit = unitTheMouseIsHover});
                //Sets true on the first iteraction to not do the same function multiple times while the mouse
                //is still on the collider
                mouseOver = true;
            }
        }
        else
        {
            if (mouseOver)
            {
                OnMouseLeaveCharacter?.Invoke(this, new OnMouseOverCharacterParameters{unit = unitTheMouseIsHover});
                mouseOver = false;
            }
        }
    }
}