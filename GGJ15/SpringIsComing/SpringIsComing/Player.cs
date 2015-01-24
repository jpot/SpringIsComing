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
        set
        { 
            this.lifeCounter.Value = value;
            this.Scale();
        } 
    }
    public AssaultRifle Weapon { get { return weapon; } set { weapon = value; } }
    private AssaultRifle weapon;
    public Image ProjectileImage { get { return projectileImage; } set { projectileImage = value; } }
    private Image projectileImage;

    public Player(double width, double height, Image projectileImage)
        : base(width, height)
    {
        lifeCounter.LowerLimit += delegate { this.Destroy(); };
        this.ProjectileImage = projectileImage;
        this.Weapon = new AssaultRifle(20, 5);
        this.Weapon.Angle = Angle.FromDegrees(270);
        this.Weapon.Power.DefaultValue = 40; // default speed for ammunition
        this.Weapon.Power.Value = this.Weapon.Power.DefaultValue; // next ammo speed
        this.Add(Weapon);
    }

    public void ThrowProjectile(PhysicsGame game, Vector direction)
    {
        PhysicsObject projectile = this.Weapon.Shoot();
        if (projectile != null)
        {
            projectile.Size = new Vector(10, 10);
            projectile.Image = projectileImage;
            
        }
    }
    public void Scale()
    {
        //this.Width = 
    }
}
