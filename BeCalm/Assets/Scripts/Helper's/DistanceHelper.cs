using UnityEngine;
using System.Collections;
using System;

public class DistanceHelper : IComparable<DistanceHelper> {
		//the distance to water
		public float distance;
		//we also need to store the name so we can form clockwise traingles
		public string name;
		//the Vector3 position of the vertices
		public Vector3 verticePos;

		public int CompareTo(DistanceHelper other)
	{
		return this.distance.CompareTo (other.distance);
	}
}
