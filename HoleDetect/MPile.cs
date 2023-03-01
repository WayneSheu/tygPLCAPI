using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TYG.HoleDetect
{
    public class MPile : IDisposable
    {
        #region Private Variables

        private Platform parent = null;
        private int sortNo;
        private string strID = "";
        private List<string> materials = new List<string>();
        private string ratio = "";
        private DateTime refreshTime = DateTime.Now;

        #endregion

        #region Constructor/Destructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parent">Parent Object (Platform)</param>
        /// <param name="paramSortNo">Mpile Sort No</paramSortNo>
        internal MPile(Platform parent, int paramSortNo)
        {
            this.parent = parent;
            this.sortNo = paramSortNo;
            this.strID += Convert.ToChar(paramSortNo + 64);
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~MPile()
        {
            this.Dispose();
        }

        #endregion

        #region Public Property

        /// <summary>
        /// Parent Object (Platform)
        /// </summary>
        public Platform Parent
        {
            get { return this.parent; }
        }

        /// <summary>
        /// Sort No        /// </summary>
        public int SortNo
        {
            get { return this.sortNo; }
        }

        /// <summary>
        /// Mpile ID
        /// </summary>
        public string ID
        {
            get { return this.strID; }
        }

        /// <summary>
        /// Materia lList
        /// </summary>
        public List<string> MaterialList
        {
            get { return this.materials; }
        }

        /// <summary>
        /// Material Ratio
        /// </summary>
        public string MaterialRatio
        {
            get { return this.ratio; }
        }

        /// <summary>
        /// Data Refresh Time
        /// </summary>
        public DateTime RefreshTime
        {
            get { return this.refreshTime; }
            private set
            {
                if (this.refreshTime != value)
                {
                    this.refreshTime = value;
                    this.Parent.RefreshTime = this.refreshTime;
                }
            }
        }

        #endregion

        #region Internal Method

        /// <summary>
        /// Set Material List
        /// </summary>
        /// <param name="paramMaterial1">Material one</paramMaterial1>
        /// <param name="paramMaterial2">Material two</paramMaterial2>
        /// <param name="paramMaterial3">Material three</paramMaterial3>
        /// <param name="paramMRatio">Material Ratio</paramMRatio>
        internal void SetMaterials(string paramMaterial1, string paramMaterial2, string paramMaterial3, string paramMRatio)
        {
            string material;
            this.materials.Clear();
            material = paramMaterial1;
            if (material != null)
            {
                if (material.Length > 0)
                {
                    if (!this.materials.Contains(material))
                        this.materials.Add(material);
                }
            }
        
            material = paramMaterial2;
            if (material != null)
            {
                if (material.Length > 0)
                {
                    if (!this.materials.Contains(material))
                        this.materials.Add(material);
                }
            }

            material = paramMaterial3;
            if (material != null)
            {
                if (material.Length > 0)
                {
                    if (!this.materials.Contains(material))
                        this.materials.Add(material);
                }
            }
            this.ratio = paramMRatio;
            this.RefreshTime = DateTime.Now;
        }

        /// <summary>
        /// Check Material List
        /// </summary>
        /// <param name="paramMaterial1">Material one</param>
        /// <param name="paramMaterial2">Material two</param>
        /// <param name="paramMaterial3">Material three</param>
        /// <param name="paramMRatio">Material ratio</param>
        /// <returns>Compared Result(True/False)</returns>
        internal bool CheckMaterials(string paramMaterial1, string paramMaterial2, string paramMaterial3, string paramMRatio)
        {
            int count = 0;
            string material;
            material = paramMaterial1.Trim();
            if (this.materials.Contains(material))
                count += 1;
            material = paramMaterial2.Trim();
            if (this.materials.Contains(material))
                count += 1;
            material = paramMaterial3.Trim();
            if (this.materials.Contains(material))
                count += 1;

            return ((count > 0) && (count == this.materials.Count) && (paramMRatio == this.MaterialRatio));
        }

        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // 偵測多餘的呼叫

        /// <summary>
        /// Dispose (IDisposable)
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (this.materials != null)
                    {
                        this.materials.Clear();
                        this.materials = null;
                    }
                    this.parent = null;
                    this.strID = null;
                }
                disposedValue = true;
            }
        }

        /// <summary>
        /// Dispose (IDisposable)
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion



    }

}
