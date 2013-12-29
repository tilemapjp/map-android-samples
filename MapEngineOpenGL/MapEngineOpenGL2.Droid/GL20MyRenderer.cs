using System;
#if __ANDROID__
using Javax.Microedition.Khronos.Opengles;
using Android.Opengl;
using Javax.Microedition.Khronos.Egl;
using Java.Lang;
using OpenTK.Platform.Android;
#elif __IOS__
using OpenTK.Platform.iPhoneOS;
#endif
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.ES20;

namespace MapEngineOpenGL
{
#if __ANDROID__
	public class GL20MyRenderer : Java.Lang.Object, GLSurfaceView.IRenderer
#elif __IOS__
	public class GL20MyRenderer 
#endif
	{
		private MapController mSC;        //MapController
		private Shader mShader;                //定義したShader
		private int muProjMatrix;        //投影変換用行列のハンドル
		private int muViewMatrix;        //視点変換用行列のハンドル
		private float[] mCoordList;        //頂点データ
		private int mVertexId;        //頂点データのVRAMの位置
		private int mNumVertices;        //頂点数
		private int mAPosition;        //位置ハンドル
		private byte[] mVertices;
		private GMatrix mFrontProjectionMatrix;        //コントローラーからの受け取り用投影変換用行列
		private GMatrix mFrontModelViewMatrix;        //コントローラーからの受け取り用視点変換用行列
		private GMatrix mBackProjectionMatrix;        //描画用投影変換用行列
		private GMatrix mBackModelViewMatrix;        //描画用視点変換用行列

		private int FSIZE = sizeof(float) / sizeof(byte);        //floatのバイトサイズ

		/*		*
         * コンストラクタ
         * @param mapView
         */
		public GL20MyRenderer(MapController SC) : base ()
		{
			mSC = SC;
			mShader = new Shader();
			mCoordList = mSC.CoordList;
			mFrontProjectionMatrix = new GMatrix();
			mFrontModelViewMatrix = new GMatrix();
			mBackProjectionMatrix = new GMatrix();
			mBackModelViewMatrix = new GMatrix();
		}

		/**
         * 行列を設定
         * @param viewmatrix
         * @param projectionmatrix
         */
		public void SetMatrix(GMatrix viewmatrix, GMatrix projectionmatrix) {
			lock (mFrontModelViewMatrix) {
				viewmatrix.Copy(mFrontModelViewMatrix);
				projectionmatrix.Copy(mFrontProjectionMatrix);
			}
		}

#if __ANDROID__
		public void OnDrawFrame(IGL10 arg0)
#elif __IOS__
		public void OnDrawFrame() 
#endif
		{
			lock (mFrontModelViewMatrix) {
				mFrontModelViewMatrix.Copy(mBackModelViewMatrix);
				mFrontProjectionMatrix.Copy(mBackProjectionMatrix);
			}
			//行列をセット
			GL.UniformMatrix4 (muViewMatrix, false, ref mBackModelViewMatrix.matrix);
			GL.UniformMatrix4 (muProjMatrix, false, ref mBackProjectionMatrix.matrix);

			//背景色を設定します。
			GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
			GL.Clear (ClearBufferMask.ColorBufferBit);

			//線幅を設定します
			GL.LineWidth (2.0f);

			//頂点バッファを設定します
			GL.BindBuffer (All.ArrayBuffer, mVertexId);
			int a = 0;
			GL.VertexAttribPointer (mAPosition, 3, All.Float, false, FSIZE * 3, (IntPtr)a); 

			//描画
			GL.DrawArrays (All.LineStrip, 0, mNumVertices);
		}

#if __ANDROID__
		public void OnSurfaceChanged(IGL10 arg0, int width, int height) 
#elif __IOS__
		public void OnSurfaceChanged(int width, int height)
#endif
		{
			//ビューポートの設定します。
			GL.Viewport(0,0,width,height);
			//再描画
			mSC.SetPosition ();
		}

#if __ANDROID__
		public void OnSurfaceCreated(IGL10 arg0, Javax.Microedition.Khronos.Egl.EGLConfig arg1) 
#elif __IOS__
		public void OnSurfaceCreated() 
#endif
		{
			int program = mShader.InitShaders();
#if __ANDROID__
			mAPosition = GL.GetAttribLocation (program, new System.Text.StringBuilder("a_Position"));
#elif __IOS__
			mAPosition = GL.GetAttribLocation (program, "a_Position");
#endif
			if (mAPosition == -1) {
				//throw new RuntimeException("a_Positionの格納場所の取得に失敗");
			}
			GL.EnableVertexAttribArray (mAPosition);
			mNumVertices = InitVertexBuffers(); 

			//ビューボリュームと視点の格納場所を取得し、を登録します。
#if __ANDROID__
			muViewMatrix = GL.GetUniformLocation (program, new System.Text.StringBuilder("u_ViewMatrix"));
			muProjMatrix = GL.GetUniformLocation (program, new System.Text.StringBuilder("u_ProjMatrix"));
#elif __IOS__
			muViewMatrix = GL.GetUniformLocation (program, "u_ViewMatrix");
			muProjMatrix = GL.GetUniformLocation (program, "u_ProjMatrix");
#endif
			if (muViewMatrix == -1 || muProjMatrix == -1) {
				//throw new RuntimeException("a_Positionの格納場所の取得に失敗");
			}
		}

		/**
         * 描画用の頂点データを作成します。
         * @return
         */
		public int InitVertexBuffers() {
			//byte[]を作成します。
			mVertices = MakeByteArray (mCoordList);
			int vertex_num = mCoordList.Length / 3;        //頂点数        

			//VRAMにデータを設定します。
			int[] vertexId = new int[1];
			GL.GenBuffers (1, vertexId);
			GL.BindBuffer (All.ArrayBuffer, vertexId [0]);
			GL.BufferData (All.ArrayBuffer, (IntPtr)mVertices.Length, mVertices, All.StaticDraw);
			mVertexId = vertexId[0];

			return vertex_num;
		}

		/**
         * フロート配列からbyte[]を作成します。
         * @param array
         * @return
         */
		public byte[] MakeByteArray (float[] array) {
			//if (array == null) throw new IllegalArgumentException();
			var len = FSIZE * array.Length;
			byte[] bytes = new byte[len];

			Buffer.BlockCopy(array, 0, bytes, 0, len);

			return bytes;
		}
	}
}

