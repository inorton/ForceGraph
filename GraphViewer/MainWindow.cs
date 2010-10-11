using System;
using System.Threading;

using Gtk;
using Cairo;

using ForceGraph;

public partial class MainWindow : Gtk.Window
{
	
	public MainWindow () : base(Gtk.WindowType.Toplevel)
	{
		Build ();
		
		CairoGraphic a = new CairoGraphic ();
		
		Box box = new HBox (true, 0);
		
		box.Add (a);
		
		this.Add (box);
		
		this.ShowAll ();
		
		
		Graph g = new Graph ();
		Node q1 = new Node ();
		Node q2 = new Node ();
		Node q3 = new Node ();
		Node q4 = new Node ();
		q1.Location = new ForceGraph.Point(){ X = 1, Y = 5, Z = 0 };
		q2.Location = new ForceGraph.Point(){ X = 0, Y = -2, Z = 0 };
		q3.Location = new ForceGraph.Point(){ X = -4, Y = 3, Z = 5 };
			
		g.AddNode( q1 );			
		g.AddNode( q2 );
		g.AddNode( q3 );	
		g.AddNode( q4 );
		
		Node.Join( q1, q3 );
		Node.Join( q1, q2 );
		Node.Join( q3, q2 );
		Node.Join( q2, q4 );
		a.ForceGraph = g;

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
    		DrawGraph (g);
		}
    	return true;
    }
	
	public void DrawGraph (Context gr)
	{
		if (ForceGraph != null) {
			ForceGraph.ComputeFull (0.0001, null);
			
			foreach (var n in ForceGraph.Nodes) {
				DrawFilledCircle (gr, 
					100 + (30 * n.Location.X),
					100 + (30 * n.Location.Y),
					5.5,
					new Color (0.1, 0.1, 0.1),
					new Color (0.2, 0.5, 0.1)
					);
			}
			
		}
	}
}
