using System;
using System.IO;

using Java.IO;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Android.Util;
using System.Collections.Generic;

namespace MapEngineOpenGL2
{
	public class MapView : FrameLayout, View.IOnTouchListener
	{
		private GestureDetector mGd;
		private GL20SurfaceView mSurfaceView = null;        //描画用ビュー
		private Context mContext = null;        //Context
		private float[] mCoordList = null;                //Float形式の座標を格納する配列
		private MapControl mMc = null;        //地図操作用
		DisplayMetrics mMetrics = null; //ディスプレイ情報

		public MapView (Context context, Stream istream) : base (context)
		{
			//ディスプレイ情報を取得する
			mMetrics = new DisplayMetrics();
			var wm = this.Context.GetSystemService (Context.WindowService).JavaCast<IWindowManager>();
			wm.DefaultDisplay.GetMetrics (mMetrics);

			//地図データを読み込む
			mCoordList = LoadData(istream);

			//Contextを設定
			mContext = context;

			//地図操作クラスを初期化
			mMc = new MapControl(this);
			mMc.SetEyePosition(140000.0f, 50000.0f, MapControl.BASE_Z * mMc.scale, 0);

			//描画のためのビューを初期化
			mSurfaceView = new GL20SurfaceView(mContext, this);

			//SurfaceViewを追加する
			this.AddView(mSurfaceView);

			//ジェスチャー操作
			mGd = new GestureDetector(mContext, mMc);

			//タッチイベントを登録
			this.SetOnTouchListener(this);
		}

		/**
         * 地図データを設定します。
         * @param is
         * @return
         * @throws IOException
         */
		private float[] LoadData(Stream istream) {//throws IOException {
			var br = new BufferedReader(new InputStreamReader(istream));

			string str;
			var list = new List<string>(); //行ごとのリスト
			//ファイルから１行ごとに読み込む
			while ((str = br.ReadLine()) != null) {
				list.Add(str);
			}
			br.Close ();
			if(!(list.Count>0)) return null;

			//読み込んだファイルをFloat配列へ変換する
			var res = new float[list.Count * 3];  //x,y,zの順に格納されたfloat配列
			for(int i=0; i<list.Count; i++){
				str = list[i];
				string[] str_coord = str.Split('\t');
				res[i * 3 + 0] = float.Parse(str_coord[0]);        //X座標
				res[i * 3 + 1] = float.Parse(str_coord[1]);        //Y座標
				res[i * 3 + 2] = 0.0f;                                                        //Z座標
			}
			list.Clear ();

			return res;
		}

		/**
         * 地図データを返します。
         * @return
         */
		public float[] GetCoordinateList() {
			return mCoordList;
		}

		/**
         * 地図操作クラスを返します。
         * @return
         */
		public MapControl GetMapControl() {
			return mMc;
		}

		/**
         * 地図データの初期化します。
         */
		public void Release() {
			mCoordList = null;
		}

		public bool OnTouch(View v, MotionEvent ev) {
			mGd.OnTouchEvent(ev);
			mMc.scaleGestureDetect.OnTouchEvent(ev);
			return true;
		}

		/**
         * SurfaceViewを取得
         * @return
         */
		public GL20SurfaceView GetSurfaceView() {
			return this.mSurfaceView;
		}

		/**
         * 再描画
         */
		public void Redraw() {
			mMc.SetPosition();
		}
	}
}

