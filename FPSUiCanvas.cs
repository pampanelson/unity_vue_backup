using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSUiCanvas : MonoBehaviour
{
  

    void setToggle(int val){
        toggle(val);
    }

    void toggle(int val){
        if(val > 0){
            this.gameObject.SetActive(true);
        }
        else
        {
            this.gameObject.SetActive(false);
        }

    }
    // Start is called before the first frame update
    void Start()
    {
        toggle(1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
