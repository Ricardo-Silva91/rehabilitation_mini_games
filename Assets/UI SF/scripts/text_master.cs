using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class text_master : MonoBehaviour {

    public TypeOutScript bestText;
    public Text normalText;

    public bool specialText;

    string currentText;

	// Use this for initialization
	void Start () {
	
	}
	
    public void setCurrentText(string newText, float typeTime=0f)
    {
        if(!newText.Equals(currentText))
        {
            if(specialText == true)
            {
                bestText.TotalTypeTime = typeTime;
                bestText.reset = true;
                bestText.FinalText = newText;
                bestText.On = true;
                //Debug.Log("text Change. last: " + currentText + "    new: " + newText);
            }
            else
            {
                normalText.text = newText;
            }
            currentText = newText;
        }
    }
    public void clearText()
    {
        if (specialText == true)
        {
            if (!bestText.reset)
            {
                bestText.reset = true;
            }
        }
        else
        {
            normalText.text = "";
        }
    }

    

    // Update is called once per frame
    void Update () {
        if (specialText == true)
        {
            bestText.On = true;
        }

    }
}
