using UnityEngine;
using System.Collections;

/// <summary>
/// Will handle camera movement, zoom , panning.
/// </summary>
public class cam_control : MonoBehaviour {

	public int movementSpeed;
	private GameObject camera;

	void Start()
	{
		camera = transform.FindChild("Main Camera").gameObject;
	}

	void FixedUpdate() 
	{

		//--KEYBOARD--

		//Forward - Backward.
		if(Input.GetKey(info_input.iForward))
			moveForward();
		else if(Input.GetKey(info_input.iBackward))
			moveBackward();

		//Left    - Right.
		if(Input.GetKey(info_input.iLeft))
			moveLeft();
		else if(Input.GetKey(info_input.iRight))
			moveRight();




		//Pan cam around.
		if(Input.GetKey(info_input.iCamPan))
		{
			panMode();
		}
		else
		{
			//--MOUSE--

			if(Input.mousePosition.x<=Screen.width/24)
			{
				moveLeft();
			}
			else if(Input.mousePosition.x>=Screen.width-Screen.width/24)
			{
				moveRight();
			}
			
			if(Input.mousePosition.y<=Screen.height/24)
			{
				moveBackward();
			}
			else if(Input.mousePosition.y>=Screen.height-Screen.height/24)
			{
				moveForward();
			}
		}



		//Zoom with mouse wheel.

		RaycastHit rchit;
		if(Physics.Raycast(transform.position,-Vector3.up,out rchit,100))
		{
			if(rchit.distance>5)
				zoomDown();
		}

		if(transform.position.y<50f)
			zoomUp();
	}


	void moveForward()
	{
		if(!Physics.Raycast(transform.position,transform.forward,5))
			transform.position+=transform.forward*movementSpeed*Time.deltaTime;
	}

	void moveBackward()
	{
		if(!Physics.Raycast(transform.position,-transform.forward,5))
			transform.position+=-transform.forward*movementSpeed*Time.deltaTime;
	}

	void moveLeft()
	{
		if(!Physics.Raycast(transform.position,-transform.right,5))
			transform.position+=-transform.right*movementSpeed*Time.deltaTime;
	}

	void moveRight()
	{
		if(!Physics.Raycast(transform.position,transform.right,5))
			transform.position+=transform.right*movementSpeed*Time.deltaTime;
	}

	void panMode()
	{
		transform.Rotate(Vector3.up*Input.GetAxis("Mouse X")*info_input.mouseSens*Time.deltaTime , Space.World);

		//camera.transform.Rotate(camera.transform.right*-Input.GetAxis("Mouse Y")*info_input.mouseSens*Time.deltaTime,Space.Self);

		if(camera.transform.localRotation.x>0 && Input.GetAxis("Mouse Y")>0)
			camera.transform.RotateAround(camera.transform.position,camera.transform.right,-Input.GetAxis("Mouse Y")*info_input.mouseSens*Time.deltaTime);

		if(camera.transform.localRotation.x<0.6f && Input.GetAxis("Mouse Y")<0)
			camera.transform.RotateAround(camera.transform.position,camera.transform.right,-Input.GetAxis("Mouse Y")*info_input.mouseSens*Time.deltaTime);

	}

	void zoomUp()
	{
		if(Input.GetAxis("Mouse ScrollWheel")>0)
			transform.position+=transform.up*movementSpeed*1*5*Time.deltaTime;
	}

	void zoomDown()
	{
		if(Input.GetAxis("Mouse ScrollWheel")<0)
			transform.position+=transform.up*movementSpeed*-1*5*Time.deltaTime;
	}
}
