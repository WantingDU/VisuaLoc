using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoleController : MonoBehaviour {
    
    Vector3 groundLevel;
    Vector3 undergroundLevel;
    public GameObject effect;
    public GameObject signalPrefab;
    public GameObject body;

    bool isOnGround = true;
    float time = 0;
    public bool badMole = false;
    float randomTime = 1f;
    Material bodyMat;
    Color originCol;
    Color redCol;
   
    void isBad()
    {
        float x = Random.Range(0f,1f);
        if (x<0.5)
        {
            badMole = false;
        }
        else
        {
            badMole = true;
        }
    }
    
    void Up()
    {
        transform.position = groundLevel;
        this.isOnGround = true;
    }

    void Down()
    {
        transform.position = this.undergroundLevel;
        this.isOnGround = false;
    }

    void Start () 
    {
        this.groundLevel = transform.position;
        this.undergroundLevel = this.groundLevel - new Vector3(0, 0.2f, 0);

        //bodyMat = body.GetComponent<Renderer>().material;

        //originCol = bodyMat.color;
        //redCol = originCol * new Color(1f, 0f, 0f, 1f);
        // 地中に隠す
        Down();
	}
    public void Hit()
    {
        GameObject g = Instantiate(effect, transform.position + new Vector3(0, 0.04f, 0), effect.transform.rotation);//instantiate effet of hit
        Destroy(g, 1.0f);//destroy the effet after 1 s
        //Destroy(GameObject.FindWithTag("signal"));
        this.time = 0;
        Down();
    }

    void Update () 
    {
        this.time += Time.deltaTime;
        
        if ( this.time > randomTime )
        {
            this.time = 0;
            randomTime = Random.Range(0.5f, 2.5f);
            isBad();
            if ( this.isOnGround) 
            {
                Down();
            }
            else 
            {
                Up();
                if (badMole)
                {
                    GameObject signal = Instantiate(signalPrefab, transform.position + new Vector3(0, 0.1f, 0), signalPrefab.transform.rotation);
                    signal.tag = "signal";
                    if (signal)
                    {
                        Destroy(signal, randomTime);
                    }

                    bodyMat.color = redCol;
                }
                else
                {
                    bodyMat.color = originCol;
                }
            }
        }
    }
}