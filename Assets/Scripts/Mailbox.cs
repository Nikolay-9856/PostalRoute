using UnityEngine;
// Почтовый ящик с номером. Засчитывает только письмо со своим адресом.
[RequireComponent(typeof(Collider2D))]
public class Mailbox : MonoBehaviour
{
    public int Address { get; private set; }
    public bool Delivered { get; private set; }
    private SpriteRenderer body;
    public void Setup(int address)
    {
        Address = address;
        body = GetComponent<SpriteRenderer>();
    }
    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }
    public void MarkDelivered()
    {
        Delivered = true;
        if (body != null) body.color = VisualTheme.Success;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent<PlayerController>(out _)) return;
        GameManager.Instance.TryDeliver(this);
    }
}
