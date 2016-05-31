using UnityEngine;
using System.Collections;

public class WQYAniamtorTest : MonoBehaviour
{

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            GetComponent<Animator>().Play("attack_01");
        }

        if (Input.GetMouseButtonDown(1))
        {
            GetComponent<Animator>().Play("walk");
        }
    }
}
