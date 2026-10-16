using UnityEngine;

public class PlayerStatusManager : MonoBehaviour
{
    public HpBar hpBar;
    public UltGauge ultGauge;
    public StaminaGauge stamina;
    // ステータス管理
    // 近接コンボの数と各コンボの時間が必用かも

    public PlayerManager playerManager;
    public MoveManager moveManager;
    public PlayerGuard playerGuard;
    public PlayerMeleeCombo playerMeleeCombo;
    public Player_GB_Attack player_GB_Attack;
    public PlayerDamageReceiver playerDamageReceiver;

    public int CharacterNum; // キャラ選択時にこれをキャラセレシーンの方からセットする

    public character character;
    public DamageData damage;

    // CharacterData部分
    public string CharacterName;
    public GameObject CharacterGameObject;
    public GameObject RangeAttackObject;
    public int MaxHp;
    public int MaxVitality;
    public float MoveSpeed;
    public float StepTime;
    public float StepDist;
    public float ComboResetTime;
    public float ShootTimer;
    public float ShootOfsTime;
    public float ShootDestroyTime;
    public float BulletSpeed;
    public float GB_AttackTime;
    public float GuardVitalityConsumption;
    public float AvoidanceVitalityConsumption;
    public float S1VitalityConsumption;
    public float S2VitalityConsumption;
    public float S3VitalityConsumption;
    public float[] comboMotionTimes;
    public float[] comboColOffset;
    public float GBColOffset;

    // damage部分
    public int MeleeAtk;
    public int RangeAtk;
    public int BreakAtk;
    public int UniqueAttack;
    public int S1Atk;
    public int S2Atk;
    public int S3Atk;

    public int U1Atk;
    public int U2Atk;
    public int U3Atk;

    public float MeleeAtkHitUltIncrease;
    public float RangeAtkHitUltIncrease;
    public float BreakAtkHitUltIncrease;

    public float HitByU1Increase;
    public float HitByU2Increase;
    public float HitByU3Increase;
    public float HitByS1Increase;
    public float HitByS2Increase;
    public float HitByS3Increase;
    public float HitByMeleeAtkIncrease;
    public float HitByRangeAtkIncrease;
    public float HitByBreakAtkIncrease;

    public int U1Consumption;
    public int U2Consumption;
    public int U3Consumption;

    public int GuardUltIncrease;
    public int GuardVitalityIncrease;

    [Header("現在の状態管理")]
    public int CurrentHp;
    public float CurrentVitality;
    public float CurrentUltGauge;
    // 現在の攻撃情報
    public string nowAtkState;

    // 判定用オブジェクト
    public GameObject MeleeBox;
    public GameObject GB_Box;
    public GameObject Guard_Box;

    // 攻撃速度
    public float attackSpeed = 1f;

    // 致命攻撃威力
    public float fatalAtkRate = 1f;

    // コンボ許容時間
    public float comboTime = 1f;

    // パリィ許容時間
    public float parryTime = 0.3f;

    // 初撃ダメージ補正
    public float firstAtkDmgRate = 1f;

    public float parryStamina; 

    public WeaponInstance weaponInstance;   // 武器のインスタンス情報(後々static状態の現在装備中のインスタンスをここに入れるor読み込む)

    bool diedFlg;

    [SerializeField] GameObject[] WeaponObjects;

    // スコア算出関連
    public int maxCombo = 0;
    public int parryCount = 0;
    public float clearTime = 0;

    void Awake()
    {
        SetStatusManager();
        //SetStatus();
    }

