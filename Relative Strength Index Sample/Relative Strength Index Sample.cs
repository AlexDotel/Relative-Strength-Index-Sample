using System;
using cAlgo.API;
using cAlgo.API.Collections;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;
using Microsoft.VisualBasic;

namespace cAlgo.Robots
{


    [Robot(AccessRights = AccessRights.None, AddIndicators = true)]
    public class RelativeStrengthIndexSample : Robot
    {
        //Declaraciones
        private RelativeStrengthIndex rsi;


        //Parametros
        [Parameter ("Initial Volume", DefaultValue = 0.1, MinValue = 0.01)]
        public double initial_volume {get;set;}
        
        [Parameter ("Volume Multiplier", DefaultValue = 2 , MinValue = 1, MaxValue = 10)]
        public double multiplier {get; set;}
        
        [Parameter ("Target ($)", DefaultValue = 10 , MinValue = 5, MaxValue = 500)]
        public int target {get; set;}

        [Parameter ("Rsi Periods", DefaultValue = 14)]
        public int rsi_periods {get; set;}
        
        [Parameter ("Rsi OverSold", DefaultValue = 30)]
        public int rsi_oversold {get; set;}


        //Inicio de Robot
        protected override void OnStart()
        {
           rsi = Indicators.RelativeStrengthIndex(Bars.ClosePrices, rsi_periods);
        }

        //Cierre de cada vela
        protected override void OnBar()
        {
            //Actualizamos el lotaje
            var vol = Symbol.QuantityToVolumeInUnits(GetLotaje());

            //Compramos
            if(BuyCondition()){
                ExecuteMarketOrder(TradeType.Buy, SymbolName, vol);
            }
        }

        //Cada tick
        protected override void OnTick()
        {
            if (ProfitReached()){
                CloseAllPositions();
            }
        }


        //---------- Mis Metodos ----------//

        //Logica de Compra
        private bool BuyCondition (){
            return rsi.Result.Last(2) < rsi_oversold && rsi.Result.Last(1) > rsi_oversold;
        }

        //Calculo del lotaje
        private double GetLotaje(){
            double lots = initial_volume;
                for (int i = 0 ; i < Positions.Count ; i++){
                    lots *= multiplier;
                }
            return lots;
        }

        //Comprobacion de Profit
        private bool ProfitReached(){
            return Account.UnrealizedNetProfit >= target;
            //Comprobamos que la ganancia flotante es mayor o igual que el target
        }

        private void CloseAllPositions()
        {
            foreach(var position in Positions){
                ClosePositionAsync(position);
            }
        }

    }
}