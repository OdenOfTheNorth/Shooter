using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar : MonoBehaviour
{
	//public List<Unit> PathFollowers; 
	
	public Transform TargetObject;
	
    public Grid grid;
    public List<Node> path;
    
    Vector3 StartPos;
    Vector3 EndPos;
    
    Vector3 Velocity;
    bool jumpPoint = false;
    int TargetIndex;
    
    // Start is called before the first frame update
    void Start()
    {
	    if (!grid)
	    {
		    print("Grid dose not exist");
		    return;
	    }

	    UpdatePath();
    }

    private void Update()
    {
	    //Vector3 dir = (path[path.Count - 1].worldPosition - transform.position).normalized;
	    //Velocity = dir * 10 * Time.deltaTime;

	    if (path.Count == -1)
	    {
		    print("path");
		    return;
	    }
	    
	    Velocity = (path[path.Count - 1].worldPosition - transform.position).normalized * 10 * Time.deltaTime;
	    
	    //FindPath(StartPos, EndPos);
	    
	    transform.position += Velocity;
    }

    private void OnDrawGizmos()
    {
	    if (path != null)
	    {
		    foreach (Node n in path)
		    {
			    Color white = new Color(1, 1, 1, 1);
			    Color red = new Color(1, 0, 0, 1);

			    Gizmos.color = Color.black;// (n.walkable) ? white : red;
			    Gizmos.DrawWireCube(n.worldPosition, Vector3.one * (grid.nodeRadius-.1f));
		    }
	    }
    }

    // Update is called once per frame
    void UpdatePath()
    {
	    EndPos = TargetObject.position;
	    StartPos = transform.position;
	    
        if (grid)
        {
	        StopCoroutine(FindPath(StartPos,EndPos));
	        StartCoroutine(FindPath(StartPos,EndPos));
	        //FindPath(StartPos,EndPos);
        }
    }
    //IEnumerator
    public IEnumerator FindPath(Vector3 startPos, Vector3 targetPos)
	{
		Node startNode = grid.NodeFromWorldPoint(startPos);
		Node targetNode = grid.NodeFromWorldPoint(targetPos);
		
		//UKismetSystemLibrary::DrawDebugBox(this, startNode->worldPosition,	
	    //FVector::OneVector * grid->nodeRadius / 2, FColor::Black, FVector::ZeroVector.Rotation(), 0.f,3);
		//UKismetSystemLibrary::DrawDebugBox(this, targetNode->worldPosition,	
		//FVector::OneVector * grid->nodeRadius / 2, FColor::Black, FVector::ZeroVector.Rotation(), 0.f,3);

		if (startNode == null || targetNode == null || startNode == targetNode)	
			yield return null;//return;
		
		List<Node> openSet = new List<Node>();
		List<Node> closedSet = new List<Node>();
		
		openSet.Add(startNode);
		
		while (openSet.Count > 0)
		{		
			Node node = openSet[0];

			for (int i = 1; i < openSet.Count; i++)
			{		
			    if (openSet[i].fCost < node.fCost || openSet[i].fCost == node.fCost)
	            {
		            if (openSet[i].hCost < node.hCost)
		            {
	            		node = openSet[i];	
		            }		    	
	            }
			}

			openSet.Remove(node);
			closedSet.Add(node);

			if (node == targetNode)
			{
				RetracePath(startNode, targetNode);
				yield return null;
				//return;
			}

	        foreach (Node neighbour in grid.GetNeighbours(node))
	        {
		        if (!neighbour.walkable || closedSet.Contains(neighbour))
		        {
			        continue;
		        }

		        int newCostToNeighbour = node.gCost + GetDistance(node, neighbour) + neighbour.ObstacleCost;

		        if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
		        {
			        neighbour.gCost = newCostToNeighbour;
			        neighbour.hCost = GetDistance(neighbour, targetNode);
			        neighbour.parent = node;			

			        neighbour.init(neighbour.walkable, neighbour.worldPosition, neighbour.gridX, neighbour.gridY,
				        neighbour.ObstacleCost, neighbour.gridIndex );

			        if (!openSet.Contains(neighbour))
				        openSet.Add(neighbour);
		        }
	        }
		}

		if (startNode != targetNode)
		{		
			PathFailed();
			Velocity = Vector3.zero;
			//return;
			yield return null;
		}

		for (int i = 1; i < closedSet.Count; i++)
		{
			if (closedSet[i] != null)
			{
				float boxSize = grid.nodeRadius - (grid.nodeRadius / 10);
				
				//UKismetSystemLibrary::DrawDebugBox(this, closedSet[i]->worldPosition,
				//FVector::OneVector * boxSize, FColor::Yellow, GetOwner()->GetActorRotation(), 0.f,1);
			}
		}

		
		yield return new WaitForSeconds(0.5f);
		//return;
		//StartCoroutine(FindPath(StartPos,EndPos));//
		UpdatePath();
	}

    void RetracePath(Node startNode, Node endNode)
    {
	    List<Node> currentPath = new List<Node>();
	    
	    
	    //path.Em;
	    //path.SetNum(0,true);
	
	    Node currentNode = endNode;

	    while (currentNode != startNode)
	    {
		    if (!currentPath.Contains(currentNode)){}
		    currentPath.Add(currentNode);

		    int dirX = (currentNode.gridX - currentNode.parent.gridX);
		    int dirY = (currentNode.gridY - currentNode.parent.gridY);

		    currentNode.Direction = new Vector2(dirX, dirY);
		
		    currentNode = currentNode.parent;
	    }

	    if (jumpPoint)
	    {
		    currentPath = SearchDirection(path);
	    }

	    path = currentPath;

	    TargetIndex = path.Count - 1;
    }

    int GetDistance(Node nodeA, Node nodeB)
    {
	    int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
	    int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);
	
	    if (dstX > dstY)
		    return 14 * dstY + 10 * (dstX - dstY);
	    return 14 * dstX + 10 * (dstY - dstX);	
    }

    List<Node> SearchDirection(List<Node> SearchPath)
    {
	    List<Node> TempPath = SearchPath;
	    Node currentNode = SearchPath[0];
	
	    while (currentNode != SearchPath[SearchPath.Count - 1])
	    {
		    if (currentNode.parent == null)
			    return TempPath;
		
		    if (currentNode.Direction == currentNode.parent.Direction)
		    {
			    TempPath.Remove(currentNode.parent);
			    currentNode = currentNode.parent;
		    }
		    else
		    {			
			    currentNode = currentNode.parent;
		    }
	    }

	    for (int i = 0; i < TempPath.Count - 1; i++)
	    {
		    float boxSize = grid.nodeRadius - (grid.nodeRadius / 10);
		
		    //UKismetSystemLibrary::DrawDebugBox(this, TempPath[i]->worldPosition,
			//    FVector::OneVector * boxSize, Color, GetOwner()->GetActorRotation(), 0.f,1);
			//
		    //UKismetSystemLibrary::DrawDebugLine(this,Start,TempPath[TempPath.Num() - 1]->worldPosition,Color);	
		    //UKismetSystemLibrary::DrawDebugLine(this,TempPath[i]->worldPosition,TempPath[i + 1]->worldPosition,Color);	
	    }

	    return TempPath;
    }
    
    void PathFailed()
    {
	    print("AStar, Did not find path");
    }
}
