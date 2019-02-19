using UnityEngine;
using System.Collections;

public class ShowMenuButton : MonoBehaviour
{
    public MenuType menuType;

    public virtual void ShowMenu() {
        Events.ShowMenu_Call(menuType);
    }

}
