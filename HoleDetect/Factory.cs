using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AreaDict = System.Collections.Generic.Dictionary<string, TYG.HoleDetect.Area>;


namespace TYG.HoleDetect
{
    /// <summary>Data Acquistion for Factory</summary>
    public class Factory : IDisposable
    {

        #region Private Variables

        private string strID = "";
        private string strName = "";
        private bool isStarted = false;
        private DAQ parent = null;
        private AreaDict areas = new AreaDict();

        #endregion

        #region Constructor/Destructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parent">Parent Object (Root)</param>
        /// <param name="paramID">Factory ID</param>
        /// <param name="paramName">Factory Name</param>
        internal Factory(DAQ parent, string paramID, string paramName)
        {
            this.strID = paramID;
            this.strName = paramName;
            this.parent = parent;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~Factory()
        {
            this.Dispose();
        }

        #endregion

        #region Public Property

        /// <summary>
        /// Parent Object (DAQ)
        /// </summary>
        public DAQ Parent
        {
            get { return this.parent; }
        }
        /// <summary>
        /// Factory ID
        /// </summary>
        public string ID
        {
            get { return this.strID; }
        }

        /// <summary>
        /// Factory Name
        /// </summary>
        public string Name
        {
            get { return this.strName; }
        }

        /// <summary>
        /// Get Start Status
        /// </summary>
        public bool Started
        {
            get { return this.isStarted; }
        }

        /// <summary>
        /// Area Key Collection
        /// </summary>
        public AreaDict.KeyCollection AreaKeys
        {
            get { return this.areas.Keys; }
        }

        #endregion

        #region Internal Property

        /// <summary>
        /// Area Dictionary
        /// </summary>
        internal AreaDict Areas
        {
            get { return this.areas; }
        }

        #endregion

        #region Public Method

        /// <summary>
        /// Add Area
        /// </summary>
        /// <param name="paramID">Area ID</param>
        /// <param name="paramName">Area Name</param>
        /// <param name="paramIPAddress">Master PLC IP Address</param>
        /// <param name="paramPort">Master PLC TCP Port</param>
        public void AddArea(string paramID, string paramName, string paramIPAddress, int paramPort, bool paramTestMode)
        {
            if ((!this.isStarted) && (!this.areas.ContainsKey(paramID)))
            {
                Area area = new Area(this, paramID, paramName, paramIPAddress, paramPort, paramTestMode);
                areas[paramID] = area;
            }
        }

        /// <summary>
        /// Get Area Object
        /// </summary>
        /// <param name="paramAreaID">Area ID</param>
        /// <returns></returns>
        public Area GetArea(string paramAreaID)
        {
            Area area = null;
            if (areas.ContainsKey(paramAreaID))
                area = areas[paramAreaID];
            return area;
        }

        /// <summary>
        /// Update MPile Materials
        /// </summary>
        /// <param name="paramOldMaterials">Existing Material List</param>
        /// <param name="paramOldMRatio">Existing Material Ratio</param>
        /// <param name="paramNewMaterials">New Material List</param>
        /// <param name="paramNewMRatio">New Material Ratio</param>
        public void UpdateMaterialData(string[] paramOldMaterials, string paramOldMRatio, string[] paramNewMaterials, string paramNewMRatio)
        {
            foreach (Area area in this.Areas.Values)
            {
                foreach(Platform platform in area.PlatForms.Values)
                {
                    platform.UpdateMaterialData(paramOldMaterials, paramOldMRatio, paramNewMaterials, paramNewMRatio);
                }
            }
        }

        #endregion

        #region Internal Method

        /// <summary>
        /// Start Data Collection
        /// </summary>
        internal void Start()
        {
            if (!this.isStarted)
            {
                foreach (Area area in areas.Values)
                    area.Start();
                this.isStarted = true;
            }
        }

        /// <summary>
        /// Stop Data Collection
        /// </summary>
        internal void Stop()
        {
            if (areas != null)
            {
                if (this.isStarted)
                {
                    this.isStarted = false;
                    foreach (Area area in areas.Values)
                        area.Stop();
                }
            }
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
                    this.Stop();
                    foreach (Area area in areas.Values)
                        area.Dispose();
                    this.areas.Clear();
                    this.areas = null;
                    this.parent = null;
                    this.strID = null;
                    this.strName = null;
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
