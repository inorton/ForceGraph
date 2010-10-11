using System;
using System.Collections.Generic;

namespace ForceGraph
{
	public class Node
	{
		public Guid NodeId { get; private set; }

		
		public Node ()
		{
			NodeId = Guid.NewGuid ();
			links = new List<Spring> ();
			Location = new Point ();
			Charge = 1;
			Mass = 0.1;
			Velocity = new Vector3D ();
			NetForce = new Vector3D ();
		}
		
		private List<Spring> links;
		public IList<Spring> Links {
			get { return links; }}
		public object Data { get; set; }
		public Point Location { get; set; }
		public double Charge { get; set; }
		public double Mass { get; set; }
		public Vector3D Velocity { get; set; }
		public Vector3D NetForce { get; set; }
		
		public override string ToString ()
		{
			return string.Format ("[Node: Location={0}, Velocity={1}]", Location, Velocity.ToString());
		}

		public override int GetHashCode ()
		{
			return NodeId.GetHashCode ();
		}
		
		public override bool Equals (object obj)
		{
			var n = obj as Node;
			if (n != null) {
				return (n.NodeId.Equals (this.NodeId));
			}
			return false;
		}
		


	}
}

