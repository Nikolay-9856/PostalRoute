using UnityEngine;
[DefaultExecutionOrder(-50)]
[RequireComponent(typeof(LevelManager), typeof(WorldBuilder))]
public class GameManager : MonoBehaviour
{
    public enum State { Menu, Playing, Paused, Won, Lost }
    public static GameManager Instance { get; private set; }
    public State CurrentState { get; private set; } = State.Menu;
    public bool IsPlaying => CurrentState == State.Playing;
    public Vector2 MobileDirection { get; private set; }
    public int Delivered { get; private set; }
    public int CarriedAddress { get; private set; }   // 0 — сумка пуста
    public int WrongDeliveries { get; private set; }
    public int Hearts { get; private set; }
    public float Remaining { get; private set; }
    public string Hint { get; private set; } = "Выберите уровень";
    public LevelManager Levels { get; private set; }
    // Исправление по итогам проверки понятности: HUD показывает адрес письма в сумке.
    // Выключите в Inspector, чтобы воспроизвести исходное состояние (рисунок 10).
    public bool hudShowsAddress = true;
    private WorldBuilder world;
    private AudioFeedback audioFeedback;
    private float immuneUntil;
    private float elapsed;
    private void Awake()
    {
        Instance = this;
        Levels = GetComponent<LevelManager>();
        world = GetComponent<WorldBuilder>();
        audioFeedback = gameObject.AddComponent<AudioFeedback>();
        gameObject.AddComponent<PauseController>();
        gameObject.AddComponent<MenuController>();
        Application.targetFrameRate = 60;
        AudioListener.volume = SaveService.Volume;
        Time.timeScale = 1;
        world.BuildCamera();
    }
    public void StartLevel(int number)
    {
        if (!Levels.Select(number)) return;
        Time.timeScale = 1;
        Delivered = 0;
        CarriedAddress = 0;
        WrongDeliveries = 0;
        Hearts = 3;
        Remaining = Levels.Data.seconds;
        elapsed = 0;
        immuneUntil = 0;
        MobileDirection = Vector2.zero;
        Hint = "Поднимите письмо и найдите ящик с тем же номером";
        world.Build(Levels.Data);
        CurrentState = State.Playing;
    }
    private void Update()
    {
        if (!IsPlaying) return;
        elapsed += Time.deltaTime;
        Remaining = Mathf.Max(0, Remaining - Time.deltaTime);
        if (Remaining <= 0) Finish(false, "Время закончилось");
    }
    public void SetDirection(Vector2 value) => MobileDirection = value;
    // Особое правило варианта 12: письмо сначала поднять; второе взять нельзя.
    public void TryPickUp(Letter letter)
    {
        if (!IsPlaying || !letter.gameObject.activeSelf) return;
        if (CarriedAddress != 0)
        {
            Hint = $"Сумка занята: сначала доставьте письмо в ящик №{CarriedAddress}";
            return;
        }
        CarriedAddress = letter.Address;
        letter.gameObject.SetActive(false);
        world.ShowCarried(CarriedAddress);
        audioFeedback.PlayCollect();
        Hint = $"Письмо для ящика №{CarriedAddress}. Найдите ящик с тем же номером";
    }
    // Письмо засчитывается только своему адресату; неверный ящик — штраф.
    public void TryDeliver(Mailbox box)
    {
        if (!IsPlaying) return;
        if (box.Delivered)
        {
            Hint = $"Ящик №{box.Address} уже получил своё письмо";
            return;
        }
        if (CarriedAddress == 0)
        {
            Hint = "Сумка пуста: сначала поднимите письмо";
            return;
        }
        if (box.Address != CarriedAddress)
        {
            if (Time.time < immuneUntil) return;
            WrongDeliveries++;
            Hearts--;
            immuneUntil = Time.time + 1;
            audioFeedback.PlayHazard();
            Hint = $"Не тот адрес: письмо №{CarriedAddress} не для ящика №{box.Address}. −1 попытка";
            if (Hearts <= 0) Finish(false, "Попытки закончились");
            return;
        }
        Delivered++;
        box.MarkDelivered();
        CarriedAddress = 0;
        world.ShowCarried(0);
        audioFeedback.PlayCollect();
        Hint = Delivered == Levels.Data.target
            ? "Все письма доставлены. Вернитесь на почту (зелёный домик)"
            : $"Доставлено {Delivered} из {Levels.Data.target}. Поднимите следующее письмо";
    }
    public void HitHazard()
    {
        if (!IsPlaying || Time.time < immuneUntil) return;
        Hearts--;
        immuneUntil = Time.time + 1;
        audioFeedback.PlayHazard();
        world.Player.ReturnToStart();
        Hint = "Собака напугала курьера: −1 попытка. Письмо осталось в сумке";
        if (Hearts <= 0) Finish(false, "Попытки закончились");
    }
    public void TryFinishLevel()
    {
        if (!IsPlaying) return;
        if (Delivered < Levels.Data.target)
        {
            Hint = $"Нужно доставить ещё {Levels.Data.target - Delivered} писем";
            return;
        }
        int score = Mathf.Max(1, 1000 + Hearts * 100 - Mathf.RoundToInt(elapsed * 5));
        SaveService.Complete(Levels.Current, score);
        audioFeedback.PlaySuccess();
        Finish(true, $"Очки: {score}. Рекорд: {SaveService.Best(Levels.Current)}");
    }
    private void Finish(bool won, string message)
    {
        CurrentState = won ? State.Won : State.Lost;
        Hint = message;
        MobileDirection = Vector2.zero;
        Time.timeScale = 0;
    }
    public void Pause()
    {
        if (!IsPlaying) return;
        CurrentState = State.Paused;
        MobileDirection = Vector2.zero;
        Time.timeScale = 0;
        PlayerPrefs.Save();
    }
    public void TogglePause()
    {
        if (IsPlaying) Pause();
        else if (CurrentState == State.Paused)
        {
            CurrentState = State.Playing;
            Time.timeScale = 1;
        }
    }
    public void ToMenu()
    {
        CurrentState = State.Menu;
        MobileDirection = Vector2.zero;
        Time.timeScale = 1;
        world.Clear();
        PlayerPrefs.Save();
    }
    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
        Time.timeScale = 1;
    }
}
