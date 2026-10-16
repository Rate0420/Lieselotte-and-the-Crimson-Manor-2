using UnityEngine;

public class WeaponLottery : MonoBehaviour
{
    // •Ší’Š‘I
    public WeaponType GetRandomWeaponType()
    {
        int r = Random.Range(0, 3);
        return (WeaponType)r;
    }
}
