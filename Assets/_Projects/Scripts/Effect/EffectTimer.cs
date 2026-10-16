using UnityEngine;

public class EffectTimer : MonoBehaviour
{
    // serializeField属性を使って、Inspectorから設定したfloat時間後にエフェクトを消す
    [SerializeField] float lifetime = 2.0f;
    void Start()
    {
        Destroy(gameObject, lifetime);
    }
}
