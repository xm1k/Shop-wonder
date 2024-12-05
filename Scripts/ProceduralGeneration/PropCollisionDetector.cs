using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropCollisionDetector : MonoBehaviour
{
    private void Start()
    {
        if (gameObject.GetComponent<Collider2D>() != null)
        {
            if (!CheckSpawn())
            {
                Destroy(gameObject);
               // gameObject.GetComponent<SpriteRenderer>().color = Color.red; To See Deleted Objects
            }
        }
    }
    private Collider2D[] results = new Collider2D[5];
    private bool CheckSpawn()
    {
        ContactFilter2D filter = new ContactFilter2D();
        int colliders = Physics2D.OverlapCollider(gameObject.GetComponent<Collider2D>(),filter.NoFilter() ,results);
        if (colliders > 0)
            return false;
        else
            return true;
    }
}
