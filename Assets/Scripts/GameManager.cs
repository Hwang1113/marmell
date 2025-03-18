using UnityEngine;
using System.Collections;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public ScoreTxt scoreTxt; // ������ ǥ���� �ؽ�Ʈ
    public ComboTxt comboTxt; // �޺��� ǥ���� �ؽ�Ʈ
    public BotController[] botControllers; // BotController �迭
    private int score = 0; // ���� ����

    void Start()
    {
        // ���� �ʱ�ȭ
        scoreTxt = GetComponentInChildren<ScoreTxt>();

        if (scoreTxt != null)
        {
            scoreTxt.UpdateScore(score);
        }
        // ���� �ʱ�ȭ
        comboTxt = GetComponentInChildren<ComboTxt>();

    }

    void Update()
    {
        // ��Ÿ�� �� BotController�� �������� �߰��� ��츦 �����Ͽ� ó��
        HandleDynamicBotControllers();
    }

    // ��Ÿ�� �� �������� �߰��� BotController�� ó��
    void HandleDynamicBotControllers()
    {
        // ���� �����ϴ� BotController�� ��������
        BotController[] currentBotControllers = FindObjectsByType<BotController>(FindObjectsSortMode.None);

        // ���ο� BotController�� �߰��� ��� ó��
        foreach (var botController in currentBotControllers)
        {
            if (!botControllers.Contains(botController))
            {
                // ���ο� BotController�� �迭�� ���ٸ� �߰��ϰ� �ݹ� ����
                AddBotController(botController);
            }
        }

        // ������ �� �̻� �������� �ʴ� BotController�� ������ ó��
        foreach (var botController in botControllers)
        {
            if (!currentBotControllers.Contains(botController))
            {
                // ������ BotController�� ���ؼ� �ݹ� ����
                RemoveBotController(botController);
            }
        }

        // botControllers �迭�� ����
        botControllers = currentBotControllers;
    }

    // ���ο� BotController �߰� �� �ݹ� ����
    void AddBotController(BotController botController)
    {
        if (botController != null)
        {
            botController.onDummyComplete += OnDummyComplete;
            botController.onCombo += OnCombo;
        }
    }

    // ������ BotController�� ���ؼ� �ݹ� ����
    void RemoveBotController(BotController botController)
    {
        if (botController != null)
        {
            botController.onDummyComplete -= OnDummyComplete;
            botController.onCombo -= OnCombo;
        }
    }

    // �ݹ� �Լ�: Dummy ���� �Ϸ� �� ���� ����
    private void OnDummyComplete()
    {
        // ���� ����
        ChangeScore(10); // ���÷� 10�� ����
    }

    private void OnCombo()
    {
        Combo(); // �޺� 1 ����
    }

    // ������ �����ϴ� �޼���
    public void ChangeScore(int amount)
    {
        score += amount;  // ���� ���� �Ǵ� ����
        scoreTxt.UpdateScore(score); // ���� �ؽ�Ʈ�� ����
    }

    public void Combo()
    {
        comboTxt.UpdateCombo();
    }
}
