using UnityEngine;

public class TCommand : MonoBehaviour
{
    private float t_time;
    private int stageNo = 5;

    [SerializeField] private float count = 5;

    private void Update()
    {
        if (Input.GetKey(KeyCode.T))
        {
            t_time += Time.deltaTime;

            if (t_time >= count)
            {
                Debug.Log("int");

                for (int i = 0; i < stageNo; i++)
                {
                    StageClearCheck.SetStageClear(i);
                }

                t_time = 0f; // 必要ならリセット
            }
        }
        else
        {
            t_time = 0f; // キー離したらリセット
        }
    }
}
