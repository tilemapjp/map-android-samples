using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Content.Res;
using Java.IO;

namespace MapEngineOpenGL2
{
	[Activity (Label = "MapEngineOpenGL2", MainLauncher = true)]
	public class MainActivity : Activity
	{
		private MapView mMapView ;                                                        //MapView
		private static string FILE_PATH = "01.txt";        //地図データ

		protected override void OnCreate(Bundle savedInstanceState) {
			base.OnCreate(savedInstanceState);

			//元からあるassetsフォルダに入れたtextファイルを読み込んでMapViewにセットします。
			try {
				var istream = Assets.Open(FILE_PATH);

				//描画領域の作成
				mMapView = new MapView(this, istream);
				SetContentView(mMapView);
			} catch(IOException e) {
			}
		}

		public override bool OnCreateOptionsMenu(IMenu menu) {
			MenuInflater.Inflate (Resource.Menu.main, menu);
			return true;
		}

		protected override void OnDestroy(){
			if(mMapView != null) {
				mMapView.Release();
				mMapView = null;
			}
			base.OnDestroy();
		}
	}
}


