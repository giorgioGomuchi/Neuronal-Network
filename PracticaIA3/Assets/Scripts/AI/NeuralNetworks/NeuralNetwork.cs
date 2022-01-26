
#region Includes
using System;
#endregion

// Clase que representa una red neuronal "alimentada" totalmente conectada.
public class NeuralNetwork
{
    #region Members
    // Las capas neurales individuales de esta red.
    public NeuralLayer[] Layers
    {
        get;
        private set;
    }

    // Una matriz de enteros sin signo que representa el recuento de nodos de cada capa de la red desde la capa de entrada hasta la de salida.
    public uint[] Topology
    {
        get;
        private set;
    }

    // La cantidad de pesos totales de las conexiones de esta red.
    public int WeightCount
    {
        get;
        private set;
    }
    #endregion

    // Inicializa una nueva red neuronal de alimentación directa totalmente conectada con una topología dada.
    //"topology" Una matriz de enteros sin signo que representa el recuento de nodos de cada capa desde la capa de entrada a la de salida.
    public NeuralNetwork(params uint[] topology)
    {
        this.Topology = topology;

        // Calcular el conteo de peso total
        WeightCount = 0;
        for (int i = 0; i < topology.Length - 1; i++)
            WeightCount += (int)((topology[i] + 1) * topology[i + 1]); // + 1 for bias node

        // Inicializar capas
        Layers = new NeuralLayer[topology.Length - 1];
        for (int i = 0; i < Layers.Length; i++)
            Layers[i] = new NeuralLayer(topology[i], topology[i + 1]);
    }

    #region Methods
    // Procesa las entradas dadas utilizando los pesos de la red actual.
    public double[] ProcessInputs(double[] inputs)
    {
        // Verificar argumentos
        if (inputs.Length != Layers[0].NeuronCount)
            throw new ArgumentException("Given inputs do not match network input amount.");

        // Procesar entradas propagando valores a través de todas las capas
        double[] outputs = inputs;
        foreach (NeuralLayer layer in Layers)
            outputs = layer.ProcessInputs(outputs);

        return outputs;

    }

    // Establece los pesos de esta red en valores aleatorios en un rango dado.
    //  "minValue" El valor mínimo en el que se puede establecer un peso. </param>
    // "maxValue"El valor máximo en el que se puede establecer un peso
    public void SetRandomWeights(double minValue, double maxValue)
    {
        if (Layers != null)
        {
            foreach (NeuralLayer layer in Layers)
                layer.SetRandomWeights(minValue, maxValue);
        }
    }

    // Devuelve una nueva instancia de NeuralNetwork con las mismas funciones de topología y activación, 
    //pero los pesos se establecen en su valor predeterminado.
    public NeuralNetwork GetTopologyCopy()
    {
        NeuralNetwork copy = new NeuralNetwork(this.Topology);
        for (int i = 0; i < Layers.Length; i++)
            copy.Layers[i].NeuronActivationFunction = this.Layers[i].NeuronActivationFunction;

        return copy;
    }

    // Copia esta NeuralNetwork incluyendo su topología y pesos.
    public NeuralNetwork DeepCopy()
    {
        NeuralNetwork newNet = new NeuralNetwork(this.Topology);
        for (int i = 0; i < this.Layers.Length; i++)
            newNet.Layers[i] = this.Layers[i].DeepCopy();

        return newNet;
    }

    /// Devuelve una cadena que representa esta red en orden de capas.
    public override string ToString()
    {
        string output = "";

        for (int i = 0; i < Layers.Length; i++)
            output += "Layer " + i + ":\n" + Layers[i].ToString();

        return output;
    }
    #endregion
}
