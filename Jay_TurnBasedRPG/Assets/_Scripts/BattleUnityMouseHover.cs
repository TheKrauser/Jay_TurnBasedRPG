using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class BattleUnityMouseHover : MonoBehaviour
{
    private bool mouseOver = false;
    [SerializeField] private LayerMask layer;

    void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1, layer);

        if (hit.collider != null)
        {
            if (!mouseOver)
            { 
                mouseOver = !mouseOver;
            }
        }
        else
        {
            mouseOver = !mouseOver;
        }
    }
}