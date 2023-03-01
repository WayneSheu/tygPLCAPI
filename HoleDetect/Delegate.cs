using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TYG.HoleDetect
{
    /// <summary>
    /// Hole Detection Changed Event Handler
    /// </summary>
    /// <param name="Sender">Sender</param>
    /// <param name="e"></param>
    public delegate void HoleChangedEventHandler(object Sender, HoleChangedArgument e);

    /// <summary>
    /// Delegate for Machine DAQ Data Changed EventHandler
    /// </summary>
    /// <param name="Sender">Sender</param>
    /// <param name="e">Event Argument</param>
    public delegate void DataChangedEventHandler(object Sender, DataChangedArgument e);

    /// <summary>
    /// Delegate for Machine Material Check EventHandler
    /// </summary>
    /// <param name="Sender">Sender</param>
    /// <param name="e">Event Argument</param>
    public delegate void MaterialCheckEventHandler(object Sender, MaterialCheckArgument e);

    /// <summary>
    /// Delegate for Connect Changed EventHandler
    /// </summary>
    /// <param name="Sender">Sender</param>
    /// <param name="e">Event Argument</param>
    public delegate void ModbusConnectChangedEventHandler(object Sender, ModbusConnectChangedArgument e);

    /// <summary>
    /// Delegate forAlarm Happened EventHandler
    /// </summary>
    /// <param name="Sender">Sender</param>
    /// <param name="e">Event Argument</param>
    public delegate void ModbusAlarmHappenedEventHandler(object Sender, ModbusAlarmHappenedArgument e);
}
