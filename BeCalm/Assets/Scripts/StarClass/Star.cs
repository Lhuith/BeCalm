using UnityEngine;
using System.Collections;

public class BaseStar : BaseStarClass {

	public BaseStar ()
	{
		StarObject =  Resources.Load("Mesh/GuideStar", typeof(GameObject)) as GameObject;
	}
}
