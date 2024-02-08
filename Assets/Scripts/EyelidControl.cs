using UnityEngine;
using System.Collections;

public class EyelidControl : MonoBehaviour
{
    [SerializeField]
    private Animator[] eyelids;
    [SerializeField]
    private int minRange;
    [SerializeField]
    private int maxRange;

    private void Awake()
    {
        StartCoroutine(PlayAnimationAtRandom());
    }

    private IEnumerator PlayAnimationAtRandom()
    {
        while (true)
        {
            int waitTime = Random.Range(minRange, maxRange + 1);

            yield return new WaitForSeconds(waitTime);

            foreach(Animator eyeAnimator in eyelids)
            {
                eyeAnimator.Play("Movement");

                if (eyeAnimator.GetCurrentAnimatorStateInfo(0).IsName("Movement"))
                {
                    eyeAnimator.SetBool("moving", false);
                }
            }
        }
    }
}
