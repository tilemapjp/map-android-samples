package jp.co.yahoo.map.codezine_sample;

import java.io.BufferedReader;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.io.IOException;
import java.util.ArrayList;
import android.content.Context;
import android.widget.FrameLayout;

public class MapView extends FrameLayout{
	
	private GL20SurfaceView mSurfaceView = null;	//描画用ビュー
	private Context mContext = null;	//Context
	private float[] mCoordList;		//Float形式の座標を格納する配列
	
	//コンストラクタ
	public MapView(Context context,InputStream is) throws IOException {
		super(context);

		//地図データを読み込む
		mCoordList = loatData(is);

		//Contextを設定
		mContext = context;

		//描画のためのビューを初期化
		mSurfaceView = new GL20SurfaceView(mContext, this);

		//SurfaceViewを追加する
		this.addView(mSurfaceView);
	}

	//地図データを設定します。
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

	//地図データを返します。
	public float[] getCoordinateList() {
		return mCoordList;
	}

	//地図データの初期化します。
	public void release() {
		mCoordList = null;
	}

}
