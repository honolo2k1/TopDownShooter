using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Cysharp.Threading.Tasks;
using System;

public class AudioManager : MonoSingleton<AudioManager>
{
    [SerializeField] private AudioSource[] bgm;

    [SerializeField] private bool playBgm;
    [SerializeField] private int bgmIndex;

    private void Start()
    {
        //PlayBGM(3);
    }

    private void Update()
    {
        //if (!playBgm && BgmIsPlaying())
        //    StopAllBGM();


        //if (!playBgm && !bgm[bgmIndex].isPlaying)
        //    PlayRandomBGM();

        if (playBgm == false && BgmIsPlaying())
            StopAllBGM();


        if (playBgm && bgm[bgmIndex].isPlaying == false)
            PlayRandomBGM();

    }

    public void PlaySFX(AudioSource sfx, bool randomPitch = false, float minPitch = .85f, float maxPitch = 1.1f)
    {
        if (sfx == null)
            return;

        float pitch = UnityEngine.Random.Range(minPitch, maxPitch);

        sfx.pitch = pitch;
        sfx.Play();
    }

    public void SFXDelayAndFade(AudioSource source, bool play, float taretVolume, float delay = 0, float fadeDuratuin = 1)
    {
        SFXDelayAndFadeCo(source, play, taretVolume, delay, fadeDuratuin).Forget();
    }

    public void PlayBGM(int index)
    {
        StopAllBGM();

        bgmIndex = index;
        bgm[index].Play();

        //playBgm = true;
    }

    public void StopAllBGM()
    {
        for (int i = 0; i < bgm.Length; i++)
        {
            bgm[i].Stop();
        }

        //playBgm = false;
    }

    [ContextMenu("Play random music")]
    public void PlayRandomBGM()
    {
        StopAllBGM();
        bgmIndex = UnityEngine.Random.Range(0, bgm.Length);
        PlayBGM(bgmIndex);
    }

    private bool BgmIsPlaying()
    {
        for (int i = 0; i < bgm.Length; i++)
        {
            if (bgm[i].isPlaying)
                return true;
        }

        return false;
    }

    private async UniTaskVoid SFXDelayAndFadeCo(AudioSource source, bool play, float targetVolume, float delay = 0, float fadeDuration = 1)
    {
        var ct = this.GetCancellationTokenOnDestroy();

        if (delay > 0)
            await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: ct);

        float startVolume = play ? 0 : source.volume;
        float endVolume = play ? targetVolume : 0;
        float elapsed = 0;

        if (play)
        {
            source.volume = 0;
            source.Play();
        }

        //Fade in/out over the duration
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, endVolume, elapsed / fadeDuration);
            await UniTask.Yield(PlayerLoopTiming.Update, ct);
        }

        source.volume = endVolume;

        if (play == false)
            source.Stop();
    }
}
