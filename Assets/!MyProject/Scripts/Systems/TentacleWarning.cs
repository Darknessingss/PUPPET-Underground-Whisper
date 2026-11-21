using System.Collections;
using UnityEngine;

public class TentacleWarning : MonoBehaviour
{
    private Coroutine _returnCoroutine;

    [SerializeField] private float TimerSeconds = 0f;

    private void OnEnable()
    {
        if (_returnCoroutine != null)
            StopCoroutine(_returnCoroutine);

        _returnCoroutine = StartCoroutine(ReturnRoutine());
    }

    private IEnumerator ReturnRoutine()
    {
        yield return new WaitForSeconds(TimerSeconds);

        if (BulletPool.Instance != null)
            BulletPool.Instance.Return(gameObject);
        else
            Destroy(gameObject);
    }
}