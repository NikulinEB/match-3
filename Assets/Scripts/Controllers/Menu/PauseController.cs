using UnityEngine;
using System.Collections;

public class PauseController : MonoBehaviour
{

    void Awake()
    {
        Events.ShowMenu += TogglePause;
    }

    private void OnDestroy()
    {
        Events.ShowMenu -= TogglePause;
    }

    private void TogglePause(MenuType menuType) {
        if (menuType == MenuType.Game)  
        {
            Time.timeScale = 1;
        } else
        {
            Time.timeScale = 0;
        }
    }
}
