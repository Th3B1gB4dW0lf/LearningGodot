using Godot;
using System;

public class Player : Area2D
{
	[Export]
	public int Speed = 400; // How fast the player will move (pixels/sec).

	private Vector2 _screenSize; // Size of the game window.
	
	public override void _Ready()
	{
		_screenSize = GetViewport().Size;
		Hide();
	}
	
	public override void _Process(float delta)
	{
		var velocity = new Vector2(); // The player's movement vector.

		if (Input.IsActionPressed("ui_right"))
		{
			velocity.x += 1;
		}

		if (Input.IsActionPressed("ui_left"))
		{
			velocity.x -= 1;
		}

		if (Input.IsActionPressed("ui_down"))
		{
			velocity.y += 1;
		}

		if (Input.IsActionPressed("ui_up"))
		{
			velocity.y -= 1;
		}

		var animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");

		if (velocity.Length() > 0)
		{
			velocity = velocity.Normalized() * Speed;
			animatedSprite.Play();
		}
		else
		{
			animatedSprite.Stop();
		}
		
		Position += velocity * delta;
		Position = new Vector2(
			x: Mathf.Clamp(Position.x, 0, _screenSize.x),
			y: Mathf.Clamp(Position.y, 0, _screenSize.y)
		);
		
		if (velocity.x != 0)
		{
			animatedSprite.Animation = "walk";
			animatedSprite.FlipV = false;
			// See the note below about boolean assignment
			animatedSprite.FlipH = velocity.x < 0;
		}
		else if (velocity.y != 0)
		{
			animatedSprite.Animation = "up";
			animatedSprite.FlipV = velocity.y > 0;
		}
	}
	
	public void Start(Vector2 pos)
	{
		Position = pos;
		Show();
		GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
	}
	
	[Signal]
	public delegate void Hit();
	
	private void OnPlayerBodyEntered(object body)
	{
		Hide(); // Player disappears after being hit.
		EmitSignal("Hit");
		GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred("disabled", true);	// Replace with function body.
	}
}
