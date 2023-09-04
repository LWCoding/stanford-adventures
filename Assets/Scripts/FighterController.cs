using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FighterType {
    Ally, Enemy
}

public enum DamageMethod {
    RunHit
}

public partial class FighterController : MonoBehaviour
{
    
    [Header("Object Assignments")]
    public GameObject spriteObject;
    public GameObject spriteScaleObject;
    public FighterController enemyFighterController;
    public HealthBarController myHPController;
    public HealthBarController enemyHPController;
    [Header("Fighter-Specific Variables")]
    public FighterType fighterType;
    [Header("Fighter Sprites")]
    public Sprite stanfordEnemy;
    public Sprite lucasEnemy;
    
    // OTHER VARIABLES
    private int maxHealth = 50;
    private int health = 0;
    private int damage = 10;
    private int calculatedDamage = 0;
    private DamageMethod damageMethod = DamageMethod.RunHit;
    [HideInInspector] public Animator objectAnimator;

    private void Start() {
        health = maxHealth;
        objectAnimator = GetComponent<Animator>();
    }

    public float GetHealthPercentage() {
        return (float)health / maxHealth;
    }

    /*
        This function takes in an Enemy object and sets
        all of its stats corresponding to that object's
        stats.
    */
    public void InitializeStats(Enemy enemy, bool healToMax = true) {
        maxHealth = enemy.maxHealth;
        damage = enemy.baseDamage;
        spriteScaleObject.transform.localScale = enemy.spriteScale;
        if (healToMax) {
            health = maxHealth;
        }
        SpriteRenderer objectSR = spriteObject.GetComponent<SpriteRenderer>();
        switch (enemy.name) {
            case "Stanford":
                objectSR.sprite = stanfordEnemy;
                break;
            case "Lucas":
                objectSR.sprite = lucasEnemy;
                break;
        }
        myHPController.SetHPBar(health / maxHealth);
    }

    /*
        This function ONLY works for enemy fighters. It
        animates the enemy scrolling in (should be async
        with the background scrolling) after initializing
        all of its values.
    */
    public void LoadNextEnemy() {
        if (fighterType != FighterType.Enemy) { return; }
        if (Globals.allEnemies.Count == 0) {
            Debug.Log("NO MORE ENEMIES REMAINING! (FighterController.cs)");
            return;
        }
        // Get the first enemy from the enemy list.
        Enemy enemy = Globals.allEnemies[0];
        Globals.allEnemies.RemoveAt(0);
        // Initialize the object sprite.
        InitializeStats(enemy);
        // Animate the `spriteObject` moving in.
        objectAnimator.Play("EnterScene");
    }

    /*
        This function takes in the damage that this
        character takes, and subtracts it from the
        health. If the character dies, this function
        also handles that.
    */
    public void TakeDamage(int damageTaken) {
        health -= damageTaken;
        if (health <= 0) {
            health = 0;
            objectAnimator.Play("Dead");
            if (fighterType == FighterType.Ally) {
                Globals.gameIsOver = true;
            } else {
                Globals.enemyDefeated = true;
            }
        } else {
            objectAnimator.Play("Hurt");
        }
    }

    /*
        This function takes in the string that was
        used to attack the other fighter, and uses
        it to calculate the damage dealt.
        3 letters = damage * 1f
        4 letters = damage * 1.5f
        5 letters = damage * 2f (etc)
    */
    public void CalculateDamage(string s) {
        calculatedDamage = (int)(damage * ((s.Length - 3) * 0.5f + 1));
    }

    /*
        Returns the position of the enemy that the
        other enemy should travel to in order to not
        clip into the current sprite.
    */
    public float CalculateTargetPos(float spriteWidth) {
        float currPos = transform.position.x;
        float thisSpriteWidth = spriteObject.GetComponent<SpriteRenderer>().bounds.size.x;
        if (fighterType == FighterType.Enemy) {
            thisSpriteWidth *= -1;
            spriteWidth *= -1;
        }
        return currPos + thisSpriteWidth / 2 + spriteWidth / 2;
    }

    /*
        Animates the `spriteObject` depending on 
        the type of `damageMethod` this object has.
        These attack methods are stored in separate
        C# files. See files with suffixes.
    */
    public IEnumerator AnimateAttackCoroutine() {
        switch (damageMethod) {
        case DamageMethod.RunHit:
            yield return RunHitCoroutine();
            break;
        }
    }

    private void DealDamageToEnemy() {
        enemyFighterController.TakeDamage(calculatedDamage);
        enemyHPController.SetHPBar(enemyFighterController.GetHealthPercentage());
    }

}
