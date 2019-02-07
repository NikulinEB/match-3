using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Swipe : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    private Cell _selectedCell;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.pointerEnter)
        {
            if (eventData.pointerEnter.CompareTag("Cell"))
            {
                _selectedCell = eventData.pointerEnter.GetComponent<Cell>();
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_selectedCell)
        {
            if (Mathf.Abs(eventData.delta.x) > Mathf.Abs(eventData.delta.y))
            {
                if (eventData.delta.x > 0)
                {
                    Events.Swipe_Call(_selectedCell, SwipeTurn.Right);
                }
                else
                {
                    Events.Swipe_Call(_selectedCell, SwipeTurn.Left);
                }
            }
            else if (eventData.delta.y > 0)
            {
                Events.Swipe_Call(_selectedCell, SwipeTurn.Up);
            }
            else
            {
                Events.Swipe_Call(_selectedCell, SwipeTurn.Down);
            }
            _selectedCell = null;   
        }
    }
}
