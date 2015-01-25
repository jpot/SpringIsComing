﻿using System;
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
    static Timer timer;

    // TODO add level selection menu
    // TODO add restart level and back to level selection menus
    int levelNumber = 1;

    int maximumLifeForPlayer1 = 100;
    int maximumLifeForPlayer2 = 100;

    int snowballThrowCost = 10;
    int snowHealAmount = 10;
    int smallDamage = 2;
    int greatDamage = 5;

    Player player1, player2;

    // TODO add water image
    // TODO add snow pile image
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
        level.SetTileMethod('S', AddFieryStone);
        level.SetTileMethod('s', AddFieryStone2);
        level.SetTileMethod('V', AddWaterContainer);
        level.Execute(TILE_SIZE, TILE_SIZE);
        Level.CreateBorders();
        Level.Background.CreateGradient(Color.White, Color.Green);
    }
    // TODO add snow piles to levels
    // TODO change some campfires to candles

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

    PhysicsObject AddPushableObject(Vector position, double width, double height, Image image, String tag)
    {
        PhysicsObject newPushableObject = new PhysicsObject(width, height, Shape.Rectangle);
        newPushableObject.Position = position;
        newPushableObject.Color = Color.Blue;
        newPushableObject.CanRotate = false;
        newPushableObject.Image = image;
        newPushableObject.Tag = tag;
        Add(newPushableObject);
        return newPushableObject;
    }

    void AddFieryStone2(Vector position, double width, double height)
    {
        AddTile(position, width, height, hehkukiviImage, false, "hehkuvatkivet2");
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

    void AddWaterContainer(Vector position, double width, double height)
    {
        // TODO switch null to water image
        PhysicsObject watercontainer = AddPushableObject(position, width, height, null, "water");
        AddCollisionHandler(watercontainer, "campfire", Extinguish);
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
        //this.player2.Animation.Start();
    }

    
    void TimerStart()
    {
        timer = new Timer();
        timer.Enabled = true;
        timer.Interval = 0.5;
        timer.Timeout += delegate
                        { 
                            AnimationCheck(player1);
                            AnimationCheck(player2);
                        };
        timer.Start();
    }
    

    Player AddPlayer(Vector position, double width, double height, Image playerImage, int life)
    {
        Player newPlayer = new Player(width, height, this, snowballImage);
        newPlayer.LifeCounter.MaxValue = life;
        newPlayer.LifeCounter.Value = newPlayer.LifeCounter.MaxValue;
        newPlayer.Scale(); // TODO make this happen automatically the first time

        newPlayer.Position = position;
        newPlayer.Mass = 1.0;
        newPlayer.Image = playerImage;
        newPlayer.LinearDamping = 0.95;
        newPlayer.CanRotate = false;
        AddCollisionHandler(newPlayer, "star", HitStar);
        AddCollisionHandler(newPlayer, "campfire", HitCampfire);
        AddCollisionHandler(newPlayer, "snow", HitSnow);
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

        ControllerOne.Listen(Button.Back, ButtonState.Pressed, Exit, "Exit");


        ControllerOne.Listen(Button.DPadLeft,  ButtonState.Down, Move, "Player 2: Move left",  player2, new Vector(-movementSpeed, 0             ));
        ControllerOne.Listen(Button.DPadRight, ButtonState.Down, Move, "Player 2: Move right", player2, new Vector( movementSpeed, 0             ));
        ControllerOne.Listen(Button.DPadUp,    ButtonState.Down, Move, "Player 2: Move up",    player2, new Vector(             0, movementSpeed ));
        ControllerOne.Listen(Button.DPadDown,  ButtonState.Down, Move, "Player 2: Move down",  player2, new Vector(             0, -movementSpeed));

        ControllerOne.Listen(Button.A, ButtonState.Pressed, ThrowSnowball, "Player 2: Throw snowball", player2);
        //ControllerOne.Listen(Button.A, ButtonState.Pressed, Jump, "Player 1: Move up", pelaaja1, hyppyNopeus);
        
        //PhoneBackButton.Listen(ConfirmExit, "Lopeta peli"); 
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
        HitGreatFire(collider, target);
    }


    void HitGreatFire(PhysicsObject collider, PhysicsObject target)
    {
        ((Player)collider).ChangeLifeCounterValue(-greatDamage);
        MessageDisplay.Add("Aargh!");
    }

    
    void HitSmallFire(PhysicsObject collider, PhysicsObject target)
    {
        ((Player)collider).ChangeLifeCounterValue(-smallDamage);
        MessageDisplay.Add("Ouch!");
    }


    void HitSnow(PhysicsObject collider, PhysicsObject target)
    {
        if (((Player)collider).LifeCounter.Value < ((Player)collider).LifeCounter.MaxValue)
        {
            ((Player)collider).ChangeLifeCounterValue(snowHealAmount);
            MessageDisplay.Add("Gained life!");
            target.Destroy();
        }
    }

    void ThrowSnowball(Player character)
    {
        // You may not throw snowballs if it would kill you
        if (character.LifeCounter > snowballThrowCost)
        {
            character.ThrowProjectile(this, character.Velocity, "snow");
            character.ChangeLifeCounterValue(-snowballThrowCost);
            //character.Width = character.LifeCounter.Value;
            //character.Height = character.LifeCounter.Value;
            
            // TODO destroy snowballs after time? Create piles when collides with wall?
        }
    }

    void Extinguish(PhysicsObject collider, PhysicsObject target)
    {
        Explosion splash = new Explosion(TILE_SIZE);
        splash.Position = target.Position;
        Add(splash);
        splash.Image = null;
        splash.ShockwaveColor = Color.Blue;
        splash.Sound = null; // TODO add soundeffect

        collider.Destroy();
        target.Destroy();
    }

    void HitGoal(PhysicsObject character, PhysicsObject goal)
    {
        // Increase level number and load the next level
        levelNumber++;
        LoadNextLevel();
    }
}