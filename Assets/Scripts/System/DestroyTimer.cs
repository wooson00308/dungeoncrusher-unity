using System.Collections;
using UnityEngine;

public class DestroyTimer : MonoBehaviour
{
    [SerializeField] private float destroyTime;

    private void OnEnable()
    {
        StartCoroutine(DestroyTime());
        
    }

    private void OnDisable()
    {
        
    }

    IEnumerator DestroyTime()
    {
        WaitForSeconds destroyWait = new(destroyTime);
        yield return destroyWait;
        DestroyThis();
    }
    
    private void DestroyThis()
    {
        ResourceManager.Instance.Destroy(gameObject);
    }
}