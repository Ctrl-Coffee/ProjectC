public struct DamageResult
{
    public float Damage;
    public bool IsCritical;

    public DamageResult(float damage, bool isCritical)
    {
        Damage = damage;
        IsCritical = isCritical;
    }
}