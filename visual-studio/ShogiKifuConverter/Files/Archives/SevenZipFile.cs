﻿namespace Grayscale.ShogiKifuConverter
{
    using System;
    using System.Diagnostics;
    using Grayscale.ShogiKifuConverter.CommonAction;
    using Grayscale.ShogiKifuConverter.Commons;
    using Grayscale.ShogiKifuConverter.Location;

    /// <summary>
    /// .7z ファイル。
    /// </summary>
    public class SevenZipFile : AbstractArchiveFile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SevenZipFile"/> class.
        /// </summary>
        /// <param name="expansionGoFile">解凍を待っているファイル。</param>
        public SevenZipFile(TraceableFile expansionGoFile)
            : base(expansionGoFile)
        {
            // Trace.WriteLine($"7zip: {this.ExpansionGoFilePath}");
            // TODO this.ExpansionGoFilePath = this.ExpansionGoFilePath.Replace(@"\", "/");
        }

        /// <summary>
        /// 解凍する。
        /// </summary>
        /// <returns>展開に成功した。</returns>
        public override bool Expand()
        {
            try
            {
                Trace.WriteLine($"Expand  : {this.ExpansionGoFile.FullName} -> {LocationMaster.ConverterExpandDirectory.FullName}");
                if (string.IsNullOrWhiteSpace(this.ExpansionGoFile.FullName))
                {
                    return false;
                }

                SevenZManager.fnExtract(this.ExpansionGoFile.FullName, LocationMaster.ConverterExpandDirectory.FullName);

                // ディレクトリーを浅くします。
                PathFlat.GoFlat(LocationMaster.ConverterExpandDirectory.FullName);

                // 解凍が終わった元ファイルは削除。
                this.ExpansionGoFile.Delete();
                return true;
            }
            catch (BadImageFormatException e)
            {
                // 32ビットのプログラムを 64ビットで動かそうとしたときなど。
                Trace.WriteLine(e);
                return false;
            }
            catch (InvalidOperationException e)
            {
                Trace.WriteLine(e);
                return false;
            }
        }
    }
}