    private void Start()
    {
        weaponInstance = WeaponInventory.Instance.GetWeapon(WeaponInventory.Instance.equipmentIndex);
        if(weaponInstance == null) weaponInstance = WeaponInventory.Instance.GetWeapon(0);

        // 付呪などの装備情報をここで読み込む
        MaxHp = weaponInstance.HP;
        hpBar.maxHp = MaxHp;
        hpBar.nowHp = MaxHp;
        MeleeAtk = weaponInstance.attackPower;
        stamina.recoverySpeed = weaponInstance.staminaRecoveryRate;
        attackSpeed = weaponInstance.attackSpeed;
        MoveSpeed = weaponInstance.moveSpeed;
        fatalAtkRate = weaponInstance.criticalDamage;
        comboTime = weaponInstance.comboBufferTime;
        parryTime = weaponInstance.parryBufferTime;
        GuardVitalityConsumption = weaponInstance.guardStaminaCost;
        if(GuardVitalityConsumption < 0) GuardVitalityConsumption = 0;
        firstAtkDmgRate = weaponInstance.firstAttackBoost;

        // 付呪で変わらないステータス読み込み
        MaxVitality = weaponInstance.weaponTemplate.MAX_Stamina;
        UniqueAttack = Mathf.FloorToInt(MeleeAtk * weaponInstance.weaponTemplate.weaponArtsDamage);
        UniqueAttack += (int)weaponInstance.UniqueAttackBoost;
        parryStamina = weaponInstance.weaponTemplate.parryFailStaminaCost;

        // 
        comboMotionTimes = weaponInstance.weaponTemplate.CharacterMotionSet.AttackMotionTimes;
        comboColOffset = weaponInstance.weaponTemplate.CharacterMotionSet.AttackColliderTiming;
        MeleeBox.transform.localScale = weaponInstance.weaponTemplate.CharacterMotionSet.AttackColliderSize;
        MeleeBox.transform.position += weaponInstance.weaponTemplate.CharacterMotionSet.AttackColliderPosition;

        // プレイヤーのanimにtemplateのanimatorControllerを入れる
        playerManager.anim.runtimeAnimatorController = weaponInstance.weaponTemplate.CharacterMotionSet.AnimatorController;
        // 未完成部分もあるので注意(モーションなど)

        // 攻撃速度
        moveManager.moveSpeed = MoveSpeed;
        playerMeleeCombo.attackSpeed = attackSpeed;
        GameObject.Find("ComboCanvas").GetComponent<ComboManager>().comboDuration = comboTime;
        playerManager.playerParry.parryGrace = parryTime;
        playerDamageReceiver.JustGuardTime = parryTime;


        CurrentHp = MaxHp;

        // 武器の見た目を変える
        for (int i = 0; i < WeaponObjects.Length; i++)
        {
            if (i == (int)weaponInstance.WeaponType)
            {
                WeaponObjects[i].SetActive(true);
            }
            else
            {
                WeaponObjects[i].SetActive(false);
            }
        }

        playerManager.uniqueAttack = weaponInstance.weaponTemplate.UniqueAttack;

        SetStatus();
    }

    private void Update()
    {
        CurrentHp = hpBar.nowHp;
        CurrentVitality = stamina.currentStamina;
        clearTime += Time.deltaTime;
    }

    /// <summary>
    ///  StatusManager上の変数にScriptableから受け取った値を入れる
    /// </summary>
    public void SetStatusManager()
    {
        var characterData = character.CharacterData[CharacterNum];
        var damageData = damage.Damage[CharacterNum];
        CharacterName = characterData.Name;
        CharacterGameObject = characterData.Character;
        RangeAttackObject = characterData.RangeAttackObject;
        MaxHp = characterData.MaxHp;
        MaxVitality = characterData.MaxVitality;
        GuardVitalityConsumption = characterData.GuardVitalityConsumption;
        AvoidanceVitalityConsumption = characterData.AvoidanceVitalityConsumption;
        S1VitalityConsumption = characterData.S1VitalityConsumption;
        S2VitalityConsumption = characterData.S2VitalityConsumption;
        S3VitalityConsumption = characterData.S3VitalityConsumption;
        // 例として移動・回避も
        MoveSpeed = characterData.MoveSpeed;
        StepTime = characterData.StepTime;
        StepDist = characterData.StepDis;

        // コンボなども
        ComboResetTime = characterData.ComboResetTime;
        ShootTimer = characterData.ShootTimer;
        ShootOfsTime = characterData.ShootOfsTime;
        ShootDestroyTime = characterData.ShootDestroyTime;
        BulletSpeed = characterData.BulletSpeed;
        GB_AttackTime = characterData.GB_AttackTime;
        //comboMotionTimes = characterData.ComboMotionTimes;
        //comboColOffset = characterData.ComboColOffset;
        GBColOffset = characterData.GBColOffset;

        // ダメージ系
        MeleeAtk = damageData.MeleeAtk;
        RangeAtk = damageData.RangeAtk;
        BreakAtk = damageData.BreakAtk;
        S1Atk = damageData.S1Atk;
        S2Atk = damageData.S2Atk;
        S3Atk = damageData.S3Atk;
        U1Atk = damageData.U1Atk;
        U2Atk = damageData.U2Atk;
        U3Atk = damageData.U3Atk;

        U1Consumption = damageData.U1Consumption;
        U2Consumption = damageData.U2Consumption;
        U3Consumption = damageData.U3Consumption;

        // DamageDataも同様
        if (damageData != null)
        {
            // 攻撃を当てたときのゲージ増加関連
            MeleeAtkHitUltIncrease = damageData.MeleeAtkHitUltIncrease;
            RangeAtkHitUltIncrease = damageData.RangeAtkHitUltIncrease;
            BreakAtkHitUltIncrease = damageData.BreakAtkHitUltIncrease;
            // 被弾時のゲージ増加関連
            HitByU1Increase = damageData.HitByU1Increase;
            HitByU2Increase = damageData.HitByU2Increase;
            HitByU3Increase = damageData.HitByU3Increase;
            HitByS1Increase = damageData.HitByS1Increase;
            HitByS2Increase = damageData.HitByU2Increase;
            HitByS3Increase = damageData.HitByU3Increase;

            HitByMeleeAtkIncrease = damageData.HitByMeleeAtkIncrease;
            HitByRangeAtkIncrease = damageData.HitByRangeAtkIncrease;
            HitByBreakAtkIncrease = damageData.HitByBreakAtkIncrease;

            // ジャスガ関連
            GuardUltIncrease = damageData.GuardUltIncrease;
            GuardVitalityIncrease = damageData.GuardVitalityIncrease;
        }

        // 各種ステータスの初期化
        CurrentHp = MaxHp;
        CurrentVitality = MaxVitality;
        CurrentUltGauge = 0;
    }

