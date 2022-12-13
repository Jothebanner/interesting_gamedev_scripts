using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPriorities : MonoBehaviour
{
	public List<GameObject> targets { get; set; }



	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}

public class priorityObject
{
    private int priorityRank;

    public priorityObject(int initalRank)
	{
        priorityRank = initalRank;
	}

    public void ChangePriority(int priorityChange)
    {
        priorityRank += priorityChange;
    }

    public int GetPriorityRank()
	{
        return priorityRank;
	}
}

