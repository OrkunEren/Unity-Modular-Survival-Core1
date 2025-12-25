using System.Collections;
using UnityEngine;

public class LightningFlash : MonoBehaviour
{
    [SerializeField] Light lightningLight;
    [SerializeField] float flashIntensity;

    private void OnEnable()
    {
        AmbientSound.OnLightningStrike += Flash;
    }

    private void OnDisable()
    {
        AmbientSound.OnLightningStrike -= Flash;
    }

    void Flash() 
    {
        if (gameObject.activeInHierarchy)
            StartCoroutine(FlashRoutine());
    }

    IEnumerator FlashRoutine() 
    {
        if (lightningLight == null) yield break;

        lightningLight.intensity = flashIntensity;
        yield return new WaitForSeconds(0.05f);

        lightningLight.intensity = 0f;
        yield return new WaitForSeconds(0.05f);

        lightningLight.intensity = flashIntensity * 0.5f;
        yield return new WaitForSeconds(0.1f);

        lightningLight.intensity = 0f;
    }

}
