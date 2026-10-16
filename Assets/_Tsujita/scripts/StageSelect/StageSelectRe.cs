using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StageSelectRe : MonoBehaviour
{
    [SerializeField] private Button[] stageButtons; // ステージボタン

    private void Start()
    {
        StageClearCheck.Init();

        for (int i = 0; i < stageButtons.Length; i++)
        {
            int stageNo = i + 1;
            bool unlocked = StageClearCheck.IsStageUnlocked(stageNo);

            // 解放しているステージのみ表示
            stageButtons[i].gameObject.SetActive(unlocked);
        }
    }
}
