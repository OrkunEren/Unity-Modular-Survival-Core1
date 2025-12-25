using System.Collections;
using UnityEngine;

public class ResourceNode : MonoBehaviour , IDamageable
{
    [Header("Settings")]
    [SerializeField] private float maxHealth;
    [SerializeField] private GameObject hitParticle;

    [Header("Loot Table")]
    [SerializeField] private ItemData itemToDrop;
    [SerializeField] private int dropCount;

    private float currentHealth;
    private Vector3 originalPos;
    private bool isShaking;

    private void Start()
    {
        currentHealth = maxHealth;
        originalPos = transform.localPosition;
        gameObject.layer = LayerMask.NameToLayer("Resource");
    }

    public void TakeDamage(float amount) 
    {
        currentHealth -= amount;

        StartCoroutine(ShakeObject());

        if (hitParticle != null)
        {
            Instantiate(hitParticle, transform.position, Quaternion.identity);
        }

        if (currentHealth <= 0)
            DestroyNode();

    }

    private void DestroyNode() 
    {
        for (int i = 0; i < dropCount; i++)
        {
            Vector3 randomPos = transform.position + new Vector3(Random.Range(-1f, 1f), 1f, Random.Range(-1f, 1f));

            if (itemToDrop.prefab != null)
                Instantiate(itemToDrop.prefab, randomPos, Quaternion.identity);

        }

        Destroy(gameObject);
    }

    IEnumerator ShakeObject() 
    {
        if(isShaking) yield break;
        isShaking = true;

        float elapsed = 0.0f;
        float duration = 0.2f; 
        float magnitude = 0.1f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float z = Random.Range(-1f, 1f) * magnitude;

           
            transform.localPosition = new Vector3(originalPos.x + x, originalPos.y, originalPos.z + z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;
        isShaking = false;
    }
}



