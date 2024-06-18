using MathNet.Numerics.LinearAlgebra;
using MatrixLibrary.Matrix;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

//Info do sprawozdania:

//Żeby nie pokazywało konsoli: properties -> Output type = Windows Application
//Zajmuje port, więc nie może działać proces NNDetector
//Zrobione na klasycznej sieci neuronowej od zera.
//3 warstwy, 11 neuronów na wejściu, 22 ukryte, 1 na wyjściu
//Funkcja aktywacji: Sigmoid
//Operacje macierzowe i wzór sieci: by vanco, https://devindeep.com/neural-network-with-c-from-scratch/
//Sieć typu Feed Forward z propagacją wstecz
//Dataset edytowałem, -1 zamieniłem na 0; (z powodu sieci działającej w zakresie 0-1 bo łatwiej, 
//a na jedno wychodzi w sumie(dużo ostrzej wyklucza)



namespace NNDetector
{
    class Program
    {
        static void Main(string[] args)
        {
            //To jest  kod do obsługi wtyczki
            //------------------------------------------------------------------------------

            //NeuralNetwork NN = new NeuralNetwork(11, 22, 1);
            //NN.Import("C:\\Program Files\\ThunderbirdDetector\\Weights");
            //TcpListener listener = new TcpListener(System.Net.IPAddress.Any, 8080);
            //listener.Start();

            //while (true)
            //{
            //    TcpClient tcp = listener.AcceptTcpClient();

            //    tcp.ReceiveBufferSize = 256;

            //    Console.WriteLine("Receiving string list ...");
            //    int received_bytes_count = -1;
            //    byte[] total_buffer = new byte[1024]; //Expected total bytes in a string list
            //    while (received_bytes_count != 0)
            //    {
            //        int total_buffer_index = received_bytes_count + 1;

            //        NetworkStream ns = tcp.GetStream();
            //        byte[] buffer = new byte[tcp.ReceiveBufferSize];
            //        received_bytes_count = ns.Read(buffer, 0, buffer.Length);

            //        buffer.CopyTo(total_buffer, total_buffer_index);
            //        List<string> list = new List<string>();
            //        list.AddRange(Encoding.ASCII.GetString(total_buffer).Split(','));

            //        // Model work on links here
            //        foreach (string s in list.ToArray())
            //            Console.WriteLine(s);

            //        List<double> response = new List<double>();
            //        foreach (string adresURL in list.ToArray())
            //        {
            //            double[] opis = PomocneFunkcje.opisz_adres_liczbami(adresURL);
            //            Matrix testInput = Matrix.CreateRowMatrix(opis);
            //            Matrix result = NN.Predict(testInput);
            //            ////float wynikowy_label = whole_output.PredictedLabel;
            //            var res = 0;
            //            if (double.Parse(result.ToString()) < 0.33)
            //            {
            //                res = -1;
            //            }
            //            else if(double.Parse(result.ToString()) < 0.66)
            //            {
            //                res = 0;
            //            }
            //            else
            //            {
            //                res = 1;
            //            }
            //            response.Add(res);
            //            Console.WriteLine(result.ToString());
            //        }

            //        //
            //        foreach (string s in list.ToArray())
            //            Console.WriteLine(s);

            //        //sending response
            //        String text_to_send = "";
            //        if (response != null)
            //        {
            //            foreach (int cl in response)
            //            {
            //                text_to_send += cl.ToString() + ',';
            //            }
            //        }
            //        else
            //        {
            //            text_to_send = "err";
            //        }
            //        byte[] bytes_to_send = ASCIIEncoding.ASCII.GetBytes(text_to_send);
            //        ns.Write(bytes_to_send, 0, bytes_to_send.Length);
            //    }
            //    tcp.Close();

            //------------------------------------------------------------------------------

            //To jest kod do trenowania i testowania sieci neuronowej

            //------------------------------------------------------------------------------

            List<Double[]> inputData = new List<Double[]>();
            using (var rd = new StreamReader("dataset.csv"))
            {
                var flag = 0;
                while (!rd.EndOfStream)
                {
                    String[] line = rd.ReadLine().Split(',');
                    if (flag == 0)
                    {
                        flag = 1;
                        continue;
                    }

                    var hasIPAddress = double.Parse(line[1]);
                    var longURL = double.Parse(line[2]);
                    var shortenedURL = double.Parse(line[3]);
                    var hasAtSymbol = double.Parse(line[4]);
                    var doubleSlashRedirect = double.Parse(line[5]);
                    var dashSymbol = double.Parse(line[6]);
                    var hasSubdomain = double.Parse(line[7]);
                    var https = double.Parse(line[8]);
                    var port = double.Parse(line[11]);
                    var httpsAfter = double.Parse(line[12]);
                    var mailTo = double.Parse(line[17]);

                    var result = double.Parse(line[31]);

                    List<double> dataLine = new List<double>()
                                {
                                    hasIPAddress,longURL,shortenedURL,hasAtSymbol,doubleSlashRedirect,dashSymbol,hasSubdomain,https,port,httpsAfter,mailTo,result
                                };
                    inputData.Add(dataLine.ToArray());
                }
            }







            //Split into train and test
            List<Double[]> trainData = null;
            List<Double[]> testData = null;
            var ratio = 0.8;
            splitDataset(inputData, ref trainData, ref testData, ratio);

            List<Double[]> outTrainTraits = null;
            List<Double> outTrainClass = null;
            separateDataset(trainData, ref outTrainTraits, ref outTrainClass);

            List<Double[]> outTestraits = null;
            List<Double> outTestClass = null;
            separateDataset(testData, ref outTestraits, ref outTestClass);
            //Train Network
            //11 neuronów na wejściu(bo tyle cech), 22 neurony ukryte, 1 neuron na wyściu
            //0-0.33 - niebezpieczny
            //0.33-0.66 - podejrzany
            //0.66-1 - bezpieczny
            NeuralNetwork NN = new NeuralNetwork(11, 22, 1);
            NN.Import("Weights");
            //trenuje długo bo jet coś koło 18k wpisów
            //NN.Train(outTrainTraits, outTrainClass, 1000);

            List<Matrix> results = new List<Matrix>();
            for (int i = 0; i < outTestraits.Count; i++)
            {
                Matrix testInput = Matrix.CreateRowMatrix(outTestraits[i]);
                Matrix result = NN.Predict(testInput);
                results.Add(result);
                Console.WriteLine(String.Format("Output: {0} Expected: {1}", result.ToString(), outTestClass[i].ToString()));
            }
            double acc = 0;
            var res = 0;
            for (int i = 0; i < results.Count; i++)
            {
                //do doboru próg, ale inaczej do wtyczki jest, out sieci jest 0-1,
                //na ifach sie opiera pod wtyczke żeby tylko zmieniać program od sieci.
                if (double.Parse(results[i].ToString()) > 0.6)
                {
                    res = 1;
                }
                else
                {
                    res = 0;
                }

                if (res == outTestClass[i])
                {
                    acc++;
                }
            }



            //Accuracy test
            //~87% ACC
            Console.WriteLine("ACC:");
            Console.WriteLine(acc / results.Count * 100);

            //Confusion Matrix
            //do sprawozdania
            double[,] confMatrix = new double[2, 2];
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    confMatrix[i, j] = 0;
                }
            }

