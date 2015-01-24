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

    PhysicsObject player1, player2;

    Image playerImage = LoadImage("Lumiukko");
    Image player2Image = LoadImage("LumiukkoPlaceholderd");
    Image starImage = LoadImage("tahti");
    Image campfireImage = LoadImage("nuotio");

    SoundEffect goalSound = LoadSoundEffect("maali");

    public override void Begin()
    {
        IsFullScreen = true;

        LoadLevel("kentta1");
        AddKeys();

        Camera.Follow(player1, player2);
        Camera.ZoomFactor = 1.2;
        //Camera.StayInLevel = true;

        //Gravity = new Vector(0, -1000);
    }

    /// <summary>
    /// Loads the level from a file
    /// </summary>
    void LoadLevel(String levelFile)
    {
        TileMap level = TileMap.FromLevelAsset(levelFile);
        level.SetTileMethod('#', AddPlatform);
        level.SetTileMethod('*', AddStar);
        level.SetTileMethod('f', AddCampfire);
        level.SetTileMethod('1', AddPlayer1);
        level.SetTileMethod('2', AddPlayer2);
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
    
    void AddCampfire(Vector position, double width, double height)
    {
        PhysicsObject campfire = PhysicsObject.CreateStaticObject(width, height);
        campfire.IgnoresCollisionResponse = true;
        campfire.Position = position;
        campfire.Image = campfireImage;
        campfire.Tag = "campfire";
        Add(campfire);
    }

    void AddPlayer1(Vector position, double width, double height)
    {
        this.player1 = AddPlayer(position, width, height*2, playerImage);
    }

    void AddPlayer2(Vector position, double width, double height)
    {
        this.player2 = AddPlayer(position, width, height*2.5, player2Image);
    }

    PhysicsObject AddPlayer(Vector position, double width, double height, Image playerImage)
    {
        PhysicsObject newPlayer = new PhysicsObject(width, height);
        newPlayer.Position = position;
        newPlayer.Mass = 1.0;
        newPlayer.Image = playerImage;
        newPlayer.LinearDamping = 0.95;
        newPlayer.CanRotate = false;
        AddCollisionHandler(newPlayer, "star", HitStar);
        AddCollisionHandler(newPlayer, "campfire", HitCampfire);
        Add(newPlayer);
        return newPlayer;
    }

    void AddKeys()
    {
        Keyboard.Listen(Key.F1, ButtonState.Pressed, ShowControlHelp, "Show help");
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Exit");

        Keyboard.Listen(Key.Left,   ButtonState.Down, Move, "Player 1: Move left",  player1, new Vector(-movementSpeed, 0             ));
        Keyboard.Listen(Key.Right,  ButtonState.Down, Move, "Player 1: Move right", player1, new Vector( movementSpeed, 0             ));
        Keyboard.Listen(Key.Up,     ButtonState.Down, Move, "Player 1: Move up",    player1, new Vector(             0, movementSpeed ));
        Keyboard.Listen(Key.Down,   ButtonState.Down, Move, "Player 1: Move down",  player1, new Vector(             0, -movementSpeed));

        Keyboard.Listen(Key.A,      ButtonState.Down, Move, "Player 2: Move left",  player2, new Vector(-movementSpeed, 0             ));
        Keyboard.Listen(Key.D,      ButtonState.Down, Move, "Player 2: Move right", player2, new Vector( movementSpeed, 0             ));
        Keyboard.Listen(Key.W,      ButtonState.Down, Move, "Player 2: Move up",    player2, new Vector(             0,  movementSpeed));
        Keyboard.Listen(Key.S,      ButtonState.Down, Move, "Player 2: Move up",    player2, new Vector(             0, -movementSpeed));



        //Keyboard.Listen(Key.Up, ButtonState.Pressed, Jump, "Player 1: Move up", pelaaja1, hyppyNopeus);

        /*ControllerOne.Listen(Button.Back, ButtonState.Pressed, Exit, "Exit");

        ControllerOne.Listen(Button.DPadLeft, ButtonState.Down, Move, "Player 1: Move left", player1, -movementSpeed);
        ControllerOne.Listen(Button.DPadRight, ButtonState.Down, Move, "Player 1: Move right", player1, movementSpeed);
        //ControllerOne.Listen(Button.A, ButtonState.Pressed, Jump, "Player 1: Move up", pelaaja1, hyppyNopeus);
        
        PhoneBackButton.Listen(ConfirmExit, "Lopeta peli"); */
    }

    void Move(PhysicsObject character, Vector direction)
    {
        character.Push(direction);
        //character.Position += direction;
    }

    /*void Jump(PlatformCharacter character, double speed)
    {
        character.Jump(speed);
    }
    */
    void HitStar(PhysicsObject character, PhysicsObject speed)
    {
        goalSound.Play();
        MessageDisplay.Add("You have collected a star!");
        speed.Destroy();
    }

    void HitCampfire(PhysicsObject collider, PhysicsObject target)
    {
        MessageDisplay.Add("Ouch!");
    }
}