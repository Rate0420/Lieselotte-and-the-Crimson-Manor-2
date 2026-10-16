using UnityEngine;
using UnityEngine.UI;

public class ComboManager : MonoBehaviour
{
    [Header("コンボ時間を表示しているスライダー")]
    [SerializeField] Slider comboSlider;

    [Header("コンボ数を表記するテキスト")]
    [SerializeField] Text comboText;

    public int maxCombo;  // 最大コンボ数
    public int nowCombo;         // 今のコンボ数
    public float comboDuration;  // コンボ継続時間
    float nowComboTime;   // 今のコンボ経過時間
    int baseMeleeAtk;

    public bool comboTimerFlg = true;

    [SerializeField] PlayerManager playerManager;

    // 固有技でnowComboとは別のコンボ(UniqueCombo)というのが欲しいので、変数を用意、nowComboと同じ条件で増減させ、
    // 固有技を使った時に0にリセットする。
    int uniqueCombo;
    public int UniqueCombo { get { return uniqueCombo; } }

    void Start()
    {
        maxCombo = 0;
        nowCombo = 0;
        uniqueCombo = 0;
        nowComboTime = 0;
        comboSlider.value = 0;
        comboText.text = "" + nowCombo;
        baseMeleeAtk = playerManager.playerStatusManager.MeleeAtk;
        // comboDuration = ;　　ここで経過時間を代入する
    }

    void Update()
    {
        if(!comboTimerFlg) return;
        nowComboTime -= Time.deltaTime;
        // 割合を計算し、スライダーに渡す
        float ratio = nowComboTime / comboDuration;
        comboSlider.value = ratio;

        // 継続時間が過ぎたらコンボをリセットする
        if (comboSlider.value == 0) comboReset();
    }

    /// <summary>
    /// コンボを増やす処理
    /// </summary>
    public void ComboContinues()
    {
        nowComboTime = comboDuration;
        nowCombo++;
        uniqueCombo++;
        comboText.text = "" + nowCombo;
        if (maxCombo <= nowCombo)
        {
            maxCombo = nowCombo;
            playerManager.playerStatusManager.maxCombo = maxCombo;
        }
        Debug.Log(uniqueCombo);
        if (uniqueCombo >= 10)
        {
            if (uniqueCombo == 10)
                SoundManager.Instance.PlaySE("固有技使用可能音");
            playerManager.ChangeColorTrail(new Color(0, 1, 1, 1));
        }
        else
        {
            playerManager.ChangeColorTrail(Color.white);
        }

        // コンボが1につき1パーセント、攻撃力が上がる処理
        playerManager.playerStatusManager.MeleeAtk = Mathf.FloorToInt(baseMeleeAtk + (baseMeleeAtk * nowCombo / 100));
        Debug.Log("現在の攻撃力:" + playerManager.playerStatusManager.MeleeAtk);
    }

    /// <summary>
    /// コンボをリセットする処理
    /// </summary>
    void comboReset()
    {
        nowCombo = 0;
        uniqueCombo = 0;
        comboText.text = "" + nowCombo;
        playerManager.playerStatusManager.MeleeAtk = baseMeleeAtk;
    }

    /// <summary>
    /// UniqueComboをリセットする処理
    /// </summary>
    public void ResetUniqueCombo()
    {
        uniqueCombo = 0;
    }
}