using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class AuidoSample : MonoBehaviour
{

    public List<AudioSource> audioSources = new List<AudioSource>();
    public int Bandscount;
    [HideInInspector]
    public bool Singing
    {
        get => singing;
        set
        {
            if (singing!=value)
                t = -Mathf.PI;
            singing = value;

        }
    }
    private bool singing;
    [HideInInspector]
    public float[] audioBandBuffers;
    [HideInInspector]
    public float[] standBand;
    [HideInInspector]
    public float t;


    private static AuidoSample _instance;
    public static AuidoSample instance => _instance;

    private List<float[]> samplesList = new List<float[]>();
    private float[] freqBands;
    private float[] bufferDecrease ;
    private float freqBandsHighest;
    private int[] keyslist;
    private float[] audioBands;


    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        freqBands = new float[Bandscount];
        bufferDecrease = new float[Bandscount];
        audioBands = new float[Bandscount];
        audioBandBuffers = new float[Bandscount];
        standBand = new float[Bandscount];
        for (var freqIndex = 0; freqIndex < Bandscount / 2; freqIndex++)
        {
            standBand[freqIndex] = MathF.Pow(4, Mathf.Lerp(-3, -0.5f, (freqIndex + 1.0f) / (Bandscount / 2))) + 0.3f;
            standBand[Bandscount - freqIndex - 1] = standBand[freqIndex];
            bufferDecrease[freqIndex] = 0.005f;
            bufferDecrease[Bandscount - freqIndex - 1] = 0.005f;
        }

        keyslist = new int[120];
        for (int key = 0; key < 120; key++)
        {

            int totalSampleCount = 1;

            do
            {
                keyslist[key] = totalSampleCount;
                totalSampleCount++;
            }
            while (totalSampleCount * (22050f / 2048f) < Mathf.Pow(Mathf.Pow(2, 1f/12f), key-57) * 440 * Mathf.Pow(2 , 1f/24f));
        }
    }
    void Update()
    {
        if (audioSources.Count == 0 || Manager.instance.state.modelstatu == modelstatu.talking)
        {
            return;
        }
        foreach (var audioSource in audioSources)
        {
            if (audioSource.isPlaying)
            {
                Singing = true;
                break;
            }
            Singing = false;
        }
        if (Singing)
        {
            ReadSamples();
            CalculateBands2();
            BufferBands();
        }
        else
            CreatWaitBands();

    }

    private void ReadSamples()
    {
        int sourceIndex = 0;
        foreach (AudioSource audioSource in audioSources)
        {
            float[] samples;
            if (samplesList.Count < sourceIndex + 1)
            {
                samples = new float[2048];
                samplesList.Add(samples);
            }
            else
            {
                samples = samplesList[sourceIndex];
            }
            sourceIndex++;
            audioSource.GetSpectrumData(samples, 0, FFTWindow.Blackman);
        }
    }
    private float GetMaxSampleAtPosition(int sampleIndex)
    {
        float maxSample = 0;
        foreach (float[] samples in samplesList)
        {
            float sample = samples[sampleIndex];
            if (sample > maxSample)
            {
                maxSample = sample;
            }
        }
        return maxSample;
    }

    private void CalculateBands2()
    {
        int totalSampleCount = 0;
        freqBandsHighest = 0;
        for (int freqIndex = 0; freqIndex < Bandscount; freqIndex++)
        {
            float average = 0;
            int sampleCount = keyslist[freqIndex];
            do
            {
                average += GetMaxSampleAtPosition(totalSampleCount) * (totalSampleCount + 1);
                totalSampleCount++;
            }
            while (totalSampleCount < sampleCount);

            average /= totalSampleCount;
            freqBands[Bandscount - 1 - freqIndex] = average;

            if (freqBands[Bandscount - 1 - freqIndex] > freqBandsHighest)
                freqBandsHighest = freqBands[Bandscount - 1 - freqIndex];
        }
        for (int freqIndex = 0; freqIndex < Bandscount; freqIndex++)
        {
            audioBands[freqIndex] = 0.3f + Mathf.Lerp(0, 0.7f, freqBands[freqIndex] / (freqBandsHighest * 1.2f));
        }
    }
 
    private void BufferBands()
    {
        for (int freqIndex = 0; freqIndex < Bandscount; freqIndex++)
        {
            // New Maximum reached
            if (audioBands[freqIndex] > audioBandBuffers[freqIndex])
            {
                audioBandBuffers[freqIndex] = audioBands[freqIndex];
                bufferDecrease[freqIndex] = 0.005f;
            }
            else if (audioBands[freqIndex] < audioBandBuffers[freqIndex])
            {
                audioBandBuffers[freqIndex] -= bufferDecrease[freqIndex];
                bufferDecrease[freqIndex] *= 1.4f;
            }
        }
    }
    private void CreatWaitBands()
    {
        var interval = Mathf.PI / (Bandscount/2);

        for (var freqIndex = 0; freqIndex < Bandscount / 2; freqIndex++)
        {
            audioBandBuffers[freqIndex] = standBand[freqIndex] + 0.1f * MathF.Sin(t + interval * freqIndex > 0 ? t + interval * freqIndex : 0f);
            audioBandBuffers[Bandscount - freqIndex - 1] = audioBandBuffers[freqIndex];
        }
        t += 0.01f;
    }
    
}
