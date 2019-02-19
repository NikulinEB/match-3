using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour {

    private Dictionary<MenuType, Menu> menuDictionary = new Dictionary<MenuType, Menu>();
    private MenuType currentMenuType;
    private MenuType previousMenuType;

	// Use this for initialization
	void Start () {
        InitMenu();
        Events.ShowMenu += ShowMenu;
        Events.ShowMenu_Call(MenuType.Start);
    }

    private void OnDestroy()
    {
        Events.ShowMenu -= ShowMenu;
    }

    private void InitMenu() {
        Menu[] menuArray = GetComponentsInChildren<Menu>(false);
        for (int i = 0; i < menuArray.Length; i++) {
            menuDictionary.Add(menuArray[i].menuType, menuArray[i]);
        }
    }

    private void ShowMenu(MenuType newMenu) {
        previousMenuType = currentMenuType;
        menuDictionary[currentMenuType].Hide(() => {
            currentMenuType = newMenu;
            menuDictionary[newMenu].Show();
        });
    }
}