    /// <summary>
    /// 各スクリプトに変数をセットする
    /// </summary>
    public void SetStatus()
    {
        moveManager.moveSpeed = MoveSpeed;
        playerMeleeCombo.comboResetTime = ComboResetTime;
        playerMeleeCombo.animTimes = comboMotionTimes;
        playerMeleeCombo.comboColOffset = comboColOffset;
        playerMeleeCombo.comboHitData = weaponInstance.weaponTemplate.comboHitData;
        player_GB_Attack.GB_AttackTime = GB_AttackTime;
        player_GB_Attack.GBColOffset = GBColOffset;

        // PlayerManagerのComboAnimsの設定
        // ComboMotionTimeと同じ長さにする
        // 例：ComboMotionTime[0] → Combo1、ComboMotionTime[1] → Combo2、ComboMotionTime[2] → Combo3
        playerManager.comboAnims = new string[weaponInstance.weaponTemplate.comboHitData.Length];
        for (int i = 0; i < weaponInstance.weaponTemplate.comboHitData.Length; i++)
        {
            playerManager.comboAnims[i] = "Combo" + (i + 1);
        }

        // ここまできたらステータスの設定系が完了したってことだからここでGM的なのを呼んでゲーム開始した方がいいかも
    }

    public void MeleeAtkOnOff(bool f)
    {
        MeleeBox.SetActive(f);
    }

    public void UA_OnOff(bool f)
    {
        MeleeBox.SetActive(f);
    }

    public void GB_AtkOnOff(bool f)
    {
        GB_Box.SetActive(f);
    }

    public void GuardOnOff(bool f)
    {
        Guard_Box.SetActive(f);
    }

    public (int, string) GetAtkDmg()
    {
        int dmg = 0;
        switch (nowAtkState)
        {
            case "MeleeCombo":
            case "MeleeComboFinish":
                dmg = MeleeAtk;
                break;
            case "RangeAttack":
                dmg = RangeAtk;
                break;
            case "GB_Attack":
                dmg = BreakAtk;
                break;
        }
        return (dmg, nowAtkState);
    }

    // ここから各スクリプトから呼び出し、ステータス管理と同時に呼び出す関数
    public void HPRend(int num)
    {
        // バトルUIを管理するスクリプトに引数(ダメージ量or回復量)を渡す。
        // 引数をHPに対して計算する
        hpBar.ApplyDamage(num);
        CurrentHp -= num;
        if (0 <= CurrentHp) return;
    }

    public void StaminaRend(float num)
    {
        // バトルUIを管理するスクリプトに引数(スタミナ消費量or回復量)を渡す。
        stamina.ConsumeStamina(num);
        // 引数をStaminaに対して計算する
    }

    public void UltRend(float num, bool fluctuation)
    {
        // バトルUIを管理するスクリプトに引数(Ultゲージの減少or回復)を渡す。
        ultGauge.Ult(num, fluctuation);
        // 引数をUltに対して計算する
    }

    public void ScoreEnter()
    { 
        GameSceneData.instance.gsdHPDeta = CurrentHp;
        GameSceneData.instance.gsdComboDeta = maxCombo;
        GameSceneData.instance.gsdParryDeta = parryCount;
        GameSceneData.instance.gsdTimeDeta = clearTime;
    }
}
