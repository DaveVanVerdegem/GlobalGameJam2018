using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartAnyButton : MonoBehaviour
{
    // Update is called once per frame
    private void Update()
    {
        if (Input.anyKeyDown)
            GameManager.Instance.RestartLevel();
    }
}