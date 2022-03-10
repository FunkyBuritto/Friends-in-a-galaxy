using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrades : MonoBehaviour
{
    GameObject player;
    ShipController ship;
    TurretController turret;
    BoosterController booster;

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

    UpgradeType upgrade;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        ship = player.GetComponent<ShipController>();
        turret = player.GetComponentInChildren<TurretController>();
        booster = player.GetComponentInChildren<BoosterController>();
        upgrade = (UpgradeType)Mathf.FloorToInt(Random.Range(0, 7));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (upgrade)
        {
            case UpgradeType.Shield:
                break;
            case UpgradeType.AttackSpeed:
                break;
            case UpgradeType.ProjectileSpeed:
                break;
            case UpgradeType.HomingStrength:
                break;
            case UpgradeType.BoosterStrength:
                break;
            case UpgradeType.MaxHp:
                break;
            case UpgradeType.RegenerateHp:
                break;
        }
    }
}
