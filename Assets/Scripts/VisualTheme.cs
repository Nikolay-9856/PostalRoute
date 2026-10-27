using UnityEngine;
public static class VisualTheme
{
    public static readonly Color Background = new(0.035f, 0.055f, 0.10f);
    public static readonly Color Surface = new(0.07f, 0.10f, 0.18f);
    public static readonly Color Panel = new(0.055f, 0.075f, 0.13f, 0.96f);
    public static readonly Color Button = new(0.12f, 0.18f, 0.30f, 0.94f);
    public static readonly Color Accent = new(0.16f, 0.55f, 0.75f, 0.98f);
    public static readonly Color Player = new(0.19f, 0.88f, 1f);
    public static readonly Color Letter = new(0.98f, 0.93f, 0.78f);
    public static readonly Color Mailbox = new(0.20f, 0.42f, 0.95f);
    public static readonly Color Danger = new(1f, 0.25f, 0.28f);
    public static readonly Color Success = new(0.25f, 0.82f, 0.45f);
    public static readonly Color PrimaryText = Color.white;
    public static readonly Color SecondaryText = new(0.82f, 0.88f, 0.98f);
    public static readonly Color DarkText = new(0.08f, 0.10f, 0.16f);
    // Цвет пары «письмо — ящик» дополняет номер, но не заменяет его.
    public static readonly Color[] AddressTint =
    {
        new(1f, 0.78f, 0.16f), new(1f, 0.55f, 0.75f), new(0.60f, 0.90f, 0.45f),
        new(0.75f, 0.65f, 1f), new(1f, 0.62f, 0.35f)
    };
}
