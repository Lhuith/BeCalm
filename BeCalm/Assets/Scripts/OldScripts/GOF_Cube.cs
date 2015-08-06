using UnityEngine;
using System.Collections;

public class GOF_Cube : MonoBehaviour {
	public int type;
	public MeshRenderer meshRend;
	public ChildRenderer childRend;
	public int futureType;
	public SpectrumAnalyzer specAny;


	void Start () {

		//childRend = this.transform.GetComponentInChildren<ChildRenderer>();
		specAny = Camera.main.GetComponent<SpectrumAnalyzer>();
		//meshRend = GetComponent<MeshRenderer>();
	}



	void Update () {
		type = futureType;
		if (type == 0) {
			transform.GetComponent<MeshRenderer>().enabled = false;
				//childRend.meshRend.enabled = false;
		}
		if (type == 1) {
			transform.GetComponent<MeshRenderer>().enabled = true;
				//childRend.meshRend.enabled = true;
		}
	}

	public void Funkup(){
		if (this.gameObject.name != "Cell") {
			this.gameObject.name = "Cell";
			this.gameObject.tag = "funkCell";
		}
	}
void OnTriggerEnter( Collider col){
		if (col.gameObject.tag == "Cubes") {
			//childRend.meshRend.enabled = true;
			type = 0;
			futureType = 0;
			if (col.GetComponent<isActive> ().active == true) {
				if (futureType == 1 || type == 1) {
					//futureType = 0;
				} else {
					//futureType = 1;
					//transform.GetComponent<MeshRenderer>().material.color = col.GetComponent<MeshRenderer>().material.color;
					if(this.gameObject.name != "Cell"){
					this.gameObject.name = "Cell";
					this.gameObject.tag = "funkCell";
				}
					}
			} else {
				//futureType = 0;
			}
		}
		}

	void OnTriggerStay(Collider col){
		if (col.gameObject.tag == "Cubes") {
			type = 0;
			futureType = 0;
			if (col.GetComponent<isActive> ().active == true) {
				//type = 0;
			}else if (col.GetComponent<isActive> ().active == false) {
				//futureType = 0;
			}
		}
	}

	void OnTriggerExit( Collider col){
		if (col.gameObject.tag == "Cubes") {
			//childRend.meshRend.enabled = false;
			if (col.GetComponent<isActive> ().active == true) {
				futureType = 1;
			}else if (col.GetComponent<isActive> ().active == false) {
				//futureType = 0;
			}
		}
	}
	}
