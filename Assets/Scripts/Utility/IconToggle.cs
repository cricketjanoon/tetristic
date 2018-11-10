using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class IconToggle : MonoBehaviour
{
    public Sprite iconTrue;
    public Sprite iconFalse;

    public bool defaultIconState = true;
    private Image image;

    // Use this for initialization
    private void Start()
    {
        image = GetComponent<Image>();
        image.sprite = (defaultIconState) ? iconTrue : iconFalse;
    }

    public void ToggleIcon(bool state)
    {
        if (!image || !iconTrue || !iconFalse)
        {
            Debug.Log("Toggle Icon missing iconTrue or iconFalse.");
            return;
        }

        image.sprite = (state) ? iconTrue : iconFalse;
    }
}