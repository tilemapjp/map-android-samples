using System;
using MonoTouch.UIKit;
using OpenTK.Platform.iPhoneOS;
using OpenTK.Graphics.ES20;
using System.Drawing;
using MonoTouch.ObjCRuntime;
using MonoTouch.CoreAnimation;
using OpenTK.Graphics;
using MonoTouch.Foundation;
using MonoTouch.OpenGLES;
using MonoTouch.GLKit;

namespace MapEngineOpenGL
{
	public class MapView : GLKView, IMapPresenter
	{
		public int Height {
			get { return (int)this.Frame.Height; }
		}

		public int Width {
			get { return (int)this.Frame.Width; }
		}

		private MapController mSC = null; //共通処理
		private GL20MyRenderer mRD = null;

		private PointF mFormer;
		private float  fScale;

		public MapView (RectangleF frame, string filename) : base (frame)
		{
			//地図データを読み込む
			mSC = new MapController(this, filename);

			//地図操作クラスを初期化
			mSC.SetEyePosition(140000.0f, 50000.0f, MapController.BASE_Z * mSC.scale, 0);

			var panRecogniser = new UIPanGestureRecognizer(this, new MonoTouch.ObjCRuntime.Selector("MapPanSelector"));
			this.AddGestureRecognizer(panRecogniser);

			var pinchRecognizer = new UIPinchGestureRecognizer (this, new MonoTouch.ObjCRuntime.Selector ("MapPinchSelector"));
			this.AddGestureRecognizer (pinchRecognizer);

			//描画のためのビューを初期化
			//mSurfaceView = new GL20SurfaceView(mContext, this);

			//SurfaceViewを追加する
			//this.AddView(mSurfaceView);

			//ジェスチャー操作
			//mGd = new GestureDetector(mContext, mMc);

			//タッチイベントを登録
			//this.SetOnTouchListener(this);
		}

		public void SetUpGL()
		{
			mRD = new GL20MyRenderer (mSC);
			mRD.OnSurfaceCreated ();
			mRD.OnSurfaceChanged (this.Width, this.Height);
		}

		public void Draw()
		{
			mRD.OnDrawFrame ();
		}

		public void SetMatrix(GMatrix viewmatrix, GMatrix projectionmatrix)
		{
			mRD.SetMatrix (viewmatrix, projectionmatrix);
		}

		[Export("MapPinchSelector")]
		protected void OnMapPinch(UIPinchGestureRecognizer sender)
		{
			var pinchRecognizer = sender as UIPinchGestureRecognizer;

			if (pinchRecognizer == null)
				return;

			switch (pinchRecognizer.State) 
			{
				case UIGestureRecognizerState.Began:
					lock (this) {
						fScale = pinchRecognizer.Scale;
					}
					//_originalPosition = DragLabel.Frame.Location;
					break;

				case UIGestureRecognizerState.Cancelled:
				case UIGestureRecognizerState.Failed:
					//DragLabel.Frame = new RectangleF(_originalPosition, DragLabel.Frame.Size);
					break;

				case UIGestureRecognizerState.Changed:
					float nScale, scaleFactor;
					lock (this) {
						nScale = pinchRecognizer.Scale;
						scaleFactor = nScale / fScale;
						fScale = nScale;
					}
					//var loc = panRecogniser.LocationInView (this);
					//var mv = panRecogniser.VelocityInView(this);

					mSC.OnScale (scaleFactor);
					break;
			}

		}

		[Export("MapPanSelector")]
		protected void OnMapPan(UIGestureRecognizer sender)
		{
			var panRecogniser = sender as UIPanGestureRecognizer;

			if (panRecogniser == null)
				return;

			switch (panRecogniser.State)
			{
				case UIGestureRecognizerState.Began:
					lock (this) {
						mFormer = panRecogniser.LocationInView (this);
					}
					//_originalPosition = DragLabel.Frame.Location;
					break;

				case UIGestureRecognizerState.Cancelled:
				case UIGestureRecognizerState.Failed:
					//DragLabel.Frame = new RectangleF(_originalPosition, DragLabel.Frame.Size);
					break;

				case UIGestureRecognizerState.Changed:
					PointF loc, mv; 
					lock (this) {
						loc = panRecogniser.LocationInView (this);
						mv = new PointF (loc.X - mFormer.X, loc.Y - mFormer.Y);
						mFormer = loc;
					}
					//var loc = panRecogniser.LocationInView (this);
					//var mv = panRecogniser.VelocityInView(this);

					mSC.OnScroll (loc.X, loc.Y, -mv.X, -mv.Y);
					break;
			}
		}


