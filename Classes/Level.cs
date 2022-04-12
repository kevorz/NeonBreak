using Godot;
using System;

public class Level : Node
{
    public int StartX = 40; //.x = +80, .y +40
    public int StartY = 20;
    public int OffsetX = 80;
    public int OffsetY = 40;
    public int LevelNumber {get;set;}
    public Column[] Row {get;set;}
    public int RowAmount {get;set;}
    public int ColumnAmount {get;set;}

    public int BrickMaxHealth;
}

public enum Column
{
    None,
    Red,
    Yellow,
    Blue
}
