using System;
using Android.Opengl;
using Android.Content;

namespace MapEngineOpenGL
{
	public class GL20SurfaceView : GLSurfaceView
	{
		private GL20MyRenderer mRenderer = null; //描画処理本体

		//コンストラクタ
		public GL20SurfaceView(Context context, MapController SC ) : base(context.ApplicationContext)
		{
			//OpenGLES2.0の使用を宣言します。
			SetEGLContextClientVersion(2);
			//レンダラーを初期化し、ビューにセットします。
			mRenderer = new GL20MyRenderer(SC);
			SetRenderer(mRenderer);
		}

		public void SetMatrix(GMatrix viewmatrix, GMatrix projectionmatrix) {
			this.mRenderer.SetMatrix(viewmatrix, projectionmatrix);
		}
	}
}

