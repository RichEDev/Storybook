using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpendManagementLibrary
{
    [Serializable()]
    public struct sCarInfo
    {
        public Dictionary<int, cCar> lstonlinecars;
        public List<int> lstcarids;
        public Dictionary<int, cOdometerReading> lstonlineodoreadings;
        public SortedList<int, int> lstodoids;
    }
}
