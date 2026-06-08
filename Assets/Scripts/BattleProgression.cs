using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleProgression : MonoBehaviour
{
    public List<EnemyData> enemiesInZone;
    private int currentIndex = 0;
    private bool isTransitioning = false;

    public Enemy enemyComponent;
    public BattleManager battleManager;

    void Start()
    {
        if (enemyComponent == null)
            enemyComponent = FindObjectOfType<Enemy>();
        if (battleManager == null)
            battleManager = FindObjectOfType<BattleManager>();

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