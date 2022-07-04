using System;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

namespace MyFramework
{

	public class AllNodePath : Path
    {
		public GraphNode startNode;
		public Vector3 startPoint;
		public Vector3 originalStartPoint;

		public uint maxGScore;

		public List<GraphNode> allNodes;

		public override bool FloodingPath => true;



		public static AllNodePath Construct(Vector3 start, int maxGScore, OnPathDelegate callback = null)
		{
			var p = PathPool.GetPath<AllNodePath>();

			p.Setup(start, maxGScore, callback);
			return p;
		}

		protected void Setup(Vector3 start, int maxGScore, OnPathDelegate callback)
		{
			this.callback = callback;
			startPoint = start;
			originalStartPoint = startPoint;

			this.maxGScore = (uint)maxGScore;
			allNodes = new List<GraphNode>();	
		}

        protected override void OnEnterPool()
        {
            base.OnEnterPool();
			allNodes.Clear();
		}

		protected override void Reset()
		{
			base.Reset();
			allNodes = new List<GraphNode>();
			originalStartPoint = Vector3.zero;
			startPoint = Vector3.zero;
			startNode = null;
			heuristic = Heuristic.None;
		}

		protected override void Prepare()
		{
			nnConstraint.tags = enabledTags;
			var startNNInfo = AstarPath.active.GetNearest(startPoint, nnConstraint);

			startNode = startNNInfo.node;
			if (startNode == null)
			{
				FailWithError("Could not find close node to the start point");
				return;
			}
		}

		protected override void Initialize()
		{
			PathNode startRNode = pathHandler.GetPathNode(startNode);

			startRNode.node = startNode;
			startRNode.pathID = pathHandler.PathID;
			startRNode.parent = null;
			startRNode.cost = 0;
			startRNode.G = GetTraversalCost(startNode);
			startRNode.H = CalculateHScore(startNode);

			startNode.Open(this, startRNode, pathHandler);

			searchedNodes++;

			startRNode.flag1 = true;
			allNodes.Add(startNode);

			if (pathHandler.heap.isEmpty)
			{
				CompleteState = PathCompleteState.Complete;
				return;
			}

			currentR = pathHandler.heap.Remove();
		}

		protected override void Cleanup()
		{
			int c = allNodes.Count;

			for (int i = 0; i < c; i++) pathHandler.GetPathNode(allNodes[i]).flag1 = false;
		}

		protected override void CalculateStep(long targetTick)
		{
			int counter = 0;

			while (CompleteState == PathCompleteState.NotCalculated)
			{
				searchedNodes++;

				if (currentR.G <= maxGScore)
				{
					if (!currentR.flag1)
					{
						allNodes.Add(currentR.node);
						currentR.flag1 = true;
					}

#if ASTARDEBUG
				Debug.DrawRay((Vector3)currentR.node.position, Vector3.up*5, Color.cyan);
#endif

					AstarProfiler.StartFastProfile(4);
					currentR.node.Open(this, currentR, pathHandler);
					AstarProfiler.EndFastProfile(4);
				}

				if (pathHandler.heap.isEmpty)
				{
					CompleteState = PathCompleteState.Complete;
					break;
				}

				AstarProfiler.StartFastProfile(7);
				currentR = pathHandler.heap.Remove();
				AstarProfiler.EndFastProfile(7);

				if (counter > 500)
				{
					if (DateTime.UtcNow.Ticks >= targetTick)
					{
						return;
					}
					counter = 0;

					if (searchedNodes > 1000000)
					{
						throw new Exception("Probable infinite loop. Over 1,000,000 nodes searched");
					}
				}

				counter++;
			}
		}
	}

}