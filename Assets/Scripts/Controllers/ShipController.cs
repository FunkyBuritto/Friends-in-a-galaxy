using UnityEngine;

public class ShipController : MonoBehaviour
{
    public static ShipController PlayerShip;
    public int Hp { get => hp; set { OnHit?.Invoke(hp, value); hp = value; } }
    [SerializeField] private int hp;

    private TerrainGenerator tg;

    public delegate void HitEventHandler(int old_hp, int new_hp);
    public event HitEventHandler OnHit;

    private void Awake()
    {
        PlayerShip = this;
        tg = FindObjectOfType<TerrainGenerator>();

        OnHit += (_, __) => CameraShake.Shake(0.35f, 0.35f);
    }

    private void FixedUpdate()
    {
        SpaceBackground.UpdateBackground(tg.TerrainColor(transform.position));
    }
}
