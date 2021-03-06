﻿namespace Grayscale.ShogiKifuConverter
{
    using System.Diagnostics;
    using Grayscale.ShogiKifuConverter.CommonAction;
    using Grayscale.ShogiKifuConverter.Commons;
    using Grayscale.ShogiKifuConverter.Location;

    /// <summary>
    /// LZH形式棋譜ファイル。
    /// </summary>
    public class LzhFile : AbstractArchiveFile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LzhFile"/> class.
        /// </summary>
        /// <param name="expansionGoFile">解凍を待っているファイル。</param>
        public LzhFile(TraceableFile expansionGoFile)
            : base(expansionGoFile)
        {
            // Trace.WriteLine($"Lzh: {this.ExpansionGoFilePath}");
        }

        /// <summary>
        /// 解凍する。
        /// </summary>
        /// <returns>展開に成功した。</returns>
        public override bool Expand()
        {
            Trace.WriteLine($"{LogHelper.Stamp}Expand  : {this.InputFile.FullName} -> {LocationMaster.ExpandedDirectory.FullName}");
            if (string.IsNullOrWhiteSpace(this.InputFile.FullName))
            {
                return false;
            }

            LzhManager.fnExtract(this.InputFile.FullName, LocationMaster.ExpandedDirectory.FullName);

            // ディレクトリーを浅くします。
            PathFlat.GoFlat(LocationMaster.ExpandedDirectory.FullName);

            // 解凍が終わった元ファイルは削除。
            this.InputFile.Delete();

            return true;
        }
    }
}
