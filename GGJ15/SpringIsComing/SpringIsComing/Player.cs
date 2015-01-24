using System;
using Jypeli;
using Jypeli.Assets;

class Player : PhysicsObject
{
    private IntMeter lifeCounter = new IntMeter(100, 0, 100);
    public IntMeter LifeCounter { 
        get
        { 
            return lifeCounter;
        } 
        /*set
        { 
            this.lifeCounter = value;
            this.Scale();
        } */
    }
    
    public AssaultRifle Weapon { get { return weapon; } set { weapon = value; } }
    private AssaultRifle weapon;
    public Image ProjectileImage { get { return projectileImage; } set { projectileImage = value; } }
    private Image projectileImage;
    private Game parentGame;

    public Player(double width, double height, Game game, Image projectileImage)
        : base(width, height)
    {
        this.parentGame = game;
        lifeCounter.LowerLimit += delegate { this.Destroy(); };
        this.ProjectileImage = projectileImage;
        this.Weapon = new AssaultRifle(20, 5);
        this.Weapon.Angle = Angle.FromDegrees(270);
        this.Weapon.Power.DefaultValue = 40; // default speed for ammunition
        this.Weapon.Power.Value = this.Weapon.Power.DefaultValue; // next ammo speed
        this.Weapon.CanHitOwner = true;
        
        // hides the weapon
        this.Weapon.Image = null;
        this.Weapon.Color = Color.Transparent;
        
        this.Add(Weapon);
    }

    public void ThrowProjectile(PhysicsGame game, Vector direction, string tag)
    {
        PhysicsObject projectile = this.Weapon.Shoot();
        if (projectile != null)
        {
            projectile.Size = new Vector(10, 10);
            projectile.Image = projectileImage;
            projectile.Tag = tag;
        }
    }
    public void ChangeLifeCounterValue(int change)
    {
        this.LifeCounter.Value += change;
        Scale();
    }
    public void Scale()
    {

        
        double multiplier = (this.LifeCounter.Value - this.LifeCounter.MinValue) / (1.0*(this.LifeCounter.MaxValue - this.LifeCounter.MinValue));
        //this.parentGame.MessageDisplay.Add("multiplier: " + multiplier.ToString());
        double widthMax = 150;
        double widthMin = 20;
        this.Width = (multiplier * (widthMax - widthMin) ) + widthMin;
        //this.parentGame.MessageDisplay.Add("width: " + this.Width.ToString());

    }
}
