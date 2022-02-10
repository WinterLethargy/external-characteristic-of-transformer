using System;
using System.Collections.Generic;
using System.Text;

namespace TransformExtChar.Infrastructure
{
    public enum MessageEnum
    {
        /// <summary>
        /// Обновить плоттер, сбросив оси
        /// </summary>
        UpdatePlotter_UpdateDataTrue,

        /// <summary>
        /// Обновить плоттер, сбросив оси
        /// </summary>
        UpdatePlotter_UpdateDataFalse,

        /// <summary>
        /// Обновить плоттер без сброса осей
        /// </summary>
        InvalidatePlot_UpdateDataFalse,

        /// <summary>
        /// Обновить плоттер без сброса осей
        /// </summary>
        InvalidatePlot_UpdateDataTrue,
        CalcParamFromDataSheet_Open,
        CalcParamFromDataSheet_Close,
        AddFixedSeries_Open,
        AddUsersSeries_Open,
        AddSeries_Close
    }
}
