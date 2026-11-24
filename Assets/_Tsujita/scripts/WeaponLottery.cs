using UnityEngine;

public class WeaponLottery : MonoBehaviour
{
    // 武器抽選(エンチャントが何枠付くか)
    public WeaponType GetRandomWeaponType()
    {
        int r = Random.Range(0, 3);
        return (WeaponType)r;
    }
}
