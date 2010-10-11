using System;
using System.Collections.Generic;

namespace ForceGraph
{
	public class Spring
	{		
		public Spring ()
		{
			
			NaturalLength = 0.75;
		}

		
		public Node NodeA { get; set; }
		public Node NodeB { get; set; }
		
		public Node Other (Node us)
		{
			if (NodeA.Equals(us))
				return NodeB;
			return NodeA;
		}
		
		public double NaturalLength { get; set; }

	}
}

