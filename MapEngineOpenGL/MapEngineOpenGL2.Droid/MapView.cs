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

namespace MapEngineOpenGL
{
	public class MapView : FrameLayout, View.IOnTouchListener, IMapPresenter
	{
		private GestureDetector mGd;
		private GL20SurfaceView mSurfaceView = null;        //描画用ビュー
		private Context mContext = null;        //Context
		//private float[] mCoordList = null;                //Float形式の座標を格納する配列
		private MapControl mMc = null;        //地図操作用
		private MapController mSC = null; //共通処理
		DisplayMetrics mMetrics = null; //ディスプレイ情報

		public MapView (Context context, string filename) : base (context)
		{
			//ディスプレイ情報を取得する
			mMetrics = new DisplayMetrics();
			var wm = this.Context.GetSystemService (Context.WindowService).JavaCast<IWindowManager>();
			wm.DefaultDisplay.GetMetrics (mMetrics);

			//地図データを読み込む
			mSC = new MapController(this, filename);

			//Contextを設定
			mContext = context;

			//地図操作クラスを初期化
			mMc = new MapControl(this, mSC);
			mSC.SetEyePosition(140000.0f, 50000.0f, MapController.BASE_Z * mSC.scale, 0);

			//描画のためのビューを初期化
			mSurfaceView = new GL20SurfaceView(mContext, mSC);

			//SurfaceViewを追加する
			this.AddView(mSurfaceView);

			//ジェスチャー操作
			mGd = new GestureDetector(mContext, mMc);

			//タッチイベントを登録
			this.SetOnTouchListener(this);
		}

		/**
         * 地図データを返します。
         * @return
         */
		public float[] GetCoordinateList() {
			return mSC.CoordList;
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
			mSC.Release ();
			mSC = null;
		}

		public void SetMatrix(GMatrix viewmatrix, GMatrix projectionmatrix)
		{
			this.GetSurfaceView ().SetMatrix (viewmatrix, projectionmatrix);
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
	}
}

