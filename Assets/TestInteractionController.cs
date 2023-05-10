using System;
using System.Collections;
using System.Collections.Generic;
using RootMotion.FinalIK;
using UnityEngine;

public class TestInteractionController : MonoBehaviour
{
    [SerializeField] private InteractionSystem interactionSystem;
    [SerializeField] private InteractionObject interactionObject;
    [SerializeField] private FullBodyBipedEffector[] effectors;


    private void OnGUI()
    {
        // if (interactionSystem == null)
        //     return;
        //
        // if (GUILayout.Button("Start Interaction")) {
        //     foreach (var e in effectors) {
        //         interactionSystem.StartInteraction(e, interactionObject, true);
        //     }
        // }
        //
        // if (effectors.Length == 0) return;
        //
        // if (interactionSystem.IsPaused(effectors[0])) {
        //     if (GUILayout.Button("Resume Interaction With " + interactionObject.name)) {
        //
        //         interactionSystem.ResumeAll();
        //     }
        // }
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WaitFor(1.0f));
    }

    IEnumerator WaitFor(float time)
    {
        yield return new WaitForSeconds(time);
        
        foreach (var e in effectors) {
            interactionSystem.StartInteraction(e, interactionObject, true);
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
