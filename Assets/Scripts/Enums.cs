public class Enums
{
    public enum WeaponType
    {
        Pistol,
        Revolver,
        Autorifle,
        Shotgun,
        SniperRifle
    }
    public enum EquipType
    {
        SideEquip,
        BackEquip
    };
    public enum HoldType
    {
        CommonHold = 1,
        LowHold,
        HighHold
    }
    public enum ShootType
    {
        Single,
        Auto
    }
    public enum HangType
    {
        LowBackHang,
        BackHang,
        SideHang
    }
    public enum AmmoBoxType
    {
        SmallBox,
        BigBox
    }
    public enum AttackType_Melee
    {
        Close,
        Charge
    }
    public enum EnemyMelee_Type
    {
        Regular,
        Shield,
        Dodge,
        AxeThrow
    }
    public enum Enemy_MeleeWeaponType
    {
        OneHand,
        Throw,
        Unarmed
    }
    public enum Enemy_RangeWeaponType
    {
        Pistol,
        Revolver,
        Shotgun,
        AutoRifle,
        SniperRifle,
        Random
    }
    public enum Enemy_RangeWeaponHoldType
    {
        CommonHold,
        LowHold,
        HighHold
    }
    public enum CoverPerk
    {
        Unavalible,
        CanTakeCover,
        CanTakeAndChangeCover
    }
    public enum UnstoppablePerk
    {
        Unavalible,
        Unstoppable
    }
    public enum GrenadePerk
    {
        Unavalible,
        CanThrowGrenade
    }
    public enum BossWeaponType
    {
        Flamethrower,
        Fist
    }
    public enum SnapPointType
    {
        Enter,
        Exit
    }
    public enum EnemyType
    {
        Melee,
        Range,
        Boss,
        Random
    }
    public enum AxelType
    {
        Front,
        Back
    }
    public enum DriveType
    {
        FrontWheelDrive,
        RearWheelDrive,
        AllWheelDrive
    }
}
