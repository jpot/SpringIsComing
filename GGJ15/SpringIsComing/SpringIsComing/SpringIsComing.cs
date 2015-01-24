using System;
using System.Collections.Generic;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Effects;
using Jypeli.Widgets;

public class SpringIsComing : PhysicsGame
{
    const double movementSpeed = 200;
    const double jumpSpeed = 750;
    const int TILE_SIZE = 40;

    PlatformCharacter player1;

    Image playerImage = LoadImage("norsu");
    Image starImage = LoadImage("tahti");

    SoundEffect goalSound = LoadSoundEffect("maali");

    public override void Begin()
    {
        IsFullScreen = true;

        LoadLevel("kentta1");
        AddKeys();

        Camera.Follow(player1);
        Camera.ZoomFactor = 1.2;
        Camera.StayInLevel = true;

        Gravity = new Vector(0, -1000);
    }

    /// <summary>
    /// Loads the level from a file
    /// </summary>
    void LoadLevel(String levelFile)
    {
        TileMap level = TileMap.FromLevelAsset(levelFile);
        level.SetTileMethod('#', AddPlatform);
        level.SetTileMethod('*', AddStar);
        level.SetTileMethod('N', AddPlayer);
        level.Execute(TILE_SIZE, TILE_SIZE);
        Level.CreateBorders();
        Level.Background.CreateGradient(Color.White, Color.SkyBlue);
    }

    void AddPlatform(Vector position, double width, double height)
    {
        PhysicsObject platform = PhysicsObject.CreateStaticObject(width, height);
        platform.Position = position;
        platform.Color = Color.Gray;
        Add(platform);
    }

    void AddStar(Vector position, double width, double height)
    {
        PhysicsObject star = PhysicsObject.CreateStaticObject(width, height);
        star.IgnoresCollisionResponse = true;
        star.Position = position;
        star.Image = starImage;
        star.Tag = "star";
        Add(star);
    }

    void AddPlayer(Vector position, double width, double height)
    {
        player1 = new PlatformCharacter(width, height);
        player1.Position = position;
        player1.Mass = 4.0;
        player1.Image = playerImage;
        AddCollisionHandler(player1, "star", HitStar);
        Add(player1);
    }

    void AddKeys()
    {
        Keyboard.Listen(Key.F1, ButtonState.Pressed, ShowControlHelp, "Show help");
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Exit");

        Keyboard.Listen(Key.Left, ButtonState.Down, Move, "Player 1: Move left", player1, -movementSpeed);
        Keyboard.Listen(Key.Right, ButtonState.Down, Move, "Player 1: Move right", player1, movementSpeed);
        //Keyboard.Listen(Key.Up, ButtonState.Pressed, Jump, "Player 1: Move up", pelaaja1, hyppyNopeus);

        ControllerOne.Listen(Button.Back, ButtonState.Pressed, Exit, "Exit");

        ControllerOne.Listen(Button.DPadLeft, ButtonState.Down, Move, "Player 1: Move left", player1, -movementSpeed);
        ControllerOne.Listen(Button.DPadRight, ButtonState.Down, Move, "Player 1: Move right", player1, movementSpeed);
        //ControllerOne.Listen(Button.A, ButtonState.Pressed, Jump, "Player 1: Move up", pelaaja1, hyppyNopeus);

        PhoneBackButton.Listen(ConfirmExit, "Lopeta peli");
    }

    void Move(PlatformCharacter character, double speed)
    {
        character.Walk(speed);
    }

    void Jump(PlatformCharacter character, double speed)
    {
        character.Jump(speed);
    }

    void HitStar(PhysicsObject character, PhysicsObject speed)
    {
        goalSound.Play();
        MessageDisplay.Add("You have collected a star!");
        speed.Destroy();
    }
}