using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dands.Modbus
{
    /// <summary>
    /// Delegate for Connect Changed EventHandler
    /// </summary>
    /// <param name="Sender">Sender</param>
    /// <param name="e">Event Argument</param>
    public delegate void ConnectChangedEventHandler(object Sender, ConnectChangedArgument e);

    /// <summary>
    /// Delegate for Alarm Happened EventHandler
    /// </summary>
    /// <param name="Sender">Sender</param>
    /// <param name="e">Event Argument</param>
    public delegate void AlarmHappenedEventHandler(object Sender, AlarmHappenedArgument e);
}
