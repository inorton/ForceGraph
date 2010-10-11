using System;
using System.Collections.Generic;

namespace ForceGraph
{
	public class Graph
	{
		

		public static double CoulombConstant = 9;
		public static double SpringConstant  = 3;

		public static double Damping = 0.4; // slow the graph compute down
		public static double RestThreshold = 2; // declare the graph done when TotalKineticEnergy is less than this
		public static double ComputeTimeStep = 0.1; // iterations per second
		
		public Graph ()
		{
			nodes = new List<Node> (10);
		}
		
		private double totalKE = 100;
		public double TotalKineticEnergy {
			get { return totalKE; } 
		}
		
		
		private List<Node> nodes;
		public IEnumerable<Node> Nodes {
			get { return nodes; }
		}
		
		
		public void AddNode (Node node)
		{
			foreach (var n in nodes) {
				if (n.Location.Equals (node.Location)) {
					throw new InvalidOperationException ("Another node already exists at this exact location ");
				}
			}
			nodes.Add (node);
		}
		
		public void ComputeFull (double KEChange, Action<Node> callback )
		{
			double fke = 100;
			do {
				Compute (callback);
				double ke = TotalKineticEnergy;
				double kediff = Math.Max (fke, ke) - Math.Min (fke, ke);	
				if (kediff < KEChange)
					break;

				fke = ke;
			} while (true);
		}
		
		// compute one step
		public void Compute ( Action<Node> callback )
		{
			totalKE = 0.0;
			foreach (var a in Nodes) {
				// calculate repulsive forces
				a.NetForce = new Vector3D ();
				foreach (var b in Nodes) {
					if (!b.Equals (a)) {
						a.NetForce += (a.Location - b.Location).UnitVector * Repulsion (a, b);
				//		Console.WriteLine ("repulsion {0}", Repulsion (a, b));
					}
				}
			
				// calculate spring forces
				foreach (var s in a.Links) {
					a.NetForce += (a.Location - s.Other (a).Location).UnitVector * SpringAttraction (a, s);
				//	Console.WriteLine ("attraction {0}", SpringAttraction (a, s));
				}
			}
			
			
			// move and calculate KE
			foreach (var x in Nodes) {
	
				Vector3D vel = new Vector3D(){ 
					X = x.NetForce.X * ComputeTimeStep * Damping,
					Y = x.NetForce.Y * ComputeTimeStep * Damping,
					Z = x.NetForce.Z * ComputeTimeStep * Damping
				};
				
				x.Location += new Point(){
					X = vel.X * ComputeTimeStep,
					Y = vel.Y * ComputeTimeStep,
					Z = vel.Z * ComputeTimeStep
				};
				
				totalKE += x.Mass * Math.Pow( vel.Length , 2 );
				
				if ( callback != null ){
					callback( x );
				}
			}
			
			Console.WriteLine("KE = {0}",totalKE);
		}
		
		
		public static double Repulsion (Node a, Node b)
		{
			double Q = a.Charge * b.Charge;
			double r = Point.Distance (a.Location, b.Location);
			double F = ( CoulombConstant * Q ) / Math.Pow( r, 2 );
			
			return F;
		}
		
		public static double SpringAttraction (Node a, Spring p)
		{
			if (!a.Links.Contains (p))
				throw new InvalidOperationException ("Node is not attached to this spring");
			
			double r = Point.Distance (a.Location , p.Other(a).Location);
			double x = r - p.NaturalLength;	
			double F = (-1 * SpringConstant * x);

			return F;
		}
	}
}

