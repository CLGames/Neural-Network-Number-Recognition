using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetUI
{
    internal class Neuron
    {
        public double[] weights;
        public double bias, output, sum;

        public Neuron(double[] weights, double bias)
        {
            this.weights = weights;
            this.bias = bias;
            sum = 0;
            output = 0;
        }

        public void SetOutput(double value)
        {
            output = value;
        }

        public void SetSum(double value)
        {
            sum = value;
        }
    }
}
