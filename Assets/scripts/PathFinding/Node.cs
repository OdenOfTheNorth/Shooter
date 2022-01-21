using UnityEngine;
using System.Collections;

public class Node : IHeapItem<Node> {
	
    public bool walkable;
    public Vector3 worldPosition;
    public Vector2 Direction;
    public int gridX;
    public int gridY;

    public int gCost;
    public int hCost;
    public Node parent;
    int heapIndex;
	
    public int ObstacleCost;
    public int gridIndex;
    
    //public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY) {
    //    walkable = _walkable;
    //    worldPosition = _worldPos;
    //    gridX = _gridX;
    //    gridY = _gridY;
    //    ObstacleCost = _ObstacleCost;
    //    gridIndex = index;
    //}
    
    public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY, int _ObstacleCost, int index)
    {                                                                                                        
        walkable = _walkable;                                                                                
        worldPosition = _worldPos;                                                                           
        gridX = _gridX;                                                                                      
        gridY = _gridY;                                                                                      
        ObstacleCost = _ObstacleCost;                                                                        
        //fCost = gCost + hCost;                                                                             
        gridIndex = index;                                                                                   
    }                                                                                                        
    
    public void init(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY, int _ObstacleCost, int index)
    {
        walkable = _walkable;
        worldPosition = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
        ObstacleCost = _ObstacleCost;
        //fCost = gCost + hCost;
        gridIndex = index;
    }

    public int fCost {
        get {
            return gCost + hCost;
        }
    }

    public int HeapIndex {
        get {
            return heapIndex;
        }
        set {
            heapIndex = value;
        }
    }

    public int CompareTo(Node nodeToCompare) {
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if (compare == 0) {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }
        return -compare;
    }
}