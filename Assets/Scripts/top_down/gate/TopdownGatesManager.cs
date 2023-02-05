using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopdownGatesManager : MonoBehaviour
{
    public GameObject gateContainer;
    List<TopdownGate> gateList;

    void Start() {
        gateList = new List<TopdownGate>();
        gateList.AddRange(gateContainer.GetComponentsInChildren<TopdownGate>());

        foreach(TopdownGate gate in gateList) {
            if(Random.Range(0, 2) == 1)
                gate.Open();
        }
    }
}
