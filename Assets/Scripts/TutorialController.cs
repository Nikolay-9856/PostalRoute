using UnityEngine;
using UnityEngine.UI;
// Три вводные карточки: управление, особое правило, ошибка и завершение.
public class TutorialController : MonoBehaviour
{
    // Отдельный ключ: факт просмотра обучения не смешивается с прогрессом уровней.
    public const string SeenKey = "PP13.Postal.Tutorial.v1";
    private readonly string[] steps =
    {
        "1 / 3 · УПРАВЛЕНИЕ\nУдерживайте экранную стрелку или клавиши WASD / стрелки — курьер идёт. Отпустите — курьер остановится. Пауза — кнопка «Пауза» или Esc.",
        "2 / 3 · ПИСЬМО И ЯЩИК\nКоснитесь конверта, чтобы поднять письмо. Номер на конверте — адрес: опустите письмо в ящик с тем же номером. Пока письмо в сумке, второе не поднимется.",
        "3 / 3 · ОШИБКА И ЗАВЕРШЕНИЕ\nЯщик с другим номером не примет письмо и отнимет попытку. Собака «!» тоже отнимает попытку. Доставьте все письма и вернитесь на почту (зелёный домик)."
    };
    private GameObject panel;
    private Text message, nextLabel;
    private int index;
    public bool IsOpen => panel != null && panel.activeSelf;
    public int Step => index;
    public void Initialize(MenuController ui, Transform parent)
    {
        panel = ui.Panel("TutorialPanel", parent, 0, 0, 1000, 580);
        ui.Label(panel.transform, "КАК ИГРАТЬ", 0, 220, 900, 60, 36);
        message = ui.Label(panel.transform, "", 0, 45, 900, 250, 27);
        var next = ui.Button(panel.transform, "TutorialNext", "Далее", 200, -200,
            360, 70, Advance);
        nextLabel = next.GetComponentInChildren<Text>();
        ui.Button(panel.transform, "TutorialSkip", "Пропустить", -200, -200,
            360, 70, Close);
        panel.SetActive(false);
        if (PlayerPrefs.GetInt(SeenKey, 0) == 0) Open();
    }
    public void Open()
    {
        if (GameManager.Instance.CurrentState != GameManager.State.Menu) return;
        index = 0;
        panel.SetActive(true);
        panel.transform.SetAsLastSibling();
        Refresh();
    }
    public void Advance()
    {
        if (!IsOpen) return;
        if (index < steps.Length - 1) { index++; Refresh(); }
        else Close();
    }
    public void Close()
    {
        PlayerPrefs.SetInt(SeenKey, 1);
        PlayerPrefs.Save();
        panel.SetActive(false);
    }
    private void Refresh()
    {
        message.text = steps[index];
        nextLabel.text = index == steps.Length - 1 ? "В меню" : "Далее";
    }
}
