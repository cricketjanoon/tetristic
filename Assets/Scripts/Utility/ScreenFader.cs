using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

[RequireComponent(typeof(MaskableGraphic))]
public class ScreenFader : MonoBehaviour
{
    public float startAlpha = 1f;
    public float targetAlpha = 0f;
    public float delay = 0f;
    public float timeToFade = 1f;

    private float inc;
    private float currentAlpha;
    private MaskableGraphic graphic;
    private Color originalColor;

    // Use this for initialization
    private void Start()
    {
        graphic = GetComponent<MaskableGraphic>();
        originalColor = graphic.color;
        currentAlpha = startAlpha;
        Color tempColor = new Color(originalColor.r, originalColor.g, originalColor.b, currentAlpha);
        graphic.color = tempColor;

        inc = ((targetAlpha - startAlpha) / timeToFade) * Time.deltaTime;

        StartCoroutine("FadeRoutine");
    }

    private IEnumerator FadeRoutine()
    {
        yield return new WaitForSeconds(delay);

        while (Mathf.Abs(targetAlpha - currentAlpha) > 0.01f)
        {
            yield return new WaitForEndOfFrame();
            currentAlpha = currentAlpha + inc;
            Color tempColor = new Color(originalColor.r, originalColor.g, originalColor.b, currentAlpha);
            graphic.color = tempColor;
        }

        //Debug.Log("ScreenFader finished.");
    }
}