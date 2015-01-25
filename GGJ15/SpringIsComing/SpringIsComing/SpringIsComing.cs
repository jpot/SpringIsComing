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
    static Timer timer;

    // TODO add restart level and back to level selection menus
    int levelNumber = 1;
    bool skipMadeWithJypeliScreen = true;

    int maximumLifeForPlayer1 = 100;
    int maximumLifeForPlayer2 = 100;

    int snowballThrowCost = 10;
    int snowHealAmount = 10;
    int smallDamage = 2;
    int greatDamage = 5;


    Vector menuPosition = Vector.Zero; // default position for menus

    Player player1, player2;

    // TODO add snow pile image
    // TODO add yellow flower image
    Image titleBackgroundImage = LoadImage("titlescreen");
    Image jypeliImage = LoadImage("madewithjypeli");
    Image creditsImage = LoadImage("creditscreen");

    Image snowpileImage = LoadImage("lumikasa");
    Image waterbucketImage = LoadImage("vesisanko");
    Image playerImage = LoadImage("Lumiukko");
    Image player2Image = LoadImage("LumiukkoPlaceholderd");
    Image starImage = LoadImage("tahti");
    Image flowerImage = LoadImage("kukka1");
    Image flowerImage2 = LoadImage("kukka2");
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
    Image[] candleAnimation = LoadImages("kynttila", "kynttila2");
                                 
    Image[] snowMan2 = LoadImages("Lumiukko", "Lumiukko2J", "Lumiukko3J");
    Image wallImage = LoadImage("Seina");
    SoundEffect goalSound = LoadSoundEffect("maali");
    Image[] deathp1 = LoadImages("BigLumiukkoJump7",
                               "BigLumiukkoJump8",
                               "BigLumiukkoJump6");
    // TODO load and use splash sound
    // TODO load and use snowball throwing sound
    // TODO draw, load and use melting/dying animation for both players

    public override void Begin()
    {
        IsFullScreen = true;

        // Center position for menus:
        this.menuPosition = new Vector(0, -Screen.Height / 8);
        if (skipMadeWithJypeliScreen)
        {
            StartMenu();
        }
        else
        {
            MadeWithJypeliScreen();
        }
    }

    void MadeWithJypeliScreen()
    {
        Level.Background.Color = Color.Black;
        GameObject textObject = new GameObject(52.5, 25.1, Shape.Rectangle);
        textObject.Position = Vector.Zero;
        textObject.Color = Color.White;
        textObject.Image = jypeliImage;
        Add(textObject);
        //Camera.ZoomTo(0 - Screen.Width / 5, 0 - Screen.Height / 4, 0 + Screen.Width / 5, 0 + Screen.Height / 4);
        Timer zoomTimer = new Timer();
        zoomTimer.Interval = 0.01;
        zoomTimer.Timeout += delegate { 
            textObject.Width *= 1.02;
            textObject.Height *= 1.02;
            // If zoomed big enough then stop zooming
            if (textObject.Width > Screen.Width / 3 || textObject.Height > Screen.Height / 2.5)
            {
                zoomTimer.Stop();
                // doesn't work for gameobjects with images :(
                //textObject.FadeColorTo(Color.Black, 5.0);
                Timer.SingleShot(1.8, StartMenu);
            }
        };
        zoomTimer.Start();
    }

    /// <summary>
    /// Shows start menu with background image
    /// </summary>
    void StartMenu()
    {
        GameObject menuBackgroundScreen = new GameObject(Screen.Width, Screen.Height, Shape.Rectangle);
        menuBackgroundScreen.Image = titleBackgroundImage;
        menuBackgroundScreen.Color = Color.Azure;
        menuBackgroundScreen.Width = Screen.Width;
        menuBackgroundScreen.Height = Screen.Height;
        Add(menuBackgroundScreen);

        MultiSelectWindow startMenu = new MultiSelectWindow("Spring is coming",
                                        "Start game", "Level selection", "Credits", "Exit");
        startMenu.AddItemHandler(0, LoadNextLevel);
        startMenu.AddItemHandler(1, LevelSelection);
        startMenu.AddItemHandler(2, Credits);
        startMenu.AddItemHandler(3, Exit);
        startMenu.DefaultCancel = -1;
        startMenu.Position = menuPosition;
        Timer.SingleShot(0.2, delegate { Add(startMenu); });
    }

    /// <summary>
    /// Shows level selection menu
    /// </summary>
    void LevelSelection()
    {
        MultiSelectWindow levelSelectionMenu = new MultiSelectWindow("Level selection",
                                                "Level 1", "Level 2", "Level 3", "Level 4", "Challenge", "Back");
        levelSelectionMenu.AddItemHandler(0, delegate { this.levelNumber = 1; LoadNextLevel(); });
        levelSelectionMenu.AddItemHandler(1, delegate { this.levelNumber = 2; LoadNextLevel(); });
        levelSelectionMenu.AddItemHandler(2, delegate { this.levelNumber = 3; LoadNextLevel(); });
        levelSelectionMenu.AddItemHandler(3, delegate { this.levelNumber = 4; LoadNextLevel(); });
        levelSelectionMenu.AddItemHandler(4, delegate { this.levelNumber = 5; LoadNextLevel(); });
        levelSelectionMenu.AddItemHandler(5, StartMenu);
        levelSelectionMenu.DefaultCancel = 5;
        Add(levelSelectionMenu);
    }

    /// <summary>
    /// Shows credits
    /// </summary>
    void Credits()
    {
        ClearAll();
        GameObject creditsScreen = new GameObject(Screen.Width, Screen.Height, Shape.Rectangle);
        creditsScreen.Image = creditsImage;
        creditsScreen.Position = Vector.Zero;
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, delegate { ClearCredits(creditsScreen); }, "Back to start menu");
        Keyboard.Listen( Key.Enter, ButtonState.Pressed, delegate { ClearCredits(creditsScreen); }, "Back to start menu");
        ControllerOne.Listen(Button.A, ButtonState.Pressed, delegate { ClearCredits(creditsScreen); }, "Back to start menu");
        ControllerOne.Listen(Button.B, ButtonState.Pressed, delegate { ClearCredits(creditsScreen); }, "Back to start menu");

        Add(creditsScreen);
    }

    /// <summary>
    /// Clears credits screen and goes back to start menu.
    /// </summary>
    /// <param name="creditsScreenToBeCleared">background image to destroy before going back to start menu</param>
    void ClearCredits(GameObject creditsScreenToBeCleared)
    {
        creditsScreenToBeCleared.Destroy();
        Keyboard.Clear();
        ControllerOne.Clear();
        StartMenu(); 
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
        else if (levelNumber == 5) LoadLevel("kentta5");
        else if (levelNumber > 5) Exit();

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
        level.SetTileMethod('k', AddFlower2);
        level.SetTileMethod('f', AddCampfire);
        level.SetTileMethod('C', AddBigCampfire);
        level.SetTileMethod('1', AddPlayer1);
        level.SetTileMethod('2', AddPlayer2);
        level.SetTileMethod('S', AddFieryStone);
        level.SetTileMethod('s', AddFieryStone2);
        level.SetTileMethod('V', AddWaterContainer);
        level.SetTileMethod('c', AddCandle);
        level.SetTileMethod('L', AddSnowpile);
        level.Execute(TILE_SIZE, TILE_SIZE);
        Level.CreateBorders();
        Level.Background.CreateGradient(Color.White, Color.Green);
    }
    // TODO add snow piles to levels
    // TODO change some campfires to candles
    // TODO use more candles in levels
    // TODO extuinqish candles with snow
    // TODO switch some walls to stones/rocks

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
        newPushableObject.LinearDamping = 0.95;
        Add(newPushableObject);
        return newPushableObject;
    }

    ///TODO: tästä kerättävä objekti:
    void AddSnowpile(Vector position, double width, double height)
    {
        AddTile(position, width, height, snowpileImage, false, "lumikasa");
    }

    void AddFieryStone2(Vector position, double width, double height)
    {
        AddTile(position, width, height, hehkukiviImage, true, "hehkuvatkivet2");
    }

    void AddFieryStone(Vector position, double width, double height)
    {
        AddTile(position, width, height, hehkuvatkivetImage, true, "hehkuvatkivet");
    }

    void AddWall(Vector position, double width, double height)
    {
        AddTile(position, width, height, wallImage, false, "wall");
    }

    void AddFlower(Vector position, double width, double height)
    {
        AddTile(position, width, height, flowerImage, true, "flower");
    }

    void AddFlower2(Vector position, double width, double height)
    {
        AddTile(position, width, height, flowerImage2, true, "flower");
    }

    void AddStar(Vector position, double width, double height)
    {
        AddTile(position, width, height, starImage, true, "star");
    }
    
    void AddCandle(Vector position, double width, double height)
    {
        PhysicsObject candle = AddTile(position, width, height, null, true, "candle");
        candle.Animation = new Animation(candleAnimation);
        candle.Animation.FPS = 5;
        candle.Animation.Start();
        AddCollisionHandler(candle, "snow", HitCandleWithSnow);
    }

    void AddCampfire(Vector position, double width, double height)
    {
        PhysicsObject campfire = AddTile(position, width, height, null, false, "campfire");
        campfire.Animation = new Animation(campFire);
        campfire.Animation.FPS = 5;
        campfire.Animation.Start();
        AddCollisionHandler(campfire, "snow", HitCampfireWithSnow);
    }

    void AddBigCampfire(Vector position, double width, double height)
    {
        PhysicsObject bigcampfire = AddTile(position+new Vector(0.5*width,-0.5*height), width*2, height*2, null, false, "campfire");
        bigcampfire.Animation = new Animation(campFire);
        bigcampfire.Animation.FPS = 5;
        bigcampfire.Animation.Start();
        AddCollisionHandler(bigcampfire, "snow", HitCampfireWithSnow);
    }

    void AddWaterContainer(Vector position, double width, double height)
    {
        PhysicsObject watercontainer = AddPushableObject(position, width, height, waterbucketImage, "vesisanko");
        AddCollisionHandler(watercontainer, "campfire", Extinguish);
    }

    void AddPlayer1(Vector position, double width, double height)
    {
        this.player1 = AddPlayer(position, width, height*2, playerImage, maximumLifeForPlayer1);
        this.player1.Animation = new Animation(snowMan);
        
        this.player1.Animation.FPS = 10;
        this.player1.Destroyed += delegate
                                        { 
                                            PhysicsObject deathanim = new PhysicsObject(player1.Width, player1.Height);
                                            deathanim.Shape = Shape.Rectangle;
                                            Add(deathanim);
                                            deathanim.X = player1.Position.X;
                                            deathanim.Y = player1.Position.Y;
                                            deathanim.Animation = new Animation(deathp1);
                                            deathanim.Animation.FPS = 5;
                                            deathanim.Animation.Start(1);
                                            deathanim.Animation.StopOnLastFrame = true;
                                            Timer.SingleShot(1.0, delegate
                                                                        {
                                                                            Death(deathanim);
                                                                            deathanim.Destroy();
                                                                        }
                                                            );
                                            MessageDisplay.Add("It's over...");
                                        };
        // TODO fix hitbox to be smaller than the actual animation
    }

    void AddPlayer2(Vector position, double width, double height)
    {
        this.player2 = AddPlayer(position, width, height*2, player2Image, maximumLifeForPlayer2);
        this.player2.Animation = new Animation(snowMan2);
        this.player2.Animation.FPS = 10;
        this.player2.Destroyed += delegate
                                    {
                                        PhysicsObject deathanim = new PhysicsObject(player2.Width, player2.Height);
                                        deathanim.Shape = Shape.Rectangle;
                                        Add(deathanim);
                                        deathanim.X = player2.Position.X;
                                        deathanim.Y = player2.Position.Y;
                                        deathanim.Animation = new Animation(deathp1);
                                        deathanim.Animation.FPS = 5;
                                        deathanim.Animation.Start(1);
                                        deathanim.Animation.StopOnLastFrame = true;
                                        Timer.SingleShot(1.0, delegate
                                                                {
                                                                    Death(deathanim);
                                                                    deathanim.Destroy();
                                                                }
                                                         );
                                        MessageDisplay.Add("It's over...");
                                    };
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
        AddCollisionHandler(newPlayer, "candle", HitCandle);
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
        
        Keyboard.Listen(Key.Left,  ButtonState.Pressed,  AnimationStart, "Player 1 Anim start", player1);
        Keyboard.Listen(Key.Left,  ButtonState.Released, AnimationStop,  "Player 1 Anim stop",  player1);
        Keyboard.Listen(Key.Right, ButtonState.Pressed,  AnimationStart, "Player 1 Anim start", player1);
        Keyboard.Listen(Key.Right, ButtonState.Released, AnimationStop,  "Player 1 Anim stop",  player1);
        Keyboard.Listen(Key.Up,    ButtonState.Pressed,  AnimationStart, "Player 1 Anim start", player1);
        Keyboard.Listen(Key.Up,    ButtonState.Released, AnimationStop,  "Player 1 Anim stop",  player1);
        Keyboard.Listen(Key.Down,  ButtonState.Pressed,  AnimationStart, "Player 1 Anim start", player1);
        Keyboard.Listen(Key.Down,  ButtonState.Released, AnimationStop,  "Player 1 Anim stop",  player1);
        
        Keyboard.Listen(Key.RightControl, ButtonState.Pressed, ThrowSnowball, "Player 1: Throw snowball", player1);
        
        Keyboard.Listen(Key.A,      ButtonState.Down, Move, "Player 2: Move left",  player2, new Vector(-movementSpeed, 0             ));
        Keyboard.Listen(Key.D,      ButtonState.Down, Move, "Player 2: Move right", player2, new Vector( movementSpeed, 0             ));
        Keyboard.Listen(Key.W,      ButtonState.Down, Move, "Player 2: Move up",    player2, new Vector(             0,  movementSpeed));
        Keyboard.Listen(Key.S,      ButtonState.Down, Move, "Player 2: Move up",    player2, new Vector(             0, -movementSpeed));

        Keyboard.Listen(Key.A, ButtonState.Pressed,  AnimationStart, "Player 2 Anim start", player2);
        Keyboard.Listen(Key.A, ButtonState.Released, AnimationStop,  "Player 2 Anim stop",  player2);
        Keyboard.Listen(Key.D, ButtonState.Pressed,  AnimationStart, "Player 2 Anim start", player2);
        Keyboard.Listen(Key.D, ButtonState.Released, AnimationStop,  "Player 2 Anim stop",  player2);
        Keyboard.Listen(Key.W, ButtonState.Pressed,  AnimationStart, "Player 2 Anim start", player2);
        Keyboard.Listen(Key.W, ButtonState.Released, AnimationStop,  "Player 2 Anim stop",  player2);
        Keyboard.Listen(Key.S, ButtonState.Pressed,  AnimationStart, "Player 2 Anim start", player2);
        Keyboard.Listen(Key.S, ButtonState.Released, AnimationStop,  "Player 2 Anim stop",  player2);

        Keyboard.Listen(Key.LeftControl, ButtonState.Pressed, ThrowSnowball, "Player 2: Throw snowball", player2);

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

    void HitCandle(PhysicsObject collider, PhysicsObject target)
    {
        HitSmallFire(collider, target);
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
            // TODO default to down when not moved yet
        }
    }

    void HitCandleWithSnow(PhysicsObject candle, PhysicsObject snow)
    {
        Extinguish(snow, candle);
    }

    void HitCampfireWithSnow(PhysicsObject campfire, PhysicsObject snow)
    {
        snow.Destroy();
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

    void Death(PhysicsObject collider)
    {
        Explosion splash = new Explosion(TILE_SIZE);
        splash.Position = collider.Position;
        Add(splash);
        splash.Image = null;
        splash.ShockwaveColor = Color.Blue;
        splash.Sound = null; // TODO add soundeffect

        collider.Destroy();
    }

    void HitGoal(PhysicsObject character, PhysicsObject goal)
    {
        // Increase level number and load the next level
        levelNumber++;
        LoadNextLevel();
    }
}