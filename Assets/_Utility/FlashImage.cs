using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class FlashImage : MonoBehaviour
{
    [Header("General")]
    [SerializeField] Image imageToFlash;
    [Range(0, 1)] [SerializeField] float minAlpha = 0f;
    [Range(0, 1)] [SerializeField] float maxAlpha = .5f;
    [SerializeField] float flashSpeed = 1f;     // flashes per second

    private void Awake()
    {
        imageToFlash = GetComponent<Image>();
    }

    private void OnDisable()
    {
        // make sure we kill all active coroutines if we disable
        StopAllCoroutines();
    }

    /// <summary>
    /// Flash the specified image. Can be one shot or looping.
    /// </summary>
    /// <param name="isLooping"></param>
    public void Flash(bool isLooping)
    {
        StartCoroutine(FlashStart(isLooping));
    }

    /// <summary>
    /// Stop Flash on this image. Useful if you want to kill the flash early.
    /// </summary>
    public void FlashStop()
    {
        StopAllCoroutines();
    }

    IEnumerator FlashStart(bool isLooping)
    {
        while (true)
        {
            // 1 flash cycle
            for (float t = 0f; t < 1.0f; t += Time.deltaTime / flashSpeed)
            {
                // adjust current alpha
                Color newColor = imageToFlash.color;
                newColor.a = Mathf.Lerp(minAlpha, maxAlpha, t);
                imageToFlash.color = newColor;
                yield return null;
            }
            // 1 flash cycle
            for (float t = 1f; t > 0; t -= Time.deltaTime / flashSpeed)
            {
                // adjust current alpha
                Color newColor = imageToFlash.color;
                newColor.a = Mathf.Lerp(minAlpha, maxAlpha, t);
                imageToFlash.color = newColor;
                yield return null;
            }
            // make sure that we end at 0 alpha, especially if the speed was quick
            Color finalColor = imageToFlash.color;
            finalColor.a = 0;
            imageToFlash.color = finalColor;
            // if we didn't tell it to loop, break out of our loop
            if (isLooping == false)
                break;
        }

    }
}

