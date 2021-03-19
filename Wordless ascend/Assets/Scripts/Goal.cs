using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    public ParticleSystem winnerParticles;

    [SerializeField] private GameObject levelEndCanvas;
    [SerializeField] private float uiDisplayDelaySeconds = 2f;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Level complete!");
        Time.timeScale = 0;
        //spawn particles
        Instantiate(winnerParticles, transform.position, Quaternion.identity);
        StartCoroutine(DisplayLevelCompleteUI(uiDisplayDelaySeconds));

    }

    IEnumerator DisplayLevelCompleteUI(float secondsDelay)
    {
        yield return new WaitForSecondsRealtime(secondsDelay);
        levelEndCanvas.SetActive(true);
        Debug.Log("Setting canvas to active");
    }
}
