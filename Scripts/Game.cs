using Godot;
using System;
using System.Collections.Generic;

public class Game : Node
{
    public static int Score;
    public int thisLevel;
    public PackedScene BrickScene;
    public PackedScene BallScene;
    public PackedScene PowerupScene;
    public int PlayerHealth;
    public List<RigidBody2D> BallCollection = new List<RigidBody2D>();
    public List<StaticBody2D> BrickCollection = new List<StaticBody2D>();
    public List<RigidBody2D> PowerUpCollection = new List<RigidBody2D>();

    public Label HealthLabel;
    public Label ScoreLabel;
    public int BallAmount;
    public int CurLevel = 1;
    public int BrickCount = 0;

    public override void _Ready()
    {
        HealthLabel = GetNode<Label>("UI/M/V/H1/HealthLabel");
        ScoreLabel = GetNode<Label>("UI/M/V/H2/ScoreLabel");
        ResetGame();
        //PlayLevel(1);
    }

    public override void _Process(float delta)
    {
        UpdateStats();

    }

    public void UpdateStats()
    {
            HealthLabel.Text = Global.PlayerHealth.ToString();
            ScoreLabel.Text = Global.PlayerScore.ToString();
    }
    public void PlayLevel(int currentLevel)
    {
        //GD.Print("Generate level!");
        CurLevel = currentLevel;

        Level level = new Level();

        switch (currentLevel)
        {
            case 1:
                level.RowAmount = 2;
                level.ColumnAmount = 12;
                level.BrickMaxHealth = 2;
                break;
            case 2:
                level.RowAmount = 6;
                level.ColumnAmount = 12;
                level.BrickMaxHealth = 4;
                break;
            case 3:
                GameFinished();
                return;

            default:
                level.RowAmount = 1;
                level.ColumnAmount = 1;
                level.BrickMaxHealth = 1;
                break;
        }

        SetupLevel(level);
        //GD.Print($"Your life is: {PlayerHealth}");
        SpawnBall();

    }

    public void GameFinished()
    {
        GD.Print($"Thank you for playing!");
    }

    public void BrickDestroyed()
    {
        BrickCount--;
        if (BrickCount < 1)
            LevelWon();
    }

    public async void LevelWon()
    {
        GD.Print($"Level won!");
        ResetObjects();
        await ToSignal(GetTree().CreateTimer(2), "timeout");
        CurLevel++;
        PlayLevel(CurLevel);
    }
    public async void SpawnBall()
    {
        GD.Print("Spawning ball.");
        BallScene = (PackedScene)ResourceLoader.Load("res://Scenes/Ball.tscn");
        var ball = (RigidBody2D)BallScene.Instance();
        if (IsInstanceValid(ball))
        {
            ball.Position = new Vector2(640,520);
            BallCollection.Add(ball);
            AddChild(ball);
            await ToSignal(GetTree().CreateTimer(1), "timeout");
            ball.Call("AllowToMove");

            BallAmount++;
        }
    }

    public void SetupLevel(Level level)
    {
        //GD.Print("Setting up level.");
        BrickScene = (PackedScene)ResourceLoader.Load("res://Scenes/Brick.tscn");
        int row = 1;
        int col = 1;
        int maxrows = level.RowAmount;
        int maxcols = level.ColumnAmount;
        int brickmaxhealth = level.BrickMaxHealth;


        while (row < level.RowAmount)
        {
            while (col < maxcols)
            {
                var brick = (StaticBody2D)BrickScene.Instance();
                BrickCollection.Add(brick);
                brick.Position = new Vector2((level.StartX + ((level.OffsetX + 20) * col)),(level.StartY  + ((level.OffsetY + 20) * row)));
                brick.Call("RandomizeHealth", level.BrickMaxHealth);
                AddChild(brick);
                BrickCount++;
                //GD.Print($"Added object at X:{brick.Position.x} Y:{brick.Position.y}");
                col++;
            }
        col = 1;
        row++;
        }
        row = 1;
    }

