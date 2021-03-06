﻿using System;

public class Events
{
    public static event Action<Cell, SwipeTurn> Swipe;
    public static void Swipe_Call(Cell cell, SwipeTurn turn) { Swipe?.Invoke(cell, turn); }

    public static event Action<bool> ToggleControl;
    public static void ToggleControl_Call(bool state) { ToggleControl?.Invoke(state); }

    public static event Action Matched;
    public static void Matched_Call() { Matched?.Invoke(); }

    public static event Action<MenuType> ShowMenu;
    public static void ShowMenu_Call(MenuType menuType) { ShowMenu?.Invoke(menuType); }

    public static event Action LevelStarted;
    public static void LevelStarted_Call() { LevelStarted?.Invoke(); }
}
