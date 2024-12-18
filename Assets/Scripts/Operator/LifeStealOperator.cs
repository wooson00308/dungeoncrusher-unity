using UnityEngine;

public static class LifeStealOperator
{
    public static bool IsLifeSteal(float lifeStealRate)
    {
        return Operator.IsRate(lifeStealRate);
    }

    public static int LifeStealForHealth(float healthValue, float lifestealPercent)
    {
        return Mathf.RoundToInt(healthValue * lifestealPercent);
    }

    public static int LifeStealForDamage(float damageValue, float lifestealPercent)
    {
        return Mathf.RoundToInt(damageValue * lifestealPercent);
    }
}