﻿namespace Grayscale.ShogiKifuConverter
{
    using System.Diagnostics;
    using System.IO;
    using Grayscale.ShogiKifuConverter.Commons;
    using Grayscale.ShogiKifuConverter.Location;

    /// <summary>
    /// Rust言語のプログラムで作った 実行ファイルを利用☆（＾～＾）
    /// </summary>
    public static class RustExe
    {
        /// <summary>
        /// 棋譜を読み取ります。
        /// </summary>
        /// <param name="inputFile">読み取る棋譜ファイル。</param>
        /// <param name="outputFile">出力先ファイル。</param>
        /// <returns>リターン コード。</returns>
        public static int ConvertAnyFileToRpm(TraceableFile inputFile, TraceableFile outputFile)
        {
            ProcessStartInfo info = new ProcessStartInfo();

            // 起動する実行ファイルのパスを設定する
            info.FileName = LocationMaster.Kw29Exe.FullName;
            info.WorkingDirectory = LocationMaster.Kw29Opt.FullName;

            // コマンドライン引数を指定する
            info.Arguments = $@"--input ""{inputFile.FullName.Replace(@"\", "/")}"" --output ""{Path.Combine(Directory.GetParent(outputFile.FullName).FullName, Path.GetFileNameWithoutExtension(outputFile.FullName)).Replace(@"\", "/")}""";

            // コンソール・ウィドウを開かない。
            info.CreateNoWindow = true;

            // シェル機能を使用しない
            info.UseShellExecute = false;

            Trace.WriteLine($"{LogHelper.Stamp}Go      : Process {info.FileName} {info.Arguments}");
            var p = Process.Start(info);

            // タイムアウト時間（秒）。１棋譜に 1分も かからないだろう。
            p.WaitForExit(60 * 1000);

            var returnCode = p.ExitCode;
            Trace.WriteLine($"{LogHelper.Stamp}Returned: Process returnCode: {returnCode}.");
            return returnCode;
        }
    }
}
