using UnityEngine;

public class OutOfBoundsScript : MonoBehaviour
{
    // ステージの範囲外に出てしまった時の処理
    public GameObject SpawnPos;

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player1" || other.tag == "Player2")
            other.transform.position = SpawnPos.transform.position;
    }
}
