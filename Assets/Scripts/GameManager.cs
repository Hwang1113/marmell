using UnityEngine;
using System.Collections;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public ScoreTxt scoreTxt; // ������ ǥ���� �ؽ�Ʈ
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

        // ���� ���� �ڷ�ƾ ����
        StartCoroutine(ScoreBoostCoroutine());
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
        BotController[] currentBotControllers = FindObjectsOfType<BotController>();

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
        }
    }

    // ������ BotController�� ���ؼ� �ݹ� ����
    void RemoveBotController(BotController botController)
    {
        if (botController != null)
        {
            botController.onDummyComplete -= OnDummyComplete;
        }
    }

    // �ݹ� �Լ�: Dummy ���� �Ϸ� �� ���� ����
    private void OnDummyComplete()
    {
        // ���� ����
        ChangeScore(10); // ���÷� 10�� ����
    }

    // ������ �����ϴ� �޼���
    public void ChangeScore(int amount)
    {
        score += amount;  // ���� ���� �Ǵ� ����
        scoreTxt.UpdateScore(score); // ���� �ؽ�Ʈ�� ����
    }

    // 10�ʸ��� ������ 10�� ������Ű�� �ڷ�ƾ
    private IEnumerator ScoreBoostCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(10f); // 10�� ���
            ChangeScore(10); // 10�ʸ��� 10�� ����
        }
    }
}
