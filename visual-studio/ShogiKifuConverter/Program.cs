﻿using System;

[assembly: CLSCompliant(true)]
namespace Grayscale.ShogiKifuConverter
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;
    using Grayscale.ShogiKifuConverter.Commons;
    using Grayscale.ShogiKifuConverter.Location;

    // [ソリューション エクスプローラー]のプロジェクトの下の[参照]を右クリック、
    // [参照の追加(R)...]をクリック。[アセンブリ] - [フレームワーク] と進み、
    // Microsoft.VisualBasic をチェックしてください。

    /// <summary>
    /// Entry point.
    ///
    /// [DynamicJSONを利用したJSONのパース (C#プログラミング)](https://www.ipentec.com/document/csharp-parse-json-by-using-dynamic-json)
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Gets or sets a 処理できなかったファイル数。
        /// </summary>
        public static int Rest { get; set; }

        /// <summary>
        /// Entry point.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        public static void Main(string[] args)
        {
            LogRotation.Logging(() =>
            {
                // Command line arguments.
                var arguments = RawArguments.Load(args);
                Trace.WriteLine($"{LogHelper.Stamp}Expand  : '{arguments.Expand}', Encode: '{arguments.Encode}', Convert: '{arguments.Convert}'.");

                // 同じフェーズをずっとやっていても１つも完成しないので、少しずつやって、ばらけさせる。
                var expandedCount = 1; // ループの初回入るように。
                var encodedCount = 0;
                var convertedCount = 0;
                var mergedCount = 0;
                var merged = false;

                // List<string> expansionOutputDirectories;
                // 処理が行われている間、繰り返す。
                while (expandedCount + encodedCount + convertedCount > 0 || merged)
                {
                    Trace.WriteLine($"{LogHelper.Stamp}Program : Running...");

                    // リセット。
                    expandedCount = 0;
                    encodedCount = 0;
                    convertedCount = 0;
                    mergedCount = 0;
                    merged = false;

                    if (arguments.Expand)
                    {
                        // 解凍フェーズ。
                        expandedCount = ExpansionPhase.ExpandLittleIt();
                    }

                    if (arguments.Encode)
                    {
                        // TODO フォルダーを探索して、棋譜のエンコーディングを変換。
                        encodedCount = EncodingPhase.ExecuteEncode();
                    }

                    if (arguments.Convert)
                    {
                        // 棋譜RPM変換フェーズ。
                        convertedCount = ConvertSomeFilesToRpm();
                    }

                    if (arguments.Merge)
                    {
                        // たまに行う程度。
                        //if (new System.Random().Next() % 3 == 0)
                        //{
                        // JSON作成フェーズ。
                        (mergedCount, merged) = JamTapesfragFiles(false);
                        //}
                    }

                    Trace.WriteLine($"{LogHelper.Stamp}Result  : expandedCount: {expandedCount}, encodedCount: {encodedCount}, readCount: {convertedCount}, mergedCount: {mergedCount}.");
                }

                // 最後の余りに対応する１回。
                {
                    Trace.WriteLine($"{LogHelper.Stamp}Program : Last run...");

                    if (arguments.Expand)
                    {
                        // 解凍フェーズ。
                        expandedCount = ExpansionPhase.ExpandLittleIt();
                    }

                    if (arguments.Encode)
                    {
                        // TODO フォルダーを探索して、棋譜のエンコーディングを変換。
                        EncodingPhase.ExecuteEncode();
                    }

                    if (arguments.Convert)
                    {
                        // 棋譜RPM変換フェーズ。
                        convertedCount = ConvertSomeFilesToRpm();
                    }

                    if (arguments.Merge)
                    {
                        // JSON作成フェーズ。
                        (mergedCount, merged) = JamTapesfragFiles(true);
                    }

                    Trace.WriteLine($"{LogHelper.Stamp}LAST    : expandedCount: {expandedCount}, readCount: {convertedCount}, mergedCount: {mergedCount}.");
                }

                // 空の go のサブ・ディレクトリは削除。
                {
                    {
                        // このディレクトリ以下のディレクトリをすべて取得する
                        IEnumerable<string> subDirectories =
                            System.IO.Directory.EnumerateDirectories(LocationMaster.InputDirectory.FullName, "*", System.IO.SearchOption.TopDirectoryOnly);

                        foreach (string subDir in subDirectories)
                        {
                            DeleteEmptyDirectory(subDir);
                        }
                    }

                    {
                        // このディレクトリ以下のディレクトリをすべて取得する
                        IEnumerable<string> subDirectories =
                            System.IO.Directory.EnumerateDirectories(LocationMaster.EncodedDirectory.FullName, "*", System.IO.SearchOption.TopDirectoryOnly);

                        foreach (string subDir in subDirectories)
                        {
                            DeleteEmptyDirectory(subDir);
                        }
                    }
                }

                int sleepSeconds = 60;
                Trace.WriteLine($"{LogHelper.Stamp}Finished: sleep={sleepSeconds} sec.");
                Thread.Sleep(sleepSeconds * 1000);
            });
        }

        /// <summary>
        /// RPM棋譜の断片（.tapesfrag ファイル）を600個ぐらい 適当にくっつけて JSONファイルにする。
        /// </summary>
        /// <param name="isLast">余り。</param>
        /// <returns>ループが回った回数、マージを１つ以上行った。</returns>
        public static (int, bool) JamTapesfragFiles(bool isLast)
        {
            Trace.WriteLine($"{LogHelper.Stamp}Merge   : Start. Tapesfrage isLast: {isLast}.");

            // 指定ディレクトリ以下のファイルをすべて取得する
            IEnumerable<string> tapesfragFileFullNames =
                System.IO.Directory.EnumerateFiles(
                    LocationMaster.ConvertedDirectory.FullName, "*.tapesfrag", System.IO.SearchOption.AllDirectories);

            var count = 0;

            // まず、ファイルを 1～600個集める。
            var usedTapesfragFiles = new List<TraceableFile>();
            foreach (string tapesfragFileFullName in tapesfragFileFullNames)
            {
                if (usedTapesfragFiles.Count > 599)
                {
                    break;
                }

                usedTapesfragFiles.Add(new TraceableFile(tapesfragFileFullName));
                count++;
            }

            if (!isLast && count < 400)
            {
                Trace.WriteLine($"{LogHelper.Stamp}Break   : 数: Count: {count}, グループ: {usedTapesfragFiles.Count} < 400。マージをパス。");

                // 400件も溜まってなければ、まだマージしない。
                return (count, false);
            }

            Trace.WriteLine($"{LogHelper.Stamp}Merge   : File count: {count}.");

            var tepeFragmentsBuilder = new StringBuilder();
            foreach (var tapesFragmentsFile in usedTapesfragFiles)
            {
                // JSONのオブジェクトの末尾にカンマが付いている。
                tepeFragmentsBuilder.AppendLine(tapesFragmentsFile.ReadAllText());
            }

            if (tepeFragmentsBuilder.Length < 1)
            {
                Trace.WriteLine($"{LogHelper.Stamp}Merge   : Empty file, Break. fileGroup.Count: {usedTapesfragFiles.Count}, builder.Length: {tepeFragmentsBuilder.Length}");

                // 空ファイルを読み込んでいたら無限ループしてしまう。 0 を返す。
                return (0, true);
            }

            // 最後のコンマを除去する。
            var tapeBoxContent = tepeFragmentsBuilder.ToString();
            var lastComma = tapeBoxContent.LastIndexOf(',');
            tapeBoxContent = tapeBoxContent.Substring(0, lastComma);

            // JSON形式として読めるように、配列のオブジェクトにする。box は Rust言語の予約語なので、tape_box とした。
            tapeBoxContent = string.Concat(@"{""tape_box"": [", tapeBoxContent, "]}");

            // ファイル名は適当。
            // ファイル名が被ってしまったら、今回はパス。
            {
                // ランダムな名前のファイル。
                var tapeBoxFile = TapeBoxJson.CreateTapeBoxFileAtRandom(LocationMaster.JammedDirectory);
                if (!File.Exists(tapeBoxFile.FullName))
                {
                    new TraceableFile(tapeBoxFile.FullName).WriteAllText(tapeBoxContent);

                    // 結合が終わったファイルは消す。
                    foreach (var file in usedTapesfragFiles)
                    {
                        file.Delete();
                    }
                }
                else
                {
                    Trace.WriteLine("Merge fail. Randome name fail. '{}'.", tapeBoxFile.FullName);
                }
            }

            // Trace.WriteLine("Merge rpmove obj(End)...");
            return (count, true);
        }

        /// <summary>
        /// いくつかの棋譜をRPMに変換。
        /// </summary>
        /// <returns>成功件数。</returns>
        public static int ConvertSomeFilesToRpm()
        {
            Trace.WriteLine($"{LogHelper.Stamp}ToRpm   : Start... Directory: {LocationMaster.EncodedDirectory.FullName}.");

            // 指定ディレクトリ以下のファイルをすべて取得する
            IEnumerable<string> eatingGoFiles =
                System.IO.Directory.EnumerateFiles(
                    LocationMaster.EncodedDirectory.FullName, "*", System.IO.SearchOption.AllDirectories);

            // 200件回す。
            var suceedCount = 0;
            foreach (string eatingGoFile in eatingGoFiles)
            {
                Trace.WriteLine($"{LogHelper.Stamp}ToRpm   : [{eatingGoFile}].");

                if (suceedCount > 199)
                {
                    break;
                }

                AbstractFile anyFile;
                switch (Path.GetExtension(eatingGoFile).ToUpper())
                {
                    case ".CSA":
                        anyFile = new CsaFile(new TraceableFile(string.Empty), new TraceableFile(eatingGoFile));
                        break;

                    case ".KIF":
                        anyFile = new KifFile(new TraceableFile(string.Empty), new TraceableFile(eatingGoFile));
                        break;

                    default:
                        anyFile = new UnexpectedFile(new TraceableFile(string.Empty));
                        Rest++;
                        break;
                }

                // 棋譜をRPMに変換。
                if (anyFile.ConvertAnyFileToRpm())
                {
                    suceedCount++;
                }
            }

            Trace.WriteLine($"{LogHelper.Stamp}ToRpm   : End.");
            return suceedCount;
        }

        /// <summary>
        /// 中身が空のディレクトリーは削除するぜ☆（＾～＾）
        /// </summary>
        /// <param name="dir">ディレクトリー。</param>
        public static void DeleteEmptyDirectory(string dir)
        {
            // このディレクトリ以下のディレクトリをすべて取得する
            IEnumerable<string> subDirectories =
                System.IO.Directory.EnumerateDirectories(dir, "*", System.IO.SearchOption.TopDirectoryOnly);

            // 再帰。
            foreach (string subDir in subDirectories)
            {
                DeleteEmptyDirectory(subDir);
            }

            try
            {
                // このディレクトリのファイル数を取得する
                if (Directory.GetFiles(dir, "*", SearchOption.AllDirectories).Length == 0)
                {
                    // ファイルがなければ削除する。
                    new TraceableDirectory(dir).Delete(false);
                }
            }
            catch (UnauthorizedAccessException e)
            {
                Trace.WriteLine(e);
            }
        }

        /// <summary>
        /// [DLL Import] SevenZipのコマンドラインメソッド
        /// </summary>
        /// <param name="hwnd">ウィンドウハンドル(=0)</param>
        /// <param name="szCmdLine">コマンドライン</param>
        /// <param name="szOutput">実行結果文字列</param>
        /// <param name="dwSize">実行結果文字列格納サイズ</param>
        /// <returns>
        /// 0:正常、0以外:異常終了
        /// </returns>
        [DllImport("7-zip32.dll", CharSet = CharSet.Ansi)]
        private static extern int SevenZip(IntPtr hwnd, string szCmdLine, StringBuilder szOutput, int dwSize);
    }
}
