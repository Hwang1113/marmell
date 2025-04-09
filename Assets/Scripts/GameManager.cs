using UnityEngine;
using System.Collections;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public ScoreTxt scoreTxt; // 점수를 표시할 텍스트
    public ComboTxt comboTxt; // 콤보를 표시할 텍스트
    public MidTxt midTxt; // 가운데 표시할 텍스트
    public BotController[] botControllers; // BotController 배열
    public PlayerController playerController;
    public LifeCntManager lifeCntManager;
    private int score = 0; // 점수 변수

    void Start()
    {
        // 점수 초기화
        scoreTxt = GetComponentInChildren<ScoreTxt>();

        if (scoreTxt != null)
        {
            scoreTxt.UpdateScore(score);
        }
        // 점수 초기화
        comboTxt = GetComponentInChildren<ComboTxt>();
        midTxt = GetComponentInChildren<MidTxt>();
        lifeCntManager = GetComponentInChildren<LifeCntManager>();
        playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        playerController.onGameOver += OnGameOver;
        playerController.onHit += OnHit;

    }

    void Update()
    {
        // 런타임 중 BotController가 동적으로 추가된 경우를 감지하여 처리
        HandleDynamicBotControllers();
    }

    // 런타임 중 동적으로 추가된 BotController를 처리
    void HandleDynamicBotControllers()
    {
        // 현재 존재하는 BotController들 가져오기
        BotController[] currentBotControllers = FindObjectsByType<BotController>(FindObjectsSortMode.None);

        // 새로운 BotController가 추가된 경우 처리
        foreach (var botController in currentBotControllers)
        {
            if (!botControllers.Contains(botController))
            {
                // 새로운 BotController가 배열에 없다면 추가하고 콜백 설정
                AddBotController(botController);
            }
        }

        // 기존에 더 이상 존재하지 않는 BotController가 있으면 처리
        foreach (var botController in botControllers)
        {
            if (!currentBotControllers.Contains(botController))
            {
                // 삭제된 BotController에 대해서 콜백 해제
                RemoveBotController(botController);
            }
        }

        // botControllers 배열을 갱신
        botControllers = currentBotControllers;
    }

    // 새로운 BotController 추가 시 콜백 설정
    void AddBotController(BotController botController)
    {
        if (botController != null)
        {
            botController.onDummyComplete += OnDummyComplete;
            botController.onCombo += OnCombo;
        }
    }

    // 삭제된 BotController에 대해서 콜백 해제
    void RemoveBotController(BotController botController)
    {
        if (botController != null)
        {
            botController.onDummyComplete -= OnDummyComplete;
            botController.onCombo -= OnCombo;
        }
    }

    // 콜백 함수: Dummy 상태 완료 시 점수 증가
    private void OnDummyComplete()
    {
        // 점수 증가
        ChangeScore(10); // 예시로 10점 증가
    }

    private void OnCombo()
    {
        Combo(); // 콤보 1 증가
    }

    // 점수를 변경하는 메서드
    public void ChangeScore(int amount)
    {
        score += amount;  // 점수 증가 또는 감소
        scoreTxt.UpdateScore(score); // 점수 텍스트를 갱신
    }

    public void Combo()
    {
        comboTxt.UpdateCombo();
    }

    public void OnGameOver()
    {
        string GameOver = "Game Over";
        midTxt.UpdateText(GameOver);
        midTxt.ShowText();
        Time.timeScale = 0.1f; // 게임 속도를 50%로 느리게

    }
    public void OnHit()
    {
        lifeCntManager.RemoveOneLife(playerController.hitCnt);
    }
}
