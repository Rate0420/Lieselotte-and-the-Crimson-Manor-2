using UnityEngine;
using UniVRM10;

public class ExpressionController : MonoBehaviour
{
    [SerializeField] private Vrm10Instance _vrm;
    private Vrm10RuntimeExpression _expression;

    // プリセット表情を使いたい場合
    public ExpressionPreset preset;

    // カスタム表情用
    public string customExpressionName;

    [Range(0, 1f)] public float _value;

    void Awake()
    {
        _expression = _vrm.Runtime.Expression;
    }

    void Update()
    {
        if (!string.IsNullOrEmpty(customExpressionName))
        {
            // カスタム表情
            _expression.SetWeight(ExpressionKey.CreateCustom(customExpressionName), _value);
        }
        else
        {
            // プリセット表情
            _expression.SetWeight(ExpressionKey.CreateFromPreset(preset), _value);
        }
    }
}
