using UnityEngine;

public class CharaSaveManager : MonoBehaviour
{
    public static CharaSaveManager Instance { get; private set; }

    private const string CharacterKey = "SelectedCharacter";

    // 現在選択されているキャラ
    private CharacterType selectedCharacter;

    public enum CharacterType
    {
        Liselotte,
        Velmilion
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            LoadCharacter();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// キャラクターを保存
    /// </summary>
    public void SaveCharacter(CharacterType character)
    {
        selectedCharacter = character;

        PlayerPrefs.SetInt(
            CharacterKey,
            (int)selectedCharacter
        );

        PlayerPrefs.Save();
    }

    /// <summary>
    /// 選択されているキャラクターを取得
    /// </summary>
    public CharacterType GetSelectedCharacter()
    {
        return selectedCharacter;
    }

    /// <summary>
    /// 保存されているキャラクターを読み込む
    /// </summary>
    private void LoadCharacter()
    {
        selectedCharacter = (CharacterType)PlayerPrefs.GetInt(
            CharacterKey,
            (int)CharacterType.Liselotte
        );
    }
}