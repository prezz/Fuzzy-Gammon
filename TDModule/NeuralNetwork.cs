using System;
using System.Collections;

namespace TDModule
{
	struct Neuron
	{
		public float[]	Weights;
		public float	Output;
	
	}


	class NeuralNetworkLayer
	{
		public Neuron[]			Nodes;
		private static Random	m_WeightGenerator = new Random();

		public NeuralNetworkLayer( int dimension, int neuronInputs )
		{
			Nodes = new Neuron[dimension];
			for ( int i = 0; i < dimension; i++ )
			{
				Nodes[i].Weights = new float[neuronInputs];
				for ( int j = 0; j < neuronInputs; j++ )
				{
					Nodes[i].Weights[j] = ( float )m_WeightGenerator.Next( -1000, 1001 ) / ( float )2000;
				}
			}
		}
	}


	class NeuralNetwork
	{
		private int	m_Inputs;
		private int	m_Hiddens;
		private int m_Outputs;

		private NeuralNetworkLayer	m_HiddenLayer;
		private NeuralNetworkLayer	m_OutputLayer;

		public NeuralNetwork( int inputs, int hiddens, int outputs )
		{
			m_Inputs = inputs;
			m_Hiddens = hiddens;
			m_Outputs = outputs;

			m_HiddenLayer = new NeuralNetworkLayer( hiddens, inputs );
			m_OutputLayer = new NeuralNetworkLayer( outputs, hiddens);
		}

		public int LayerSize( int layer )
		{
			switch ( layer )
			{
				case ( 0 ):
					return m_Inputs;
				case ( 1 ):
					return m_Hiddens;
				case ( 2 ):
					return m_Outputs;
				default:
					return m_Inputs + m_Hiddens + m_Outputs;
			}
		}

		public float[] GetWeights()
		{
			ArrayList result = new ArrayList();

			for ( int i = 0; i < m_Hiddens; i++ )
				for ( int j = 0; j < m_HiddenLayer.Nodes[i].Weights.Length; j++ )
					result.Add( m_HiddenLayer.Nodes[i].Weights[j] );

			for ( int i = 0; i < m_Outputs; i++ )
				for ( int j = 0; j < m_OutputLayer.Nodes[i].Weights.Length; j++ )
					result.Add( m_OutputLayer.Nodes[i].Weights[j] );
		
			return ( float[] )result.ToArray( typeof( float ) );
		}

		public void PutWeights( float[] weights )
		{
			int idx = 0;

			for ( int i = 0; i < m_Hiddens; i++ )
				for ( int j = 0; j < m_HiddenLayer.Nodes[i].Weights.Length; j++ )
					m_HiddenLayer.Nodes[i].Weights[j] = weights[idx++];

			for ( int i = 0; i < m_Outputs; i++ )
				for ( int j = 0; j < m_OutputLayer.Nodes[i].Weights.Length; j++ )
					m_OutputLayer.Nodes[i].Weights[j] = weights[idx++];
		}

		public bool Train( float[] data, float[] target, float max_MeanSquareError, float learningRate )
		{
			bool adjustmentDone = false;

			if ( m_Inputs == 0 || m_Hiddens == 0 || m_Outputs == 0 ) 
				return adjustmentDone;

			float[] output, output_weight_delta, hidden_weight_delta;
			float sum, MSE;
	
			output = new float [m_Outputs];
			output_weight_delta = new float [m_Outputs];
			hidden_weight_delta = new float [m_Hiddens];

			while ( true ) 
			{
				Run( data, output );
		
				//Calculate output layer error terms
				MSE = 0;
				for ( int k = 0; k < m_Outputs; k++ ) 
				{
					output_weight_delta[k] = target[k] - output[k];
					MSE += output_weight_delta[k] * output_weight_delta[k];
					output_weight_delta[k] *= output[k] * ( 1 - output[k] );
				}
		
				if ( MSE < max_MeanSquareError )
					return adjustmentDone;
				else
					adjustmentDone = true;

				//Hidden layer error terms
				for ( int j = 0; j < m_Hiddens; j++ ) 
				{
					sum = 0;
					for ( int k = 0; k < m_Outputs; k++ )
						sum += output_weight_delta[k] * m_OutputLayer.Nodes[k].Weights[j];

					hidden_weight_delta[j] = sum * m_HiddenLayer.Nodes[j].Output * ( 1 - m_HiddenLayer.Nodes[j].Output );
				}

				//Update output weights.
				for ( int k = 0; k < m_Outputs; k++ )
					for ( int j = 0; j < m_Hiddens; j++ )
						m_OutputLayer.Nodes[k].Weights[j] += learningRate * output_weight_delta[k] * m_HiddenLayer.Nodes[j].Output;

				//The hidden weights.
				for ( int j = 0; j < m_Hiddens; j++ )
					for ( int i = 0; i < m_Inputs; i++ )
						m_HiddenLayer.Nodes[j].Weights[i] += learningRate * hidden_weight_delta [j] * data [i];
			}
		}

		public void Run( float[] data, float[] output )
		{
			if ( m_Inputs == 0 || m_Hiddens == 0 || m_Outputs == 0 ) 
				return;

			float sum;
			for ( int j = 0; j < m_Hiddens; j++ ) 
			{
				sum = 0;
				for ( int i = 0; i < m_Inputs; i++ )
					sum += m_HiddenLayer.Nodes[j].Weights[i] * data[i];

				m_HiddenLayer.Nodes[j].Output = Sigmoid( sum );
			}

			for ( int k = 0; k < m_Outputs; k++ ) 
			{
				sum = 0;
				for ( int j = 0; j < m_Hiddens; j++ )
					sum += m_OutputLayer.Nodes[k].Weights[j] * m_HiddenLayer.Nodes[j].Output;

				output[k] = Sigmoid( sum );
			}
		}


		private float Sigmoid( float data )
		{
			return ( 1.0f / ( 1.0f + ( float )Math.Exp( -data ) ) );
		}
	}
}
