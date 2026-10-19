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

    public int MaxHp;
    public int MaxVitality;
    public float MoveSpeed;

    public float ComboResetTime;

    public float GB_AttackTime;
    public float GuardVitalityConsumption;

    public float[] comboMotionTimes;
    public float[] comboColOffset;
    public float GBColOffset;

    // damage部分
    public int MeleeAtk;
    public int UniqueAttack;

    [Header("現在の状態管理")]
    public int CurrentHp;
    public float CurrentVitality;
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
        if (weaponInstance == null) weaponInstance = WeaponInventory.Instance.GetWeapon(0);

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
        if (GuardVitalityConsumption < 0) GuardVitalityConsumption = 0;
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
        playerManager.comboManager.comboDuration = comboTime;
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

        MaxHp = characterData.MaxHp;
        MaxVitality = characterData.MaxVitality;
        GuardVitalityConsumption = characterData.GuardVitalityConsumption;

        // 例として移動・回避も
        MoveSpeed = characterData.MoveSpeed;

        // コンボなども
        ComboResetTime = characterData.ComboResetTime;

        GB_AttackTime = characterData.GB_AttackTime;
        //comboMotionTimes = characterData.ComboMotionTimes;
        //comboColOffset = characterData.ComboColOffset;
        GBColOffset = characterData.GBColOffset;

        // ダメージ系
        MeleeAtk = damageData.MeleeAtk;

        // 各種ステータスの初期化
        CurrentHp = MaxHp;
        CurrentVitality = MaxVitality;
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

    public void ScoreEnter()
    {
        GameSceneData.instance.gsdHPDeta = CurrentHp;
        GameSceneData.instance.gsdComboDeta = maxCombo;
        GameSceneData.instance.gsdParryDeta = parryCount;
        GameSceneData.instance.gsdTimeDeta = clearTime;
    }
}
