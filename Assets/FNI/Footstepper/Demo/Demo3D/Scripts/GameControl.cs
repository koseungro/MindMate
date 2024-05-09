
using UnityEngine;
using UnityEngine.SceneManagement;
using GamingIsLove.Footsteps;
using System.Collections.Generic;

public class GameControl : MonoBehaviour
{
	public GameObject thirdPersonPlayer;

	public FootIK footIK;

	public GameObject firstPersonPlayer;


	// cameras
	public GameObject feetCamera;

	public GameObject thirdPersonCamera;


	// hide UI
	public List<GameObject> hideWithUI = new List<GameObject>();


	// GUI
	public Rect cameraButton = new Rect(10, 10, 200, 30);

	public Rect robotButton = new Rect(10, 50, 200, 30);

	public Rect ikButton = new Rect(10, 90, 200, 30);

	public Rect controlTextRect = new Rect(10, 130, 200, 100);

	[TextArea]
	public string controlText = "Move: WASD or arrow keys\n" +
		"Jump: Space\n" +
		"Walk: Shift (hold)";

	public Rect loadSceneButton = new Rect(220, 10, 200, 30);

	public string sceneName = "Footstepper Demo 2D";


	// in-game
	private bool isHide = false;

	private int cameraMode = 0;

	private bool isRobot = false;

	protected virtual void Update()
	{
		if(Input.GetKeyDown(KeyCode.F))
		{
			this.SetCamera(this.cameraMode + 1);
		}
		else if(Input.GetKeyDown(KeyCode.R))
		{
			this.SetRobot(!this.isRobot);
		}
		else if(Input.GetKeyDown(KeyCode.T))
		{
			if(this.footIK != null)
			{
				this.footIK.enableIK = !this.footIK.enableIK;
			}
		}
		else if(Input.GetKeyDown(KeyCode.H))
		{
			this.isHide = !this.isHide;
			for(int i = 0; i < this.hideWithUI.Count; i++)
			{
				if(this.hideWithUI[i] != null)
				{
					this.hideWithUI[i].SetActive(!this.isHide);
				}
			}
		}
	}

	protected virtual void OnGUI()
	{
		if(this.isHide)
			return;

		if(this.cameraMode == 0)
		{
			if(GUI.Button(this.cameraButton, "(F) to Feet Camera"))
			{
				this.SetCamera(1);
			}
		}
		else if(this.cameraMode == 1)
		{
			if(GUI.Button(this.cameraButton, "(F) to 1st Person Camera"))
			{
				this.SetCamera(2);
			}
		}
		else if(this.cameraMode == 2)
		{
			if(GUI.Button(this.cameraButton, "(F) to 3rd Person Camera"))
			{
				this.SetCamera(0);
			}
		}

		if(this.isRobot)
		{
			if(GUI.Button(this.robotButton, "(R) to Normal Effects"))
			{
				this.SetRobot(false);
			}
		}
		else
		{
			if(GUI.Button(this.robotButton, "(R) to Robot Effects"))
			{
				this.SetRobot(true);
			}
		}

		if(this.cameraMode < 2)
		{
			if(this.footIK != null)
			{
				if(this.footIK.enableIK)
				{
					if(GUI.Button(this.ikButton, "(T) to disable IK"))
					{
						this.footIK.enableIK = false;
					}
				}
				else
				{
					if(GUI.Button(this.ikButton, "(R) to enable IK"))
					{
						this.footIK.enableIK = true;
					}
				}
			}
		}

		if(GUI.Button(this.loadSceneButton, "Load 2D Scene"))
		{
			SceneManager.LoadScene(this.sceneName);
		}

		GUI.Label(this.controlTextRect, this.controlText);
	}

	protected void SetCamera(int mode)
	{
		if(mode > 2)
		{
			mode = 0;
		}
		this.cameraMode = mode;

		if(this.cameraMode == 0)
		{
			this.thirdPersonPlayer.transform.SetPositionAndRotation(
				this.firstPersonPlayer.transform.position - new Vector3(0, 0.8f, 0),
				this.firstPersonPlayer.transform.rotation);
			this.thirdPersonCamera.transform.position = this.thirdPersonPlayer.transform.TransformPoint(new Vector3(0, 5, 5));
			this.firstPersonPlayer.SetActive(false);
			this.thirdPersonPlayer.SetActive(true);
			this.thirdPersonCamera.SetActive(true);
		}
		else if(this.cameraMode == 1)
		{
			this.feetCamera.SetActive(true);
			this.thirdPersonCamera.SetActive(false);
		}
		else if(this.cameraMode == 2)
		{
			this.feetCamera.SetActive(false);
			this.firstPersonPlayer.transform.SetPositionAndRotation(
				this.thirdPersonPlayer.transform.position + new Vector3(0, 0.8f, 0),
				this.thirdPersonPlayer.transform.rotation);
			this.firstPersonPlayer.SetActive(true);
			this.thirdPersonPlayer.SetActive(false);
		}
	}

	protected void SetRobot(bool value)
	{
		Footstepper footstepper = this.thirdPersonPlayer.GetComponent<Footstepper>();
		if(footstepper != null)
		{
			footstepper.effectTag = value ? "robot" : "";
		}
		footstepper = this.firstPersonPlayer.GetComponent<Footstepper>();
		if(footstepper != null)
		{
			footstepper.effectTag = value ? "robot" : "";
		}
		this.isRobot = value;
	}
}
