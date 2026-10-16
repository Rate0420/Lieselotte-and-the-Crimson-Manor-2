using Unity.VisualScripting;
using UnityEngine;

public class PlayerDamageReceiver : MonoBehaviour
{
    [SerializeField] PlayerManager playerManager;
    [SerializeField] PlayerStatusManager playerStatusManager;
    [SerializeField] PlayerParry playerParry;
    [SerializeField] ComboManager comboManager;
    public float JustGuardTime = 0.3f; // ジャストガードの時間
    public float justguardTimer = 0f; // ジャストガードのタイマー

    // パリィの判定が当たった時に再生するエフェクト(ひとまずパブリックで指定)
    public GameObject parryEffect;
    // パリィエフェクトの生成位置用オブジェクト
    public Transform parryEffectPos;

    //ダメージを受けたときのエフェクト
    public GameObject DamageEffect;

    // 2倍ダメージ回避用フラグ
    public bool isDamagedThisAttack = false;

    // チュートリアル用オブジェクト
    [SerializeField] private TutorialManager tutorialManager;

    private void Update()
    {
        justguardTimer += Time.deltaTime;
    }


    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("PlayerDamageReceiver: OnTriggerEnter");

        if (other.gameObject.tag == "EnemyAtk")
        {
            Debug.Log("PlayerDamageReceiver: Hit by Enemy Attack");
            EnemyController enemyController = other.gameObject.GetComponentInParent<EnemyController>();
            if (enemyController == null) return;
            // 多段ヒット防止フラグは、敵の攻撃判定だと分かってから立てる(他のトリガーで立つと本物の攻撃を無視してしまうため)
            if (isDamagedThisAttack) return;
            isDamagedThisAttack = true;
            //パリィ成功の条件
            if (playerParry.isParrying) Debug.Log("PlayerDamageReceiver: Player is Parrying");
            if (playerParry.parryState == enemyController.currentAttackDir) Debug.Log("PlayerDamageReceiver: Parry Direction Matches Attack Direction");
            if (playerParry.isParrying && playerParry.parryState == enemyController.currentAttackDir)
            {
                Debug.Log("Parry Success!");
                // ひとまずboxcolliderを無効化して攻撃を当てないようにする
                var atkCol = other.GetComponent<BoxCollider>();
                if (atkCol != null) atkCol.enabled = false;
                var atkMesh = other.GetComponent<MeshRenderer>();
                if (atkMesh != null) atkMesh.enabled = false;
                enemyController.GetParry();
                comboManager.ComboContinues();
                // パリィエフェクトの生成
                Instantiate(parryEffect, parryEffectPos.position, Quaternion.identity);
                // PlayerManagerのParryFlgをfalseにする
                playerManager.ParryFlg = false;
            }
            else
            {
                if (playerManager.UAFlg)
                {
                    return;
                }
                if (playerManager.GuardFlg)
                {
                    // PlayerGuard.Guard()の判定(<で弾く)と揃えて、ちょうど足りている場合もガード成立にする
                    if (playerStatusManager.CurrentVitality >= playerStatusManager.GuardVitalityConsumption)
                    {
                        if (tutorialManager != null)
                            if (tutorialManager.tutorialNum == 1)
                                tutorialManager.TutorialCount();
                        // ジャストガードの判定
                        if (justguardTimer <= JustGuardTime)
                        {
                            Debug.Log("Just Guard!");
                            // ジャストガード成功のエフェクトを生成
                            Instantiate(parryEffect, parryEffectPos.position, Quaternion.identity);
                            // チュートリアル
                            if (tutorialManager != null)
                                if (tutorialManager.tutorialNum == 2)
                                    tutorialManager.TutorialCount();
                            comboManager.ComboContinues();
                            // パリィSE再生
                            int seIndex = Random.Range(0, 3);

                            switch (seIndex)
                            {
                                case 0:
                                    SoundManager.Instance.PlaySE("パリィ音高");
                                    break;
                                case 1:
                                    SoundManager.Instance.PlaySE("パリィ音中");
                                    break;
                                case 2:
                                    SoundManager.Instance.PlaySE("パリィ音低");
                                    break;
                            }

                            return;
                        }
                        playerStatusManager.StaminaRend(playerStatusManager.GuardVitalityConsumption);
                        Debug.Log(playerStatusManager.CurrentVitality);
                        return;
                    }
                    else
                    {
                        // スタミナ切れ。GuardFlgだけでなくGuard_Boxも一緒に解除する
                        playerManager.playerGuard.GuardRelease();
                        Debug.Log("Take Damage");
                        TakeDamage(enemyController.currentAttackDamage, other.transform, enemyController.currentAttackDir);
                    }
                }
                else
                {
                    Debug.Log("Take Damage");
                    TakeDamage(enemyController.currentAttackDamage, other.transform, enemyController.currentAttackDir);
                }
            }
        }
    }
    public void TakeDamage(int damage, Transform attacker, int damageDir, bool down = false)
    {
        // ダメージが0以下なら無効化
        if (damage <= 0)
        {

            return;
        }

        // 無敵中・ダウン中はダメージを受けない(PlayerManager.OnDamageと条件を揃える)
        if (playerManager.mutekiFlg || playerManager.DownFlg) return;

        // SE再生
        // ダメージSEの再生
        int seIndex = Random.Range(0, 3);

        switch (seIndex)
        {
            case 0:
                SoundManager.Instance.PlaySE("命中音血高");
                break;
            case 1:
                SoundManager.Instance.PlaySE("命中音血中");
                break;
            case 2:
                SoundManager.Instance.PlaySE("命中音血低");
                break;
        }

        // ダメージボイスの再生
        SoundManager.Instance.DamageVoice();

        // ダメージエフェクトの生成
        Instantiate(DamageEffect, transform.position + new Vector3(0, 1, 0), Quaternion.identity);

        // 攻撃中ならキャンセルし、ダメージ時のモーションを再生
        playerManager.OnDamage(damageDir, down, attacker);
        if (tutorialManager != null)
            if (!tutorialManager.isTutorialCompleted)
                return;
        playerStatusManager.HPRend(damage);
    }
}
