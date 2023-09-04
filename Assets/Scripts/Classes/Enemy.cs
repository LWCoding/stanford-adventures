using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy {

    public string name;
    public int maxHealth;
    public int baseDamage;
    public Vector3 spriteScale;
    public bool shouldRotate;

    public Enemy(string name, int hp, int dam, Vector3 scale, bool rotate) {
        this.name = name;
        this.maxHealth = hp;
        this.baseDamage = dam;
        this.spriteScale = new Vector3(scale.x * ((rotate) ? -1 : 1), scale.y, scale.z);
    }

}