using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;

public class SoundToggleButton : MonoBehaviour
{
    private Sprite soundOnSprite;
    private Sprite soundOffSprite;
    private Image buttonImage;

    void Start()
    {
        buttonImage = GetComponent<Image>();
        LoadButtonSprites();
        SetButtonSprite();
    }

    private void LoadButtonSprites()
    {
        soundOnSprite = Resources.Load<Sprite>(Path.Combine("ButtonsSprites", "SoundOn"));
        soundOffSprite = Resources.Load<Sprite>(Path.Combine("ButtonsSprites", "SoundOff"));
    }

    public void ToggleSound()
    {
        GameController.Instance.Sound = !GameController.Instance.Sound;
        SetButtonSprite();
    }

    private void SetButtonSprite()
    {
        if (GameController.Instance.Sound)
        {
            buttonImage.sprite = soundOnSprite;
        }
        else
        {
            buttonImage.sprite = soundOffSprite;
        }
    }
}
