using UnityEngine;

// Procedural hover/click blips so the UI has sound feedback without any audio assets.
public static class UIAudio
{
    private static AudioSource source;
    private static AudioClip hoverClip;
    private static AudioClip clickClip;

    public static void PlayHover()
    {
        if (hoverClip == null) hoverClip = MakeBlip(880f, 0.04f);
        Play(hoverClip, 0.25f);
    }

    public static void PlayClick()
    {
        if (clickClip == null) clickClip = MakeBlip(520f, 0.08f);
        Play(clickClip, 0.45f);
    }

    private static void Play(AudioClip clip, float gain)
    {
        if (source == null)
        {
            GameObject go = new GameObject("UIAudio");
            Object.DontDestroyOnLoad(go);
            source = go.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.spatialBlend = 0f;
        }
        source.PlayOneShot(clip, gain * SettingsManager.volume);
    }

    private static AudioClip MakeBlip(float frequency, float length)
    {
        const int rate = 44100;
        int samples = Mathf.CeilToInt(rate * length);
        float[] data = new float[samples];
        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / rate;
            float env = 1f - (float)i / samples;
            data[i] = Mathf.Sin(2f * Mathf.PI * frequency * t) * env * env;
        }
        AudioClip clip = AudioClip.Create("ui_blip", samples, 1, rate, false);
        clip.SetData(data, 0);
        return clip;
    }
}
