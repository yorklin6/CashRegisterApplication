﻿using CashRegisterApplication.model;
using CashRegiterApplication;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CashRegisterApplication.comm
{
    //异步处理数据库
    class MyTimerTask
    {
        public static void UpdateLocalGoodsMsg()
        {
            //查db里面,最后一次更新时间是多少
            int iLastAllGoodsUpdateTime = 0;
            if (!Dao.GetLocalMsgLastUpdateAllDataGoods(out iLastAllGoodsUpdateTime))
            {
                CommUiltl.Log("GetLocalMsgLastUpdateAllDataGoods error");
                return;
            }
            long iNow=CommUiltl.GetTimeStamp();
            long diff = iNow - iLastAllGoodsUpdateTime;
            //一天拉一次全量
            if (iLastAllGoodsUpdateTime == 0 || diff > 24*60*60)
            {
                _UpdateAllGoodsdate();
                //记录今天已经更新全量信息
                Dao.UpdateLocalMsgLastUpdateAllDataGoods(iNow);
            }
        }

        public static void _UpdateAllGoodsdate()
        {
            //拉出全量商品数据
            //全量数据商品少于1个的时候，表示数据有问题，不删除

            //老数据打上 老数据标志
            //插入新数据

            //删除 老数据标志 的商品
            
            //老数据的表
        
            return;
        }
        public static void AddStaockOut()
        {
            List<StockOutDTO> oStockList = new List<StockOutDTO>();
            StockOutDTO oState = new StockOutDTO();
            oState.Base.cloudAddFlag = HttpUtility.CLOUD_SATE_HTTP_FAILD;

            GetStockOutPutByDbWithCloudeState(oState, ref oStockList);

            //http redo
            foreach (var oStock in oStockList)
            {
                StockOutDTORespone oResp =new StockOutDTORespone();
               oStock.Base.cloudAddFlag = HttpUtility.GenerateOrder(oStock, ref oResp);

                if (oStock.Base.cloudAddFlag == HttpUtility.CLOUD_SATE_HTTP_SUCESS)
                {
                    //重试成功
                    oStock.Base.stockOutId = oResp.data.Base.stockOutId;
                    if (oResp.data.details.Count != oStock.details.Count)
                    {
                        //说明是有问题的
                        CommUiltl.Log("oRespond.data.details.Count[" + oResp.data.details.Count + "] != Main.oStockOutDTO.details.Count [" + oStock.details.Count + "]");
                        continue;
                    }

                    for (int i = 0; i < oStock.details.Count; ++i)
                    {
                        //更新数据库，这个流水单下面的id全部变成云端返回的id，以云端的为主。
                        oStock.details[i].id = oResp.data.details[i].id;
                    }
                    oStock.Base.cloudUpdateFlag = HttpUtility.CLOUD_SATE_HTTP_SUCESS;//新增成功，相当于无需更新
                    Dao.updateRetailStock(oStock);
                }
                else
                {
                    //重试失败，则不管，后面队列继续重试
                }
               
            }

        }//AddStaockOut

        //修改零售单
        public static void UpdateStaockOut()
        {
            List<StockOutDTO> oStockList = new List<StockOutDTO>();

            StockOutDTO oState = new StockOutDTO();
            oState.Base.cloudUpdateFlag = HttpUtility.CLOUD_SATE_HTTP_FAILD;

            GetStockOutPutByDbWithCloudeState(oState, ref oStockList);
            //http redo
            foreach (var oStock in oStockList)
            {
                StockOutDTORespone oResp = new StockOutDTORespone();
                oStock.Base.cloudUpdateFlag = HttpUtility.updateRetailStock(oStock, ref oResp);

                if (oStock.Base.cloudUpdateFlag == HttpUtility.CLOUD_SATE_HTTP_SUCESS)
                {
                    for (int i = 0; i < oStock.details.Count; ++i)
                    {

                       // oStock.details[i].id = oResp.data.details[i].id;
                    }
                    Dao.updateRetailStock(oStock);
                }
                else
                {
                    //重试失败，则不管，后面队列继续重试
                }
            }

        }//UpdateStaockOut

        //删除零售单
        public static void DeleteStaockOut()
        {
            //List<StockOutDTO> oStockList = new List<StockOutDTO>();
            //StockOutBase Base = new StockOutBase();
            //Base.cloudDeleteFlag = HttpUtility.CLOUD_SATE_HTTP_FAILD;
            //GetStockOutPutByDbWithCloudeState(Base, ref oStockList);
            ////http redo
            //foreach (var oStock in oStockList)
            //{
            //    StockOutDTORespone oResp = new StockOutDTORespone();
            //    oStock.Base.cloudUpdateFlag = HttpUtility.updateRetailStock(oStock, ref oResp);

            //    if (oStock.Base.cloudUpdateFlag == HttpUtility.CLOUD_SATE_HTTP_SUCESS)
            //    {
            //        for (int i = 0; i < oStock.details.Count; ++i)
            //        {

            //            // oStock.details[i].id = oResp.data.details[i].id;
            //        }
            //        Dao.updateRetailStock(oStock);
            //    }
            //    else
            //    {
            //        //重试失败，则不管，后面队列继续重试
            //    }
            //}

        }//DeleteStaockOut

        //关闭单
        public static void CloseStaockOut()
        {
            List<StockOutDTO> oStockList = new List<StockOutDTO>();
            StockOutDTO oState = new StockOutDTO();
            oState.Base.cloudCloseFlag = HttpUtility.CLOUD_SATE_HTTP_FAILD;
            GetStockOutPutByDbWithCloudeState(oState, ref oStockList);
            //http redo
            foreach (var oStock in oStockList)
            {
                HttpBaseRespone oRespond = new HttpBaseRespone();
                oStock.Base.cloudCloseFlag = HttpUtility.RetailSettlement(oStock, ref oRespond);

                if (oStock.Base.cloudCloseFlag == HttpUtility.CLOUD_SATE_HTTP_SUCESS)
                {
                    Dao.UpdateOrderCloudState(oStock);
                }
                else
                {
                    //重试失败，则不管，后面队列继续重试
                }
            }
        }//DeleteStaockOut

        public static void GetStockOutPutByDbWithCloudeState(StockOutDTO oState, ref List<StockOutDTO> oStockList)
        {
            List<StockOutDTO> oJsonList = new List<StockOutDTO>();
            //取出数据
            if (!Dao.GetCloudStateFailedStockOutList(oState, ref oJsonList))
            {
                return;
            }
            CommUiltl.Log("Count:" + oJsonList.Count);
            if (0 == oJsonList.Count)
            {
                return;
            }
            //把json数据还原成obj
            foreach (var item in oJsonList)
            {
                try
                {
                    StockOutDTO oTmp = JsonConvert.DeserializeObject<StockOutDTO>(item.Base.baseDataJson);
                    oStockList.Add(oTmp);
                }
                catch (Exception e)
                {
                    Console.WriteLine("DeserializeObject content error ,and coanot parse:" + e + " conten:" + item.Base.baseDataJson);
                    continue;
                }
            }
        }

    }
}
