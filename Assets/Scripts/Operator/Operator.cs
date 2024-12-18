using UnityEngine;

public static class Operator
{
    public static bool IsRate(float rateValue)
    {
        int randomValue = Random.Range(0, 100);

        return randomValue <= rateValue; //ex) rateValue = 20  randomValue가 20이하로 나올 확률 
    }
}