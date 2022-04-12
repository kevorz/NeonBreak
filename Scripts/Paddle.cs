using Godot;
using System;

public class Paddle : KinematicBody2D
{

    Vector2 velocity = Vector2.Zero;
    [Export] public int Speed = 500;
    public override void _Ready()
    {
        
    }

    public override void _PhysicsProcess(float delta)
    {
        if (Input.IsActionPressed("move_left"))
        {
            velocity.x = -Speed;
        }
        else if (Input.IsActionPressed("move_right"))
        {
            velocity.x = Speed;
        }
        else
            velocity.x = 0;

        MoveAndSlide(velocity);
    }

}
