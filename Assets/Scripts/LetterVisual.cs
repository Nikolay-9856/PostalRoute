using UnityEngine;
// Лёгкое покачивание конверта, чтобы письмо было заметно на поле. Номер не вращается.
public class LetterVisual : MonoBehaviour
{
    [SerializeField, Range(0f, 0.3f)] private float bobAmount = 0.08f;
    [SerializeField, Min(0.1f)] private float bobSpeed = 3f;
    private Vector3 startPosition;
    private void Awake()
    {
        startPosition = transform.position;
    }
    private void Update()
    {
        transform.position = startPosition +
            Vector3.up * Mathf.Sin(Time.time * bobSpeed) * bobAmount;
    }
    private void OnDisable()
    {
        transform.position = startPosition;
    }
}
