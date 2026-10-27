using UnityEngine;
// Экран «Об игре»: название, версия из Player Settings, авторство и источники ресурсов.
public class AboutPanel : MonoBehaviour
{
    // Замените на свои ФИО и группу перед итоговым коммитом.
    public const string Author = "[ФИО обучающегося], группа [шифр группы]";
    private GameObject panel;
    public bool IsOpen => panel != null && panel.activeSelf;
    public void Initialize(MenuController ui, Transform parent)
    {
        panel = ui.Panel("AboutPanel", parent, 0, 0, 1000, 580);
        ui.Label(panel.transform, "ОБ ИГРЕ", 0, 220, 900, 60, 36);
        ui.Label(panel.transform, "Почтовый маршрут · версия " + Application.version +
            "\nАвтор: " + Author +
            "\nПП.ПМ.13, Университет «Синергия», вариант 12\n" +
            "Три района: поднимите письмо и опустите его в ящик с тем же номером.\n" +
            "Графика (примитивы) и звуки (синтез тонов) созданы кодом проекта.\n" +
            "Unity UI; шрифт LegacyRuntime из комплекта Unity.\n" +
            "Источники перечислены в Release/CREDITS.md.",
            0, 10, 940, 320, 24);
        ui.Button(panel.transform, "AboutBack", "Назад", 0, -215,
            420, 65, () => panel.SetActive(false));
        panel.SetActive(false);
    }
    public void Open()
    {
        if (GameManager.Instance.CurrentState != GameManager.State.Menu) return;
        panel.SetActive(true);
        panel.transform.SetAsLastSibling();
    }
}
