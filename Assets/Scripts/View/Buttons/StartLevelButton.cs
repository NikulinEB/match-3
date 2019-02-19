using UnityEngine;
using System.Collections;

public class StartLevelButton : ShowMenuButton
{
    public override void ShowMenu()
    {
        GameController.Instance.StartLevel();
        base.ShowMenu();
    }
}
