using System;

public class Events
{
    public static event Action<Cell, SwipeTurn> Swipe;
    public static void Swipe_Call(Cell cell, SwipeTurn turn) { Swipe?.Invoke(cell, turn); }
}
