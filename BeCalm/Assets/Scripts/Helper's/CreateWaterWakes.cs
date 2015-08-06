using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[ExecuteInEditMode]
public class CreateWaterWakes : MonoBehaviour {

	//Water wakes parameters
	//Velocity damping term
	//Useful to help suppress numerical instabilities tat can arise
	public static float alpha;
	
	//P = kernel size
	//6 is the smallest value tat gives water-like motion
	public static int P = 8;
	//Should be the neg or the waves will movein the wrong direction
	public static float g;
	
	//Store the precomputed kernel values here
	public static float [,] storedKernelArray;
	
	//Used in the Iwave loop
	public static Vector3[][] height;
	public static Vector3[][] previousHeight;
	public static Vector3[][] verticalDerivative;
	public static Vector3[][] source;
	public static Vector3[][] obstruction;

	//To be able to add ambient waves
	public static Vector3[][] heightDiffrence;
	//Faster to calculate this once
	public static int arrayLength;


	public static void WaterWakes(float [,] _storedKernelArray, float _dt, int _arrayLength, Vector3[][] _height, Vector3[][] _source ,Vector3[][] _heightDiffrence,  Vector3[][] _previousHeight, float _alpha, Vector3[][] _obstruction, float _g ,Vector3[][] _verticalDerivative)
	{

		source = _source;
		arrayLength = _arrayLength;
		height = _height;
		heightDiffrence = _heightDiffrence;
		previousHeight = _previousHeight;
		obstruction = _obstruction;
		g = _g;
		verticalDerivative = _verticalDerivative;
		storedKernelArray = _storedKernelArray;
		alpha = _alpha;
		AddWaterWakes(_dt);
	}

	public static void AddWaterWakes(float dt)
	{
		//if strange gigantic waves happens, adjust alpha
		
		//add sources and obstructions
		for (int j = 0; j < arrayLength; j++) 
		{
			for (int i = 0; i < arrayLength; i++) 
			{
				//Add height from source
				height[j][i].y += source[j][i].y;
				
				//Clear the source
				source[j][i].y = 0f;
				
				//Add obstruction
				height[j][i].y *= obstruction[j][i].y;
			}
		}
		
		//Convolve to update verticalDerivative
		Convolve ();
		
		//Same for all
		float twoMinusAlphaTimesDt = 2f - alpha * dt;
		float onePlusAlphaTimesDt = 1f + alpha * dt;
		float gravityTimesDtTimesDt = g * dt * dt;
		
		for (int j = 0; j < arrayLength; j++) 
		{
			for (int i = 0; i < arrayLength; i++)
			{
				//Faster to do this once
				float currentHeight = height[j][i].y;
				
				//Calculate the new height
				float newHeight = 0f;
				
				newHeight += currentHeight * twoMinusAlphaTimesDt;
				newHeight -= previousHeight[j][i].y;
				newHeight -= gravityTimesDtTimesDt * verticalDerivative[j][i].y;
				newHeight /= onePlusAlphaTimesDt;
				
				//Save the old height
				previousHeight[j][i].y = currentHeight;
				
				//Add the new height
				height[j][i].y = newHeight;
				
				//if wehave ambientwaves we can add them here
				//just replace this with a call to a method where you find the current height of the ambient wave
				//At the current coorndate
				//waveScript.GetWaveYPos(i,j);
				//waveScript.GetWaveYPos(height[j][i].x, height[j][i].z)
				//float heightAmbientWave = waveScript.GetWaveYPos(height[j][i].x, height[j][i].z);
				float heightAmbientWave = 0;
				
				heightDiffrence[j][i].y = heightAmbientWave + newHeight;
				
				//waterMat.SetFloat ("_WaveYPos", heightDiffrence[j][i].y);
				//waterMat.SetFloat ("_WaveZPos", heightDiffrence[j][i].z);
			}
		}
	}
	
	
	//Convolve to update verticalDerivative
	//This might seem unnecessary, but we will save one array if we are doing it before the main loop
	public static void Convolve()
	{
		for (int j = 0; j < arrayLength; j++) 
		{
			for (int i = 0; i < arrayLength; i++)
			{
				float vDeriv = 0f;
				
				//Will include when k, l = 0
				for (int k = -P; k <= P; k++) 
				{
					for(int l = -P; l <= P; l++)
					{
						//Get the precomputed values
						//Need "+ P" becuase we itirate from -P and not 0, which is how they are stored in the array
						
						float kernelValue = storedKernelArray[k + P, l + P];
						
						//Make sure we are within the water
						if(j+k >= 0 && j+k < arrayLength && i+l >= 0 && i+l < arrayLength)
						{
							vDeriv += kernelValue * height[j+k][i+l].y;
						}
						
						//Outside
						else
						{
							//Right
							if(j+k >= arrayLength && i+l >= 0 && i+l < arrayLength)
							{
								vDeriv += kernelValue * height[2 * arrayLength - j - k - 1][i+l].y;
							}
							
							//Top
							if(i+l >= arrayLength && j+k >= 0 && j+k < arrayLength)
							{
								vDeriv += kernelValue * height[j+k][2 * arrayLength - i - l - 1].y;
							}
							
							//Left
							if(j+k < 0 && i+l >= 0 && i+l < arrayLength)
							{
								vDeriv += kernelValue * height[-j-k][i+l].y;
							}
							
							//Bottom
							else if(i+l < 0 && j+k >= 0 && j+k < arrayLength)
							{
								vDeriv += kernelValue * height[j+k][-i-l].y;
							}
						}
					}
					
				}
				
				verticalDerivative[j][i].y = vDeriv;
				
				
				
			}
		}
	}
}
