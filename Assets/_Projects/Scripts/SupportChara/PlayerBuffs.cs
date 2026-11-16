using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// サポートなどによるバフの管理。
/// ・時間付きバフ(攻撃力・攻撃速度・コンボ受付時間・体勢削り量)。同じ種類は倍率が掛け合わさる
/// ・使い切りバフ(次の攻撃の攻撃力)
/// 攻撃力は攻撃を出すときに(PlayerStatusManager.BeginAttack)、体勢削り量は弾き/パリィ成功時に参照される。
/// 攻撃速度とコンボ受付時間は、バフの増減のたびに実際の値へ反映する。
/// </summary>
public class PlayerBuffs : MonoBehaviour
{
    public enum BuffType
    {
        Attack,         // 攻撃力
        AttackSpeed,    // 攻撃速度
        ComboTime,      // コンボ受付時間
        PostureDamage   // 弾き・パリィでの体勢削り量
    }

    class ActiveBuff
    {
        public BuffType type;
        public float multiplier;
        public float remaining;
    }

    public PlayerManager playerManager;
    public PlayerStatusManager playerStatusManager;

    readonly List<ActiveBuff> activeBuffs = new List<ActiveBuff>();
    float nextAttackRate = 1f;

    /// <summary>
    /// 次の攻撃に使い切りバフが残っているか(表示用)
    /// </summary>
    public bool HasNextAttackBuff
    {
        get { return nextAttackRate > 1f; }
    }

    /// <summary>
    /// 今かかっている時間付きバフの倍率(なければ1)
    /// </summary>
    public float GetRate(BuffType type)
    {
        float rate = 1f;
        foreach (var b in activeBuffs)
        {
            if (b.type == type) rate *= b.multiplier;
        }
        return rate;
    }

    public void AddTimedBuff(BuffType type, float multiplier, float duration)
    {
        activeBuffs.Add(new ActiveBuff { type = type, multiplier = multiplier, remaining = duration });
        ApplyStats(type);
    }

    public void AddNextAttackBuff(float multiplier)
    {
        nextAttackRate *= multiplier;
    }

    /// <summary>
    /// 攻撃を出すときに呼ぶ。使い切りバフの倍率を返して消費する(なければ1)
    /// </summary>
    public float ConsumeNextAttackRate()
    {
        float rate = nextAttackRate;
        nextAttackRate = 1f;
        return rate;
    }

    void Update()
    {
        if (activeBuffs.Count == 0) return;

        bool speedChanged = false;
        bool comboChanged = false;
        for (int i = activeBuffs.Count - 1; i >= 0; i--)
        {
            activeBuffs[i].remaining -= Time.deltaTime;
            if (activeBuffs[i].remaining > 0f) continue;

            if (activeBuffs[i].type == BuffType.AttackSpeed) speedChanged = true;
            if (activeBuffs[i].type == BuffType.ComboTime) comboChanged = true;
            activeBuffs.RemoveAt(i);
        }
        if (speedChanged) ApplyStats(BuffType.AttackSpeed);
        if (comboChanged) ApplyStats(BuffType.ComboTime);
    }

    // 実際の値に反映する(元の値はPlayerStatusManagerが武器から読んだもの)
    void ApplyStats(BuffType type)
    {
        switch (type)
        {
            case BuffType.AttackSpeed:
                playerManager.playerMeleeCombo.attackSpeed = playerStatusManager.attackSpeed * GetRate(BuffType.AttackSpeed);
                // 攻撃中ならモーションの速度にもすぐ反映する
                if (playerManager.ComboFlg) playerManager.anim.speed = playerManager.playerMeleeCombo.attackSpeed;
                break;
            case BuffType.ComboTime:
                playerManager.comboManager.comboDuration = playerStatusManager.comboTime * GetRate(BuffType.ComboTime);
                break;
        }
    }
}
