using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainScript : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject CellPrefab;

    void Start()
    {
        //Instantiate(CellPrefab,new Vector2(3,4),Quaternion.identity);
        //Instantiate(CellPrefab,new Vector2(6,-1),Quaternion.identity);
        //Instantiate(CellPrefab,new Vector2(-3,2),Quaternion.identity);

        //for (int i = 1; i <= 50; i++)
        //    for (int j=1; j<=10; j++)
        //        Instantiate(CellPrefab, new Vector2(i, j), Quaternion.identity);


        for (int i = 0; i < 20; i++)
        {
            GameObject g = Instantiate(CellPrefab, new Vector2(Random.Range(-10, 10), Random.Range(-5, 5)), Quaternion.identity);
            float a = Random.Range(0.5f, 5f);
            g.transform.localScale = new Vector2(a, a);
        }
            
        

    }

    // Update is called once per frame
    void Update()
    {
        
    }

   
    void OrderCells()
    {
        GameObject[] myTargets = GameObject.FindGameObjectsWithTag("Target1");
        int l = myTargets.Length;

        float minSize = myTargets[0].transform.localScale.x;
        for (int i=1; i<l-1; i++)
        {
            if (myTargets[i].transform.localScale.x < myTargets[i+1].transform.localScale.x)
                minSize = myTargets[i].transform.localScale.x;

        }

    }


}
