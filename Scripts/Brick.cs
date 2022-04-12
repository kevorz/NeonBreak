using Godot;
using System;

public class Brick : StaticBody2D
{
    Random rnd = new Random();
    public int Health = 3;
    

    public override void _Ready()
    {
        base._Ready();
        
    }

    public void _BallTouchedMe(Node body)
    {
        //GD.Print($"Ew, this {body.GetType()} touched me.");
        if (body.GetType() != typeof(Ball))
            return;
        Health--;
        GetParent()?.Call("IncreaseScore", Global.BrickScoreWorth);

        ChangeColor();
        if (Health < 1)
            {
                if (rnd.Next(0,100) <= (100 - Global.PowerUpChance))
                {
                    GD.Print($"Yay, a powerup chance.");
                    GetParent()?.Call("SpawnPowerUp", (int)this.Position.x, (int)this.Position.y);
                }
                try
                {
                    CallDeferred("queue_free");
                }
                catch (Exception ex)
                {
                    GD.Print($"Error: {ex.Message}");
                }
                
                GetParent()?.Call("BrickDestroyed");
            }

    }
    public void RandomizeHealth(int max)
    {
        this.Health = rnd.Next(1, max+1);
        //GD.Print($"My health is {Health}");
        ChangeColor();
    }

    public void ChangeColor()
    {
        if (Health == 1)
            this.Modulate = new Color(3f,0,0);
        if (Health == 2)
            this.Modulate = new Color(0,3f,0);
        if (Health == 3)
            this.Modulate = new Color(0,0,3f);
    }
}
