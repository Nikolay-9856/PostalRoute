using UnityEngine;
[System.Serializable]
public class LevelData
{
    public string title;
    [Range(1, 5)] public int target;   // число писем = число ящиков
    [Range(1, 6)] public int hazards;  // число сторожевых собак
    [Min(20)] public float seconds;
}
public class LevelManager : MonoBehaviour
{
    public LevelData[] levels =
    {
        new() { title = "Тихая улица", target = 3,
            hazards = 2, seconds = 90 },
        new() { title = "Рыночная площадь", target = 4,
            hazards = 4, seconds = 80 },
        new() { title = "Вечерний квартал", target = 5,
            hazards = 6, seconds = 70 }
    };
    public int Current { get; private set; }
    public LevelData Data => levels[Current - 1];
    public bool Select(int number)
    {
        if (number < 1 || number > levels.Length ||
            number > SaveService.Unlocked) return false;
        Current = number;
        return true;
    }
}
