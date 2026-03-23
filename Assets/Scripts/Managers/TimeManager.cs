using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

public class TimeManager : MonoSingleton<TimeManager>
{
    [SerializeField] private float resumeRate = 1.5f;
    [SerializeField] private float pauseRate = 7;

    private float timeAdjustRate;
    private float targetTimeScale = 1f;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) 
        {
            SlowMotion(1);
        }


        if (Mathf.Abs(Time.timeScale - targetTimeScale) > 0.05f)
        {
            float adjustRate = Time.unscaledDeltaTime * timeAdjustRate;
            Time.timeScale = Mathf.Lerp(Time.timeScale, targetTimeScale, adjustRate);
        }
        else
        {
            Time.timeScale = targetTimeScale;
        }
    }

    public void PauseTime()
    {
        timeAdjustRate = pauseRate;
        targetTimeScale = 0;
    }

    public void ResumeTime()
    {
        timeAdjustRate = resumeRate;
        targetTimeScale = 1;
    }
    public void SlowMotion(float seconds) => SlowTimeCoroutine(seconds).Forget();
    private async UniTaskVoid SlowTimeCoroutine(float seconds)
    {
        var ct = this.GetCancellationTokenOnDestroy();
        targetTimeScale = 0.5f;
        Time.timeScale = targetTimeScale;
        await UniTask.Delay(TimeSpan.FromSeconds(seconds), ignoreTimeScale: true, cancellationToken: ct);
        ResumeTime();
    }
}
