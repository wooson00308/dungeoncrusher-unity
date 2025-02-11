public static class CriticalOperator
{
    public static bool IsCritical(float criticalValue)
    {
        return Operator.IsRate(criticalValue);
    }

    public static int GetCriticalDamageIntValue(float damage, float criticalValue)
    {
        return (int)(damage * criticalValue);
    }

    public static float GetCriticalDamageFloatValue(float damage, float criticalValue)
    {
        return damage * criticalValue;
    }
}