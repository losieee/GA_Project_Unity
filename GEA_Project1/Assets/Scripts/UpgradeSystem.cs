using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeSystem : MonoBehaviour
{
    [System.Serializable]
    public class Stone
    {
        public string name;
        public int exp;
        public int gold;

        public Stone(string name, int exp, int gold)
        {
            this.name = name;
            this.exp = exp;
            this.gold = gold;
        }
    }

    List<Stone> stones = new List<Stone>();

    public Text levelText;
    public Text needExpText;
    public Slider expSlider;
    public Text resultText;

    public int fromLevel = 1;
    public int toLevel = 2;

    int needExp;

    void Start()
    {
        stones.Add(new Stone("소", 3, 8));
        stones.Add(new Stone("중", 5, 12));
        stones.Add(new Stone("대", 12, 30));
        stones.Add(new Stone("특대", 20, 45));

        UpdateUI();
    }

    public void SetLevelUp()
    {
        fromLevel++;
        toLevel++;
        UpdateUI();
    }

    void UpdateUI()
    {
        levelText.text = $"+{fromLevel} -> +{toLevel}";
        needExp = 8 * (2 * fromLevel + 1);
        needExpText.text = $"필요 경험치 {needExp}/{needExp}";
        expSlider.maxValue = needExp;
        expSlider.value = needExp;
    }

    public void OnClick_BruteForce()
    {
        var result = BruteForce(needExp);
        PrintResult(result, "Brute Force");
    }

    public void OnClick_SmallWaste()
    {
        var result = BuyBySmallWaste(needExp);
        PrintResult(result, "경험치 낭비 최소");
    }

    public void OnClick_Efficiency()
    {
        var result = BuyByEfficiency(needExp);
        PrintResult(result, "골드 효율 최대");
    }

    public void OnClick_BigExp()
    {
        var result = BuyByBigExp(needExp);
        PrintResult(result, "exp 큰 것 우선");
    }

    Dictionary<string, int> BuyBySmallWaste(int needExp)
    {
        stones.Sort((a, b) => a.exp.CompareTo(b.exp));
        return GreedyBuy(needExp);
    }

    Dictionary<string, int> BuyByEfficiency(int needExp)
    {
        stones.Sort((a, b) =>
            ((float)b.exp / b.gold).CompareTo((float)a.exp / a.gold));
        return GreedyBuy(needExp);
    }

    Dictionary<string, int> BuyByBigExp(int needExp)
    {
        stones.Sort((a, b) => b.exp.CompareTo(a.exp));
        return GreedyBuy(needExp);
    }

    Dictionary<string, int> GreedyBuy(int needExp)
    {
        Dictionary<string, int> result = new Dictionary<string, int>();

        foreach (var s in stones)
        {
            int cnt = needExp / s.exp;
            result[s.name] = cnt;
            needExp -= cnt * s.exp;
        }

        return result;
    }

    Dictionary<string, int> BruteForce(int needExp)
    {
        int minGold = int.MaxValue;
        Dictionary<string, int> answer = null;

        for (int a = 0; a <= 30; a++)
        {
            for (int b = 0; b <= 30; b++)
            {
                for (int c = 0; c <= 30; c++)
                {
                    for (int d = 0; d <= 30; d++)
                    {
                        int expSum = a * 3 + b * 5 + c * 12 + d * 20;
                        if (expSum < needExp) continue;

                        int goldSum = a * 8 + b * 12 + c * 30 + d * 45;

                        if (goldSum < minGold)
                        {
                            minGold = goldSum;
                            answer = new Dictionary<string, int>()
                            {
                                {"소", a},
                                {"중", b},
                                {"대", c},
                                {"특대", d}
                            };
                        }
                    }
                }
            }
        }

        return answer;
    }

    void PrintResult(Dictionary<string, int> dic, string title)
    {
        int totalGold = 0;
        string text = $"[{title} 결과]\n\n";

        foreach (var kv in dic)
        {
            Stone s = stones.Find(x => x.name == kv.Key);
            int gold = kv.Value * s.gold;
            totalGold += gold;
            text += $"{kv.Key} x {kv.Value} (exp {s.exp}) → {gold} gold\n";
        }

        text += $"\n총 가격 : {totalGold} gold";
        resultText.text = text;
    }
}
