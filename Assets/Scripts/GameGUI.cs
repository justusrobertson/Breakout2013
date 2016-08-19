using UnityEngine;
using System.Collections;

public class GameGUI : MonoBehaviour
{
	// A GUI Text that represents how many tries the player has left.
	public GUIText tries;	
	
	// The game's state.
	private GameState state;
	
	// Use this for initialization
	void Start ()
	{
		// Find the game's state and store a reference.
		state = (GameState)FindObjectOfType(typeof(GameState));
	}
	
	// Called every time the GUI is redrawn
	void OnGUI()
	{
		// Update the display text to represent the number of tries the player has left.
		tries.text = "" + state.BallsLeft;
	}
}