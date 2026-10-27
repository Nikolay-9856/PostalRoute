using System.Collections.Generic;
using UnityEngine;
public class WorldBuilder : MonoBehaviour
{
    public PlayerController Player { get; private set; }
    private GameObject root;
    private GameObject carried;
    private TextMesh carriedLabel;
    private Font font;
    private readonly Dictionary<int, Sprite> sprites = new();
    // Письмо i адресовано ящику i; ящики стоят в другой части поля, чем письма.
    private readonly Vector2[] letters =
    {
        new(-5.6f, 0.6f), new(-2.4f, 3.3f), new(-1.3f, -1.6f),
        new(1.8f, 1.3f), new(3.4f, -3.5f)
    };
    private readonly Vector2[] mailboxes =
    {
        new(6.7f, -3.3f), new(6.9f, 0.4f), new(3.0f, 3.5f),
        new(-0.3f, -3.6f), new(5.6f, 2.6f)
    };
    private readonly Vector2[] hazards =
    {
        new(-4.8f, -1.8f), new(-1.5f, 0.9f), new(2.6f, -0.6f),
        new(5.0f, -1.5f), new(-6.4f, 2.0f), new(1.3f, -3.7f)
    };
    public void BuildCamera()
    {
        Camera cam = Camera.main;
        if (cam == null)
        {
            var go = new GameObject("Main Camera");
            go.tag = "MainCamera";
            cam = go.AddComponent<Camera>();
        }
        cam.orthographic = true;
        cam.orthographicSize = FitSize(cam.aspect);
        cam.transform.position = new Vector3(0, 0, -10);
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = VisualTheme.Background;
        if (!cam.GetComponent<AudioListener>())
            cam.gameObject.AddComponent<AudioListener>();
    }
    // Узкое окно (4:3) получает больший обзор, чтобы поле 17 единиц не обрезалось.
    private static float FitSize(float aspect) => Mathf.Max(6.2f, 9f / aspect);
    private void LateUpdate()
    {
        if (Camera.main != null)
            Camera.main.orthographicSize = FitSize(Camera.main.aspect);
    }
    public void Clear()
    {
        if (!root) return;
        root.SetActive(false);
        Destroy(root);
        carried = null;
    }
    public void Build(LevelData data)
    {
        Clear();
        font ??= Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        root = new GameObject("LevelObjects");
        Item("Field", Vector2.zero, new(17, 9.5f), VisualTheme.Surface, 0, -10);
        Wall("Top", new(0, 4.7f), new(17, 0.3f));
        Wall("Bottom", new(0, -4.7f), new(17, 0.3f));
        Wall("Left", new(-8.4f, 0), new(0.3f, 9.6f));
        Wall("Right", new(8.4f, 0), new(0.3f, 9.6f));
        Wall("A", new(-3.8f, 1.2f), new(0.35f, 3));
        if (data.target >= 4)
            Wall("B", new(1.4f, -2.2f), new(3.2f, 0.35f));
        if (data.target >= 5)
            Wall("C", new(4.4f, 1.6f), new(0.35f, 2.5f));
        var office = Item("PostOffice", new(-6.9f, 3.4f), new(1.2f, 1.2f),
            VisualTheme.Success, 7, 2);
        office.AddComponent<BoxCollider2D>();
        office.AddComponent<ExitZone>();
        Label(office.transform, "ПОЧТА", new(0, -0.85f), 0.22f, VisualTheme.Success, 20);
        var player = Item("Player", new(-6.4f, -3.0f), new(0.7f, 0.7f),
            VisualTheme.Player, 1, 10);
        player.AddComponent<Rigidbody2D>();
        player.AddComponent<CircleCollider2D>();
        Player = player.AddComponent<PlayerController>();
        carried = Item("CarriedLetter", Vector2.zero, new(0.55f, 0.4f),
            VisualTheme.Letter, 5, 12);
        carried.transform.SetParent(player.transform, false);
        carried.transform.localPosition = new Vector3(0, 0.95f, 0);
        carried.transform.localScale = new Vector3(0.8f, 0.58f, 1);
        carriedLabel = Label(carried.transform, "", Vector2.zero, 0.3f, VisualTheme.DarkText, 13);
        carried.SetActive(false);
        int count = Mathf.Clamp(data.target, 1, letters.Length);
        for (int i = 0; i < count; i++)
        {
            int address = i + 1;
            Color tint = VisualTheme.AddressTint[i];
            var l = Item("Letter" + address, letters[i], new(0.9f, 0.62f),
                Color.Lerp(VisualTheme.Letter, tint, 0.35f), 5, 5);
            l.AddComponent<BoxCollider2D>();
            l.AddComponent<LetterVisual>();
            l.AddComponent<Letter>().Setup(address);
            Label(l.transform, address.ToString(), Vector2.zero, 0.42f, VisualTheme.DarkText, 6);
            var m = Item("Mailbox" + address, mailboxes[i], new(0.95f, 1.05f),
                VisualTheme.Mailbox, 6, 3);
            m.AddComponent<BoxCollider2D>();
            m.AddComponent<Mailbox>().Setup(address);
            Label(m.transform, "№" + address, new(0, -0.12f), 0.36f, tint, 7);
        }
        for (int i = 0; i < Mathf.Clamp(data.hazards, 1, hazards.Length); i++)
        {
            var h = Item("Dog" + (i + 1), hazards[i], new(0.8f, 0.8f),
                VisualTheme.Danger, 1, 4);
            h.AddComponent<CircleCollider2D>();
            h.AddComponent<Hazard>();
            Label(h.transform, "!", Vector2.zero, 0.5f, Color.white, 8);
        }
    }
    // Значок письма над курьером: показывает адрес груза прямо на поле.
    public void ShowCarried(int address)
    {
        if (carried == null) return;
        carried.SetActive(address != 0);
        if (address != 0)
        {
            carriedLabel.text = address.ToString();
            carried.GetComponent<SpriteRenderer>().color =
                Color.Lerp(VisualTheme.Letter, VisualTheme.AddressTint[address - 1], 0.35f);
        }
    }
    private void Wall(string name, Vector2 p, Vector2 scale)
    {
        Item("Wall" + name, p, scale, VisualTheme.Button, 0, 1)
            .AddComponent<BoxCollider2D>();
    }
    private GameObject Item(string name, Vector2 p, Vector2 scale,
        Color color, int shape, int order)
    {
        var go = new GameObject(name);
        go.transform.SetParent(root.transform);
        go.transform.position = p;
        go.transform.localScale = scale;
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = GetSprite(shape);
        sr.color = color;
        sr.sortingOrder = order;
        return go;
    }
    // Подпись в мире (номер адреса). Масштаб компенсирует неравномерный масштаб родителя.
    private TextMesh Label(Transform parent, string value, Vector2 offset,
        float height, Color color, int order)
    {
        var go = new GameObject("Label");
        go.transform.SetParent(parent, false);
        Vector3 s = parent.lossyScale;
        go.transform.localPosition = new Vector3(offset.x / s.x, offset.y / s.y, -0.1f);
        go.transform.localScale = new Vector3(height / s.x, height / s.y, 1);
        var text = go.AddComponent<TextMesh>();
        text.font = font;
        text.text = value;
        text.fontSize = 64;
        text.characterSize = 1f / 6.4f;
        text.anchor = TextAnchor.MiddleCenter;
        text.alignment = TextAlignment.Center;
        text.color = color;
        var mr = go.GetComponent<MeshRenderer>();
        mr.sharedMaterial = font.material;
        mr.sortingOrder = order;
        return text;
    }
    private Sprite GetSprite(int shape)
    {
        if (sprites.TryGetValue(shape, out Sprite cached)) return cached;
        const int size = 64;
        var texture = new Texture2D(size, size);
        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                float a = (x + 0.5f) / size * 2 - 1;
                float b = (y + 0.5f) / size * 2 - 1;
                bool inside = InShape(shape, a, b);
                texture.SetPixel(x, y, inside ? Color.white : Color.clear);
            }
        texture.Apply();
        var sprite = Sprite.Create(texture, new Rect(0, 0, size, size),
            new Vector2(0.5f, 0.5f), size);
        sprites[shape] = sprite;
        return sprite;
    }
    // Форма спрайта в координатах a, b от −1 до 1.
    private static bool InShape(int shape, float a, float b)
    {
        switch (shape)
        {
            case 1: return a * a + b * b < 0.9f;                                   // круг
            case 5: return Mathf.Abs(a) < 0.96f && Mathf.Abs(b) < 0.94f &&         // конверт
                           Mathf.Abs(b - (0.94f - Mathf.Abs(a) * 0.9f)) > 0.08f;
            case 6: return Mathf.Abs(a) < 0.86f && Mathf.Abs(b) < 0.94f &&         // ящик со щелью
                           !(Mathf.Abs(b - 0.6f) < 0.07f && Mathf.Abs(a) < 0.55f);
            case 7: return (Mathf.Abs(a) < 0.75f && b > -0.95f && b < 0.15f) ||    // домик почты
                           (b >= 0.15f && b < 0.95f - Mathf.Abs(a) * 0.85f);
            default: return true;
        }
    }
    private void OnDestroy()
    {
        foreach (Sprite sprite in sprites.Values)
        {
            Destroy(sprite.texture);
            Destroy(sprite);
        }
    }
}
