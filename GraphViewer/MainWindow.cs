using System;
using System.Threading;

using Gtk;
using Cairo;

using ForceGraph;

public partial class MainWindow : Gtk.Window
{
	private CairoGraphic area;
	private Node drag = null;
	
	public MainWindow () : base(Gtk.WindowType.Toplevel)
	{
		Build ();
		
		area = new CairoGraphic ();
		
		Box box = new HBox (true, 0);
		
		box.Add (area);
		
		this.Add (box);
		
		this.ShowAll ();
		
		
		Graph g = new Graph ();
		
		Graph.CoulombConstant += 15;
		Graph.SpringConstant += 25;
		Graph.Damping = 0.8;
		
		Node q1 = new Node (){ Mass = 3 };
		Node q2 = new Node (){ Mass = 2 };
		Node q3 = new Node ();
		Node q4 = new Node ();
		Node q5 = new Node ();
		Node q6 = new Node ();
		Node q7 = new Node ();
			
		
		q1.Charge = 1.5;
		q1.Data = new NodeData(){ 
			Label = "CA KGroup",
			Stroke = new Color( 0.5, 0.5, 0.5, 0.5 ),
			Fill = new Color( 1, 1, 1, 1 ),
			Size = 60 };
		

		q2.Charge = 1.5;
		q2.Data = new NodeData(){ 
			Label = "CA AGroup",
			Stroke = new Color( 0.5, 0.5, 0.5, 0.5 ),
			Fill = new Color( 1, 1, 1, 1 ),
			Size = 60 };
		
		q1.Location = new ForceGraph.Point(){ X = 12, Y = 5, Z = 0 };
		q2.Location = new ForceGraph.Point(){ X = 0, Y = -2, Z = 0 };
		q3.Location = new ForceGraph.Point(){ X = -4, Y = 13, Z = 5 };
		q5.Location = new ForceGraph.Point(){ X = 1, Y = 0, Z = 0 };
		q6.Location = new ForceGraph.Point(){ X = 2, Y = 10, Z = 0 };	
		q7.Location = new ForceGraph.Point(){ X = 3, Y = -6, Z = 0 };
		
		
		g.AddNode( q1 );			
		g.AddNode( q2 );
		g.AddNode( q3 );	
		g.AddNode( q4 );
		g.AddNode( q5 );
		g.AddNode( q6 );
		g.AddNode( q7 );
		
		g.Join( q1, q5 ).Data = new NodeData(){ Stroke = new Color( 0,0,0,0.1 ) };
		g.Join( q1, q6 ).Data = new NodeData(){ Stroke = new Color( 0,0,0,0.1 ) };
		g.Join( q1, q7 ).Data = new NodeData(){ Stroke = new Color( 0,0,0,0.1 ) };
		
		
		var grant = g.Join( q1, q2 );
		grant.NaturalLength = 12;
		
		g.Join( q2, q3 ).Data = new NodeData(){ Stroke = new Color( 0,0,0,0.1 ) };
		g.Join( q2, q4 ).Data = new NodeData(){ Stroke = new Color( 0,0,0,0.1 ) };
		
		area.ForceGraph = g;
		area.Magnification = 30;
		
				
		
		Thread t = new Thread( new ThreadStart( this.Render) );
		t.Start();
		
		area.AddEvents( (int) Gdk.EventMask.ButtonPressMask );
		area.AddEvents( (int) Gdk.EventMask.ButtonReleaseMask );
		area.AddEvents( (int) Gdk.EventMask.Button1MotionMask );
		area.Sensitive = true;

		area.ButtonPressEvent += HandleHandleButtonPressEvent;
		area.ButtonReleaseEvent += HandleAreaButtonReleaseEvent;
		
	}

	void HandleAreaButtonReleaseEvent (object o, ButtonReleaseEventArgs args)
	{
		if (args.Event.Button == 1) {
			Console.WriteLine("mouse up");
			area.MotionNotifyEvent -= HandleAreaMotionNotifyEvent;
			drag = null;
		}
	}

