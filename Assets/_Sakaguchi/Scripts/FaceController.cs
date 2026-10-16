using System.Collections;
using UnityEngine;
using UniVRM10;

public class FaceController : MonoBehaviour
{
    [SerializeField] private Vrm10Instance _vrm;
    private Vrm10RuntimeExpression _expression;

    void Awake()
    {
        _expression = _vrm.Runtime.Expression;
        StartCoroutine(BlinkCoroutine());
    }

    // 自動瞬き
    IEnumerator BlinkCoroutine()
    {
        var key = ExpressionKey.CreateFromPreset(ExpressionPreset.blink);

        while (true)
        {
            yield return new WaitForSeconds(Random.Range(3f, 7f));
            _expression.SetWeight(key, 1.0f);
            //_expression.SetWeight(ExpressionKey.CreateCustom("damaged"), 1.0f);
            yield return new WaitForSeconds(0.1f);
            _expression.SetWeight(key, 0.0f);
            //_expression.SetWeight(ExpressionKey.CreateCustom("damaged"), 0.0f);
            PlayExpression(ExpressionPreset.neutral);
        }
    }

    // 他スクリプトから呼べる：指定表情を一定時間出す
    public void PlayExpression(ExpressionPreset preset, float duration = 0.5f)
    {
        StartCoroutine(ExpressionCoroutine(preset, duration));
    }

    public void PlayCustomExpression(string expressionName, float duration = 0.5f)
    {
        StopAllCoroutines(); // 実行中の表情コルーチンを止めて上書き
        StartCoroutine(ExpressionCoroutine(expressionName, duration));
    }

    private IEnumerator ExpressionCoroutine(string expressionName, float duration)
    {
        var key = ExpressionKey.CreateCustom(expressionName);

        _expression.SetWeight(key, 1.0f);
        yield return new WaitForSeconds(duration);
        _expression.SetWeight(key, 0.0f);
    }


    private IEnumerator ExpressionCoroutine(ExpressionPreset preset, float duration)
    {
        var key = ExpressionKey.CreateFromPreset(preset);

        _expression.SetWeight(key, 1.0f);
        yield return new WaitForSeconds(duration);
        _expression.SetWeight(key, 0.0f);
    }
}
