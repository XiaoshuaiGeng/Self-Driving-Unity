using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using MathNet.Numerics.LinearAlgebra;
using System;

public class NerualNetwork : MonoBehaviour
{
    // three sensors so we need three input
    public Matrix<float> inputlayer = Matrix<float>.Build.Dense(1, 3);
    public List<Matrix<float>> hiddenLayers = new List<Matrix<float>>();
    public Matrix<float> outputLayer = Matrix<float>.Build.Dense(1, 2);
    public List<Matrix<float>> weights = new List<Matrix<float>>();
    public List<float> biases = new List<float>();
    public float fitness;

    public void Initialise(int hiddenLayerNum, int hiddenNaeutonNum) 
    {
        inputlayer.Clear();
        hiddenLayers.Clear();
        outputLayer.Clear();
        weights.Clear();
        biases.Clear();

        for (int i = 0; i < hiddenLayerNum + 1; i++)
        {
            Matrix<float> f = Matrix<float>.Build.Dense(1, hiddenNaeutonNum);
            hiddenLayers.Add(f);
            biases.Add(Random.Range(-1f, 1f));

            //WEIGHTS
            if (i == 0)
            {
                Matrix<float> inputToH1 = Matrix<float>.Build.Dense(3, hiddenLayerNum);
                weights.Add(inputToH1);
            }

            Matrix<float> HiddenToHidden = Matrix<float>.Build.Dense(hiddenNaeutonNum, hiddenNaeutonNum);
            weights.Add(HiddenToHidden);

        }

        Matrix<float> OutputWeight = Matrix<float>.Build.Dense(hiddenNaeutonNum, 2);
        weights.Add(OutputWeight);
        biases.Add(Random.Range(-1f, 1f));

        RandomiseWeights();
    }

    public void RandomiseWeights()
    {
        for (int i = 0; i < weights.Count; i++) 
        {
            for (int x = 0; x < weights[i].RowCount; x++)
            {
                for (int y = 0; y < weights[i].ColumnCount; y++)
                {
                    weights[i][x, y] = Random.Range(-1f, 1f);
                }
            }
        }
    }

    public (float, float) RunNetwork(float a, float b, float c)
    {
        inputlayer[0, 0] = a;
        inputlayer[0, 1] = b;
        inputlayer[0, 2] = c;

        inputlayer = inputlayer.PointwiseTanh();

        hiddenLayers[0] = ((inputlayer * weights[0])+ biases[0]).PointwiseTanh();

        for (int i = 1; i < hiddenLayers.Count; i++)
        {
            hiddenLayers[i] = ((hiddenLayers[i - 1] * weights[i])+ biases[i]).PointwiseTanh();
        }

        outputLayer = ((hiddenLayers[hiddenLayers.Count - 1] * weights[weights.Count - 1]) +biases[biases.Count - 1]).PointwiseTanh();

        
        
        //First output is acceleration and second output is steering
        return (Sigmoid(outputLayer[0, 0]), (float)Math.Tanh(outputLayer[0, 1]));
    }

    private float Sigmoid(float s)
    {
        return (float)(1 /(1 + Math.Exp(-s)));
    }


}
