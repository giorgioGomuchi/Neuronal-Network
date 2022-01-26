
#region Includes
using System;
#endregion

// Clase que representa una sola capa de una red neuronal "alimentada" totalmente 
public class NeuralLayer
{
    #region Members
    private static Random randomizer = new Random();

    // Delegado que representa la función de activación de una neurona artificial.
    public delegate double ActivationFunction(double xValue);

    // La función de activación utilizada por las neuronas de esta capa.
    public ActivationFunction NeuronActivationFunction = MathHelper.SigmoidFunction;

    // La cantidad de neuronas en esta capa.
    public uint NeuronCount
    {
        get;
        private set;
    }

    // La cantidad de neuronas a las que está conectada esta capa, es decir, la cantidad de neuronas de la siguiente capa.
    public uint OutputCount
    {
        get;
        private set;
    }

    // Los pesos de las conexiones de esta capa a la siguiente capa.
    //Por ejemplo, el peso [i, j] es el peso de la conexión desde el peso i-th de esta capa al peso j-th de la siguiente capa.
    public double[,] Weights
    {
        get;
        private set;
    }
    #endregion

    #region Constructors
    // Inicializa una nueva capa neural para una red neuronal de alimentación directa totalmente 
    //conectada con una cantidad dada de nodo y con conexiones a la cantidad dada de nodos de la siguiente capa.
    public NeuralLayer(uint nodeCount, uint outputCount)
    {
        this.NeuronCount = nodeCount;
        this.OutputCount = outputCount;

        Weights = new double[nodeCount + 1, outputCount]; // + 1 for bias node
    }
    #endregion

    #region Methods
    // Establece los pesos de esta capa a los valores dados.
    // Weights: los valores para establecer los pesos de las conexiones de esta capa a la siguiente.
    // Param:Los valores están ordenados en orden neuronal. Por ejemplo, en una capa con dos neuronas con una capa siguiente de tres neuronas
    // los valores [0-2] son ​​los pesos de la neurona 0 de esta capa a las neuronas 0-2 de la siguiente capa respectivamente y
    /// los valores [3-5] son ​​los pesos de la neurona 1 de esta capa a las neuronas 0-2 de la siguiente capa, respectivamente.
    public void SetWeights(double[] weights)
    {
        // Verificar argumentos
        if (weights.Length != this.Weights.Length)
            throw new ArgumentException("Input weights do not match layer weight count.");

        // Copiar pesos de una matriz de valores dada
        int k = 0;
        for (int i = 0; i < this.Weights.GetLength(0); i++)
            for (int j = 0; j < this.Weights.GetLength(1); j++)
                this.Weights[i, j] = weights[k++];
    }

    // Procesa las entradas dadas utilizando los pesos actuales para la siguiente capa.
    public double[] ProcessInputs(double[] inputs)
    {
        // Verificar argumentos
        if (inputs.Length != NeuronCount)
            throw new ArgumentException("Given xValues do not match layer input count.");

        // Calcular la suma de cada neurona a partir de entradas ponderadas
        double[] sums = new double[OutputCount];
        // Añadir neurona sesgada (siempre activada) a las entradas
        double[] biasedInputs = new double[NeuronCount + 1];
        inputs.CopyTo(biasedInputs, 0);
        biasedInputs[inputs.Length] = 1.0;

        for (int j = 0; j < this.Weights.GetLength(1); j++)
            for (int i = 0; i < this.Weights.GetLength(0); i++)
                sums[j] += biasedInputs[i] * Weights[i, j];

        // Aplica la función de activación a la suma, si está configurado
        if (NeuronActivationFunction != null)
        {
            for (int i = 0; i < sums.Length; i++)
                sums[i] = NeuronActivationFunction(sums[i]);
        }

        return sums;
    }

    // Copia este NeuralLayer incluyendo sus pesos
    public NeuralLayer DeepCopy()
    {
        // Copiar pesos
        double[,] copiedWeights = new double[this.Weights.GetLength(0), this.Weights.GetLength(1)];

        for (int x = 0; x < this.Weights.GetLength(0); x++)
            for (int y = 0; y < this.Weights.GetLength(1); y++)
                copiedWeights[x, y] = this.Weights[x, y];

        // Crear copia
        NeuralLayer newLayer = new NeuralLayer(this.NeuronCount, this.OutputCount);
        newLayer.Weights = copiedWeights;
        newLayer.NeuronActivationFunction = this.NeuronActivationFunction;

        return newLayer;
    }

    // Establece los pesos de la conexión desde esta capa a la próxima a valores aleatorios en un rango dado.
    //"minValue" El valor mínimo en el que se puede establecer un peso.
    //"maxValue"> El valor máximo en el que se puede establecer un peso.
    public void SetRandomWeights(double minValue, double maxValue)
    {
        double range = Math.Abs(minValue - maxValue);
        for (int i = 0; i < Weights.GetLength(0); i++)
            for (int j = 0; j < Weights.GetLength(1); j++)
                Weights[i, j] = minValue + (randomizer.NextDouble() * range);  // double aleatorio entre minValue y maxValue
    }

    // Devuelve una cadena que representa los pesos de conexión de esta capa.
    public override string ToString()
    {
        string output = "";

        for (int x = 0; x < Weights.GetLength(0); x++)
        {
            for (int y = 0; y < Weights.GetLength(1); y++)
                output += "[" + x + "," + y + "]: " + Weights[x, y];

            output += "\n";
        }

        return output;
    }
    #endregion
}