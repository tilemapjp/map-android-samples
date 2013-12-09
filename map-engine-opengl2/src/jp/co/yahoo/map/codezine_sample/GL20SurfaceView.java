package jp.co.yahoo.map.codezine_sample;

import android.content.Context;
import android.opengl.GLSurfaceView;

public class GL20SurfaceView extends GLSurfaceView{
	private GL20MyRenderer mRenderer = null;	//描画処理本体
	private Context mContext;	//Context
	private MapView mMapView = null;	//MapView
	
	//コンストラクタ
	public GL20SurfaceView(Context context, MapView mapView) {
		super(context.getApplicationContext());
		mContext = context.getApplicationContext();
		mMapView = mapView;
		//OpenGLES2.0の使用を宣言します。
		setEGLContextClientVersion(2);
		//レンダラーを初期化し、ビューにセットします。
		mRenderer = new GL20MyRenderer(mContext,this, mMapView);
		setRenderer(mRenderer);
	}
	
	public void setMatrix(GMatrix viewmatrix, GMatrix projectionmatrix) {
		this.mRenderer.setMatrix(viewmatrix, projectionmatrix);
	}
}
