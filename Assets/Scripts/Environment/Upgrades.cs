using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrades : MonoBehaviour
{
    GameObject player;
    ShipController ship;
    TurretController turret;
    BoosterController booster;

    public List<Sprite> sprites = new List<Sprite>();
    public enum UpgradeType
    {
        Shield,
        AttackSpeed,
        ProjectileSpeed,
        HomingStrength,
        BoosterStrength,
        MaxHp,
        RegenerateHp,
    }
    public UpgradeType upgrade;

    public void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        ship = player.GetComponent<ShipController>();
        turret = player.GetComponentInChildren<TurretController>();
        booster = player.GetComponentInChildren<BoosterController>();
        upgrade = (UpgradeType)Mathf.FloorToInt(Random.Range(0, 7));
        GetComponent<SpriteRenderer>().sprite = sprites[((int)upgrade)];
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        switch (upgrade)
        {
            case UpgradeType.Shield:
                booster.shield.transform.localScale += new Vector3(0.2f, 0.2f);
                break;
            case UpgradeType.AttackSpeed:
                turret.attackDelay = Mathf.Clamp(turret.attackDelay - 0.05f, 0.05f, 1);
                break;
            case UpgradeType.ProjectileSpeed:
                turret.projectileSpeed++;
                break;
            case UpgradeType.HomingStrength:
                turret.homingSpeed += 0.1f;
                break;
            case UpgradeType.BoosterStrength:
                booster.boostSpeed++;
                break;
            case UpgradeType.MaxHp:
                ship.MaxHp += 25;
                break;
            case UpgradeType.RegenerateHp:
                ship.Hp = Mathf.Clamp(ship.Hp + 50, -1, ship.MaxHp);
                break;
        }

        Destroy(gameObject);
    }
}
