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
    public static int TILE_SIZE = 40;
    static Timer _timer;

    int levelNumber = 1;

    int maximumLifeForPlayer1 = 100;
    int maximumLifeForPlayer2 = 100;

    int snowballThrowCost = 10;

    Player player1, player2;

    Image playerImage = LoadImage("Lumiukko");
    Image player2Image = LoadImage("LumiukkoPlaceholderd");
    Image starImage = LoadImage("tahti");
    Image flowerImage = LoadImage("kukka1");
    Image snowballImage = LoadImage("lumipallo");
    Image grill1Image = LoadImage("Grilli1");
    Image grill2Image = LoadImage("Grilli2");
    Image hehkukiviImage = LoadImage("hehkukivi2");
    Image hehkuvatkivetImage = LoadImage("hehkuvatkivet");
    Image[] campFire = LoadImages("nuotio", "nuotio2", "nuotio", "nuotio3");
    Image[] snowMan = LoadImages("BigLumiukkoJump8",
                                 "BigLumiukkoJump1",
                                 "BigLumiukkoJump2",
                                 "BigLumiukkoJump3",
                                 "BigLumiukkoJump4",
                                 "BigLumiukkoJump5",
                                 "BigLumiukkoJump6");
                                 
    Image[] snowMan2 = LoadImages("Lumiukko", "Lumiukko2J", "Lumiukko3J");
    Image wallImage = LoadImage("Seina");
    SoundEffect goalSound = LoadSoundEffect("maali");

    public override void Begin()
    {
        IsFullScreen = true;
        LoadNextLevel();    
    }

    /// <summary>
    /// Clears the game and loads next level.
    /// </summary>
    void LoadNextLevel()
    {
        ClearAll();

        if (levelNumber == 1) LoadLevel("kentta1");
        else if (levelNumber == 2) LoadLevel("kentta2");
        else if (levelNumber == 3) LoadLevel("kentta3");
        else if (levelNumber == 4) LoadLevel("kentta4");
        else if (levelNumber > 4) Exit();

        AddKeys();

        Camera.Follow(player1, player2);
        Camera.ZoomFactor = 1.2;
        //Camera.StayInLevel = true;

        //Gravity = new Vector(0, -1000);
        //TimerStart();
    }

    /// <summary>
    /// Loads the level from a file
    /// </summary>
    void LoadLevel(String levelFile)
    {
        TileMap level = TileMap.FromLevelAsset(levelFile);
        level.SetTileMethod('#', AddWall);
        level.SetTileMethod('*', AddStar);
        level.SetTileMethod('v', AddFlower);
        level.SetTileMethod('f', AddCampfire);
        level.SetTileMethod('1', AddPlayer1);
        level.SetTileMethod('2', AddPlayer2);
        level.SetTileMethod('s', AddFieryStone);
        level.Execute(TILE_SIZE, TILE_SIZE);
        Level.CreateBorders();
        Level.Background.CreateGradient(Color.White, Color.Green);
    }

    PhysicsObject AddTile(Vector position, double width, double height, Image image, bool ignoresCollisionResponse, String tag)
    {
        int layerNumber = 0;
        PhysicsObject newTile = PhysicsObject.CreateStaticObject(width, height);
        newTile.IgnoresCollisionResponse = ignoresCollisionResponse;

        // if it ignores collisions, it is drawn in the background layer
        if (ignoresCollisionResponse)
        {
            layerNumber = -1;
        }
        newTile.Position = position;
        newTile.Color = Color.Gray;
        newTile.Image = image;
        newTile.Tag = tag;
        Add(newTile, layerNumber);
        return newTile;
    }

    void AddFieryStone(Vector position, double width, double height)
    {
        AddTile(position, width, height, hehkuvatkivetImage, false, "hehkuvatkivet");
    }

    void AddWall(Vector position, double width, double height)
    {
        AddTile(position, width, height, wallImage, false, "wall");
    }

    void AddFlower(Vector position, double width, double height)
    {
        AddTile(position, width, height, flowerImage, true, "flower");
    }

    void AddStar(Vector position, double width, double height)
    {
        AddTile(position, width, height, starImage, true, "star");
    }
    
    void AddCampfire(Vector position, double width, double height)
    {
        PhysicsObject campfire = AddTile(position, width, height, null, false, "campfire");
        campfire.Animation = new Animation(campFire);
        campfire.Animation.FPS = 5;
        campfire.Animation.Start();
    }

    void AddPlayer1(Vector position, double width, double height)
    {
        this.player1 = AddPlayer(position, width, height*2, playerImage, maximumLifeForPlayer1);
        this.player1.Animation = new Animation(snowMan);
        this.player1.Animation.FPS = 10;
        //this.player1.Animation.Start();
    }

    void AddPlayer2(Vector position, double width, double height)
    {
        this.player2 = AddPlayer(position, width, height*2.5, player2Image, maximumLifeForPlayer2);
        this.player2.Animation = new Animation(snowMan2);
        this.player2.Animation.FPS = 10;
        this.player2.Animation.Start();
    }

    /*
    void TimerStart()
    {
        _timer = new Timer();
        _timer.Enabled = true;
        _timer.Interval = 0.5;
        _timer.Timeout += delegate { AnimationCheck(player1);
                                     AnimationCheck(player2);
                                   };
        _timer.Start();
    }
    */

    Player AddPlayer(Vector position, double width, double height, Image playerImage, int life)
    {
        Player newPlayer = new Player(width, height, snowballImage);
        newPlayer.LifeCounter.MaxValue = life;
        newPlayer.LifeCounter.Value = newPlayer.LifeCounter.MaxValue;
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
        
        Keyboard.Listen(Key.Left, ButtonState.Pressed, AnimationStart, "hio", player1);
        Keyboard.Listen(Key.Left, ButtonState.Released, AnimationStop, "hio", player1);
        Keyboard.Listen(Key.Right, ButtonState.Pressed, AnimationStart, "hio", player1);
        Keyboard.Listen(Key.Right, ButtonState.Released, AnimationStop, "hio", player1);
        Keyboard.Listen(Key.Up, ButtonState.Pressed, AnimationStart, "hio", player1);
        Keyboard.Listen(Key.Up, ButtonState.Released, AnimationStop, "hio", player1);
        Keyboard.Listen(Key.Down, ButtonState.Pressed, AnimationStart, "hio", player1);
        Keyboard.Listen(Key.Down, ButtonState.Released, AnimationStop, "hio", player1);
        
        Keyboard.Listen(Key.RightControl, ButtonState.Pressed, ThrowSnowball, "Player 1: Throw snowball", player1);

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

    void AnimationCheck(PhysicsObject character)
    {
        Vector temp = character.Velocity.Normalize();
        if (Math.Abs(temp.Y) > 0 || Math.Abs(temp.X) > 0)
        {
            character.Animation.Start();
        }
        else
        {
            character.Animation.Stop();
        }
    }
    
    void AnimationStart(PhysicsObject character)
    {
        character.Animation.Start();
    }

    void AnimationStop(PhysicsObject character)
    {
        character.Animation.Stop();
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

    void ThrowSnowball(Player character)
    {
        // You may not throw snowballs if it would kill you
        if (character.LifeCounter > snowballThrowCost)
        {
            character.ThrowProjectile(this, new Vector(0, -1));
            character.LifeCounter.Value -= snowballThrowCost;
        }
    }

    void HitGoal(PhysicsObject character, PhysicsObject goal)
    {
        // Increase level number and load the next level
        levelNumber++;
        LoadNextLevel();
    }
}