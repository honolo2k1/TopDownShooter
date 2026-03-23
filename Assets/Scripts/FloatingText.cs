using UnityEngine;

public class FloatingText : MonoBehaviour
{
    [SerializeField] private float lifeTime = 2f;
    private Vector3 offset = new Vector3(0, 1.5f, 0);
    private Vector3 randomizeIntensity = new Vector3(0.5f, 0, 0);

    private void OnEnable()
    {
        transform.localPosition += offset;
        transform.localPosition += new Vector3(
            Random.Range(-randomizeIntensity.x, randomizeIntensity.x),
            Random.Range(-randomizeIntensity.y, randomizeIntensity.y),
            Random.Range(-randomizeIntensity.z, randomizeIntensity.z));

        // Return to pool after lifetime instead of Destroy
        ObjectPool.Instance.ReturnObject(gameObject, lifeTime);
    }
}
