using System;
using Android.Content;
using Javax.Microedition.Khronos.Opengles;
using Android.Opengl;
using Javax.Microedition.Khronos.Egl;
using Java.Nio;
using Java.Lang;

namespace MapEngineOpenGL2
{
	public class GL20MyRenderer : Java.Lang.Object, GLSurfaceView.IRenderer
	{
		private MapView mMapView;        //MapView
		private Shader mShader;                //定義したShader
		private int muProjMatrix;        //投影変換用行列のハンドル
		private int muViewMatrix;        //視点変換用行列のハンドル
		private float[] mCoordList;        //頂点データ
		private int mVertexId;        //頂点データのVRAMの位置
		private int mNumVertices;        //頂点数
		private int mAPosition;        //位置ハンドル
		private GMatrix mFrontProjectionMatrix;        //コントローラーからの受け取り用投影変換用行列
		private GMatrix mFrontModelViewMatrix;        //コントローラーからの受け取り用視点変換用行列
		private GMatrix mBackProjectionMatrix;        //描画用投影変換用行列
		private GMatrix mBackModelViewMatrix;        //描画用視点変換用行列

		private int FSIZE = sizeof(float) / sizeof(byte);        //floatのバイトサイズ

		/*		*
         * コンストラクタ
         * @param context
         * @param parent
         * @param mapView
         */
		public GL20MyRenderer(Context context, GL20SurfaceView parent , MapView mapView) : base ()
		{
			mMapView = mapView;
			mShader = new Shader();
			mCoordList = mMapView.GetCoordinateList();
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

		public void OnDrawFrame(IGL10 arg0) {
			lock (mFrontModelViewMatrix) {
				mFrontModelViewMatrix.Copy(mBackModelViewMatrix);
				mFrontProjectionMatrix.Copy(mBackProjectionMatrix);
			}
			//行列をセット
			GLES20.GlUniformMatrix4fv(muViewMatrix, 1, false, mBackModelViewMatrix.matrix, 0);
			GLES20.GlUniformMatrix4fv(muProjMatrix, 1, false, mBackProjectionMatrix.matrix, 0);

			//背景色を設定します。
			GLES20.GlClearColor(0.0f, 0.0f, 0.0f, 1.0f); 
			GLES20.GlClear(GLES20.GlColorBufferBit);

			//線幅を設定します
			GLES20.GlLineWidth(2.0f);

			//頂点バッファを設定します
			GLES20.GlBindBuffer(GLES20.GlArrayBuffer, mVertexId);
			GLES20.GlVertexAttribPointer(mAPosition, 3, GLES20.GlFloat, false, FSIZE*3, 0);

			//描画
			GLES20.GlDrawArrays(GLES20.GlLineStrip, 0, mNumVertices);        
		}

		public void OnSurfaceChanged(IGL10 arg0, int width, int height) {
			//ビューポートの設定します。
			GLES20.GlViewport(0,0,width,height);
			//再描画
			mMapView.Redraw();
		}

		public void OnSurfaceCreated(IGL10 arg0, Javax.Microedition.Khronos.Egl.EGLConfig arg1) {
			//位置ハンドル（a_Position）の取得と有効化。
			int program = mShader.InitShaders();
			mAPosition = GLES20.GlGetAttribLocation(program, "a_Position");
			if (mAPosition == -1) {
				throw new RuntimeException("a_Positionの格納場所の取得に失敗");
			}
			GLES20.GlEnableVertexAttribArray(mAPosition);
			mNumVertices = InitVertexBuffers(); 

			//ビューボリュームと視点の格納場所を取得し、を登録します。
			muViewMatrix = GLES20.GlGetUniformLocation(program, "u_ViewMatrix");
			muProjMatrix = GLES20.GlGetUniformLocation(program, "u_ProjMatrix");
			if (muViewMatrix == -1 || muProjMatrix == -1) {
				throw new RuntimeException("a_Positionの格納場所の取得に失敗");
			}
		}

		/**
         * 描画用の頂点データを作成します。
         * @return
         */
		public int InitVertexBuffers() {
			//FloatBufferオブジェクトを作成します。
			FloatBuffer vertices = makeFloatBuffer(mCoordList);
			int vertex_num = mCoordList.Length / 3;        //頂点数        

			//VRAMにデータを設定します。
			int[] vertexId = new int[1];
			GLES20.GlGenBuffers(1, vertexId, 0);
			GLES20.GlBindBuffer(GLES20.GlArrayBuffer, vertexId[0]);
			GLES20.GlBufferData(GLES20.GlArrayBuffer, FSIZE * vertices.Limit(), vertices, GLES20.GlStaticDraw);
			mVertexId = vertexId[0];

			return vertex_num;
		}

		/**
         * フロート配列からフロートバッファを作成します。
         * @param array
         * @return
         */
		public FloatBuffer makeFloatBuffer(float[] array) {
			if (array == null) throw new IllegalArgumentException();
			ByteBuffer byteBuffer = ByteBuffer.AllocateDirect(4 * array.Length);
			byteBuffer.Order(ByteOrder.NativeOrder());
			FloatBuffer floatBuffer = byteBuffer.AsFloatBuffer();
			floatBuffer.Put(array);
			floatBuffer.Position(0);
			return floatBuffer;
		}
	}
}

