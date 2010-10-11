using System;
using System.Threading;

using Gtk;
using Cairo;

using ForceGraph;

public partial class MainWindow : Gtk.Window
{
	private CairoGraphic area;
	
	public MainWindow () : base(Gtk.WindowType.Toplevel)
	{
		Build ();
		
		area = new CairoGraphic ();
		
		Box box = new HBox (true, 0);
		
		box.Add (area);
		
		this.Add (box);
		
		this.ShowAll ();
		
		
		Graph g = new Graph ();
		
		Graph.CoulombConstant += 5;
		Graph.SpringConstant += 5;
		
		Node q1 = new Node ();
		Node q2 = new Node ();
		Node q3 = new Node ();
		Node q4 = new Node ();
		Node q5 = new Node ();
		Node q6 = new Node ();
		
		q1.Location = new ForceGraph.Point(){ X = 12, Y = 5, Z = 0 };
		q2.Location = new ForceGraph.Point(){ X = 0, Y = -2, Z = 0 };
		q3.Location = new ForceGraph.Point(){ X = -4, Y = 13, Z = 5 };
		q5.Location = new ForceGraph.Point(){ X = 1, Y = 0, Z = 0 };
		q6.Location = new ForceGraph.Point(){ X = 2, Y = 10, Z = 0 };	
		
		
		g.AddNode( q1 );			
		g.AddNode( q2 );
		g.AddNode( q3 );	
		g.AddNode( q4 );
		g.AddNode( q5 );
		g.AddNode( q6 );
		
		g.Join( q1, q3 );
		g.Join( q1, q2 );
		g.Join( q3, q2 );
		g.Join( q2, q4 );
		g.Join( q1, q5 );
		g.Join( q1, q6 );
		
		area.ForceGraph = g;
		
		
		Thread t = new Thread( new ThreadStart( this.Render) );
		t.Start();
	}
	
	public void Render()
	{
		do {
			lock ( area.ForceGraph ){
				if ( area.ForceGraph.TotalKineticEnergy > 0.0025 ){
					area.ForceGraph.Compute( null );
					
					Application.Invoke( delegate {
						using ( var ctx = Gdk.CairoHelper.Create( area.GdkWindow ) ){
							area.DrawGraph( ctx ); 	
						}
					} );
				}
			}
			Thread.Sleep(100);
		} while ( true );
	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}
}


public class CairoGraphic : DrawingArea
{
	
	public Graph ForceGraph { get; set; }
	
    static double min (params double[] arr)
    {
	int minp = 0;
	for (int i = 1; i < arr.Length; i++)
	    if (arr[i] < arr[minp])
		minp = i;
	
	return arr[minp];
    }
 
    static void DrawRoundedRectangle (Cairo.Context gr, double x, double y, double width, double height, double radius)
    {
	gr.Save ();
	
	if ((radius > height / 2) || (radius > width / 2))
	    radius = min (height / 2, width / 2);
	
	gr.MoveTo (x, y + radius);
	gr.Arc (x + radius, y + radius, radius, Math.PI, -Math.PI / 2);
	gr.LineTo (x + width - radius, y);
	gr.Arc (x + width - radius, y + radius, radius, -Math.PI / 2, 0);
	gr.LineTo (x + width, y + height - radius);
	gr.Arc (x + width - radius, y + height - radius, radius, 0, Math.PI / 2);
	gr.LineTo (x + radius, y + height);
	gr.Arc (x + radius, y + height - radius, radius, Math.PI / 2, Math.PI);
	gr.ClosePath ();
	gr.Restore ();
    }
    
	static void DrawFilledCircle (Cairo.Context g, double cx, double cy, double radius, Color stroke, Color fill)
	{
		double x = cx - radius;
		double y = cy - radius;
		
		g.Save ();
		g.Antialias = Antialias.Subpixel;
	    DrawRoundedRectangle (g, x, y, radius * 2, radius * 2, radius);
		g.Color = fill;
		g.FillPreserve ();
		g.Color = stroke;
		g.LineWidth = 5;
		g.Stroke ();
		
		g.Restore ();
	}
    
    protected override bool OnExposeEvent (Gdk.EventExpose args)
    {
    	using (Context g = Gdk.CairoHelper.Create (args.Window)) {
			lock ( ForceGraph ){
    			DrawGraph (g);
			}
		}
    	return true;
    }
	
	public void DrawGraph (Context gr)
	{
		int offset = 100;
		int mag    = 30;
		if (ForceGraph != null) {
			this.GdkWindow.Clear();
			gr.Antialias = Antialias.Subpixel;
			foreach (var p in ForceGraph.Springs){
				gr.LineWidth = 1.5;
				gr.Color = new Color( 0,0,0,1);
				gr.MoveTo( offset + ( p.NodeA.Location.X * mag ), offset + ( p.NodeA.Location.Y * mag ) );
				gr.LineTo( offset + ( p.NodeB.Location.X * mag ), offset + ( p.NodeB.Location.Y * mag ) );
				gr.Stroke();
				
			}
			
			foreach (var n in ForceGraph.Nodes) {
				DrawFilledCircle (gr, 
					offset + (mag * n.Location.X),
					offset + (mag * n.Location.Y),
					5.5,
					new Color (0.1, 0.1, 0.1),
					new Color (0.2, 0.5, 0.1)
					);
			}
			
		}
	}
}
