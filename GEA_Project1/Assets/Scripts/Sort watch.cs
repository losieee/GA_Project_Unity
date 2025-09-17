using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class Sortwatch : MonoBehaviour
{

    static Stopwatch sw = new Stopwatch();
    public Text Text;

    public void Start()
    {
        
       
    }

    // 선택 정렬
    int[] GenerateRandomArray(int size)
    {
        int[] arr = new int[size];
        System.Random rand = new System.Random();
        for (int i = 0; i < size; i++)
        {
            arr[i] = rand.Next(0, 10000);
        }
        return arr;
    }

    public void Onflick()
    {
        int[] data = GenerateRandomArray(10000);
        sw.Reset();
        sw.Start();
        StartSelectionSort(data);
        sw.Stop();
        long selectionTime = sw.ElapsedMilliseconds;

        Text.text = $"선택정렬 시간 : {selectionTime}ms";
    }
    public void OnSlick()
    {
        int[] data1 = GenerateRandomArray(10000);
        sw.Reset();
        sw.Start();
        StartBubbleSort(data1);
        sw.Stop();
        long BubbleTime = sw.ElapsedMilliseconds;

        Text.text = $"버블정렬 시간 : {BubbleTime}ms";
    }
    public void Onqlick()
    {
        int[] data2 = GenerateRandomArray(10000);
        sw.Reset();
        sw.Start();
        StartQuickSort(data2, 0, data2.Length - 1);
        sw.Stop();
        long QuickTime = sw.ElapsedMilliseconds;

        Text.text = $"퀵 정렬 시간 : {QuickTime}ms";
    }

    public static void StartSelectionSort(int[] arr)
    {
        int n = arr.Length;
        for (int i = 0; i < n - 1; i++)
        {
            int minIndex = i;
            for (int j = i + 1; j < n; j++)
            {
                {
                    if (arr[j] < arr[minIndex])
                    {
                        minIndex = j;
                    }
                }
                int temp = arr[minIndex];
                arr[minIndex] = arr[i];
                arr[i] = temp;

            }
        }
    }
    public static void StartBubbleSort(int[] arr)
    {
        int n = arr.Length;
        for (int i = 0; i < n - 1; i++)
        {
            bool swapped = false;
            for (int j = 0; j < n - 1; j++)
            {
                if (arr[j] > arr[j + 1])
                {
                    int temp = arr[j];
                    arr[j] = arr[j + 1];
                    arr[j + 1] = temp;
                    swapped = true;
                }
            }
            if (!swapped) break;
        }
    }
    public static void StartQuickSort(int[] arr, int low, int high)
    {
        if (low < high)
        {
            int pivotIndex = Partition(arr, low, high);

            StartQuickSort(arr, low, pivotIndex - 1);
            StartQuickSort(arr, pivotIndex + 1, high);
        }
    }

    private static int Partition(int[] arr, int low, int high)
    {
        int pivot = arr[high];
        int i = (low - 1);

        for (int j = low; j < high; j++)
        {
            if (arr[j] <= pivot)
            {
                i++;
                int temp = arr[i];
                arr[i] = arr[j];
                arr[j] = temp;
            }
        }

        int temp2 = arr[i + 1];
        arr[i + 1] = arr[high];
        arr[high] = temp2;

        return i + 1;
    }
}
