using System;
#if __ANDROID__
using OpenTK.Platform.Android;
#elif __IOS__
using OpenTK.Platform.iPhoneOS;
#endif
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.ES20;

namespace MapEngineOpenGL
{
	public class Shader
	{
		/**
         * 頂点シェーダプログラム
         */
		private string VSHADER_SOURCE =
			"precision highp float;" +
			"attribute vec4 a_Position;\n" +
			"uniform mat4 u_ViewMatrix;\n" +
			"uniform mat4 u_ProjMatrix;\n" +
			"varying vec4 v_Color;\n" +
			"void main() {\n" +
			"  gl_Position = u_ProjMatrix * u_ViewMatrix * a_Position;\n" +
			"  gl_PointSize = 16.0;" +
			"}\n";

		/**
         * フラグメントシェーダプログラム
         */
		private string FSHADER_SOURCE =
			"precision highp float;" +
			"void main() {\n" +
			"  gl_FragColor = vec4(1.0,1.0,1.0,1.0);\n" +
			"}\n";

		/**
         * シェーダープログラムを読み込みコンパイルします。
         * @return
         */
		public int InitShaders() {
			int program = CreateProgram();
			GL.UseProgram (program);
			return program;
		}

		/**
         * プログラムオブジェクトを作成します。
         * @return
         */
		public int CreateProgram() {
			// シェーダオブジェクトを作成する
			int vertexShader   = LoadShader((int)All.VertexShader,   this.VSHADER_SOURCE);
			int fragmentShader = LoadShader((int)All.FragmentShader, this.FSHADER_SOURCE);

			// プログラムオブジェクトを作成する
			var program = GL.CreateProgram ();
			if (program == 0) {
				//throw new RuntimeException("failed to create program");
			}

			// シェーダオブジェクトを設定する
			GL.AttachShader (program, vertexShader);
			GL.AttachShader (program, fragmentShader);

			// プログラムオブジェクトをリンクする
			GL.LinkProgram (program);

			// リンク結果をチェックする
			int[] linked = new int[1];
			GL.GetProgram(program, All.LinkStatus, linked);
			if (linked[0] != (int)All.True) {
				var error = GL.GetProgramInfoLog (program);
				//throw new RuntimeException("failed to link program: " + error);
			}
			return program;
		}

		/**
         * シェーダーを読み込みます。
         * @param type
         * @param source
         * @return
         */
		public int LoadShader(int type, string source) {
			// シェーダオブジェクトを作成する
			var shader = GL.CreateShader((All)type);
			if (shader == 0) {
				//throw new RuntimeException("unable to create shader");
			}
			// シェーダのプログラムを設定する
			GL.ShaderSource (shader, source);
			// シェーダをコンパイルする
			GL.CompileShader (shader);
			// コンパイル結果を検査する
			int[] compiled = new int[1];
			GL.GetShader (shader, All.CompileStatus, compiled);
			if (compiled[0] != (int)All.True) {
				var error = GL.GetShaderInfoLog (shader);
				//throw new RuntimeException("failed to compile shader: " + error);
			}
			return shader;
		}
	}
}

