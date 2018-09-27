using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnEnable()
    {
        StartCoroutine("LoadingPage");
    }

    IEnumerator LoadingPage()
    {
        float rotateValue = 0;
        while(true)
        {
            rotateValue -= 30f;
            transform.GetComponent<RectTransform>().eulerAngles = new Vector3(0, 0, rotateValue % 360);
            yield return new WaitForSeconds(0.05f);
        }
    }

    public void OnDisable()
    {
        StopCoroutine("LoadingPage");
    }
}
