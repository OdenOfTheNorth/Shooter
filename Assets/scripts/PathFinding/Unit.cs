using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour
{
    private const float minPathUpdateTime = .2f;
    private const float pathupdateMoveTreshold = 0.5f;

    public Transform target;
    public float speed = 20;
    
    public float turnSpeed = 3;
    public float turnDist = 5;

    private Path path;

    void Start() {
        StartCoroutine(UpdatePath());
    }

    public void OnPathFound(Vector3[] wayPoints, bool pathSuccessful) {
        if (pathSuccessful) {
            path = new Path(wayPoints , transform.position, turnDist);
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    IEnumerator UpdatePath()
    {
        if (Time.timeSinceLevelLoad < .3f)
        {
            yield return new WaitForSeconds(.3f);
        }
        
        PathRequestManager.RequestPath(transform.position,target.position,OnPathFound);
        
        float sqrMoveTreshold = pathupdateMoveTreshold * pathupdateMoveTreshold;

        Vector3 targetPosOld = target.position;
        
        while (true)
        {
            yield return new WaitForSeconds(minPathUpdateTime);
            
            if ((target.position - targetPosOld).sqrMagnitude > sqrMoveTreshold)
            {
                PathRequestManager.RequestPath(transform.position,target.position,OnPathFound);
                targetPosOld = target.position;
            }
        }
    }
    
    IEnumerator FollowPath()
    {
        bool followingPath = true;

        int pathIndex = 0;
     
        if (path.lookPoints.Length == -1)
        {
            yield return null;
        }
        
        transform.LookAt(path.lookPoints[0]);
        
        while (true)
        {
            Vector2 pos2d = new Vector2(transform.position.x, transform.position.z);
            while (path.turnBoundries[pathIndex].HasCrossedLine(pos2d))
            {
                if (pathIndex == path.finishLineIndex)
                {
                    followingPath = false;
                    break;
                }
                else
                {
                    pathIndex++;
                }
            }

            if (followingPath)
            {
                Quaternion targetRotation = Quaternion.LookRotation(path.lookPoints[pathIndex] - transform.position);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
                transform.Translate(Vector3.forward * Time.deltaTime * speed, Space.Self);
            }
            
            yield return null;
        }
    }

    public void OnDrawGizmos() {
        if (path != null) {
            path.DrawWithGizmos();
        }
    }
}