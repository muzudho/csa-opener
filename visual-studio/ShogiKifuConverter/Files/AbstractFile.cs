﻿namespace Grayscale.ShogiKifuConverter
{
    using System.IO;
    using Grayscale.ShogiKifuConverter.Commons;
    using Grayscale.ShogiKifuConverter.Location;

    /// <summary>
    /// さまざまなファイル。
    /// </summary>
    public abstract class AbstractFile
    {
        private TraceableFile expansionGoFileInstance;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractFile"/> class.
        /// </summary>
        /// <param name="expansionGoFile">解凍を待っているファイル。</param>
        protected AbstractFile(TraceableFile expansionGoFile)
        {
            this.ExpansionGoFile = expansionGoFile;
        }

        /// <summary>
        /// Gets or sets a 解凍を待っているファイル。
        /// </summary>
        public TraceableFile ExpansionGoFile
        {
            get
            {
                return this.expansionGoFileInstance;
            }

            protected set
            {
                this.expansionGoFileInstance = value;
            }
        }

        /// <summary>
        /// 解凍する。
        /// </summary>
        /// <returns>展開に成功した。</returns>
        public virtual bool Expand()
        {
            return false;
        }

        /// <summary>
        /// 任意の棋譜をRPMに変換する。
        /// </summary>
        /// <returns>成功。</returns>
        public virtual bool ConvertAnyFileToRpm()
        {
            // TODO OpenerConfig.Instance
            return false;
        }
    }
}
