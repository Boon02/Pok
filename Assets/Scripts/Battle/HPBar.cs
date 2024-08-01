using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPBar : MonoBehaviour
{
    [SerializeField] private GameObject health;
    
    public bool IsUpdating { get; private set; }
        
    public void SetHP(float hpNormalized)
    {
        health.transform.localScale = new Vector3(hpNormalized, 1);
    }

    public IEnumerator SetHPSmooth(float newHp)
    {
        IsUpdating = true;
        
        float curHp = health.transform.localScale.x;
        float changeHp = curHp - newHp;

        while (curHp - newHp > Mathf.Epsilon)
        {
            curHp -= changeHp * Time.deltaTime;
            health.transform.localScale = new Vector3(curHp,1f);
            yield return null;
        }
        health.transform.localScale = new Vector3(newHp,1f);

        IsUpdating = false;
    }
}
