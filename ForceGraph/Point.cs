using System;
namespace ForceGraph
{
	public class Vector3D
	{
		public double X { get; set; }
		public double Y { get; set; }
		public double Z { get; set; }
		
		public bool Equals (Point obj)
		{
			return ((obj.X == X) && (obj.Y == Y) && (obj.Z == Z));
		}
	
		public Vector3D UnitVector
		{
			get {
				double sqdotp = Math.Sqrt((X * X) + (Y * Y) + (Z * Z));
				return new Vector3D(){ X = this.X / sqdotp, Y = this.Y / sqdotp, Z = this.Z / sqdotp };
			}
		}
		
		public double Length 
		{
			get {
				return Math.Sqrt (Math.Pow (X, 2) + Math.Pow (Y, 2) + Math.Pow (Z, 2));
			}
		}
		
		public static Vector3D operator +(Vector3D a, Vector3D b)
		{
			return new Vector3D(){ X = a.X + b.X, Y = a.Y + b.Y , Z = a.Z + b.Z };
		}		
		
		public static Vector3D operator * (Vector3D a, Vector3D b)
		{
			return new Vector3D(){ X = a.X * b.X, Y = a.Y * b.Y , Z = a.Z * b.Z };
		}
		
		public static Vector3D operator * (Vector3D a, double x)
		{
			return new Vector3D(){ X = a.X * x, Y = a.Y * x , Z = a.Z * x };
		}
		
		public static Vector3D operator - (Vector3D a, Vector3D b)
		{
			return new Vector3D(){ X = a.X - b.X, Y = a.Y - b.Y , Z = a.Z - b.Z };
		}
		
		public override string ToString ()
		{
			return string.Format ("[{0},{1},{2}]",X,Y,Z);
		}
		
	}
	
	public class Point : Vector3D
	{
		public static double Distance (Point a, Point b)
		{
			Vector3D tmp = a - b;
			
			return Math.Sqrt (
				Math.Pow (tmp.X, 2) +
				Math.Pow (tmp.Y, 2) +
				Math.Pow (tmp.Z, 2));
			
		}
	
		public static Point operator +(Point a, Vector3D b)
		{
			return new Point(){ X = a.X + b.X, Y = a.Y + b.Y , Z = a.Z + b.Z };
		}		
		

		
	}
}

