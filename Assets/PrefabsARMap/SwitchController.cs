using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void Switch2Mine()
	{
		Application.LoadLevel(0);
	}
	public void Switch2Normal()
	{
		Application.LoadLevel(1);
	}
	public void Switch2Serialized()
	{
		Application.LoadLevel(2);
	}
}
