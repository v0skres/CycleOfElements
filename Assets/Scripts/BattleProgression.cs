using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleProgression : MonoBehaviour
{
    public List<EnemyData> enemiesInZone;
    private int currentIndex = 0;
    private bool isTransitioning = false;
    private int zoneIndex; // индекс текущей зоны

    public Enemy enemyComponent;
    public BattleManager battleManager;

    void Start()
    {
        if (enemyComponent == null) enemyComponent = FindObjectOfType<Enemy>();
        if (battleManager == null) battleManager = FindObjectOfType<BattleManager>();

        // Получаем индекс зоны из GameManager
        if (GameManager.Instance != null)
            zoneIndex = GameManager.Instance.currentZoneIndex;
        else
            zoneIndex = 0; // запасной вариант

        LoadCurrentEnemy();
    }

    private string GetBossDialogue(int zone, bool isStart)
    {
        if (isStart)
        {
            switch (zone)
            {
                case 0: return "«Ты пришёл за моим осколком? Докажи свою силу.»";
                case 1: return "«Я не верю в твою миссию, но посмотрю, что ты можешь.»";
                case 2: return "«Хочешь изменить мир? Покажи, что твои решения крепки как камень.»";
                case 3: return "«Я оценила риски. Теперь проверю тебя лично.»";
                case 4: return "«Ты устал, но идёшь дальше. Давай, последний шаг.»";
                case 5: return "«Я Хранитель Баланса. Докажи, что достоин.»";
                default: return "";
            }
        }
        else
        {
            switch (zone)
            {
                case 0: return "«Забирай. Посмотрим, на что ты способен.»";
                case 1: return "«Возможно, я ошибалась. Действуй.»";
                case 2: return "«Ты всё ещё сомневаешься. Это хорошо. Осколок твой.»";
                case 3: return "«Решение принято. Осколок передан.»";
                case 4: return "«Теперь всё зависит от тебя.»";
                case 5: return "«Баланс — это выбор. Ты понял это. Мир изменится.»";
                default: return "";
            }
        }
    }

    void LoadCurrentEnemy()
    {
        if (currentIndex >= enemiesInZone.Count)
        {
            Debug.Log("Локация пройдена! Переход на карту мира.");
            return;
        }

        if (currentIndex == enemiesInZone.Count - 1 && DialogueManager.Instance != null) // босс
        {
            DialogueManager.Instance.ShowDialogue(GetBossDialogue(zoneIndex, true));
        }

        EnemyData data = enemiesInZone[currentIndex];
        Debug.Log($"=== Загрузка врага #{currentIndex}: {data.enemyName} ===");
        enemyComponent.InitializeFromData(data);

        var ui = FindObjectOfType<UIManager>();
        if (ui != null)
        {
            ui.UpdateEnemyName(data.enemyName);
            ui.ShowNextBattleButton(false);
        }

        isTransitioning = false;
    }

    public void ProceedToNextEnemy()
    {
        if (isTransitioning) return;
        isTransitioning = true;

        currentIndex++;

        if (currentIndex == enemiesInZone.Count && DialogueManager.Instance != null) // только что победили босса
        {
            DialogueManager.Instance.ShowDialogue(GetBossDialogue(zoneIndex, false));
        }

        if (currentIndex < enemiesInZone.Count)
        {
            Debug.Log($"Переход к следующему врагу: {enemiesInZone[currentIndex].enemyName}");
            StartCoroutine(DelayedLoadNextEnemy());
        }
        else
        {
            Debug.Log("=== ВСЕ ВРАГИ ПОБЕЖДЕНЫ! Локация завершена. ===");
            if (GameManager.Instance != null)
            {
                // Если это не последняя зона (не финальная) – разблокируем следующую
                if (zoneIndex < GameManager.Instance.GetZonesCount() - 1)
                    GameManager.Instance.UnlockNextZone(zoneIndex);
                else
                    Debug.Log("Поздравляем! Игра пройдена!"); // можно показать титры
            }
            GameManager.Instance.BackToWorldMap();
            isTransitioning = false;
        }
    }

    IEnumerator DelayedLoadNextEnemy()
    {
        yield return new WaitForSeconds(0.5f);
        // Сначала загружаем нового врага (обновляем currentEnemy)
        LoadCurrentEnemy();
        // Затем сбрасываем состояние боя (колода, поле, здоровье) и перезапускаем цикл
        battleManager.ResetBattleForNextEnemy();
        isTransitioning = false;
    }
}