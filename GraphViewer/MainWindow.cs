using System;
using Gtk;


using ForceGraph;

public partial class MainWindow : Gtk.Window
{
		
	public MainWindow () : base(Gtk.WindowType.Toplevel)
	{
		Build ();
		
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
		
		Node.Join( q1, q3 );
		Node.Join( q1, q2 );
		
		g.ComputeFull( 0.0001, delegate( Node n ){ 
			
		} );

	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}
}

