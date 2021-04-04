using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MathNet.Numerics.LinearAlgebra;

public class CarGenesController : MonoBehaviour
{
    public GameObject car;
    public keyboardController carController;

    [Header("General Configurations")]
    public int population;

    [Range(0f,1f)]
    public float mutationRate = 0.1f;

    [Header("Crossover Connfigurations")]
    public int  numOfBestAgentSelctions;
    public int numOfWorstAgentSelections;
    public int numberOfChildrenCrossovered;

    private List<int> genePool = new List<int>();

    private int naturalSelected;

    private NerualNetwork[] neuralNetworkPopulation;

    public int currentGeneration;
    public int currentGenome = 0;

    private void Awake(){
        carController = car.GetComponent<keyboardController>();
    }

    private void Start(){
        
        neuralNetworkPopulation = new NerualNetwork[population];
        RandomizeChildren(neuralNetworkPopulation, 0);
        ResetToCurrentGenome();
    }

    private void ResetToCurrentGenome(){
        carController.ResetWithNeuralNetwork(neuralNetworkPopulation[currentGenome]);
    }

    private void RandomizeChildren(NerualNetwork[] networkPopulation, int startIndex){
        while(startIndex < population){
            networkPopulation[startIndex] = new NerualNetwork();
            networkPopulation[startIndex].Initialize(carController.neuralLayers, carController.neurons);
            startIndex++;            
        }
    }

    public void Death(float fitness, NerualNetwork network){
        if (currentGenome < neuralNetworkPopulation.Length - 1){
            neuralNetworkPopulation[currentGenome].fitness = fitness;
            currentGenome++;
            ResetToCurrentGenome();
        }else{
            RePopulate();
        }
    }

    private void RePopulate(){
        genePool.Clear();
        currentGeneration++;
        naturalSelected = 0;
        SortPopulation();

        NerualNetwork[] newPopulation = PickBestPopulation();

        Crossover(newPopulation);
        Mutate(newPopulation);

        RandomizeChildren(newPopulation, naturalSelected);

        neuralNetworkPopulation = newPopulation;
        currentGenome = 0;
        ResetToCurrentGenome();

    }

    private NerualNetwork[] PickBestPopulation(){
        NerualNetwork[] newPopulation = new NerualNetwork[population];

        for (int i = 0; i < numOfBestAgentSelctions; i++){
            newPopulation[naturalSelected] = neuralNetworkPopulation[i].InitialiseCopy(carController.neuralLayers, carController.neurons);
            newPopulation[naturalSelected].fitness = 0;
            naturalSelected++;

            int f = Mathf.RoundToInt(neuralNetworkPopulation[i].fitness * 10);

            // fitness times 10 to increase the change
            // that the neuralnetwork be selected from the gene pool
            for (int c = 0; c < f; c++)
            {
                genePool.Add(i);
            }

            
        }

        for (int i = 0; i < numOfWorstAgentSelections; i++){

            int last = neuralNetworkPopulation.Length - (i+1);
            // last -= i;

            int f = Mathf.RoundToInt(neuralNetworkPopulation[last].fitness * 10);

            for (int c = 0; c < f; c++)
            {
                genePool.Add(last);
            }

        }

        return newPopulation;
    }

    // private NerualNetwork[] CrossoverPopulation(){
    //     List<NerualNetwork> test = new List<NerualNetwork>(5);
    // }

    private void Crossover(NerualNetwork[] newPopulation){

        
        // for (int i = 0; i < numberOfChildrenCrossovered; i+= 2){
        for (int i = naturalSelected; i < population - 20; i++){
            int AIndex = i;
            int BIndex = i;

            if (genePool.Count >= 1)
            {
                for (int l = 0; l < 100; l++)
                // while(true)
                {
                    AIndex = genePool[Random.Range(0, genePool.Count)];
                    BIndex = genePool[Random.Range(0, genePool.Count)];

                    if (AIndex != BIndex)
                        break;
                }
            }

            NerualNetwork Child1 = new NerualNetwork();
            NerualNetwork Child2 = new NerualNetwork();

            Child1.Initialize(carController.neuralLayers, carController.neurons);
            // Child2.Initialize(carController.neuralLayers, carController.neurons);

            Child1.fitness = 0;
            // Child2.fitness = 0;


            for (int w = 0; w < Child1.weights.Count; w++)
            {

                if (Random.Range(0.0f, 1.0f) < 0.5f)
                {
                    Child1.weights[w] = neuralNetworkPopulation[AIndex].weights[w];
                    // Child2.weights[w] = neuralNetworkPopulation[BIndex].weights[w];
                }
                else
                {
                    // Child2.weights[w] = neuralNetworkPopulation[AIndex].weights[w];
                    Child1.weights[w] = neuralNetworkPopulation[BIndex].weights[w];
                }

            }


            for (int w = 0; w < Child1.biases.Count; w++)
            {
                
                if (Random.Range(0.0f, 1.0f) < 0.5f)
                {
                    Child1.biases[w] = neuralNetworkPopulation[AIndex].biases[w];
                    // Child2.biases[w] = neuralNetworkPopulation[BIndex].biases[w];
                }
                else
                {
                    // Child2.biases[w] = neuralNetworkPopulation[AIndex].biases[w];
                    Child1.biases[w] = neuralNetworkPopulation[BIndex].biases[w];
                }

            }

            newPopulation[naturalSelected] = Child1;
            naturalSelected++;

            // newPopulation[naturalSelected] = Child2;
            // naturalSelected++;
        }
    }


    private void Mutate(NerualNetwork[] newPopulation){
        for (int i = 0; i < naturalSelected; i++){
            for (int j = 0; j < newPopulation[i].weights.Count; j++){
                if(Random.Range(0f, 1f) < mutationRate){
                    newPopulation[i].weights[j] = MutateMatrix(newPopulation[i].weights[j]);
                }
            }
        }
    }

    private Matrix<float> MutateMatrix(Matrix<float> matrix){

        int randomPoints = Random.Range(1, (matrix.RowCount * matrix.ColumnCount) / 5);

        Matrix<float> C = matrix;

        for (int i = 0; i < randomPoints; i++)
        {
            int randomColumn = Random.Range(0, C.ColumnCount);
            int randomRow = Random.Range(0, C.RowCount);

            C[randomRow, randomColumn] = Mathf.Clamp(C[randomRow, randomColumn] + Random.Range(-1f, 1f), -1f, 1f);
        }

        return C;
    }

    private void SortPopulation(){
        for (int i = 0; i < neuralNetworkPopulation.Length; i++)
        {
            for (int j = i; j < neuralNetworkPopulation.Length; j++)
            {
                if (neuralNetworkPopulation[i].fitness < neuralNetworkPopulation[j].fitness)
                {
                    NerualNetwork temp = neuralNetworkPopulation[i];
                    neuralNetworkPopulation[i] = neuralNetworkPopulation[j];
                    neuralNetworkPopulation[j] = temp;
                }
            }
        }
    }
    private void SortPopulation(NerualNetwork[] networks)
    {
        for (int i = 0; i < networks.Length; i++)
        {
            for (int j = i; j < networks.Length; j++)
            {
                if (networks[i].fitness < networks[j].fitness)
                {
                    NerualNetwork temp = networks[i];
                    networks[i] = networks[j];
                    networks[j] = temp;
                }
            }
        }
    }
}
