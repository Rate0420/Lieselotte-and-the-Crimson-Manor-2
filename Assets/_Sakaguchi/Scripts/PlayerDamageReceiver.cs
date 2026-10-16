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
        if (isDamagedThisAttack) return;
        Debug.Log("PlayerDamageReceiver: Not Damaged Yet");
        isDamagedThisAttack = true;

        if (other.gameObject.tag == "EnemyAtk")
        {
            Debug.Log("PlayerDamageReceiver: Hit by Enemy Attack");
            EnemyController enemyController = other.gameObject.GetComponentInParent<EnemyController>();
            //パリィ成功の条件
            if(playerParry.isParrying) Debug.Log("PlayerDamageReceiver: Player is Parrying");
            if(playerParry.parryState == enemyController.currentAttackDir) Debug.Log("PlayerDamageReceiver: Parry Direction Matches Attack Direction");
            if (playerParry.isParrying && playerParry.parryState == enemyController.currentAttackDir)
            {
                Debug.Log("Parry Success!");
                // ひとまずboxcolliderを無効化して攻撃を当てないようにする
                other.GetComponent<BoxCollider>().enabled = false;
                other.GetComponent<MeshRenderer>().enabled = false;
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
                    if (playerStatusManager.CurrentVitality > playerStatusManager.GuardVitalityConsumption)
                    {
                        if (tutorialManager != null)
                            if (tutorialManager.tutorialNum == 1)
                                tutorialManager.TutorialCount();
                        // ジャストガードの判定
                        if(justguardTimer <= JustGuardTime)
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
                        playerManager.GuardFlg = false;
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
    public void TakeDamage(int damage, Transform attacker, int damageDir,bool down = false)
    {
        // ダメージが0以下なら無効化
        if (damage <= 0)
        {
            
            return;
        }

        // SE再生
        // ダメージSEの再生
        int seIndex = Random.Range(0, 2);

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
        playerManager.OnDamage(damageDir,down, attacker);
        if (tutorialManager != null)
            if (!tutorialManager.isTutorialCompleted)
                return;
        playerStatusManager.HPRend(damage);
    }
}
