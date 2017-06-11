﻿using System;
using System.Collections.Generic;
using System.Text;
using CashRegisterApplication.comm;
using CashRegiterApplication;

namespace CashRegisterApplication.model
{
    public class StockOutDTORespone
    {
        public int errorCode { get; set; }
        public string msg { get; set; }
        public StockOutDTO data;
    }
    public class LocalSaveStock
    {
        public  List<StockOutDTO> listStock;
        public  int index { get; set; }
        public LocalSaveStock()
        {
            index = 0;
            listStock = new List<StockOutDTO>();
        }
    }

    public class StockOutDTO
    {
        public StockOutBase Base;
        public List<StockOutDetail> details;
        public List<Checkout> checkouts;

        public Member oMember;
        
        public StockOutDTO()
        {
            Base = new StockOutBase();
            details = new List<StockOutDetail>();
            checkouts = new List<Checkout>();
            oMember = new Member();
        }

        internal void addChecout(Checkout oPayWay)
        {
            CommUiltl.Log("RecieveFee before:" + Base.RecieveFee);
            Base.RecieveFee += oPayWay.payAmount;
            CommUiltl.Log("RecieveFee after:" + Base.RecieveFee);
            if (Base.RecieveFee > Base.orderAmount)
            {
                Base.ChangeFee = Base.RecieveFee - Base.orderAmount;
            }
            checkouts.Add(oPayWay);
        }
    }


    public class PayWayHttpRequet
    {
        public long memberId { get; set; }

        public int tradeTime { get; set; }

        public List<Checkout> list;
        public PayWayHttpRequet()
        {
            memberId = 0;
            tradeTime = 0;
            list = new List<Checkout>();
        }
    }

    public class Checkout
    {
        public const int PAY_TYPE_CASH = 1;
        public const string PAY_TYPE_CASH_DESC = "现金";

        public const int PAY_TYPE_WEIXIN = 2;
        public const string PAY_TYPE_WEIXIN_DESC = "微信支付";

        public const int PAY_TYPE_ZHIFUBAO = 3;
        public const string PAY_TYPE_ZHIFUBAO_DESC = "支付宝支付";

       
        public int payType { get; set; }
        public long payAmount { get; set; }

        public string stockOutSerialNumber { get; set; }

        public string serialNumber { get;  set; }
        public int cloudState { get; set; }
        public int id { get; set; }
        public int posId { get; set; }
        public int payStatus { get; set; }
        public long relatedOrder { get; set; }
        public long storeId { get; set; }
        public int isDeleted { get; set; }

        public string reqMemberZfJson { get; set; }

        public string  payTypeDesc { get; set; }

        public Checkout()
        {
            id = 0;
            payType = 0;
            payAmount = 0;
            payStatus = 0;
            cloudState = 0;
            serialNumber = "";
            stockOutSerialNumber = "";
            posId = CenterContral.iPostId;
        }

        internal void generatePayOrderNumber()
        {
            serialNumber = "JZ-"+CenterContral.oStoreWhouse.storeWhouseId + "-"
                + CenterContral.iPostId + "-"
                + DateTime.Now.ToString("yyyyMMddHHmmssfff") +"-"+ CommUiltl.GetRandomNumber();
        }
    }

    public class StockOutBase
    {
        public long stockOutId { get; set; }
        public String serialNumber { get; set; }
        public Byte type { get; set; }
        public long storeId { get; set; }
        public long whouseId { get; set; }
        public long relatedOrder { get; set; }
        public long clientId { get; set; }
        public long posId { get; set; }
        public long cashierId { get; set; }
        public long orderAmount { get; set; }
        public String creator { get; set; }
        public long createTime { get; set; }
        public long updateTime { get; set; }
        public long stockOutTime { get; set; }
        public Byte status { get; set; }
        public String remark { get; set; }

        //************本地缓存数据
        public long RecieveFee { get; set; }
        public String ProductList { get; set; }//辅助变量，帮助查看商品列表是否有修改
        public long ChangeFee { get; set; }
        public long totalProductCount { get; set; }//总件数

        public String baseDataJson { get; set; }
        public int cloudAddFlag { get; set; }
        public int cloudCloseFlag { get; set; }
        public int cloudDeleteFlag { get; set; }
        public int cloudUpdateFlag { get; set; }
        public String cloudRespJson { get; set; }

      
        public int dbGenerateFlag { get; set; }
        public int localSaveFlag { get; set; }//挂单字段

        public int cancaelFlag { get; set; }

        // 折扣
        public long discountAmount { get; set; }
        public long discountRate { get; set; }
        // 全班商品累计价格
        public long allGoodsMoneyAmount { get; set; }

        public void generateSeariseNumber()
        {
            CenterContral.oStockOutDTO.Base.serialNumber = "LSCK-" +CenterContral.oStoreWhouse.storeWhouseId + "-"
                + DateTime.Now.ToString("yyyyMMddHHmmssfff") +"-" + CommUiltl.GetRandomNumber();
        }
    }
    public class StockOutDetail
    {

        public long id { get; set; }
        public long stockOutId { get; set; }
        public long goodsId { get; set; }
        public String goodsName { get; set; }
        public String barcode { get; set; }
        public String specification { get; set; }
        public String unit { get; set; }
        public long produceTime { get; set; }
        public long expireTime { get; set; }
        public long orderCount { get; set; }
        public long actualCount { get; set; }
        public long actualDifference { get; set; }
        public long unitPrice { get; set; }
        public long subtotal { get; set; }
        public String remark { get; set; }
        public   int status { get; set; }
        internal int cloudState { get; set; }

        public long categoryId;

        public long spaceId;

        public string goodsShowSpecification { get; set; }
        public ProductPricing cloudProductPricing;

        public String keyWord { get; set; }
    }



}