	void HandleAreaMotionNotifyEvent (object o, MotionNotifyEventArgs args)
	{
		if ( drag != null ){
			Console.WriteLine("move");
			double x = ( args.Event.X + (area.Allocation.Width * -0.5)  )/ area.Magnification;
			double y = ( args.Event.Y + (area.Allocation.Height * -0.5)  )/ area.Magnification;
			
			drag.Location.X = x;
			drag.Location.Y = y;
			
			area.ForceGraph.Compute(null);
			area.QueueDraw();
			
		}
	}

	void HandleHandleButtonPressEvent (object o, ButtonPressEventArgs args)
	{
		if (drag != null ) return;
		if (args.Event.Button == 1) {
			double x = ( args.Event.X + (area.Allocation.Width * -0.5)  )/ area.Magnification;
			double y = ( args.Event.Y + (area.Allocation.Height * -0.5)  )/ area.Magnification;
			
			double size = 0.5;
			
			Console.WriteLine( "mouse down {0},{1}", x,y );
			
			foreach ( var n in area.ForceGraph.Nodes ){
				double minx = n.Location.X - size;
				if ( x > minx ){
					double maxx = n.Location.X + size;
					if ( x < maxx ){
						double miny = n.Location.Y - size;
						if ( y > miny ){
							double maxy = n.Location.Y + size;
							if ( y < maxy ){
								drag = n;
								break;
							}
						}
					}
				}				
			}
			
			if ( drag != null ){
				area.MotionNotifyEvent += HandleAreaMotionNotifyEvent;
			}
		}
	}
	

	
	public void Render ()
	{
		do {
			lock (area.ForceGraph) {
				if (area.ForceGraph.TotalKineticEnergy > 0.0025) {
					area.ForceGraph.Compute (null);
					
					Application.Invoke (delegate {
						area.QueueDraw ();
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


public class NodeData {
	public string Label { get; set; }
	public double Size { get; set; }
	public Color Stroke { get; set; }
	public Color Fill { get; set; }
	public double StrokeSize { get; set; }
}

public class CairoGraphic : DrawingArea
{
	
	public int Magnification { get; set; }
	
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
    		lock (ForceGraph) {
    			DrawGraph (g);
			}
		}
    	return true;
    }
	
	public void DrawGraph (Context gr)
	{
		int mag = Magnification;
		double xoffset =  (Allocation.Width/2) ;
		double yoffset =  (Allocation.Height/2) ;
		
		if (ForceGraph != null) {
			this.GdkWindow.Clear ();
			gr.Antialias = Antialias.Subpixel;
			foreach (var p in ForceGraph.Springs){			
				gr.LineWidth = 1.5;
				
				if ( p.Data != null ){
					var data = p.Data as NodeData;
					if ( data != null )
						gr.Color = data.Stroke;
				} else {
					gr.Color = new Color( 0,0,0,1);
				}
				gr.MoveTo( xoffset + ( p.NodeA.Location.X * mag ), yoffset + ( p.NodeA.Location.Y * mag ) );
				gr.LineTo( xoffset + ( p.NodeB.Location.X * mag ), yoffset + ( p.NodeB.Location.Y * mag ) );
				gr.Stroke();
				
			}
			
			foreach (var n in ForceGraph.Nodes) {
				var stroke = new Color( 0.1, 0.1, 0.1, 0.8 );
				var fill   = new Color( 0.2, 0.7, 0.7, 0.8 );
				var size = 5.5;
				
				NodeData nd = n.Data as NodeData;
				if ( nd != null ){
					stroke = nd.Stroke;
					fill = nd.Fill;
					size = nd.Size;
					
					
				}
				
				DrawFilledCircle (gr, 
					xoffset + (mag * n.Location.X),
					yoffset + (mag * n.Location.Y),
					size,
					stroke,
					fill
					);
				
				if ( nd != null ) {
					if ( nd.Label != null ){
						gr.Color = new Color(0,0,0,0.7);
						gr.SetFontSize(24);
						gr.MoveTo( 25 + xoffset + (mag * n.Location.X), 25 + yoffset + (mag * n.Location.Y));
						gr.ShowText( nd.Label );						
					}
				}
				
			}
			
		}
	}
}
