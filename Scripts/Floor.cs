using Godot;
using System;

public class Floor : Area2D
{
    public static CollisionShape2D coll;
    public override void _Ready()
    {
        coll = GetChild<CollisionShape2D>(0);
    }

    public void _BodyEntered(Node body)
    {
        CallDeferred("ToggleCollision");
    }

    public void ToggleCollision()
    {
        //GD.Print("Toggle collision");
        if (coll.Disabled)
            coll.SetDeferred("Disabled", false);
        else 
            coll.SetDeferred("Disabled", true);
    }

}

