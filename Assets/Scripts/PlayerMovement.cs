using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Vector3 target;

    //all bounds are global space, relative to world origin
    [SerializeField]
    private float upperBoundX = 8F;
    [SerializeField]
    private float upperBoundZ = 6f;
    [SerializeField]
    private float lowerBoundX = -8F;
    [SerializeField]
    private float lowerBoundZ = -6f;
    [SerializeField]
    private float moveRadius = 3f;
    [SerializeField]
    private float currSpeed = 2f;
    private float maxSpeed = 2f;
    private bool atTarget = false;
    private float travelDist = 0f;
    Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        target = GenerateTargetPosition();
        animator = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!atTarget && transform.position == target) //if reached target, generate a new one
        {
            atTarget = true;
            animator.SetBool("Stop", true);
            IEnumerator cooldownCR = MoveCooldown(Random.Range(0.5f, 2f));
            StartCoroutine(cooldownCR);
        }
        if (!atTarget)
        {
            currSpeed = maxSpeed * 0.3f + Mathf.Sin(Vector3.Distance(transform.position, target) * Mathf.PI / travelDist);
            Move(target); //move one step at a time to target
        }
    }

    Vector3 GenerateTargetPosition()
    {
        float targetX = Random.Range(transform.position.x - moveRadius, transform.position.x + moveRadius);
        
        //ensure character within wander range
        if (targetX > upperBoundX) {targetX = upperBoundX;} //cant exceed upper bound
        if (targetX < lowerBoundX) {targetX = lowerBoundX;} //cant exceed lower bound

        float targetZ = Random.Range(transform.position.z - moveRadius, transform.position.z + moveRadius);
        
        //ensure character within wander range
        if (targetZ > upperBoundZ) {targetZ = upperBoundZ;} //cant exceed upper bound
        if (targetZ < lowerBoundZ) {targetZ = lowerBoundZ;} //cant exceed lower bound

        Vector3 target = new Vector3(targetX, transform.position.y, targetZ);

        travelDist = Vector3.Distance(transform.position, target);

        return target;
    }

    void Move(Vector3 target)
    {
        var step = currSpeed * Time.deltaTime; // calculate distance to move

        transform.position = Vector3.MoveTowards(transform.position, target, step);
        transform.LookAt(target);
    }

    IEnumerator MoveCooldown(float seconds)
    {
        int randNum = Random.Range(0, 2);
        if(randNum == 1) {animator.SetTrigger("Kneel"); yield return new WaitForSeconds(5.5f);}
        yield return new WaitForSeconds(seconds);
        target = GenerateTargetPosition();
        animator.SetBool("Stop", false);
        atTarget = false;
    }
}
