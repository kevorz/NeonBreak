using Godot;
using System;

public class PowerUp : RigidBody2D
{

    Random rnd = new Random();
    public int PowerUpType;
    public override void _Ready()
    {
        PowerUpType = rnd.Next(0, 3);
        ChangeColor(PowerUpType);
    }

    public void ChangeColor(int i)
    {
        switch (i)
        {
            case 0:
                this.Modulate = new Color(3f,0,0);
                break;
            case 1:
                this.Modulate = new Color(0,3f,0);
                break;
            case 2:
                this.Modulate = new Color(0,0,3f);
                break;

            default:
                break;
        }
    }
    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);
    }
    
    public void _TouchedSomething(Node body)
    {
        if (body.GetType() == typeof(Paddle))
        {
            GetParent()?.Call("ApplyPowerUp", PowerUpType);
            CallDeferred("queue_free");
        }
        
    }

}