		/*public void DrawInRect (GLKView view, RectangleF rect)
		{
			Console.WriteLine ("Draw");
		}*/

		/*iPhoneOSGraphicsContext _context;

		[Export ("layerClass")]
		public static Class LayerClass ()
		{
			return new Class (typeof (CAEAGLLayer));
		}

		public MapView (RectangleF frame)
			: this (frame, All.Rgb565, 0, false)
		{
		}

		public MapView (RectangleF frame, All format)
			: this (frame, format, 0, false)
		{
		}

		public MapView (RectangleF frame, All format, All depth, bool retained) : base (frame)
		{
			CAEAGLLayer eaglLayer = (CAEAGLLayer) Layer;
			//eaglLayer.DrawableProperties = NSDictionary.FromObjectsAndKeys (
			//	new NSObject [] {NSNumber.FromBoolean (true),           EAGLColorFormat.RGBA8},
			//	new NSObject [] {EAGLDrawableProperty.RetainedBacking,  EAGLDrawableProperty.ColorFormat}
			//);
			//_format = format;
			//_depthFormat = depth;

			_context = (iPhoneOSGraphicsContext) ((IGraphicsContextInternal) GraphicsContext.CurrentContext).Implementation;
			CreateSurface ();
		}

		/*protected override void Dispose (bool disposing)
		{
			DestroySurface ();
			_context.Dispose();
			_context = null;
		}*/

		/*void CreateSurface ()
		{
			CAEAGLLayer eaglLayer = (CAEAGLLayer) Layer;
			if (!_context.IsCurrent)
				_context.MakeCurrent(null);

			var newSize = eaglLayer.Bounds.Size;
			newSize.Width  = (float) Math.Round (newSize.Width);
			newSize.Height = (float) Math.Round (newSize.Height);

			int oldRenderbuffer = 0, oldFramebuffer = 0;
			GL.GetInteger (All.RenderbufferBindingOes, ref oldRenderbuffer);
			GL.GetInteger (All.FramebufferBindingOes, ref oldFramebuffer);

			GL.Oes.GenRenderbuffers (1, ref _renderbuffer);
			GL.Oes.BindRenderbuffer (All.RenderbufferOes, _renderbuffer);

			if (!_context.EAGLContext.RenderBufferStorage ((uint) All.RenderbufferOes, eaglLayer)) {
				GL.Oes.DeleteRenderbuffers (1, ref _renderbuffer);
				GL.Oes.BindRenderbuffer (All.RenderbufferBindingOes, (uint) oldRenderbuffer);
				throw new InvalidOperationException ("Error with RenderbufferStorage()!");
			}

			GL.Oes.GenFramebuffers (1, ref _framebuffer);
			GL.Oes.BindFramebuffer (All.FramebufferOes, _framebuffer);
			GL.Oes.FramebufferRenderbuffer (All.FramebufferOes, All.ColorAttachment0Oes, All.RenderbufferOes, _renderbuffer);
			if (_depthFormat != 0) {
				GL.Oes.GenRenderbuffers (1, ref _depthbuffer);
				GL.Oes.BindFramebuffer (All.RenderbufferOes, _depthbuffer);
				GL.Oes.RenderbufferStorage (All.RenderbufferOes, _depthFormat, (int) newSize.Width, (int) newSize.Height);
				GL.Oes.FramebufferRenderbuffer (All.FramebufferOes, All.DepthAttachmentOes, All.RenderbufferOes, _depthbuffer);
			}
			_size = newSize;
			if (!_hasBeenCurrent) {
				GL.Viewport (0, 0, (int) newSize.Width, (int) newSize.Height);
				GL.Scissor (0, 0, (int) newSize.Width, (int) newSize.Height);
				_hasBeenCurrent = true;
			}
			else
				GL.Oes.BindFramebuffer (All.FramebufferOes, (uint) oldFramebuffer);
			GL.Oes.BindRenderbuffer (All.RenderbufferOes, (uint) oldRenderbuffer);

			Action<EAGLView> a = OnResized;
			if (a != null)
				a (this);
		}

		void DestroySurface ()
		{
			EAGLContext oldContext = EAGLContext.CurrentContext;

			if (!_context.IsCurrent)
				_context.MakeCurrent(null);

			if (_depthFormat != 0) {
				GL.Oes.DeleteRenderbuffers (1, ref _depthbuffer);
				_depthbuffer = 0;
			}

			GL.Oes.DeleteRenderbuffers (1, ref _renderbuffer);
			_renderbuffer = 0;

			GL.Oes.DeleteFramebuffers (1, ref _framebuffer);
			_framebuffer = 0;

			EAGLContext.SetCurrentContext (oldContext);
		}*/
	}
}

