using UnityEngine;

[CreateAssetMenu(fileName = "StoryData", menuName = "Scriptable Objects/StoryData")]
public class StoryData : ScriptableObject
{
    [Header("キャラ1画像")] public Sprite[] char1Sprite;
    [Header("キャラ2画像")] public Sprite[] char2Sprite;
    [Header("背景画像")] public Sprite[] bgSprite;

    [Header("名前")] public string[] name;
    [Header("テキスト")] public string[] text;
    [Header("シーン")] public string[] sceneName;

    [Header("BGM")] public AudioClip[] bgm;
    [Header("SE")] public AudioClip[] se;
    [Header("VOICE")] public AudioClip[] voice;

    [Header("画面効果")] public int[] scEffect;
}