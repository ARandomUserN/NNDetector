using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MatrixLibrary.Matrix;
using System.IO;

namespace NNDetector
{
    public class NeuralNetwork
    {
        private Random _objRandom;
        Matrix weightsLayer1;
        Matrix weightsLayer2;

        public NeuralNetwork(int inputNeurons = 2,int hiddenNeurons = 4, int outputNeurons = 1) 
        {
            _objRandom = new Random();
            weightsLayer1 = new Matrix(inputNeurons, hiddenNeurons);
            weightsLayer2 = new Matrix(hiddenNeurons, outputNeurons);
            InitMatrix(weightsLayer1);
            InitMatrix(weightsLayer2);
        }

        private void InitMatrix(Matrix matrix)
        {
            int r = matrix.Rows;
            int c = matrix.Cols;

            for(int i = 0; i < r; i++)
            {
                for(int j = 0; j < c; j++)
                {
                    matrix[i, j] = _objRandom.NextDouble();
                }
            }
        }

        public void Export(String filename)
        {
            String[] toExport = new string[]
            {
                weightsLayer1.ToString(),
                weightsLayer2.ToString(),
            };
            File.WriteAllLines(filename, toExport);
        }

        public void Import(String filename)
        {
            String[] toImport = File.ReadAllText(filename).Split('\n');

            List<String[]> Layer1Strings = new List<string[]>();
            List<String[]> Layer2Strings = new List<string[]>();

            for (int i = 0;i < toImport.Length-2;i++)
            {
                String line =  toImport[i] ;

                if (line.Equals("\r"))
                {
                    for (int j = i + 1; j < toImport.Length - 2; j++)
                    {
                        line = toImport[j];
                        String[] lineItems2 = null;
                        if (line.Contains("\r"))
                        {
                            lineItems2 = line.Replace(" \r", "").Split(' ');
                        }

                        Layer2Strings.Add(lineItems2);
                    }
                    break;
                }

                String[] lineItems = null;
                if (line.Contains("\r"))
                {
                    lineItems = line.Replace(" \r", "").Split(' ');
                }
                
                Layer1Strings.Add(lineItems);
                
            }

            weightsLayer1 = new Matrix(Layer1Strings.Count, Layer1Strings[0].Length);

            int r = weightsLayer1.Rows;
            int c = weightsLayer1.Cols;

            for (int i = 0; i < r; i++)
            {
                for (int j = 0; j < c; j++)
                {
                    weightsLayer1[i, j] = double.Parse(Layer1Strings[i][j]);
                }
            }

            weightsLayer2 = new Matrix(Layer2Strings.Count, Layer2Strings[0].Length);
            r = weightsLayer2.Rows;
            c = weightsLayer2.Cols;
            for (int i = 0; i < r; i++)
            {
                for (int j = 0; j < c; j++)
                {
                    weightsLayer2[i, j] = double.Parse(Layer2Strings[i][j]);
                }
            }
        }

        public void Train(List<double[]> trainData, List<double> trainExpected, int epochs = 100, double learningRate = 0.5) {
            for (int i = 0; i<epochs; i++)
            {
                Console.WriteLine(i);
                for (int batch = 0; batch< trainData.Count; batch++)
                {
                    double[] input = trainData[batch];
                    double[] output = new double[] { trainExpected[batch] };
                    Matrix desiredOutputMatrix = Matrix.CreateRowMatrix(output);
                    
                    Matrix Layer0 = Matrix.CreateRowMatrix(input);

                    Matrix Layer1 = MatrixMath.Sigmoid(MatrixMath.Multiply(Layer0, weightsLayer1));
                    Matrix Layer2 = MatrixMath.Sigmoid(MatrixMath.Multiply(Layer1, weightsLayer2));

                    Matrix error = MatrixMath.Pow(MatrixMath.Subtract(Layer2, desiredOutputMatrix), 2);

                    Matrix Layer2Delta = MatrixMath.ElementWiseMultiplication(MatrixMath.Subtract(Layer2, desiredOutputMatrix),
                        MatrixMath.SigmoidDerivative(Layer2));

                    Matrix Layer1Delta = MatrixMath.ElementWiseMultiplication(
                        MatrixMath.Multiply(Layer2Delta, MatrixMath.Transpose(weightsLayer2)),
                        MatrixMath.SigmoidDerivative(Layer1));

                    Matrix weights2Delta = MatrixMath.Multiply(MatrixMath.Transpose(Layer1), Layer2Delta);
                    Matrix weights1Delta = MatrixMath.Multiply(MatrixMath.Transpose(Layer0), Layer1Delta);

                    weightsLayer1 = MatrixMath.ElementWiseSubtraction(weightsLayer1,
                        MatrixMath.Multiply(weights1Delta, learningRate));
                    weightsLayer2 = MatrixMath.ElementWiseSubtraction(weightsLayer2,
                        MatrixMath.Multiply(weights2Delta, learningRate));
                }
            }
        }
        
        public Matrix Predict(Matrix testInput)
        {
            Matrix Layer1FF = MatrixMath.Sigmoid(MatrixMath.Multiply(testInput, weightsLayer1));
            Matrix Layer2FF = MatrixMath.Sigmoid(MatrixMath.Multiply(Layer1FF, weightsLayer2));
            return Layer2FF;
        }

    }
}
