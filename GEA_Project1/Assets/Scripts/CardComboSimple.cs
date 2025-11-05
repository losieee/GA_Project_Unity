using UnityEngine;

public class CardComboSimple : MonoBehaviour
{
    void Start()
    {
        int maxCost = 15;

        // 카드 정보
        int quickDamage = 6;
        int quickCost = 2;
        int quickCount = 2;

        int heavyDamage = 8;
        int heavyCost = 3;
        int heavyCount = 2;

        int multiDamage = 16;
        int multiCost = 5;
        int multiCount = 1;

        int tripleDamage = 24;
        int tripleCost = 7;
        int tripleCount = 1;

        int bestDamage = 0;
        string bestCombo = "";

        // 4중 for문으로 모든 조합 탐색
        for (int q = 0; q <= quickCount; q++) // 퀵샷
        {
            for (int h = 0; h <= heavyCount; h++) // 헤비샷
            {
                for (int m = 0; m <= multiCount; m++) // 멀티샷
                {
                    for (int t = 0; t <= tripleCount; t++) // 트리플샷
                    {
                        int totalCost = q * quickCost + h * heavyCost + m * multiCost + t * tripleCost;
                        int totalDamage = q * quickDamage + h * heavyDamage + m * multiDamage + t * tripleDamage;

                        // 코스트 초과는 제외
                        if (totalCost <= maxCost)
                        {
                            if (totalDamage > bestDamage)
                            {
                                bestDamage = totalDamage;
                                bestCombo = $"퀵샷 x{q}, 헤비샷 x{h}, 멀티샷 x{m}, 트리플샷 x{t}";
                            }
                        }
                    }
                }
            }
        }

        Debug.Log("=== 결과 ===");
        Debug.Log($"최대 데미지: {bestDamage}");
        Debug.Log($"최적 조합: {bestCombo}");
    }
}
