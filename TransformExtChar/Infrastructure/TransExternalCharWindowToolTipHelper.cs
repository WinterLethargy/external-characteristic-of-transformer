using System;
using System.Collections.Generic;
using System.Text;
using TransformExtChar.Model;

namespace TransformExtChar.Infrastructure
{
    public class TransExternalCharWindowToolTipHelper
    {
        public Dictionary<string, string> ToolTipDictionary { get; } 
        public TransExternalCharWindowToolTipHelper()
        {
            ToolTipDictionary = new Dictionary<string, string>()
            {
                #region TransformerDatasheet

                {nameof(TransformerDatasheet.I0), "Ток холостого хода\n(Линейный в случае трехфазного трансформатора)" },
                {nameof(TransformerDatasheet.I0_Percent), "Проценты от номинального тока первичной обмотки" },
                {nameof(TransformerDatasheet.U1r), "Номинальное первичное напряжение\n(Линейное в случае трехфазного трансформатора)" },
                {nameof(TransformerDatasheet.U2r),  "Номинальное вторичное напряжение\n(Линейное в случае трехфазного трансформатора)" },
                {nameof(TransformerDatasheet.I1r), "Номинальный ток первичной обмотки\n(Линейный в случае трехфазного трансформатора)" },
                {nameof(TransformerDatasheet.P0),  "Мощность холостого хода\n(Суммарная мощность трех фаз в случае трехфазного трансформатора)" },
                {nameof(TransformerDatasheet.U1sc), "Напряжение короткого замыкания (при номинальном токе первичной обмотки)\n(Линейное в случае трехфазного трансформатора)"},
                {nameof(TransformerDatasheet.U1sc_Percent), "Проценты от номинального напряжения первичной обмотки"},
                {nameof(TransformerDatasheet.Psc), "Мощность короткого замыкания (при номинальном токе первичной обмотки)\n(Суммарная мощность трех фаз в случае трехфазного трансформатора)" },

                #endregion

                #region TransformerTypeEnum и StarOrTriangleEnum

                {nameof(TransformerTypeEnum), "В случае не выбранного типа трансформатора рассчитываются приведенные ток и напряжение\nСхема замещения расчитывается по паспортным данным как в случае однофазного трансформатора\n\nВ остальных случаях расчитываются реальные ток и напряжение" },
                {nameof(StarOrTriangleEnum), "Соединение первичной обмотки влияет на расчет схемы замещения из паспортных данных\nСоединение вторичной обмотки влияет на пересчет приведенных тока и напряжения к реальным" },

                #endregion

                #region EquivalentCurcuit

                {nameof(EquivalentCurcuit.R1), "Активное сопротивление первичной обмотки" },
                {nameof(EquivalentCurcuit.R2_Corrected), "Приведенное активное сопротивление вторичной обмотки" },
                {nameof(EquivalentCurcuit.Rm), "Активное сопротивление ветви намагничивания"},
                {nameof(EquivalentCurcuit.X1), "Реактивное сопротивление первичной обмотки" },
                {nameof(EquivalentCurcuit.X2_Corrected), "Приведенное реактивное сопротивление вторичной обмотки" },
                {nameof(EquivalentCurcuit.Xm), "Реактивное сопротивление первичной обмотки" },
                {nameof(EquivalentCurcuit.K), "Коэффициент трансформации" },

                #endregion

                #region ModeParameters

                {nameof(ModeParameters.U1),  "Первичное напряжение\n(Линейное для трехфазного транснорматора)" },
                {nameof(ModeParameters.Fi2), "Угол сопротивления нагрузки\n(Симмитричная нагрузка в случае трехфазного транснорматора)" },
                {nameof(ModeParameters.I2_start),  "Начальный ток\n(Линейный для трехфазного транснорматора)" },
                {nameof(ModeParameters.I2_end), "Конечный ток\n(Линейный для трехфазного транснорматора)" },
                {nameof(ModeParameters.I2_step), "Шаг расчета" }

	            #endregion
            };
        }
    }
}
