﻿namespace Grayscale.CsaOpener
{
    using System;
    using System.Diagnostics;
    using System.IO;

    /// <summary>
    /// .7z ファイル。
    /// </summary>
    public class SevenZipFile : AbstractArchiveFile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SevenZipFile"/> class.
        /// </summary>
        /// <param name="config">設定。</param>
        /// <param name="expansionGoFilePath">解凍を待っているファイルパス。</param>
        public SevenZipFile(Config config, string expansionGoFilePath)
            : base(config, expansionGoFilePath)
        {
            Trace.WriteLine($"7zip: {this.ExpansionGoFilePath}");

            // 中に何入ってるか分からん。名前が被るかもしれない。
            this.ExpansionOutputDir = Path.Combine(
                this.Config.ExpansionOutputPath,
                Directory.GetParent(this.ExpansionGoFilePath).Name,
                $"extracted-{Path.GetFileNameWithoutExtension(this.ExpansionGoFilePath)}").Replace(@"\", "/");
            this.ExpansionGoFilePath = this.ExpansionGoFilePath.Replace(@"\", "/");
        }

        /// <summary>
        /// 解凍する。
        /// </summary>
        public override void Expand()
        {
            try
            {
                Trace.WriteLine($"Un7z: {this.ExpansionGoFilePath} -> {this.ExpansionOutputDir}");
                if (string.IsNullOrWhiteSpace(this.ExpansionGoFilePath))
                {
                    return;
                }

                SevenZManager.fnExtract(this.ExpansionGoFilePath, this.ExpansionOutputDir);

                var wentDir = Path.Combine(this.Config.ExpansionWentPath, Directory.GetParent(this.ExpansionGoFilePath).Name);
                Commons.CreateDirectory(wentDir);
                var wentFile = Path.Combine(wentDir, Path.GetFileName(this.ExpansionGoFilePath));

                // 解凍が終わった元ファイルを移動。
                File.Move(this.ExpansionGoFilePath, wentFile);
            }
            catch (BadImageFormatException e)
            {
                // 32ビットのプログラムを 64ビットで動かそうとしたときなど。
                Trace.WriteLine(e);
            }
            catch (InvalidOperationException e)
            {
                Trace.WriteLine(e);
            }
        }
    }
}