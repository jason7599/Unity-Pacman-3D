using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvokeTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Hello();
        }
    }

    private void Hello()
    {
        print("Hello");

        CancelInvoke(nameof(Bye));
        Invoke(nameof(Bye), 2f);
    }

    private void Bye()
    {
        print("Bye");
    }
}
