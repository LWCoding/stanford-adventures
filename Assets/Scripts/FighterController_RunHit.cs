using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class FighterController : MonoBehaviour
{
    
    private IEnumerator RunHitCoroutine() {
        float MoveSpeed = 0.1f;
        float origX = spriteObject.transform.position.x;
        float spriteWidth = spriteObject.GetComponent<SpriteRenderer>().bounds.size.x;
        float targetX = enemyFighterController.CalculateTargetPos(spriteWidth);
        if (fighterType == FighterType.Ally) {
            // Move current sprite to targetX.
            while (spriteObject.transform.position.x < targetX) {
                spriteObject.transform.position += new Vector3(MoveSpeed, 0, 0);
                yield return new WaitForSeconds(0.01f);
            }
            DealDamageToEnemy();
            while (spriteObject.transform.position.x > origX) {
                spriteObject.transform.position -= new Vector3(MoveSpeed, 0, 0);
                yield return new WaitForSeconds(0.01f);
            }
        } else {
            // Move current sprite to targetX.
            while (spriteObject.transform.position.x > targetX) {
                spriteObject.transform.position -= new Vector3(MoveSpeed, 0, 0);
                yield return new WaitForSeconds(0.01f);
            }
            DealDamageToEnemy();
            while (spriteObject.transform.position.x < origX) {
                spriteObject.transform.position += new Vector3(MoveSpeed, 0, 0);
                yield return new WaitForSeconds(0.01f);
            }
        }
    }

}
