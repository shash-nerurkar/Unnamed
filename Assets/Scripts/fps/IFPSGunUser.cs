public interface IFPSWeaponUser
{
    public bool IsAttacking { get; set; }

    public void OnWeaponAttack(bool IsAttacking);
}
