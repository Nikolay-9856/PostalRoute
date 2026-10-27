using UnityEngine;
// Письмо с адресом (номером почтового ящика). Поднимается, только если сумка пуста.
[RequireComponent(typeof(Collider2D))]
public class Letter : MonoBehaviour
{
    public int Address { get; private set; }
    public void Setup(int address) => Address = address;
    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent<PlayerController>(out _)) return;
        GameManager.Instance.TryPickUp(this);
    }
}
