using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anim_GrowShrink : MonoBehaviour {

    [SerializeField] float minSize = 1f;
    [SerializeField] float maxSize = 1.1f;
    [SerializeField] float speed = 1f;

    private void OnEnable()
    {
        StartCoroutine(GrowShrink());
    }

    private void OnDisable()
    {
        StopCoroutine(GrowShrink());
    }

    IEnumerator GrowShrink()
    {
        while(true)
        {
            float sizeAdjustment;
            // growth cycle - use unscaled time so that we ignore pause
            for (float t = 0; t < 1.0f; t += Time.unscaledDeltaTime / speed)
            {
                // adjust current size
                sizeAdjustment = Mathf.Lerp(minSize, maxSize, t);
                transform.localScale = new Vector3(sizeAdjustment, sizeAdjustment, sizeAdjustment);
                yield return null;
            }
            // shrink cycle
            for (float t = 1; t > 0; t -= Time.unscaledDeltaTime / speed)
            {
                // adjust current size
                sizeAdjustment = Mathf.Lerp(minSize, maxSize, t);
                transform.localScale = new Vector3(sizeAdjustment, sizeAdjustment, sizeAdjustment);
                yield return null;
            }
        }
    }
}
