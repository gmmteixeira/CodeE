using System;

public static class WeaponEvents
{
    public static event Action OnWeaponFired;

    public static void WeaponFired()
    {
        OnWeaponFired?.Invoke();
    }
}