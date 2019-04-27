﻿namespace Grayscale.CsaOpener
{
    using System.IO;
    using Grayscale.CsaOpener.Commons;
    using Grayscale.CsaOpener.Location;

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
        public TraceableFile ExpansionGoFile {
            get
            {
                return this.expansionGoFileInstance;
            }

            protected set
            {
                this.expansionGoFileInstance = value;
                this.ExpansionWentFile = new TraceableFile(PathHelper.Combine(ExpansionWentDirectory.Instance.FullName, Path.GetFileName(this.ExpansionGoFile.FullName)));
            }
        }

        /// <summary>
        /// Gets or sets a 解凍が終わったファイル。
        /// </summary>
        public TraceableFile ExpansionWentFile { get; private set; }

        /// <summary>
        /// 解凍する。
        /// </summary>
        /// <returns>展開に成功した。</returns>
        public virtual bool Expand()
        {
            return false;
        }

        /// <summary>
        /// 棋譜を読み取る。
        /// </summary>
        public virtual void ReadGameRecord()
        {
            // OpenerConfig.Instance
        }
    }
}
