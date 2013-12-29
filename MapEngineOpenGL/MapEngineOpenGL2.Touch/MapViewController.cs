using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.GLKit;
using MonoTouch.OpenGLES;

namespace MapEngineOpenGL
{
	public partial class MapViewController : GLKViewController
	{
		private static string FILE_PATH = "01.txt";        //地図データ
		EAGLContext context;
		MapView mapView;
		int meshFactor;
		Size size;



		public MapViewController () : base ()
		{
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

		public override void LoadView ()
		{
			base.LoadView ();
			mapView = new MapView (this.View.Frame, FILE_PATH);
			this.View = mapView;
		}

		public override void ViewDidLoad ()
		{
			bool isPad = UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad;
			base.ViewDidLoad ();

			context = new EAGLContext (EAGLRenderingAPI.OpenGLES2);
			mapView.Context = context;
			mapView.MultipleTouchEnabled = true;
			mapView.DrawInRect += Draw;

			PreferredFramesPerSecond = 60;
			size = UIScreen.MainScreen.Bounds.Size.ToSize ();
			View.ContentScaleFactor = UIScreen.MainScreen.Scale;

			meshFactor = isPad ? 8 : 4;
			SetupGL ();
			//SetupAVCapture (isPad ? AVCaptureSession.PresetiFrame1280x720 : AVCaptureSession.Preset640x480);
			/*base.ViewDidLoad ();
			var mView = (MapView)this.View;

			mView.Context = new EAGLContext (EAGLRenderingAPI.OpenGLES2);
			EAGLContext.SetCurrentContext(((GLKView)this.View).Context);

			mView.InitMap ();*/
		}

		void SetupGL ()
		{
			EAGLContext.SetCurrentContext (context);
			mapView.SetUpGL ();
		}

		void Draw (object sender, GLKViewDrawEventArgs args)
		{
			mapView.Draw ();
			//GL.Clear ((int)All.ColorBufferBit);
			//if (ripple != null)
			//	GL.DrawElements (All.TriangleStrip, ripple.IndexCount, All.UnsignedShort, IntPtr.Zero);
		}
	}
}

