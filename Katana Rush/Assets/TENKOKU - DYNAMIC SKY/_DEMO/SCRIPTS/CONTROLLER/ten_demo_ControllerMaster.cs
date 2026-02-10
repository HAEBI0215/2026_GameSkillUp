using UnityEngine;
using System.Collections;

public class ten_demo_ControllerMaster : MonoBehaviour {

	
	public enum Ten_Demo_ControllerType{
			none,character,boat,camera
			}

	public Transform cameraObject;
	public Ten_Demo_ControllerType currentControllerType = Ten_Demo_ControllerType.character;

	private ten_demo_InputController IC;
	private bool resetController = false;
	private Ten_Demo_ControllerType useController;// = currentControllerType;


	void Start(){
		IC = this.gameObject.GetComponent<ten_demo_InputController>() as ten_demo_InputController;
	}



	void LateUpdate(){

		//check for mode switch
		if (IC.inputKeySHIFTL && IC.inputKey1){
			currentControllerType = Ten_Demo_ControllerType.camera;
		}

		//check for reset
		if (currentControllerType != useController){
			resetController = true;
		} else {
			resetController = false;
		}

		//reset
		if (resetController){
			resetController = false;
			useController = currentControllerType;
		}


	}

}
