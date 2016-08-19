using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour
{
	// The range of forces that can be applied to the ball in the x direction.
	private Range<int> xForceRange;
	
	// The range of forces that can be applied to the ball in the y direction.
	private Range<int> yForceRange;
	
	// The range of velocities at which the ball can travel.
	private Range<float> yVelocityRange;
	
	// Controls the amount of spin the paddle puts on the ball.
	private int paddleSpin;
	
	// Holds information about the game's state.
	public GameState state;
	
	// Controls the game's sound.
	// Set this in the editor.
	public Sounds sounds;
	
	// Helps manage collisions.
	// Set this in the editor.
	public DontGoThroughThings dontGo;
	
	// Use this for initialization
	void Start () 
	{
		// Initialize the force of the x direction.
		xForceRange = new Range<int> (-200, 200);
		
		// Initialize the force of the y direction.
		yForceRange = new Range<int> (750, 750);
		
		// Initialize the y velocity range.
		yVelocityRange = new Range<float> (15, 40);
		
		// Set the amount of spin the paddle puts on the ball.
		paddleSpin = 750;
	}
	
	// Update is called once per frame
	void Update () 
	{	
		CheckSpeed ();
		CheckDeadBall ();
		CheckServe ();
	}
	
	/// <summary>
	/// Ensures the ball stays within the speed range.
	/// </summary>
	private void CheckSpeed ()
	{
		// Store the direction the ball is traveling.
		float sign = Mathf.Sign (rigidbody.velocity.y);
		
		// If the ball is travelling slower than the minimum velocity...
		if (Mathf.Abs (rigidbody.velocity.y) < yVelocityRange.Minimum)
		{
			// ...set the ball's velocity to the minimum velocity.
			rigidbody.velocity = new Vector3 (rigidbody.velocity.x, yVelocityRange.Minimum * sign, rigidbody.velocity.z);	
		}
		// Otherwise, if the ball is travelling faster than the maximum velocity...
		else if (Mathf.Abs (rigidbody.velocity.y) > yVelocityRange.Maximum)
		{
			// ...set the ball's velocity to the maximum velocity.
			rigidbody.velocity = new Vector3 (rigidbody.velocity.x, yVelocityRange.Maximum * sign, rigidbody.velocity.z);	
		}
	}
	
	/// <summary>
	/// Checks to see if the ball has moved out of the court.
	/// </summary>
	private void CheckDeadBall ()
	{
		// If the ball is out of bounds, update the game state!
		if (transform.position.y < -15 && state.CurrentState == GameState.State.Play) state.NextState ();
	}
	
	/// <summary>
	/// Checks to see if the ball should be served.
	/// </summary>
	private void CheckServe ()
	{
		// Check to see if the ball is out of play...
		if (state.CurrentState != GameState.State.Play)
		{
			// Pause the collision helper.
			dontGo.paused = true;
			
			// Set the ball's velocity to zero.
			rigidbody.velocity = new Vector3(0,0,0);
			
			// Check to see if the player has served the ball...
			if (Input.GetButton("Serve")) Serve ();
		}	
	}
	
	/// <summary>
	/// Serves the ball.
	/// </summary>
	private void Serve ()
	{
		// Update the current state.
		state.NextState ();
		
		// Place the ball in center court.
		transform.position = new Vector3(0,0,0);
		
		// Let the collision manager know where the ball is located.
		dontGo.previousPosition = new Vector3(0,0,0);
		
		// Unpause the collision manager.
		dontGo.paused = false;
		
		// Set the ball in motion!
		rigidbody.AddForce(Random.Range(xForceRange.Minimum, xForceRange.Maximum), -Random.Range(yForceRange.Minimum, yForceRange.Maximum), 0);
	}
	
	/// <summary>
	/// Calculates the force to be applied to the ball from the paddle.
	/// </summary>
	/// <returns>
	/// The force to apply to the ball.
	/// </returns>
	/// <param name='playerPosition'>
	/// The position of the player's paddle.
	/// </param>
	private float CalculateForce (float playerPosition)
	{
		// Return the force to be applied to the ball:
		// (ball position - paddle position) * spin
		return (transform.position.x - playerPosition) * paddleSpin;	
	}
	
	// When a collision occurs.
	void OnCollisionEnter(Collision collision)
	{
		// Check to see if the ball has collided with the player.
		Player player = collision.gameObject.GetComponent<Player>();
		
		// If so...
		if (player != null)
		{
			// Add the appropriate force to the ball.
			rigidbody.AddForce(CalculateForce (player.transform.position.x), 0, 0);	
			
			// Play the paddle collision sound.
			sounds.PlaySound (1);
		}
		else
		{
			// Check to see if the ball has collided with a block.
			Block block = collision.gameObject.GetComponent<Block>();
			
			// If so...
			if (block != null)
			{
				// Destroy the block! AHAHA!!
				Destroy (collision.gameObject);	
				
				// Play the block sound!
				sounds.PlaySound (0);
			}
			else
			{
				// We've hit a wall... play the wall bounce sound!
				sounds.PlaySound (2);	
			}
		}
	}
}