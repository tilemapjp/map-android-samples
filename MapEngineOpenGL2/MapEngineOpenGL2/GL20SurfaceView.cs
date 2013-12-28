using System;
using Android.Opengl;
using Android.Content;

namespace MapEngineOpenGL2
{
	public class GL20SurfaceView : GLSurfaceView
	{
		private GL20MyRenderer mRenderer = null;        //描画処理本体
		private Context mContext;        //Context
		private MapView mMapView = null;        //MapView

		//コンストラクタ
		public GL20SurfaceView(Context context, MapView mapView) : base(context.ApplicationContext)
		{
			mContext = context.ApplicationContext;
			mMapView = mapView;
			//OpenGLES2.0の使用を宣言します。
			SetEGLContextClientVersion(2);
			//レンダラーを初期化し、ビューにセットします。
			mRenderer = new GL20MyRenderer(mContext,this, mMapView);
			SetRenderer(mRenderer);
		}

		public void SetMatrix(GMatrix viewmatrix, GMatrix projectionmatrix) {
			this.mRenderer.SetMatrix(viewmatrix, projectionmatrix);
		}
	}
}

