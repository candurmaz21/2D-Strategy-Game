using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public SpriteRenderer healthBarSprite;
    //Change color values.
    public Color highHealthColor = Color.green;
    public Color mediumHealthColor = Color.yellow;
    public Color lowHealthColor = Color.red;

    //Values.
    private float maxHealth;
    private Vector3 originalScale;

    private void Awake()
    {
        originalScale = healthBarSprite.transform.localScale;
    }
    public void Initialize(float maxHealth)
    {
        this.maxHealth = maxHealth;
        UpdateHealth(maxHealth);
    }
    public void UpdateHealth(float currentHealth)
    {
        float healthPercentage = currentHealth / maxHealth;

        if (healthPercentage > 0.66f)
        {
            //Above 66%.
            healthBarSprite.color = highHealthColor;
        }
        else if (healthPercentage > 0.33f)
        {
            //Between 33% and 66%.
            healthBarSprite.color = mediumHealthColor;
        }
        else
        {
            //Below 33%.
            healthBarSprite.color = lowHealthColor;
        }

        //Use animated.
        StopAllCoroutines();
        if (this.isActiveAndEnabled)
            StartCoroutine(SmoothHealthChange(healthPercentage));
    }
    public void StopCoroutines()
    {
        StopAllCoroutines();
    }

    private IEnumerator SmoothHealthChange(float targetPercentage)
    {
        Vector3 currentScale = healthBarSprite.transform.localScale;
        Vector3 targetScale = new Vector3(originalScale.x * targetPercentage, originalScale.y, originalScale.z);

        float elapsedTime = 0f;
        float duration = 0.5f;

        while (elapsedTime < duration)
        {
            healthBarSprite.transform.localScale = Vector3.Lerp(currentScale, targetScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        healthBarSprite.transform.localScale = targetScale;
    }
}
