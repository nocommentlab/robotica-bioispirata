using Encog.Engine.Network.Activation;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.ML.Train;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Training.Propagation.Resilient;
using System;

namespace XORSolver
{
    class Program
    {
        /* Declares jagged input array */
        public static double[][] XORInput = {
            new[] {0.0, 0.0},
            new[] {1.0, 0.0},
            new[] {0.0, 1.0},
            new[] {1.0, 1.0}
        };

        /* Declares jagged input array */
        public static double[][] XORIdeal = {
            new[] {0.0},
            new[] {1.0},
            new[] {1.0},
            new[] {0.0}
        };

        /* Ciao! Sono un bel commento */
        /// <summary>
        /// Main function
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            /*  
             * Funzione di propagazioni degli errori:
             * 
             * Input: BasicLayer: 2 Neuroni, no biasWeights, no biasActivation 
             * Hidden: BasicLayer: 2 Neuroni, 2 biasWeights, 2 biasActivation 
             * Output: BasicLayer: 1 Neurone, 1 biasWeights, 1 biasActivation 
             * 
             * - Synapse 1: Input to Hidden 
             * Hidden1Activation = (Input1Output * Input1->Hidden1Weight) + (Input2Output * Input2->Hidden1Weight) + (HiddenBiasActivation * Hidden1BiasWeight) 
             * Hidden1Output = calculate(Hidden1Activation, HiddenActivationFunction) 

             * Hidden2Activation = (Input1Output * Input1->Hidden2Weight) + (Input2Output * Input2->Hidden2Weight) + (HiddenBiasActivation * Hidden2BiasWeight) 
             * Hidden2Output = calculate(Hidden2Activation, HiddenActivationFunction) 

             * Synapse 2: Hidden to Output 
             * Output1Activation = (Hidden1Output * Hidden1->Output1Weight) + (Hidden2Output * Hidden2->Output1Weight) + (OutputBiasActivation * Output1BiasWeight) 
             * Output1Output = calculate(Output1Activation, OutputActivationFunction) 
            
             */

            /* La classe BasicNetwork implementa una rete neurale.
             * Questa classe lavora in congiunzione con le classi Layer al fine di creare la struttra della rete neurale.
             * Il primo layer identifica la l'INPUT LAYER, l'ultimo layer identifica l'OUTPUT LAYER. 
             * Tutti i layer aggiunti tra il primo e l'ultimo identificano l'HIDDEN LAYER
             * É di fondamentale importanza richiamare la funzione FinalizeStructure una volta terminata la dichiarazione della struttura della MLP.
             */
            var network = new BasicNetwork();
            /* Il BasicLayer rappresenta le funzioni basilari che ogni MLP deve avere. Essa viene utilizzata per implementare layer FEEDFORWARD o di BACKPROPAGATION*/
            network.AddLayer(new BasicLayer(null, true, 2));
            /* Dichiarazione di un Hidden layer con funzione di attivazione sigmoidea. Da utilizzare solo nel caso si vogliano ottenere valori di output positivi! */
            /* Sigmoid Function:
             * ^
             * 1|               _________
             *  |              /
             *  |             /
             *  |            /
             *  |  _________/
             * 0| ----------------------------->
             */

            /* Il bias consente di spostare la funzione di attivazione basata sulla sigmoide a destra oppure a sinistra.
             * Questa scelta puo' risultare essenziale nella riuscita dell'apprendimento.
             * Si consideri una rete neurale basilare composta da un neurone di input ed uno di output:
             *  ------   w0    ------
             *  | I1 | ------> | O1 |
             *  ------         ------
             * 
             * L'output della rete viene calcolato moltiplicando il valore di input(I1) * il peso(w0) e passa il valore alla funzione di attivazione.
             * Fondamentalmente il valore del bias incrementa la pendenza di salita della sigmoide; quanto piu' e' alto tanto piu' sara' ripido il gradino.
             * 
             */

            network.AddLayer(new BasicLayer(new ActivationSigmoid(), true, 2));
            network.AddLayer(new BasicLayer(new ActivationSigmoid(), true, 1));
            network.Structure.FinalizeStructure();
            /* Ridistribuisce i pesi sinaptici in maniera casuale */
            network.Reset();

            /* Crea il training set */
            IMLDataSet trainingSet = new BasicMLDataSet(XORInput, XORIdeal);

            /* Avvia la fase di training della rete neurale mediante algoritmo FEEDFORWARD in cui il calcolo dell'errore viene propagato al layer successivo. */
            IMLTrain train = new ResilientPropagation(network, trainingSet);

            /* Indice del ciclo di apprendimento */
            int epoch = 1;

            do
            {
                /* Avvia la redistribuzione dei pesi */
                train.Iteration();
                Console.WriteLine(@"Epoch #" + epoch + @" Error:" + train.Error);
                epoch++;
            } while (train.Error > 0.01); /* Itera finche' non viene raggiunto un errore tollerabile */

            /* Test la MLP */
            Console.WriteLine("\r\n+------------------------------------+");
            Console.WriteLine("|Neural Network Results:             |");
            Console.WriteLine("+------------------------------------+");
            foreach (IMLDataPair pair in trainingSet)
            {
                IMLData output = network.Compute(pair.Input);
                Console.WriteLine("Input:" + pair.Input[0] + @" ^ " + pair.Input[1]
                                  + @", actual=" + Math.Round(output[0], 2) + @",ideal=" + pair.Ideal[0]);
            }
            Console.Read();
        }

        
    }
}
