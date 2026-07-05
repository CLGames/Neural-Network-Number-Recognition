using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetUI
{
    internal class NeuralNetwork
    {
        private int inputSize, hiddenSize, outputSize;
        private Neuron[] hidden;
        public Neuron[] output;
        private double hiddenLimit, outputLimit, learningRate, cost;
        private Random random;
        private double[] input;
        public NeuralNetwork(int inputSize, int hiddenSize, int outputSize, bool load)
        {
            this.inputSize = inputSize;
            this.hiddenSize = hiddenSize;
            this.outputSize = outputSize;
            input = new double[inputSize];
            hidden = new Neuron[hiddenSize];
            output = new Neuron[outputSize];

            random = new Random();

            hiddenLimit = Math.Sqrt(2.0 / inputSize);
            outputLimit = Math.Sqrt(2.0 / hiddenSize);
            learningRate = 0.001;
            cost = 0;

            if (load)
            {
                LoadParameters();
            }
            else
            {
                InitParameters();
            }

        }

        private void LoadParameters()
        {
            string[] b1 = File.ReadAllLines("data/b-1.txt");
            string[] b2 = File.ReadAllLines("data/b-2.txt");
            string[] w1 = File.ReadAllLines("data/w-1.txt");
            string[] w2 = File.ReadAllLines("data/w-2.txt");

            double[] allHiddenWeights = ExtractWeights(w1);
            double[] allOutputWeights = ExtractWeights(w2);
            for (int i = 0; i < hiddenSize; i++)
            {
                double[] weights = new double[inputSize];
                for (int j = 0; j < inputSize; j++)
                {
                    weights[j] = allHiddenWeights[i * inputSize + j];
                }
                double bias = double.Parse(b1[i]);
                hidden[i] = new Neuron(weights, bias);
            }
            for (int i = 0; i < outputSize; i++)
            {
                double[] weights = new double[hiddenSize];
                for (int j = 0; j < hiddenSize; j++)
                {
                    weights[j] = allOutputWeights[i * hiddenSize + j];
                }
                double bias = double.Parse(b2[i]);
                output[i] = new Neuron(weights, bias);
            }

        }

        public void SaveParameters()
        {
            List<double> b1 = new List<double>();
            List<double> b2 = new List<double>();
            List<double> w1 = new List<double>();
            List<double> w2 = new List<double>();
            foreach (Neuron n in hidden)
            {

                b1.Add(n.bias);
                foreach (double w in n.weights)
                {
                    w1.Add(w);

                }
            }
            foreach (Neuron n in output)
            {
                b2.Add(n.bias);
                foreach (double w in n.weights)
                {
                    w2.Add(w);
                }
            }

            WriteToFile(b1, "b-1.txt");
            WriteToFile(b2, "b-2.txt");
            WriteToFile(w1, "w-1.txt");
            WriteToFile(w2, "w-2.txt");

        }

        private void WriteToFile(List<double> vals, string path)
        {
            using (StreamWriter writer = new StreamWriter(path))
            {
                foreach (double v in vals)
                {
                    writer.WriteLine(v);
                }
            }
        }

        private void InitParameters()
        {
            for (int i = 0; i < hiddenSize; i++)
            {
                double[] weights = GenerateWeights(inputSize, -hiddenLimit, hiddenLimit);
                hidden[i] = new Neuron(weights, 0);
            }
            for (int i = 0; i < outputSize; i++)
            {
                double[] weights = GenerateWeights(hiddenSize, -outputLimit, outputLimit);
                output[i] = new Neuron(weights, 0);
            }
        }

        private double[] GenerateWeights(int size, double min, double max)
        {
            double[] weights = new double[size];
            for (int i = 0; i < size; i++)
            {
                weights[i] = GetRandomDouble(min, max);
            }

            return weights;
        }

        private double[] ExtractWeights(string[] w)
        {
            double[] output = new double[w.Length];
            for (int i = 0; i < w.Length; i++)
            {
                output[i] = double.Parse(w[i]);
            }

            return output;
        }

        private double GetRandomDouble(double min, double max)
        {
            return random.NextDouble() * (max - min) + min;
        }

        public int FeedForward(double[] input)
        {
            this.input = input;
            SetHidden(input);
            SetOutput();
            return Softmax();
        }

        public void Backpropagate(int target)
        {
            int[] targetList = SetTarget(target);
            double[] outputDeltas = GetOutputDeltas(targetList);
            double[] hiddenDeltas = GetHiddenDeltas(outputDeltas);
            UpdateW2(outputDeltas);
            UpdateB2(outputDeltas);
            UpdateW1(hiddenDeltas);
            UpdateB1(hiddenDeltas);
        }

        private int[] SetTarget(int target)
        {
            int[] targetList = new int[outputSize];
            for (int i = 0; i < outputSize; i++)
            {
                targetList[i] = 0;
            }
            targetList[target] = 1;
            return targetList;
        }

        private double[] GetOutputDeltas(int[] targetList)
        {
            double[] outputDeltas = new double[outputSize];
            cost = 0;
            for (int i = 0; i < outputSize; i++)
            {
                double error = output[i].output - targetList[i];
                cost -= targetList[i] * Math.Log(output[i].output + 1e-12);
                outputDeltas[i] = error;
            }

            return outputDeltas;
        }

        private double[] GetHiddenDeltas(double[] outputDeltas)
        {
            double[] hiddenDeltas = new double[hiddenSize];
            for (int h = 0; h < hiddenSize; h++)
            {
                double error = 0;
                for (int o = 0; o < outputSize; o++)
                {
                    error += output[o].weights[h] * outputDeltas[o];
                }
                if (hidden[h].sum > 0) hiddenDeltas[h] = error;
                else hiddenDeltas[h] = 0;
            }

            return hiddenDeltas;
        }

        private void UpdateW2(double[] outputDeltas)
        {
            for (int o = 0; o < outputSize; o++)
            {
                for (int h = 0; h < hiddenSize; h++)
                {
                    double gradient = outputDeltas[o] * hidden[h].output;
                    output[o].weights[h] -= gradient * learningRate;
                }
            }
        }

        private void UpdateW1(double[] hiddenDeltas)
        {
            for (int h = 0; h < hiddenSize; h++)
            {
                for (int i = 0; i < inputSize; i++)
                {
                    double gradient = hiddenDeltas[h] * input[i];
                    hidden[h].weights[i] -= gradient * learningRate;
                }
            }
        }

        private void UpdateB2(double[] outputDeltas)
        {
            for (int i = 0; i < outputSize; i++)
            {
                output[i].bias -= learningRate * outputDeltas[i];
            }
        }

        private void UpdateB1(double[] hiddenDeltas)
        {
            for (int i = 0; i < hiddenSize; i++)
            {
                hidden[i].bias -= learningRate * hiddenDeltas[i];
            }
        }

        public double GetCost()
        {
            return cost;
        }

        private void SetHidden(double[] input)
        {
            foreach (Neuron n in hidden)
            {
                double sum = 0;
                for (int i = 0; i < inputSize; i++)
                {
                    sum += n.weights[i] * input[i];
                }
                sum += n.bias;
                n.SetSum(sum);
                n.SetOutput(ReLU(sum));
            }
        }

        private void SetOutput()
        {
            foreach (Neuron n in output)
            {
                double sum = 0;
                for (int i = 0; i < hiddenSize; i++)
                {
                    sum += n.weights[i] * hidden[i].output;
                }
                sum += n.bias;
                n.SetSum(sum);
                n.SetOutput(sum);
            }
        }

        private int Softmax()
        {
            double max = output.Max(n => n.output);
            double sum = 0;

            foreach (Neuron n in output)
            {
                double e = Math.Exp(n.output - max);
                n.SetOutput(e);
                sum += e;
            }
            foreach (Neuron n in output)
            {
                double val = n.output;
                n.SetOutput(val / sum);
            }
            double highest = -1;
            for (int i = 0; i < outputSize; i++)
            {
                if (output[i].output > highest) highest = output[i].output;
            }
            for (int i = 0; i < outputSize; i++)
            {
                if (output[i].output == highest) return i;
            }
            return -1;
        }

        private double ReLU(double x)
        {
            if (x > 0) return x;
            return 0;
        }

        public void DisplayOutput()
        {
            foreach (Neuron n in output)
            {
                Console.WriteLine(n.output);
            }

            Console.WriteLine();
        }

        public void DisplayHidden()
        {
            foreach (Neuron n in hidden)
            {
                Console.WriteLine(n.output);
            }

            Console.WriteLine();
        }

    }
}
