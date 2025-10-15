using System;
using System.Collections.Generic;
using UnityEngine;

public class TurnBattle : MonoBehaviour
{
    class Unit
    {
        public string name;
        public float speed;
        public float coolTime;

        public Unit(string name, float speed)
        {
            this.name = name;
            this.speed = speed;
            this.coolTime = speed;
        }
    }

    SimplePriorityQueue<Unit> queue = new SimplePriorityQueue<Unit>();
    List<Unit> units = new List<Unit>();

    void Start()
    {
        units.Add(new Unit("전사", 5));
        units.Add(new Unit("마법사", 7));
        units.Add(new Unit("궁수", 10));
        units.Add(new Unit("도적", 12));

        foreach (var u in units)
        {
            queue.Enqueue(u, u.coolTime);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (queue.Count > 0)
            {
               
                var unit = queue.Dequeue();
                Debug.Log($"{unit.name} 의 턴입니다.");
 
                unit.coolTime += unit.speed;

                queue.Enqueue(unit, unit.coolTime);
            }
        }
    }
}
