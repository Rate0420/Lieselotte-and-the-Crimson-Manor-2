using UnityEngine;

public class PlayerGuard : MonoBehaviour
{
    public PlayerManager playerManager;
    public PlayerStatusManager playerStatusManager;

    public void Guard()
    {
        Debug.Log("がーど");
        // guardフラグをオンにする
        if (playerStatusManager.CurrentVitality < playerStatusManager.GuardVitalityConsumption) return;
        playerManager.GuardFlg = true;
        playerManager.ContinuousPlayAnim(playerManager.aNameGuard, 0.02f);
        playerStatusManager.GuardOnOff(true);
    }

    public void GuardRelease()
    {
        Debug.Log("りりーす");
        // guardフラグをオフにする
        playerManager.GuardFlg = false;
        playerStatusManager.GuardOnOff(false);
    }
}
