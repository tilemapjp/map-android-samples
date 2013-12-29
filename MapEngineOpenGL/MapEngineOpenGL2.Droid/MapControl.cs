using System;
using Android.Views;

namespace MapEngineOpenGL
{
	public class MapControl : GestureDetector.SimpleOnGestureListener
	{
		private MapController mSC;
		public ScaleGestureDetector scaleGestureDetect = null;

		public MapControl(MapView mv, MapController sc) : base()
		{
			mSC = sc;
			OnScaleGesture = new OnScaleGestureLocal(this);
			scaleGestureDetect = new ScaleGestureDetector(mv.Context, OnScaleGesture);
		}

		/** 
         * フリック移動
         */
		public override bool OnFling(MotionEvent e1, MotionEvent e2, float velocityX, float velocityY) {
			float dx = e2.GetX() - e1.GetX();
			float dy = e2.GetY() - e1.GetY();
			Console.WriteLine ("Map: OnFling : {0},{1}", dx, dy);
			return true;
		}

		/**
         * シングルタップ
         */
		public override bool OnSingleTapConfirmed(MotionEvent ev) {
			Console.WriteLine ("Map: OnSingleTapConfirmed");
			return true;
		}

		/*
		 *
         * シングルタップ（ダブルタップ時も呼ばれる）
         */
		public override bool OnSingleTapUp(MotionEvent ev) {
			Console.WriteLine ("Map: OnSingleTapUp");
			return true;
		}


		/**
         * ダブルタップ
         */
		public override bool OnDoubleTap(MotionEvent ev) {
			Console.WriteLine ("Map: OnDoubleTap");
			return true;
		}

		/**
         * ダブルタップ中イベント
         */
		public override bool OnDoubleTapEvent(MotionEvent ev) {
			Console.WriteLine ("Map: OnDoubleTapEvent");
			return true;
		}

		/**
         * 押下
         */
		public override bool OnDown(MotionEvent ev) {
			Console.WriteLine ("Map: OnDown");
			return true;
		}

		/**
         * 長押し
         */
		public override void OnLongPress(MotionEvent ev) {
			Console.WriteLine ("Map: OnLongPress");
		}

		/**
         * ドラッグ
         */
		public override bool OnScroll(MotionEvent e1, MotionEvent e2,float distanceX, float distanceY) {
			mSC.OnScroll (e2.GetX (), e2.GetY (), distanceX, distanceY);
			return true;
		}

		/**
         * 押下（押下時のドラッグなどでは呼ばれない）
         */
		public override void OnShowPress(MotionEvent ev) {
			Console.WriteLine ("Map: OnShowPress");
		}

		/**
         * 縮尺変更:OnScaleGestureから
         */
		private bool OnScale(ScaleGestureDetector detector) {
			mSC.OnScale (detector.ScaleFactor);
			return true;
		}

		/**
         * ピンチのジェスチャー取得
         */
		private OnScaleGestureLocal OnScaleGesture;
		private class OnScaleGestureLocal : ScaleGestureDetector.SimpleOnScaleGestureListener
		{
			WeakReference _mC;
			public OnScaleGestureLocal (MapControl mC) : base()
			{
				_mC = new WeakReference(mC);
			}

			/**
             * 縮尺変更開始
             */
			public override bool OnScaleBegin (ScaleGestureDetector detector) 
			{
				Console.WriteLine ("Map: OnScaleBegin : {0}", detector.ScaleFactor);
				return base.OnScaleBegin (detector);
			}

			/**
             * 縮尺変更終了
             */
			public override void OnScaleEnd(ScaleGestureDetector detector) {
				Console.WriteLine ("Map: OnScaleEnd : {0}", detector.ScaleFactor);
				base.OnScaleEnd (detector);
			}

			/**
             * 縮尺変更
             */
			public override bool OnScale(ScaleGestureDetector detector) {
				Console.WriteLine ("Map: OnScale : {0}", detector.ScaleFactor);
				return ((MapControl)_mC.Target).OnScale (detector);
			}
		};
	}
}

