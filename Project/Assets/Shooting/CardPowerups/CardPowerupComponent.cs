using Unity.Entities;

public struct CardPowerup : IComponentData
{
    public bool active;
    public float lifeTime;
    public float cooldownIncrement;
    public int projectileCountIncrement;
    public float spreadIncrement;
}