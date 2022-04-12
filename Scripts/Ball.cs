using Godot;
using System;

public class Ball : RigidBody2D
{
    Vector2 velocity = Vector2.Zero;
    public int StartposX = 640;
    public int StartposY = 520;
    public bool SpedUp = false;
    public bool MaxSpeedReached = false;
    public bool IsAllowedToMove = false;
    public float TickTime = 5000.0f;
    public float CurTickTime = 0.0f;
    public int Tick = 0;

    private float MinClamp = 300;
    private float MaxClamp = 400;
    private Vector2 TmpVector;
    private bool NegativeX;
    private bool NegativeY;
    public bool BallDebug = false;
    Label debuglabelx;
    Label debuglabely;

    //pos x640 y520
    public override void _Ready()
    {
        ChangeColor();
        if (BallDebug)
        {
            debuglabelx = GetNode<Label>("DebugX");
            debuglabely = GetNode<Label>("DebugY");
            debuglabelx.Show();
            debuglabely.Show();
        }
    }

    public override void _PhysicsProcess(float delta)
    {
        if (BallDebug)
        {
            debuglabelx.Text = $"X: {LinearVelocity.x}";
            debuglabely.Text = $"Y: {LinearVelocity.y}";      
            //Min 100 Max 400
            //Min -100 Max -400


            if (LinearVelocity.x < 0 )
                NegativeX = true;
            else
                NegativeX = false;
            if (LinearVelocity.y < 0)
                NegativeY = true;
            else
                NegativeY = false;

            if (NegativeX)
                LinearVelocity = new Vector2( (float)Mathf.Clamp( (int)LinearVelocity.x ,(int)-MinClamp , (int)-MaxClamp), LinearVelocity.y);
            if (!NegativeX)
                LinearVelocity = new Vector2( (float)Mathf.Clamp( (int)LinearVelocity.x, (int)MinClamp, (int)MaxClamp), LinearVelocity.y);
            if (NegativeY)
                LinearVelocity = new Vector2( LinearVelocity.x, (float)Mathf.Clamp( (int)LinearVelocity.y ,(int)-MinClamp , (int)-MaxClamp));
            if (!NegativeY)
                LinearVelocity = new Vector2( LinearVelocity.x, (float)Mathf.Clamp( (int)LinearVelocity.y, (int)MinClamp, (int)MaxClamp));


            //QuickMaths!
        }

    }
    public void AllowToMove()
    {
        Random rnd = new Random();
        velocity = new Vector2(rnd.Next(300,500),-rnd.Next(300,500));
        LinearVelocity = velocity;
        IsAllowedToMove = true;
    }

    public void ChangeColor()
    {        
        Random rnd = new Random();
        float r = (float)rnd.Next(1,6);
        float g = (float)rnd.Next(1,6);
        float b = (float)rnd.Next(1,6);
        Modulate = new Color(r,g,b);
    }

    public void SpeedMeUp()
    {
        LinearVelocity = new Vector2(LinearVelocity.x * 1.1f, LinearVelocity.y * 1.1f);
        if (LinearVelocity.x > 1000f)
            MaxSpeedReached = true;
    }


//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
