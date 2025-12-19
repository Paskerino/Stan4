using CommonLogic.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stand4
{
    public interface IStatusLineParsingService
    {
        string ParseStatusLine(SensorReading statusLine);
    }
}
