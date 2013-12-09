package jp.co.yahoo.map.codezine_sample;

import java.io.InputStream;
import java.io.IOException;

import com.example.codezine_sample.R;

import android.os.Bundle;
import android.app.Activity;
import android.content.res.AssetManager;
import android.view.Menu;

public class MainActivity extends Activity {
	private MapView mMapView ;							//MapView
	private static final String FILE_PATH = "01.txt";	//地図データ
	
	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		
		//元からあるassetsフォルダに入れたtextファイルを読み込んでMapViewにセットします。
		try{
			AssetManager assets = getAssets();
			InputStream is = assets.open(FILE_PATH);
			
			//描画領域の作成
			mMapView = new MapView(this,is);
			setContentView(mMapView);
		}catch(IOException e) {
		}
	}
	
	@Override
	public boolean onCreateOptionsMenu(Menu menu) {
		getMenuInflater().inflate(R.menu.main, menu);
		return true;
	}
	
	@Override
	public void onDestroy(){
		if(mMapView != null) {
			mMapView.release();
			mMapView = null;
		}
		super.onDestroy();
	}
}
