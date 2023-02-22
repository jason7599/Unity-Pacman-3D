using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineTest : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        // StartCoroutine(MyCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            ToggleActive();
        }
    }

    private void ToggleActive()
    {
        gameObject.SetActive(false);

        Invoke(nameof(_SetActive), 1f);
    }

    private void _SetActive()
    {
        gameObject.SetActive(true);
    }


    private IEnumerator MyCoroutine()
    {
        print("Starting Routine..");

        int intervalIndex = 0;
        float[] intervals = {1f, 2f};
        float time = 0f;
        int iter = 0;

        while (true)
        {
            time += Time.deltaTime;

            if (time >= intervals[intervalIndex])
            {
                intervalIndex = (intervalIndex + 1) % 2;
                time = 0f;
                print($"{++iter} switch!");
            }

            yield return null;
        }
    }
}
