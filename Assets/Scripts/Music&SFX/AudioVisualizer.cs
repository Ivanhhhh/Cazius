using UnityEngine;

public enum FrequencyFocusWindow
{
    Entire = 1,
    FirstHalf = 2,
    FirstQuarter = 4,
    FirstEight = 8,
    FirstSixteenth = 16
}

public class AudioVisualizer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform[] bars;

    [Header("Settings")]
    public FrequencyFocusWindow frequencyFocusWindow = FrequencyFocusWindow.FirstQuarter;
    public FFTWindow fftWindow = FFTWindow.BlackmanHarris;
    public float amplification = 1.0f;
    public float baseHeight = 0.0f;
    public bool useDecibels;

    [Header("State")]
    [SerializeField] private float[] spectrumData;

    void Awake()
    {
        spectrumData = new float[4096];
    }

    void Update()
    {
        if (bars == null || bars.Length == 0) return;

        AudioListener.GetSpectrumData(spectrumData, 0, fftWindow);

        int blockSize = spectrumData.Length / bars.Length / (int)frequencyFocusWindow;
        if (blockSize < 1) blockSize = 1;

        for (int i = 0; i < bars.Length; ++i)
        {
            float sum = 0f;
            for (int j = 0; j < blockSize; j++)
            {
                sum += spectrumData[i * blockSize + j];
            }
            sum /= blockSize;

            float amplitude = Mathf.Clamp(sum, 1e-7f, 1f);
            Vector3 scale = bars[i].localScale;

            if (useDecibels)
                scale.y = -Mathf.Log10(amplitude) * amplification / 200f;
            else
                scale.y = sum * amplification + baseHeight;

            bars[i].localScale = scale;
        }
    }
}