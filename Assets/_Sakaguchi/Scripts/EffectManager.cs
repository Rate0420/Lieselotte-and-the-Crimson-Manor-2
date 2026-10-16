using System.Collections;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public GameObject[] Effects;
    public PlayerManager pm;
    GameObject nowEffect;

    public void PlayEffect(int effectnum ,Transform tf)
    {
        nowEffect = Instantiate(Effects[effectnum], tf.position, tf.rotation, tf.transform);
    }

    public void PlayEffect(int effectnum, Transform tf,Vector3 rot)
    {
        nowEffect = Instantiate(Effects[effectnum], tf.position, tf.rotation, tf.transform);
        nowEffect.transform.Rotate(rot);
    }

    public void EndEffect()
    {
        Destroy(nowEffect);
    }

    public void PlayEffect(int effectnum,float time,Transform tf)
    {
        StartCoroutine(PE(effectnum, time, tf));
    }

    public void PlayEffect(int effectnum, float time, Transform tf,Vector3 move)
    {
        StartCoroutine(PE(effectnum, time, tf,move));
    }

    public void PlayEffect(int effectnum, float time, Transform tf, Vector3 move,Vector3 rot)
    {
        StartCoroutine(PE(effectnum, time, tf, move,rot));
    }

    public void PlayEffectStay(int effectnum,float time,Transform tf)
    {
        StartCoroutine(PES(effectnum, time,tf));
    }

    public void PlayEffectStay(int effectnum, float time, Transform tf, Vector3 move)
    {
        StartCoroutine(PES(effectnum, time, tf, move));
    }

    IEnumerator PE(int effectnum, float time,Transform tf)
    {
        GameObject effect;
        effect = Instantiate(Effects[effectnum], tf.position, tf.rotation, tf.transform);
        yield return new WaitForSeconds(time);
        Destroy(effect);
    }

    IEnumerator PE(int effectnum, float time, Transform tf,Vector3 move)
    {
        GameObject effect;
        effect = Instantiate(Effects[effectnum], tf.position, tf.rotation, tf.transform);
        effect.transform.Translate(move);
        yield return new WaitForSeconds(time);
        Destroy(effect);
    }


    IEnumerator PE(int effectnum, float time, Transform tf, Vector3 move, Vector3 rot)
    {
        GameObject effect;
        effect = Instantiate(Effects[effectnum], tf.position, tf.rotation, tf.transform);
        effect.transform.Translate(move);
        effect.transform.Rotate(rot);
        yield return new WaitForSeconds(time);
        Destroy(effect);
    }

    IEnumerator PES(int effectnum, float time, Transform tf)
    {
        GameObject effect;
        effect = Instantiate(Effects[effectnum], tf.position, tf.rotation);
        yield return new WaitForSeconds(time);
        Destroy(effect);
    }

    IEnumerator PES(int effectnum, float time, Transform tf,Vector3 move)
    {
        GameObject effect;
        effect = Instantiate(Effects[effectnum], tf.position, tf.rotation);
        effect.transform.Translate(move);
        yield return new WaitForSeconds(time);
        Destroy(effect);
    }
}
