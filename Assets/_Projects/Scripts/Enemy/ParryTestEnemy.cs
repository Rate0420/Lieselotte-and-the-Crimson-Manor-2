using System.Collections;
using UnityEngine;

public class ParryTestEnemy : MonoBehaviour
{
    //パリィテスト用の敵のスクリプト。
    //W,A,S,Dキーでそれぞれ上、左、下、右の属性(stringで管理)の攻撃を出す。

    [SerializeField] float attackInterval = 1f; //攻撃間隔
    [SerializeField] float attackTime = 1f; //攻撃時間
    [SerializeField] int attackDamage = 10; //攻撃力
    int currentAttackDir = -1; //現在の攻撃属性。-1は無属性。
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            Debug.Log("Enemy Attack Up");
            currentAttackDir = (int)PlayerParry.parryDir.上;
            StartCoroutine(AttackCoroutine(attackInterval, (PlayerParry.parryDir)currentAttackDir));
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("Enemy Attack Left");
            currentAttackDir = (int)PlayerParry.parryDir.左;
            StartCoroutine(AttackCoroutine(attackInterval, (PlayerParry.parryDir)currentAttackDir));
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("Enemy Attack Down");
            currentAttackDir = (int)PlayerParry.parryDir.下;
            StartCoroutine(AttackCoroutine(attackInterval, (PlayerParry.parryDir)currentAttackDir));
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            Debug.Log("Enemy Attack Right");
            currentAttackDir = (int)PlayerParry.parryDir.右;
            StartCoroutine(AttackCoroutine(attackInterval, (PlayerParry.parryDir)currentAttackDir));
        }
    }

    IEnumerator AttackCoroutine(float atkInterval,PlayerParry.parryDir dir)
    {
        //各キーを押すと呼び出される想定。
        //atkIntervalの間隔後、boxColliderを有効化し、相手オブジェクトがEnemyのTagを持っていたら
        //PlayerParryを呼び出し、ParryStateがdirと同じかつisParryingがtrueならパリィ成功の処理を行う。
        //どちらかの条件が満たされなかったらPlayerDamageRecieverを呼び出し、TakeDamageを呼び出す。
        yield return new WaitForSeconds(attackInterval);
        GetComponent<BoxCollider>().enabled = true;
        GetComponent<MeshRenderer>().enabled = true;
        yield return new WaitForSeconds(attackTime);
        GetComponent<BoxCollider>().enabled = false;
        GetComponent<MeshRenderer>().enabled = false;
        yield return null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            PlayerParry playerParry = other.gameObject.GetComponent<PlayerParry>();
            PlayerDamageReceiver playerDamageReceiver = other.gameObject.GetComponent<PlayerDamageReceiver>();
            //パリィ成功の条件
            if (playerParry.isParrying && playerParry.parryState == currentAttackDir)
            {
                Debug.Log("Parry Success!");
                //パリィ成功時の処理をここに追加
                // ひとまずboxcolliderを無効化して攻撃を当てないようにする
                GetComponent<BoxCollider>().enabled = false;
                GetComponent<MeshRenderer>().enabled = false;
            }
            else
            {
                Debug.Log("Take Damage");
                playerDamageReceiver.TakeDamage(attackDamage, transform,currentAttackDir);
            }
        }
    }
}
