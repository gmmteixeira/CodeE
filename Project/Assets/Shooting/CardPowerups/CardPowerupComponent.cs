using Unity.Entities;
using UnityEngine;

public struct CardPowerup : IComponentData
{
    public float cooldownIncrement;
    public int projectileCountIncrement;
    public float spreadIncrement;
}