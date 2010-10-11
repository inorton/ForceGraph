using System;
using ForceGraph;

using NUnit.Framework;
namespace Vector3DTest
{
	[TestFixture]
	public class Test
	{
		[Test]
		public void ComputeDistances ()
		{
			Point start = new Point ();
			
			int chk = 3;
			
			Point finish = new Point (){ X = start.X + chk, Y = start.Y + chk, Z = start.Z = -chk };
			
			double dist = Point.Distance( start, finish );
			
			double Qchk = ( chk * chk );
			
			double middle = Math.Sqrt( Qchk );
			
			double longest = Math.Sqrt( (middle*middle) + Qchk );
			
			Assert.AreEqual( longest, dist );
		}
		
		[Test]
		public void ComputeRepulstion()
		{
			Node q1 = new Node(){ Charge = 2 };
			Node q2 = new Node(){ Charge = 2 };
			q2.Location.X = 10;
			
			// F = Ke*Q1*Q2 / r*2
			
			double r = Point.Distance( q1.Location, q2.Location );
			
			double f = ( Graph.CoulombConstant * q1.Charge * q2.Charge ) / Math.Pow( r , 2 );
			
			Assert.AreEqual( f, Graph.Repulsion( q1, q2 ) );
		}
		
		
		[Test]
		public void Simple2NodeGraph ()
		{
			Graph g = new Graph ();
			Node q1 = new Node ();
			Node q2 = new Node ();
			
			q1.Location = new Point(){ X = 1, Y = 5, Z = 0 };
			q2.Location = new Point(){ X = 0, Y = -2, Z = 0 };
			
			g.AddNode( q1 );
			g.AddNode( q2 );
			
			g.Join( q1, q2 );
			
			int x = 0;
			double fke = 100;
			do {
				
				g.Compute(null);
				double ke = g.TotalKineticEnergy;
				
				double kediff = Math.Max( fke, ke ) - Math.Min( fke, ke );
				Console.WriteLine("kediff = {0}", kediff );
				
				if ( kediff < 0.001 ) break; 
				
				if ( x++ > 1000 ) break;
				
				fke = ke;
				
				
			} while ( true );
			
		}
		
		[Test]
		public void Simple3NodeGraph ()
		{
			Graph g = new Graph ();
			Node q1 = new Node ();
			Node q2 = new Node ();
			Node q3 = new Node ();
			
			q1.Location = new Point(){ X = 1, Y = 5, Z = 0 };
			q2.Location = new Point(){ X = 0, Y = -2, Z = 0 };
			q3.Location = new Point(){ X = -4, Y = 3, Z = 5 };
			
			g.AddNode( q1 );			
			g.AddNode( q2 );
			g.AddNode( q3 );	
			
			g.Join( q1, q3 );
			g.Join( q1, q2 );
			
			g.ComputeFull( 0.0001 , null );
		}
	}
}


