﻿namespace Grayscale.CsaOpener.Location
{
    using System.IO;

    /// <summary>
    /// ディレクトリ。
    /// </summary>
    public class ExpansionWentDirectory
    {
        private static ExpansionWentDirectory thisInstance;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpansionWentDirectory"/> class.
        /// </summary>
        protected ExpansionWentDirectory()
        {
        }

        /// <summary>
        /// Gets a このインスタンス。
        /// </summary>
        public static ExpansionWentDirectory Instance
        {
            get
            {
                if (thisInstance == null)
                {
                    thisInstance = new ExpansionWentDirectory();
                    if (!Directory.Exists(thisInstance.Path))
                    {
                        Directory.CreateDirectory(thisInstance.Path);
                    }
                }

                return thisInstance;
            }
        }

        /// <summary>
        /// Gets a パス。
        /// </summary>
        public string Path
        {
            get { return KifuwarabeWcsc29Config.Instance.expansion.went; }
        }
    }
}
