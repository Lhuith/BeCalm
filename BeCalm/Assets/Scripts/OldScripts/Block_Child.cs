using UnityEngine;
using System.Collections;

public class Block_Child : MonoBehaviour {
	public MeshRenderer childRend, parRend;
	public Transform chilParTran;
	// Use this for initialization
	void Start () {
		childRend = GetComponent<MeshRenderer> ();
		parRend = GetComponentInParent<MeshRenderer> ();
		chilParTran = this.gameObject.transform.parent;
		parRend = chilParTran.GetComponent<MeshRenderer> ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		childRend.material.color = parRend.material.color;
	}
}
