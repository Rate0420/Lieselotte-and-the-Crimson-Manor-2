using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "CharacterDataBase", menuName = "CharacterDataBase")]
public class character : ScriptableObject
{
     public List<CharacterData> CharacterData;
}

[Serializable]
public class CharacterData
{
    [Header("キャラクターの名前")] public string Name;
    [Header("キャラクターのオブジェクト")] public GameObject Character;
    [Header("遠距離攻撃のオブジェクト")] public GameObject RangeAttackObject;

    [Header("最大HP")] public int MaxHp;
    [Header("スタミナの最大値")] public int MaxVitality;
    [Header("最大速度")] public float MoveSpeed;

    [Header("回避時間")] public float StepTime;
    [Header("回避距離")] public float StepDis;
    [Header("コンボが発動してからの予約入力受付時間")] public float ComboResetTime = 0.5f;
    [Header("各コンボアニメーションの時間")] public float[] ComboMotionTimes;
    [Header("近接コンボのコライダー出現をどのくらい遅らせるか")] public float[] ComboColOffset;
    [Header("各ガードブレイクのコライダー出現をどのくらい遅らせるか")] public float GBColOffset;

    [Header("遠距離攻撃モーションの時間")] public float ShootTimer = 0.3f;
    [Header("遠距離攻撃モーションの弾を発射するまでの時間")] public float ShootOfsTime = 0.3f;
    [Header("遠距離攻撃の消滅までの時間(モーション開始から)")] public float ShootDestroyTime = 0.3f;
    [Header("遠距離攻撃の弾の速さ")] public float BulletSpeed = 0.3f;
    [Header("ガーブレ攻撃モーションの時間")] public float GB_AttackTime = 0.5f;

    [Header("ガードのスタミナ消費量")]  public float GuardVitalityConsumption;
    [Header("回避のスタミナ消費量")]    public float AvoidanceVitalityConsumption;
    [Header("スキル1のスタミナ消費量")] public float S1VitalityConsumption;
    [Header("スキル2のスタミナ消費量")] public float S2VitalityConsumption;
    [Header("スキル3のスタミナ消費量")] public float S3VitalityConsumption;
}