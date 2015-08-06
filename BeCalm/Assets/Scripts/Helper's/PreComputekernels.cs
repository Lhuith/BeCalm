using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class PreComputekernels : MonoBehaviour{
	//P = kernel size
	//6 is the smallest value tat gives water-like motion
	public static int P = 8;
	//Should be the neg or the waves will movein the wrong direction
	public static float g = -9.81f;
	public static float [,] storedKernelArray;

	public static void ComputeKernel(){

	storedKernelArray = new float[P * 2 + 1, P * 2 + 1];
	PrecomputateKernalValues ();
	}

	//Precomputate the kernel values G(k,1)
	public static void PrecomputateKernalValues(){
		float G_zero = CalculateG_zero ();
		
		//P - kernel size
		for (int k = -P; k <= P; k++) 
		{
			for(int l = -P; l <= P; l++)
			{
				//Need "+ P" becuase we itrate from -P and not 0, which is how they are stored in the array
				storedKernelArray[k + P, l + P] =  CalculateG((float)k,(float)l, G_zero);
			}
		}
	}
	
	
	//G(k,l)
	public static float CalculateG(float k, float l, float G_zero)
	{
		float delta_q = 0.001f;
		float sigma = 1f;
		float r = Mathf.Sqrt (k * k + l * l);
		
		float G = 0f;
		
		for (int n = 1; n <= 10000; n++) 
		{
			float q_n = ((float)n * delta_q);
			float q_n_square = q_n * q_n;
			
			G += q_n_square * Mathf.Exp(-sigma * q_n_square) * BesselFunction(q_n * r);
		}
		
		G /= G_zero;
		
		return G;
	}
	
	public static float CalculateG_zero()
	{
		float delta_q = 0.001f;
		float sigma = 1f;
		
		float G_zero = 0f;
		for (int n = 1; n <= 10000; n++) 
		{
			float q_n_square = ((float) n * delta_q) * ((float)n * delta_q);
			
			G_zero += q_n_square * Mathf.Exp(-sigma * q_n_square);
		}
		
		return G_zero;
	}
	
	public static float BesselFunction(float x)
	{
		//from http://people.math.sfu.ca/~cbm/aands/frameindex.htm
		//page 369-370
		
		float J_zero_of_X = 0f;
		
		//Test to see if the bessel functions are valid
		//Has to be above -3
		if (x <= -3f) 
		{
			Debug.Log("Smaller");
		}
		
		//9.4.1
		//-3 <= x <= 3
		if (x <= 3f) 
		{
			//Ignored the small rest term at the end
			J_zero_of_X =
				1f -
					2.2499997f * Mathf.Pow (x/ 3f, 2f) +
					1.2656208f * Mathf.Pow (x/ 3f, 4f) -
					0.3163866f * Mathf.Pow (x/ 3f, 6f) +
					0.0444479f * Mathf.Pow (x/ 3f, 8f) -
					0.0038444f * Mathf.Pow (x/ 3f, 10f) +
					0.0002100f * Mathf.Pow (x/ 3f, 12f) ;
		}
		//9.4.3
		//3 <= x <= infinity
		else
		{
			//Ignored the small rest term at the end
			float f_zero =
				0.79788456f -
					0.00000077f * Mathf.Pow (3f/ x, 1f) -
					0.00552740f * Mathf.Pow (3f/ x, 2f) -
					0.00009512f * Mathf.Pow (3f/ x, 3f) -
					0.00137237f * Mathf.Pow (3f/ x, 4f) -
					0.00072805f * Mathf.Pow (3f/ x, 5f) +
					0.00014476f * Mathf.Pow (3f/ x, 6f) ;
			
			
			//Ignored the small rest term at the end
			float theta_zero =
				x -
					0.78539816f -
					0.04166397f * Mathf.Pow (3f/ x, 1f) -
					0.00003954f * Mathf.Pow (3f/ x, 2f) -
					0.00262573f * Mathf.Pow (3f/ x, 3f) -
					0.00054125f * Mathf.Pow (3f/ x, 4f) -
					0.00029333f * Mathf.Pow (3f/ x, 5f) +
					0.00013558f * Mathf.Pow (3f/ x, 6f) ;
			
			//Should be cos and not acos
			J_zero_of_X = Mathf.Pow(x, -1f/3f) * f_zero * Mathf.Cos (theta_zero);
		}
		
		return J_zero_of_X;
	}
}
