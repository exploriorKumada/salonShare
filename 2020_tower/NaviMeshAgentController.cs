using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NaviMeshAgentController : MonoBehaviour
{
    // Update is called once per frame
    [SerializeField] Transform targetTF;
    NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if(targetTF!=null)
        {
            agent.destination = targetTF.position;
        }        
    }
}
