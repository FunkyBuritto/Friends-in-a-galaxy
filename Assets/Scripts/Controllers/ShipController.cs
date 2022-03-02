using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    public static ShipController PlayerShip;
    public int Hp;

    private void Start()
    {
        PlayerShip = this;
    }
}
