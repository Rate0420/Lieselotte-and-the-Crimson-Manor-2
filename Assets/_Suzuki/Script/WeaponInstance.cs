using System;
using UnityEngine;

/// <summary>
/// 武器の実体データ。JSONファイル化が出来るようにシリアライズ化する想定。
/// 付呪の付けられる数は3つ固定(nullも許容とする)
/// </summary>
[Serializable]
public class WeaponInstance
{
    public string templateName;    // 参照先であるテンプレート名の格納先

    // ステータスに影響をもたらす付呪
    // 最大3つまで
    public EnchantmentSlot[] enchantmentSlots;

    // 付呪適用後のステータス
    public int HP;
    public int attackPower;
    public float staminaRecoveryRate;
    public float attackSpeed;
    public float moveSpeed;
    public float criticalDamage;
    public float comboBufferTime;
    public float parryBufferTime;
    public float guardStaminaCost;
    public float firstAttackBoost;
    public float UniqueAttackBoost;

    // 非シリアライズの参照テンプレート
    [NonSerialized] public WeaponTemplate weaponTemplate;

    public Enchantment enchantmentDB;

    // 以下、UI表示に参照する用
    public WeaponType WeaponType;  // 武器種名

    public string DisplayName // 武器名
    {
        get
        {
            return weaponTemplate != null
                ? weaponTemplate.displayName
                : "謎の武器"; // nullの場合は仮の名前で返す
        }
    }

    public Sprite Icon // 武器のアイコン
    {
        get
        {
            return weaponTemplate != null ? weaponTemplate.weaponIcon : null;
        }
    }

    public int EnchantmentCount // 付呪がいくつ付いているか
    {
        get
        {
            int enchantCount = 0;
            if (enchantmentSlots == null) return 0;

            foreach (var s in enchantmentSlots)
                if (s != null) enchantCount++;

            return enchantCount;
        }
    }


    /// <summary>
    /// テンプレートを基にインスタンスを作る
    /// </summary>
    /// <param name="template"></param>
    /// <param name="enchantment"></param>
    public WeaponInstance(WeaponTemplate template,Enchantment enchantment)
    {
        weaponTemplate = template;
        enchantmentDB = enchantment;
        templateName = template != null ? template.weaponName : "unknown";
        enchantmentSlots = new EnchantmentSlot[0];

        // 武器ステータスの初期値をコピー
        ResetToBaseStats();
    }

    /// <summary>
    /// 既存データからテンプレートのデータにを差し替える
    /// </summary>
    /// <param name="t"></param>
    public void SetTemplate(WeaponTemplate t)
    {
        weaponTemplate = t;
        templateName = t != null ? t.weaponName : null;
        ResetToBaseStats();
    }

    /// <summary>
    /// 武器テンプレートの基礎値をコピーする
    /// </summary>
    public void ResetToBaseStats()
    {
        if (weaponTemplate == null) return;
        HP = weaponTemplate.baseHP;
        attackPower = weaponTemplate.basePower;
        staminaRecoveryRate = weaponTemplate.baseStaminaRecoverySpeed;
        attackSpeed = weaponTemplate.baseAttackSpeed;
        moveSpeed = weaponTemplate.baseMoveSpeed;
        criticalDamage = weaponTemplate.baseFatalAttackDamage;
        comboBufferTime = weaponTemplate.baseComboGraceTime;
        parryBufferTime = weaponTemplate.baseParryReceptionTime;
        guardStaminaCost = weaponTemplate.baseGuradStaminaLoss;
        firstAttackBoost = 1;
        WeaponType = weaponTemplate.weaponType;
    }


    /// <summary>
    /// スロットの数分、付呪の内容を参照して
    /// 内容にあった強化を施す関数
    /// </summary>
    public void EnhanceStats()
    {
        ResetToBaseStats();

        foreach (var slot in enchantmentSlots)
        {
            if (slot == null) continue;

            var value = slot.GetEfficacy(enchantmentDB);

            switch (slot.enchantName)
            {
                case EnchantmentDataBase.EnchantmentName.HP増加:
                    HP += (int)value;
                    break;

                case EnchantmentDataBase.EnchantmentName.攻撃力増加:
                    attackPower += (int)value;
                    break;

                case EnchantmentDataBase.EnchantmentName.スタミナ回復上昇:
                    staminaRecoveryRate += value;
                    break;

                case EnchantmentDataBase.EnchantmentName.移動速度上昇:
                    moveSpeed += value;
                    break;

                case EnchantmentDataBase.EnchantmentName.致命強化:
                    criticalDamage += value;
                    break;

                case EnchantmentDataBase.EnchantmentName.コンボ時間増加:
                    comboBufferTime += value;
                    break;

                case EnchantmentDataBase.EnchantmentName.パリィ猶予増加:
                    parryBufferTime += value;
                    break;

                case EnchantmentDataBase.EnchantmentName.ガード強化:
                    guardStaminaCost += value;
                    break;

                case EnchantmentDataBase.EnchantmentName.初撃強化:
                    firstAttackBoost += value; break;

                case EnchantmentDataBase.EnchantmentName.攻撃速度上昇:
                    attackSpeed += value;
                    break;

                case EnchantmentDataBase.EnchantmentName.戦技強化:
                    UniqueAttackBoost += value;
                    break; 
            }
        }

    }

    /// <summary>
    /// 付呪スロットを付いている数分セットする関数
    /// </summary>
    /// <param name="newSlots"></param>
    public void SetEnchantments(EnchantmentSlot[] newSlots)
    {
        enchantmentSlots = new EnchantmentSlot[newSlots.Length];
        for (int i = 0; i < 3 && i < newSlots.Length; i++)
        {
            enchantmentSlots[i] = newSlots[i];
        }
        EnhanceStats();
    }
}


/// <summary>
/// 武器に付いた付呪1枠分を示すクラス
/// </summary>
[Serializable]
public class EnchantmentSlot
{
    public EnchantmentDataBase.EnchantmentName enchantName;


    [Range(1, 11)] public int enchantLevel; // 0 = Lv1

    public EnchantmentSlot(EnchantmentDataBase.EnchantmentName name, int level )
    {
        enchantName = name;
        enchantLevel = Mathf.Clamp(level, 1, 11);
    }

    /// <summary>
    /// 付呪の名前を参照して上昇値を取得する
    /// </summary>
    /// <param name="enchantmentDB"></param>
    /// <returns></returns>
    public float GetEfficacy(Enchantment enchantmentDB)
    {
        if (enchantmentDB == null)
        {
            Debug.LogError("EnchantmentDB が null");
            return 0;
        }

        if (enchantmentDB.EnchantmentData == null)
        {
            Debug.LogError("EnchantmentData が null");
            return 0;
        }

        // 付呪データを名前で検索
        var data = enchantmentDB.EnchantmentData
            .Find(d => d.Name == enchantName);

        if (data == null || data.Efficacy == null)
            return 0;

        if (enchantLevel >= data.Efficacy.Length)
            return 0;

        //return enchantLevel;
        //こうか？
        return data.Efficacy[enchantLevel];
    }
}
