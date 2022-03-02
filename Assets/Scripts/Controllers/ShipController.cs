using UnityEngine;

public class ShipController : MonoBehaviour
{
    public static ShipController PlayerShip;
    public int Hp;

    private TerrainGenerator tg;

    private void Start()
    {
        PlayerShip = this;
        tg = FindObjectOfType<TerrainGenerator>();
    }

    private void FixedUpdate()
    {
        SpaceBackground.UpdateBackground(tg.TerrainColor(transform.position));
    }
}
