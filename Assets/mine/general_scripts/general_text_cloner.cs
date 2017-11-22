using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class general_text_cloner : MonoBehaviour {

    public Text textObjectToClone;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        this.GetComponent<Text>().text = textObjectToClone.text;

	}
}
