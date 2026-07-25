using System.CodeDom.Compiler;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Vector3 target;
    [SerializeField]
    private float boundX = 8F;
    [SerializeField]
    private float boundZ = 6f;
    [SerializeField]
    private float moveRadius = 3f;
    [SerializeField]
    private float speed = 2f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        target = GenerateTargetPosition();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position == target) //if reached target, generate a new one
        {
            target = GenerateTargetPosition();
        }
        Move(target); //move one step at a time to target
    }

    Vector3 GenerateTargetPosition()
    {
        float targetX = Random.Range(transform.position.x - moveRadius, transform.position.x + moveRadius);
        
        //ensure character within wander range
        if (targetX > boundX) {targetX = boundX;} //cant exceed upper bound
        if (targetX < -boundX) {targetX = -boundX;} //cant exceed lower bound

        float targetZ = Random.Range(transform.position.z - moveRadius, transform.position.z + moveRadius);
        
        //ensure character within wander range
        if (targetZ > boundZ) {targetZ = boundZ;} //cant exceed upper bound
        if (targetZ < -boundZ) { targetZ = -boundZ; } //cant exceed lower bound

        Vector3 target = new Vector3(targetX, transform.position.y, targetZ);
        return target;
    }

    void Move(Vector3 target)
    {
        var step = speed * Time.deltaTime; // calculate distance to move

        transform.position = Vector3.MoveTowards(transform.position, target, step);
        transform.LookAt(target);
    }
}
