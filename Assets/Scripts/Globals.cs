using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Globals
{

    public static bool isYourTurn = true;
    public static bool gameIsOver = false;
    public static bool enemyDefeated = false;
    public static List<Enemy> allEnemies = new List<Enemy>() {
        new Enemy("Stanford", 30, 10, new Vector3(1, 1, 1), false),
        new Enemy("Stanford", 30, 10, new Vector3(1, 1, 1), false),
        new Enemy("Stanford", 30, 10, new Vector3(1, 1, 1), false)
    };

}
