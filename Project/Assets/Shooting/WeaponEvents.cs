using System;

public static class WeaponEvents
{
    public static event Action OnWeaponFired;
    public static event Action OnWeaponAltFired;

    public static void WeaponFired()
    {
        OnWeaponFired?.Invoke();
    }
    public static void WeaponAltFired()
    {
        OnWeaponAltFired?.Invoke();
    }
}