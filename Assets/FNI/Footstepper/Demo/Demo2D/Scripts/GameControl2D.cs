
using UnityEngine;
using UnityEngine.SceneManagement;
using GamingIsLove.Footsteps;
using System.Collections.Generic;

public class GameControl2D : MonoBehaviour
{
	public GameObject player;


	// GUI
	public Rect robotButton = new Rect(10, 10, 200, 30);

	public Rect controlTextRect = new Rect(10, 50, 200, 100);

	[TextArea]
	public string controlText = "Move: AD or arrow keys\n" +
		"Jump: Space\n" +
		"Crouch: CTRL (hold)";

	public Rect loadSceneButton = new Rect(220, 10, 200, 30);

	public string sceneName = "Footstepper Demo 3D";


	// in-game
	private bool isHide = false;

	private bool isRobot = false;

	protected virtual void Update()
	{
		if(Input.GetKeyDown(KeyCode.R))
		{
			this.SetRobot(!this.isRobot);
		}
		else if(Input.GetKeyDown(KeyCode.H))
		{
			this.isHide = !this.isHide;
		}
	}

	protected virtual void OnGUI()
	{
		if(this.isHide)
			return;

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

		if(GUI.Button(this.loadSceneButton, "Load 3D Scene"))
		{
			SceneManager.LoadScene(this.sceneName);
		}

		GUI.Label(this.controlTextRect, this.controlText);
	}

	protected void SetRobot(bool value)
	{
		Footstepper footstepper = this.player.GetComponent<Footstepper>();
		if(footstepper != null)
		{
			footstepper.effectTag = value ? "robot" : "";
		}
		this.isRobot = value;
	}
}
