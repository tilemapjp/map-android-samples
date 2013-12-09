package jp.co.yahoo.map.codezine_sample;

import java.nio.ByteBuffer;
import java.nio.ByteOrder;
import java.nio.FloatBuffer;

import javax.microedition.khronos.egl.EGLConfig;
import javax.microedition.khronos.opengles.GL10;

import jp.co.yahoo.map.codezine_sample.GL20SurfaceView;
import jp.co.yahoo.map.codezine_sample.MapView;
import jp.co.yahoo.map.codezine_sample.Shader;

import android.content.Context;
import android.opengl.GLES20;
import android.opengl.GLSurfaceView;

public class GL20MyRenderer implements GLSurfaceView.Renderer{
	private MapView mMapView;	//MapView
	private Shader mShader;		//定義したShader
	private int muProjMatrix;	//投影変換用行列のハンドル
	private int muViewMatrix;	//視点変換用行列のハンドル
	private float[] mCoordList;	//頂点データ
	private int mVertexId;	//頂点データのVRAMの位置
	private int mNumVertices;	//頂点数
	private int mAPosition;	//位置ハンドル
	private GMatrix mFrontProjectionMatrix;	//コントローラーからの受け取り用投影変換用行列
	private GMatrix mFrontModelViewMatrix;	//コントローラーからの受け取り用視点変換用行列
	private GMatrix mBackProjectionMatrix;	//描画用投影変換用行列
	private GMatrix mBackModelViewMatrix;	//描画用視点変換用行列
	
	private final int FSIZE = Float.SIZE / Byte.SIZE;	//floatのバイトサイズ
	
	/**
	 * コンストラクタ
	 * @param context
	 * @param parent
	 * @param mapView
	 */
	public GL20MyRenderer(Context context, GL20SurfaceView parent , MapView mapView){
		mMapView = mapView;
		mShader = new Shader();
		mCoordList = mMapView.getCoordinateList();
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
	public void setMatrix(GMatrix viewmatrix, GMatrix projectionmatrix) {
		synchronized (mFrontModelViewMatrix) {
			viewmatrix.copy(mFrontModelViewMatrix);
			projectionmatrix.copy(mFrontProjectionMatrix);
		}
	}
	
	@Override
	public void onDrawFrame(GL10 arg0) {
		synchronized (mFrontModelViewMatrix) {
			mFrontModelViewMatrix.copy(mBackModelViewMatrix);
			mFrontProjectionMatrix.copy(mBackProjectionMatrix);
		}
		//行列をセット
		GLES20.glUniformMatrix4fv(muViewMatrix, 1, false, mBackModelViewMatrix.matrix, 0);
		GLES20.glUniformMatrix4fv(muProjMatrix, 1, false, mBackProjectionMatrix.matrix, 0);
		
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
		//ビューポートの設定します。
		GLES20.glViewport(0,0,width,height);
		//再描画
		mMapView.redraw();
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
	}
	
	/**
	 * 描画用の頂点データを作成します。
	 * @return
	 */
	public int initVertexBuffers() {
		//FloatBufferオブジェクトを作成します。
		FloatBuffer vertices = makeFloatBuffer(mCoordList);
		int vertex_num = mCoordList.length / 3;	//頂点数	
		
		//VRAMにデータを設定します。
		int[] vertexId = new int[1];
		GLES20.glGenBuffers(1, vertexId, 0);
		GLES20.glBindBuffer(GLES20.GL_ARRAY_BUFFER, vertexId[0]);
		GLES20.glBufferData(GLES20.GL_ARRAY_BUFFER, FSIZE * vertices.limit(), vertices, GLES20.GL_STATIC_DRAW);
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
		ByteBuffer byteBuffer = ByteBuffer.allocateDirect(4 * array.length);
		byteBuffer.order(ByteOrder.nativeOrder());
		FloatBuffer floatBuffer = byteBuffer.asFloatBuffer();
		floatBuffer.put(array);
		floatBuffer.position(0);
		return floatBuffer;
	}
}
