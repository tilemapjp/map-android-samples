using System;
using Android.Opengl;
using Java.Lang;

namespace MapEngineOpenGL2
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
			GLES20.GlUseProgram(program);
			return program;
		}

		/**
         * プログラムオブジェクトを作成します。
         * @return
         */
		public int CreateProgram() {
			// シェーダオブジェクトを作成する
			int vertexShader = loadShader(GLES20.GlVertexShader, this.VSHADER_SOURCE);
			int fragmentShader = loadShader(GLES20.GlFragmentShader, this.FSHADER_SOURCE);

			// プログラムオブジェクトを作成する
			int program = GLES20.GlCreateProgram();
			if (program == 0) {
				throw new RuntimeException("failed to create program");
			}

			// シェーダオブジェクトを設定する
			GLES20.GlAttachShader(program, vertexShader);
			GLES20.GlAttachShader(program, fragmentShader);

			// プログラムオブジェクトをリンクする
			GLES20.GlLinkProgram(program);

			// リンク結果をチェックする
			int[] linked = new int[1];
			GLES20.GlGetProgramiv(program, GLES20.GlLinkStatus, linked, 0);
			if (linked[0] != GLES20.GlTrue) {
				string error = GLES20.GlGetProgramInfoLog(program);
				throw new RuntimeException("failed to link program: " + error);
			}
			return program;
		}

		/**
         * シェーダーを読み込みます。
         * @param type
         * @param source
         * @return
         */
		public int loadShader(int type, string source) {
			// シェーダオブジェクトを作成する
			int shader = GLES20.GlCreateShader(type);
			if (shader == 0) {
				throw new RuntimeException("unable to create shader");
			}
			// シェーダのプログラムを設定する
			GLES20.GlShaderSource(shader, source);
			// シェーダをコンパイルする
			GLES20.GlCompileShader(shader);
			// コンパイル結果を検査する
			int[] compiled = new int[1];
			GLES20.GlGetShaderiv(shader, GLES20.GlCompileStatus, compiled, 0);
			if (compiled[0] != GLES20.GlTrue) {
				string error = GLES20.GlGetShaderInfoLog(shader);
				throw new RuntimeException("failed to compile shader: " + error);
			}
			return shader;
		}
	}
}