            for (int i = 0; i < results.Count; i++)
            {
                if (double.Parse(results[i].ToString()) > 0.6)
                {
                    res = 1;
                }
                else
                {
                    res = 0;
                }

                if (res == outTestClass[i])
                {
                    if (res == 1)
                    {
                        confMatrix[0, 0] += 1;
                    }
                    else
                    {
                        confMatrix[1, 1] += 1;
                    }
                }
                else
                {
                    if (res == 1)
                    {
                        confMatrix[1, 0] += 1;
                    }
                    else
                    {
                        confMatrix[0, 1] += 1;
                    }
                }
            }

            Console.WriteLine("Confusion Matrix");
            String cm = "";
            for (int i = 0; i < 2; i++)
            {
                cm = "";
                for (int j = 0; j < 2; j++)
                {
                    cm = cm + confMatrix[i, j].ToString() + ", ";
                }
                Console.WriteLine(cm);
            }



            //export Weights
            //musi być w folderze z programem
            //NN.Export("Weights");


            //stopuje konsole
            string userName = Console.ReadLine();

        }

        private static void separateDataset(List<Double[]> input, ref List<double[]> outTraits, ref List<double> outClass)
        {
            outTraits = new List<double[]>();
            outClass = new List<double>();
            for (int i = 0; i < input.Count; i++)
            {
                double[] doubles = new double[11];
                Array.Copy(input[i], 0, doubles, 0, 11);

                outTraits.Add(doubles);

                outClass.Add(input[i][11]);
            }
        }

        private static void splitDataset(List<double[]> inputData, ref List<double[]> trainData, ref List<double[]> testData, double ratio)
        {
            trainData = new List<double[]>();
            testData = new List<double[]>();

            for (int i = 0; i < inputData.Count; i++)
            {
                if (i < inputData.Count * ratio)
                {
                    trainData.Add(inputData[i]);
                }
                else
                {
                    testData.Add(inputData[i]);
                }
            }
        }
        //------------------------------------------------------------------------------
    }
}
