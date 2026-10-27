using UnityEngine;
// Сторожевая собака: касание отнимает попытку и возвращает курьера к старту.
[RequireComponent(typeof(Collider2D))]
public class Hazard : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent<PlayerController>(out _)) return;
        GameManager.Instance.HitHazard();
    }
}
