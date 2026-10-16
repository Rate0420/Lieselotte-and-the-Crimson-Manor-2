using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    [SerializeField] Image hpBar;           // 表HPバー
    [SerializeField] Image hpBarAnimation;  // 裏HPバー(遅れて追従する)

    [SerializeField] float animationTime = 0.5f; // 追従時間
    [SerializeField] int damage;  // ダメージ量
    public int maxHp = 100;
    public int nowHp = 100;

    public bool enemyFlg;
    public bool tutorialFlg = false;


    float targetFill;    // 緑バーのターゲットfill
    float animFill;      // 赤バーの現在fill
    float animTimer;
    bool animating;

    bool isGameOver = false;

    [SerializeField] bool nodead = false;

    //[SerializeField] character character;

    void Start()
    {
        //nowHp = maxHp;
        hpBar.fillAmount = 1f;
        hpBarAnimation.fillAmount = 1f;
    }

    void Update()
    {
        // デバッグ用：スペースキーでダメージ
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ApplyDamage(damage);
        }

        // アニメーション処理
        if (animating)
        {
            animTimer += Time.deltaTime;
            float t = animTimer / animationTime;
            t = Mathf.Clamp01(t);

            hpBarAnimation.fillAmount = Mathf.Lerp(animFill, targetFill, t);

            if (t >= 1f)
            {
                animating = false;
            }
        }
    }

    public void ApplyDamage(int dmg)
    {
        Debug.Log(dmg);
        nowHp -= dmg;
        if (nowHp <= 0)
        {
            nowHp = 0;
            if (enemyFlg)
            {
                if (!isGameOver)
                {
                    isGameOver = true;
                    GameObject.FindWithTag("Player").GetComponent<PlayerStatusManager>().ScoreEnter();
                    if(!tutorialFlg)FadeSceneChanger.WaitAndFade(1f,"ResultScene");
                }
            }
            else
            {
                if (tutorialFlg) return;
                if (!isGameOver)
                {
                    isGameOver = true;
                    GameObject player = GameObject.FindWithTag("Player");
                    player.GetComponent<PlayerManager>().dummyFlg = true;
                    player.GetComponent<PlayerManager>().mutekiFlg = true;
                    Debug.Log(GameObject.FindWithTag("Enemy"));
                    GameObject.FindWithTag("Enemy").GetComponent<EnemyController>().StopAllCoroutines();
                    GameOver GameOverManager = GameObject.FindWithTag("GameOverManager").GetComponent<GameOver>();
                    GameOverManager.ShowGameOver();
                }
            }
        }

        // 即座に緑バーを減らす
        targetFill = (float)nowHp / maxHp;
        hpBar.fillAmount = targetFill;

        // 赤バーは遅れて追従するためにセット
        animFill = hpBarAnimation.fillAmount;
        animTimer = 0f;
        animating = true;
    }
}
