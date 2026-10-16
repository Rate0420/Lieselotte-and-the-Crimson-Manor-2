using UnityEngine;

public class SoundPush : MonoBehaviour
{
    /// <summary>
    /// 決定音
    /// </summary>
    public void DecisionPush()
    {
        SoundManager.Instance.PlaySE("DecisionSE");
    }

    /// <summary>
    /// キャンセル音
    /// </summary>
    public void CancelPush()
    {
        SoundManager.Instance.PlaySE("CancelSE");
    }
    
    /// <summary>
    /// カーソル音
    /// </summary>
    public void CursorPush()
    {
        SoundManager.Instance.PlaySE("CursorSE");
    }

    /// <summary>
    /// 武器選択音
    /// </summary>
    public void WeaponPush()
    {
        SoundManager.Instance.PlaySE("WeaponSE");
    }

    /// <summary>
    /// 付呪選択音
    /// </summary>
    public void ChoicePush() 
    {
        SoundManager.Instance.PlaySE("ChoiceSE");
    }

    /// <summary>
    /// 合成音
    /// </summary>
    public void EnchantPush() 
    {
        SoundManager.Instance.PlaySE("EnchantSE");
    }
}
