using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(CanvasGroup))]
public class Anim_PanelPop : MonoBehaviour
{
    [Header("Bounce Settings")]
    [SerializeField] bool bounce = false;
    [SerializeField] float startingXScale = 1.5f;
    [SerializeField] float scaleSpeedInSeconds = 0.1f;
    [SerializeField] float overBounceAmount = -0.1f;
    [SerializeField] float bounceBackSpeedInSeconds = .1f;

    [Header("Fade Settings")]
    [SerializeField] bool fade = true;
    [SerializeField] float startingOpacity = .5f;
    [SerializeField] float opacityChangeSpeedInSeconds = 0.1f;

    CanvasGroup canvasGroup;
    RectTransform panelToAnimate;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        panelToAnimate = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        // animate our panel pop
        if(bounce)
            StartCoroutine(PopIn());
        if(fade)
            StartCoroutine(FadeIn());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    IEnumerator PopIn()
    {
        float newXScale;
        float destinationXScale = 1 + overBounceAmount;
        // growth cycle. Use unscaled time so that we're immune to timescale (for example, pause menus)
        for(float t = 0; t < 1.0f; t += Time.unscaledDeltaTime / scaleSpeedInSeconds)
        {
            Debug.Log("Grow " + t);
            // adjust current size
            newXScale = Mathf.Lerp(startingXScale, destinationXScale, t);
            panelToAnimate.localScale = new Vector3(newXScale, 1, 1);
            yield return null;
        }
        // bounce back
        for (float t = 0; t < 1.0f; t += Time.unscaledDeltaTime / bounceBackSpeedInSeconds)
        {
            // adjust current size
            newXScale = Mathf.Lerp(destinationXScale, 1, t);
            panelToAnimate.localScale = new Vector3(newXScale, 1, 1);
            yield return null;
        }
        // ensure that we've hit our normal scale
        panelToAnimate.localScale = new Vector3(1, 1, 1);
    }

    IEnumerator FadeIn()
    {
        float newOpacityValue;

        // fade in
        for (float t = 0; t < 1.0f; t += Time.unscaledDeltaTime / opacityChangeSpeedInSeconds)
        {
            // adjust current size
            newOpacityValue = Mathf.Lerp(startingOpacity, 1, t);
            canvasGroup.alpha = newOpacityValue;
            yield return null;
        }
        // ensure that we've hit 1
        canvasGroup.alpha = 1;
    }
}

