using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GS3.Utility
{
    [RequireComponent(typeof(Image))]
    public class FlashImage : MonoBehaviour
    {
        [Header("General")]
        [Range(0, 1)] [SerializeField] float minAlpha = 0f;
        [Range(0, 1)] [SerializeField] float maxAlpha = 1f;
        [SerializeField] float flashSpeed = 1f;     // flashes per second

        Image flashImage;

        private void Awake()
        {
            // initialize alpha start, so that it doesn't jump on the first frame
            flashImage = GetComponent<Image>();
        }

        private void OnEnable()
        {
            StartCoroutine(Flash());
        }

        void OnDisable()
        {
            StopCoroutine(Flash());
        }

        IEnumerator Flash()
        {
            while (true)
            {
                // 1 flash cycle
                for (float t = 0f; t < 1.0f; t += Time.deltaTime / flashSpeed)
                {
                    // adjust current alpha
                    Color newColor = flashImage.color;
                    newColor.a = Mathf.Lerp(minAlpha, maxAlpha, t);
                    flashImage.color = newColor;
                    yield return null;
                }
                // 1 flash cycle
                for (float t = 1f; t > 0; t -= Time.deltaTime / flashSpeed)
                {
                    // adjust current alpha
                    Color newColor = flashImage.color;
                    newColor.a = Mathf.Lerp(minAlpha, maxAlpha, t);
                    flashImage.color = newColor;
                    yield return null;
                }
            }

        }

    }
}

