using UnityEngine;

public abstract class Enemy_AttackBase : ScriptableObject
{
    // 敵の攻撃の基底クラス
    // 各攻撃スクリプトはこのクラスを継承してAttackを実装、EnemyControllerで呼び出して使う。
    public abstract void Attack(EnemyController ec);
}
