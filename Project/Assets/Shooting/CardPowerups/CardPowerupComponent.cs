using Unity.Entities;

public struct CardPowerup : IComponentData
{
    public bool active;
    public float lifeTime;
    public float cooldownModifier;
    public float projectileCountModifier;
    public float spreadModifier;
    public float explosionModifier;
}