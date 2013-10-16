package jp.co.yahoo.map.codezine_sample;

import java.nio.ByteBuffer;
import java.nio.ByteOrder;
import java.nio.FloatBuffer;

import javax.microedition.khronos.egl.EGLConfig;
import javax.microedition.khronos.opengles.GL10;

import jp.co.yahoo.map.codezine_sample.GL20SurfaceView;
import jp.co.yahoo.map.codezine_sample.MapView;
import jp.co.yahoo.map.codezine_sample.Shader;


import android.annotation.SuppressLint;
import android.content.Context;
import android.opengl.GLES20;
import android.opengl.GLSurfaceView;
import android.opengl.Matrix;

public class GL20MyRenderer implements GLSurfaceView.Renderer{

	private MapView mMapView;	//MapView
	private Context mContext;	//Context
	private GL20SurfaceView mParentView;	//親のSurfaceView
	private Shader mShader;		//定義したShader
	private float[] mProjMatrix = new float[16];	//投影変換用行列
	private int muProjMatrix;	//投影変換用行列のハンドル
	private float[] mViewMatrix = new float[16];	//視点変換用行列
	private int muViewMatrix;	//視点変換用行列のハンドル
	private float[] mCoordList;	//頂点データ
	private int mVertexId;	//頂点データのVRAMの位置
	private int mNumVertices;	//頂点数
	private float mEyeX = 140000.0f, mEyeY = 50000.0f, mEyeZ = 10000000.0f; //視点の座標
	private int mAPosition;	//位置ハンドル

	private final int FSIZE = Float.SIZE / Byte.SIZE;	//floatのバイトサイズ

	//コンストラクタ
	public GL20MyRenderer(Context context, GL20SurfaceView parent , MapView mapView){
		mContext = context;
		mParentView = parent;
		mMapView = mapView;
		mShader = new Shader();
		mCoordList = mMapView.getCoordinateList();
	}

	@Override
	public void onDrawFrame(GL10 arg0) {
		//背景色を設定します。
		GLES20.glClearColor(0.0f, 0.0f, 0.0f, 1.0f); 
		GLES20.glClear(GLES20.GL_COLOR_BUFFER_BIT);

		//線幅を設定します
		GLES20.glLineWidth(2.0f);

		//頂点バッファを設定します
		GLES20.glBindBuffer(GLES20.GL_ARRAY_BUFFER, mVertexId);
		GLES20.glVertexAttribPointer(mAPosition, 3, GLES20.GL_FLOAT, false, FSIZE*3, 0);

		//描画
		GLES20.glDrawArrays(GLES20.GL_LINE_STRIP, 0, mNumVertices);	
	}

	@Override
	public void onSurfaceChanged(GL10 arg0, int width, int height) {
		int size = (width <= height) ? width : height;

		//ビューポートの設定します。
		GLES20.glViewport((width - size) / 2, (height - size) / 2, size, size);

		//ビューボリュームを設定します。
		Matrix.orthoM(mProjMatrix, 0, -1000000.0f, 1000000.0f, -1000000.0f, 1000000.0f, 0.0f, 10000000.0f);
		GLES20.glUniformMatrix4fv(muProjMatrix, 1, false, mProjMatrix, 0);
	}

	@Override
	public void onSurfaceCreated(GL10 arg0, EGLConfig arg1) {
		//位置ハンドル（a_Position）の取得と有効化。
		int program = mShader.initShaders();
		mAPosition = GLES20.glGetAttribLocation(program, "a_Position");
		if (mAPosition == -1) {
			throw new RuntimeException("a_Positionの格納場所の取得に失敗");
		}
		GLES20.glEnableVertexAttribArray(mAPosition);
		mNumVertices = initVertexBuffers(); 

		//ビューボリュームと視点の格納場所を取得し、を登録します。
		muViewMatrix = GLES20.glGetUniformLocation(program, "u_ViewMatrix");
		muProjMatrix = GLES20.glGetUniformLocation(program, "u_ProjMatrix");
		if (muViewMatrix == -1 || muProjMatrix == -1) {
			throw new RuntimeException("a_Positionの格納場所の取得に失敗");
		}

		//視点（カメラ）の位置を設定します。
		Matrix.setLookAtM(mViewMatrix, 0, mEyeX, mEyeY, mEyeZ, mEyeX, mEyeY, 0.0f, 0.0f, 1.0f, 0.0f);
	  	GLES20.glUniformMatrix4fv(muViewMatrix, 1, false, mViewMatrix, 0);
	}

	//描画用の頂点データを作成します。
	@SuppressLint("NewApi")
	public int initVertexBuffers() {
		//FloatBufferオブジェクトを作成します。
		FloatBuffer vertices = makeFloatBuffer(mCoordList);
		int vertex_num = mCoordList.length / 3;			//頂点数	

		//VRAMにデータを設定します。
		int[] vertexId = new int[1];
		GLES20.glGenBuffers(1, vertexId, 0);
		GLES20.glBindBuffer(GLES20.GL_ARRAY_BUFFER, vertexId[0]);
		GLES20.glBufferData(GLES20.GL_ARRAY_BUFFER, FSIZE * vertices.limit(), vertices, GLES20.GL_STATIC_DRAW);
		mVertexId = vertexId[0];

		return vertex_num;
	}

	//フロート配列からフロートバッファを作成します。  
	public FloatBuffer makeFloatBuffer(float[] array) {
		if (array == null) throw new IllegalArgumentException();
		ByteBuffer byteBuffer = ByteBuffer.allocateDirect(4 * array.length);
		byteBuffer.order(ByteOrder.nativeOrder());
		FloatBuffer floatBuffer = byteBuffer.asFloatBuffer();
		floatBuffer.put(array);
		floatBuffer.position(0);
		return floatBuffer;
	}
}
