using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;


public class TestCoroutine : MonoBehaviour
{
    private bool _canJump = false;

    private void Start()
    {
        if (Thread.CurrentThread.Name == null)
        {
            Thread.CurrentThread.Name = "UnityMain";
        }
        Debug.Log(Thread.CurrentThread.Name);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Task.Run(() => MyJob());
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_canJump == true)
            {
                Debug.Log("점프");
                _canJump = false;
            }
            else
            {
                Debug.Log("점프준비");
            }
        }
    }

    private void MyJob()
    {
        Debug.Log(Thread.CurrentThread.Name);
        Thread.Sleep(3000);
        Debug.Log(Thread.CurrentThread.Name);
        _canJump = true;
    }
}