    public async void ResetGame()
    {
        BallAmount = 0;
        Global.PlayerScore = 0;

        ResetObjects();

        PlayerHealth = Global.PlayerStartingHealth;
        await ToSignal(GetTree().CreateTimer(2), "timeout");
        
        GD.Print("Ready!)");

        Global.PlayerHealth = PlayerHealth;
        CurLevel = 1;
        PlayLevel(CurLevel);
    }

    public void ResetObjects()
    {
        BallAmount = 0;
        if (BallCollection != null && BallCollection.Count > 0)
        {
            try 
            {
                foreach (var ball in BallCollection)
                    if (IsInstanceValid(ball))
                        //ball.Free();
                        ball.CallDeferred("queue_free");
                BallCollection.Clear();

            }
            catch (Exception ex)
            {
                GD.Print($"Error: {ex.Message}");
            }
        }

        if (BrickCollection != null && BrickCollection.Count > 0)
        {
            foreach (var brick in BrickCollection)
                if (IsInstanceValid(brick))
                    brick.CallDeferred("queue_free");
            BrickCollection.Clear();
        }

        if (PowerUpCollection != null && PowerUpCollection.Count >= 0)
        {
            foreach (var powerup in PowerUpCollection)
                if (IsInstanceValid(powerup))
                    powerup.CallDeferred("queue_free");
            PowerUpCollection.Clear();
        }
    }
    public void _BallOutOfBounds(Node body)
    {
        if (body.GetType() == typeof(PowerUp))
        {
            GD.Print("Missed a powerup, dummy!");
            body.CallDeferred("queue_free");
            return;
        }
        
        body.CallDeferred("queue_free");
        BallAmount--;
        GD.Print($"Ball is out, man, Balls: {BallAmount}");
        if (BallAmount < 1 && BrickCount > 0)
        {
            PlayerHealth--;
            ResetPaddleWidth();
            Global.PlayerHealth = PlayerHealth;
        }
        if (PlayerHealth <= 0)
            GameOver();
        else if (BallAmount < 1)
            SpawnBall();
    }

    public void GameOver()
    {
        GD.Print("GameOver!");
        ResetGame();
    }

    public void IncreaseScore(int i)
    {
        Global.PlayerScore += i;
        if (Global.PlayerScore % Global.ExtraLifeScore == 0)
            PlayerHealth++;
        if (Global.PlayerScore % Global.ExtraBallScore == 0)
            SpawnBall();
        Global.PlayerHealth = PlayerHealth;
    }

    public void SpawnPowerUp(int x, int y)
    {
        if (BrickCount < 1)
            return;
            
        var PowerupScene = (PackedScene)ResourceLoader.Load("res://Scenes/PowerUp.tscn");
        var powerup = (RigidBody2D)PowerupScene.Instance();
        AddChild(powerup);
        PowerUpCollection.Add(powerup);

        powerup.Position = new Vector2((float)x, (float)y + 10f);
    }

    public async void ApplyPowerUp(int i)
    {
        switch (i)
        {
            case 1:
                IncreasePaddleWidth();
                break;
            case 2:
                SpawnBall();
                await ToSignal(GetTree().CreateTimer(0.25f), "timeout");
                SpawnBall();
                break;
            case 3:
                PlayerHealth++;
                Global.PlayerHealth = PlayerHealth;
                break;
        }

    }

    public void IncreasePaddleWidth()
    {
        GD.Print("Increasing PaddleWidth");
        var paddle = (KinematicBody2D)GetNode<KinematicBody2D>("Paddle");
        Vector2 tmpscale = new Vector2(paddle.Scale.x + 0.25f, paddle.Scale.y);
        paddle.Scale = tmpscale;
    }
    public void ResetPaddleWidth()
    {
        var paddle = (KinematicBody2D)GetNode<KinematicBody2D>("Paddle");
        Vector2 tmpscale = new Vector2(1f, paddle.Scale.y);
        paddle.Scale = tmpscale;
    }

}
