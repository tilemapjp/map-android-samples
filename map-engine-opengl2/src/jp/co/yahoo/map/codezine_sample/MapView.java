package jp.co.yahoo.map.codezine_sample;

import java.io.BufferedReader;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.io.IOException;
import java.util.ArrayList;
import android.content.Context;
import android.widget.FrameLayout;
import android.view.WindowManager;
import android.view.View;
import android.view.View.OnTouchListener;
import android.view.GestureDetector;
import android.view.MotionEvent;
import android.util.DisplayMetrics;

public class MapView extends FrameLayout implements OnTouchListener {
	private GestureDetector mGd;
	private GL20SurfaceView mSurfaceView = null;	//描画用ビュー
	private Context mContext = null;	//Context
	private float[] mCoordList = null;		//Float形式の座標を格納する配列
	private MapControl mMc = null;	//地図操作用
	DisplayMetrics mMetrics = null; //ディスプレイ情報
	
	/**
	 * コンストラクタ
	 * @param context
	 * @param is
	 * @throws IOException
	 */
	public MapView(Context context,InputStream is) throws IOException {
		super(context);
		
		//ディスプレイ情報を取得する
		mMetrics = new DisplayMetrics();
		WindowManager wm = (WindowManager)this.getContext().getSystemService(Context.WINDOW_SERVICE);
		wm.getDefaultDisplay().getMetrics(mMetrics);
		
		//地図データを読み込む
		mCoordList = loatData(is);
		
		//Contextを設定
		mContext = context;
		
		//地図操作クラスを初期化
		mMc = new MapControl(this);
		mMc.setEyePosition(140000.0f, 50000.0f, MapControl.BASE_Z * mMc.scale, 0);
		
		//描画のためのビューを初期化
		mSurfaceView = new GL20SurfaceView(mContext, this);
		
		//SurfaceViewを追加する
		this.addView(mSurfaceView);
		
		//ジェスチャー操作
		mGd = new GestureDetector(mContext, mMc);
		
		//タッチイベントを登録
		this.setOnTouchListener(this);
	}
	
	/**
	 * 地図データを設定します。
	 * @param is
	 * @return
	 * @throws IOException
	 */
	private float[] loatData(InputStream is) throws IOException {
		BufferedReader br = new BufferedReader(new InputStreamReader(is));
		
		String str;
		ArrayList<String> list = new ArrayList<String>(); //行ごとのリスト
		//ファイルから１行ごとに読み込む
		while ((str = br.readLine()) != null) {
			list.add(str);
		}
		br.close();
		if(!(list.size()>0)) return null;
		
		//読み込んだファイルをFloat配列へ変換する
		float[] res = new float[list.size() * 3];  //x,y,zの順に格納されたfloat配列
		for(int i=0; i<list.size(); i++){
			str = list.get(i);
			String[] str_coord = str.split("\t");
			res[i * 3 + 0] = Float.valueOf(str_coord[0]);	//X座標
			res[i * 3 + 1] = Float.valueOf(str_coord[1]);	//Y座標
			res[i * 3 + 2] = 0.0f;							//Z座標
		}
		list.clear();
		
		return res;
	}
	
	/**
	 * 地図データを返します。
	 * @return
	 */
	public float[] getCoordinateList() {
		return mCoordList;
	}
	
	/**
	 * 地図操作クラスを返します。
	 * @return
	 */
	public MapControl getMapControl() {
		return mMc;
	}
	
	/**
	 * 地図データの初期化します。
	 */
	public void release() {
		mCoordList = null;
	}
	
	@Override
	public boolean onTouch(View v, MotionEvent event) {
		mGd.onTouchEvent(event);
		mMc.scaleGestureDetect.onTouchEvent(event);
		return true;
	}
	
	/**
	 * SurfaceViewを取得
	 * @return
	 */
	public GL20SurfaceView getSurfaceView() {
		return this.mSurfaceView;
	}
	
	/**
	 * 再描画
	 */
	public void redraw() {
		mMc.setPosition();
	}
}
