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

    void LoadCurrentEnemy()
    {
        if (currentIndex >= enemiesInZone.Count)
        {
            Debug.Log("Локация пройдена! Переход на карту мира.");
            return;
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