﻿using CashRegisterApplication.model;
using CashRegisterApplication.window;
using CashRegisterApplication.window.member;
using CashRegisterApplication.window.Member;
using CashRegisterApplication.window.productList;
using CashRegiterApplication;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace CashRegisterApplication.comm
{

    public static class CenterContral
    {

        public static bool initFlag = false;
        public static ProductListWindow Window_ProductList;//全局窗口
        public static RecieveMoneyWindow Window_RecieveMoney;//收款窗口
        public static ReceiveMoneyByCashWindow Window_ReceiveMoneyByCash;//现金收款窗口
        public static ReceiveMoneyByMember Window_ReceiveMoneyByMember;//会员收款窗口



        public static RechargeMoneyForMember Window_RechargeMoneyForMember;//充值会员窗口


        public static MemberInfoWindows Window_MemberInfoWindows;//输入会员弹窗
        public static DiscountWindows Window_DiscountWindows;

        public static StockOutDTO oStockOutDTO;//当前单据信息


        public static StockOutDTORespone oStockOutDToRespond;
        public static HttpBaseRespone oHttpRespone;

        public static LocalSaveStock oLocalSaveStock;//挂单信息

        public static PayTypeData oPayTypeList;

        public static Member oMember;//用户账户

        public static PayWay oPayWay;//支付信息
        public static PayType oCurrentPayType;// 支付类型全局

        public static UserLogin oLoginer;//登录用户


        public const int PAY_STATE_INIT = 0;
        public const int PAY_STATE_SUCCESS = 1;
        public const int PAY_TYPE_CASH = 1;
        public const int MEMBER_PAY_TYPE = 0;


        public const int STOCK_BASE_STATUS_INIT = 0;
        public const int STOCK_BASE_STATUS_NORMAL = 1;
        public const int STOCK_BASE_STATUS_OUT = 2;



        public const int CLOUD_SATE_PAY_SUCESS = 0;
        public const int CLOUD_SATE_PAY_FAILD = 0;



        public const int STOCK_BASE_DB_GENERATE_INIT = 0;
        public const int STOCK_BASE_DB_GENERATE_DONE = 1;//已经存在DB


        public const int CLOUD_SATE_PAY_GENERATE_INIT = 0;
        public const int CLOUD_SATE_PAY_GENERATE_SUCCESS = 1;
        public const int CLOUD_SATE_PAY_GENERATE_FAILED = 2;

        public const int CLOUD_SATE_PAY_UPDATE_SUCCESS = 3;


        public static int PRODUCTlIST_WINDOW = 0;//商品列表页
        public static int MEMBER_RECHAREGE_WINDOWS = 1;//支付页面
        public static int MEMBER_RECIEVE_MONEY_WINDOWS = 2;//会员收款页面


        public static int flagCallShowMember = MEMBER_RECIEVE_MONEY_WINDOWS;

        public static int flagCallShowRecharge = PRODUCTlIST_WINDOW;

        public static StoreWhouse oStoreWhouse;
        public static int iPostId = 0;
        public static int store_house_selete_flag;
        public static StoreWhouseData oStoreWhouseData;
        public static int store_whouse_id = 0;
        public const int CLOUD_SATE_PAY_UPDATE_FAILED = 4;

        public const int STORE_HOUSE_UNSET_SELETED = 0;
        public const int STORE_HOUSE_SELETED = 1;
        public static void Init()
        {
            if (initFlag == true)
            {
                return;
            }
            //Window_ProductList = new ProductListWindow();//全局窗口
            Window_RecieveMoney = new RecieveMoneyWindow();//收款窗口
            Window_ReceiveMoneyByCash = new ReceiveMoneyByCashWindow();//现金收款窗口
            Window_ReceiveMoneyByMember = new ReceiveMoneyByMember();
            Window_RechargeMoneyForMember = new RechargeMoneyForMember();

            Window_MemberInfoWindows = new MemberInfoWindows();
            Window_DiscountWindows = new DiscountWindows();
            oStockOutDTO = new StockOutDTO();//商品列表
            oStockOutDToRespond = new StockOutDTORespone();
            oHttpRespone = new HttpBaseRespone();
            oMember = new Member();
            oPayWay = new PayWay();
            oStoreWhouse = new StoreWhouse();
            store_house_selete_flag = STORE_HOUSE_UNSET_SELETED;
            oStoreWhouseData = new StoreWhouseData();
            oLoginer = new UserLogin();

            initFlag = true;

            oLocalSaveStock = new LocalSaveStock();
            oPayTypeList = new PayTypeData();
            //先默认登陆，取可信任的登陆态
           // CenterContral.InitDefaultLogin();
            Dao.ConnecSql();
            GetDbMsgToCenterConalMsg();//设置默认数据
  
        }
        public static void GetDbMsgToCenterConalMsg()
        {
            _InitDbLocalMsg();
            _GetSaveStock();//挂单数据
            //门店信息
            GetStoreMsgFromDb();
            //Post机Id设置
            GetPostIdFromDb();
            _GetPayTypeList();//支付类型
        }


        public static void InitDefaultLogin()
        {
            HttpUtility.LoginDefault();
        }

        
        public static void _GetSaveStock()
        {
            //查出挂单的单据
            StockOutDTO oState = new StockOutDTO();
            oState.Base.localSaveFlag = Dao.STOCK_BASE_SAVE_FLAG_SAVING;
            List<StockOutDTO> oJsonList = new List<StockOutDTO>();
            Dao.GetCloudStateFailedStockOutList(oState, ref oJsonList);
            CommUiltl.Log("GetCloudStateFailedStockOutList：" + oJsonList.Count);
            if (0 == oJsonList.Count)
            {
                return;
            }
            foreach (var item in oJsonList)
            {
                try
                {
                    StockOutDTO oTmp = JsonConvert.DeserializeObject<StockOutDTO>(item.Base.baseDataJson);
                    oLocalSaveStock.listStock.Add(oTmp);
                }
                catch (Exception e)
                {
                    CommUiltl.Log("DeserializeObject content error ,and coanot parse:" + e + " conten:" + item.Base.baseDataJson);
                    continue;
                }
            }
        }

        /******************门店信息******************/
        internal static void GetStoreMsg()
        {
            if (!HttpUtility.GetStoreMsg(ref CenterContral.oStoreWhouseData))
            {
                return;
            }
            return;
        }
        internal static void GetStoreMsgFromDb()
        {
            string strLoacalJson = "";
            if (!Dao.GetStoreWhouseDefault(ref strLoacalJson))
            {
                return;
            }
            if (strLoacalJson != "")
            {
                store_house_selete_flag = STORE_HOUSE_SELETED;
                CenterContral.oStoreWhouse = JsonConvert.DeserializeObject<StoreWhouse>(strLoacalJson);
            }
            return;
        }
        /******************登陆**********************/
        internal static bool Login(string userName, string password, long storeId)
        {
            if (!HttpUtility.Login(userName, password,  storeId))
            {
                return false;
            }

            return true;
        }

        //****************************会员收款和充值
        //显示会员收款
        internal static void Show_MemberInfoWindow_By_RecieveMoeneyByMember()
        {
            flagCallShowMember = MEMBER_RECIEVE_MONEY_WINDOWS;
            if (CenterContral.Window_MemberInfoWindows == null)
            {
                CommUiltl.Log("CenterContral.Window_MemberInfoWindows == null");
            }
            CenterContral.Window_MemberInfoWindows.ShowWhithMember();
            
        }
        internal static void Show_MemberInfoWindow_By_RechargeMoeneyByMember()
        {
            flagCallShowMember = MEMBER_RECHAREGE_WINDOWS;
            CenterContral.Window_MemberInfoWindows.ShowWhithMember();
        }


        //当获取会员信息成功后进行显示页面
        internal static void ShowWindowWhenGetMemberSuccess()
        {
            if (flagCallShowMember == MEMBER_RECIEVE_MONEY_WINDOWS)
            {
                CenterContral.Window_ReceiveMoneyByMember.ShowWithMemberInfo();
                return;
            }
            if (flagCallShowMember == MEMBER_RECHAREGE_WINDOWS)
            {
                CenterContral.Window_RechargeMoneyForMember.ShowWithMemberInfo();
                return;
            }
        }
        //更新会员价
        internal static void UpdateStockOrderByMemberInfo()
        {
            //StockOutDTO
            _CaculateMemberPrice();
            CenterContral.GetGoodsStringWithoutMemberPrice();
            return;
        }
        internal static void _CaculateMemberPrice()
        {
            long totalPrice = 0;
            for (var i = 0; i < CenterContral.oStockOutDTO.details.Count; ++i)
            {
                //设置会员价
                if (CenterContral.oStockOutDTO.details[i].unitPrice == CenterContral.oStockOutDTO.details[i].cloudProductPricing.retailPrice)
                {
                    CenterContral.oStockOutDTO.details[i].unitPrice = CenterContral.oStockOutDTO.details[i].cloudProductPricing.memberPrice;
                }
                totalPrice += CenterContral.oStockOutDTO.details[i].unitPrice;
            }
            CenterContral.oStockOutDTO.Base.orderAmount = totalPrice;
            CenterContral.Window_ProductList.SetProductListWindowByStockOut(CenterContral.oStockOutDTO);
            //更新数据库里面订单信息
            if (!Dao.updateRetailStock(CenterContral.oStockOutDTO))
            {
                return;
            }
        }

        internal static bool SetCurrentPayTypeById(int payTypeId)
        {
            //找出支付类型为paytypid元素
            for(int index=0; index < CenterContral.oPayTypeList.list.Count;++index)
            {
               if( CenterContral.oPayTypeList.list[index].payTypeId == payTypeId)
                {
                   
                    CenterContral.oCurrentPayType = CenterContral.oPayTypeList.list[index];
                    //支付信息预设
                    CenterContral.oPayWay.payType = CenterContral.oCurrentPayType.payTypeId;
                    CenterContral.oPayWay.generatePayOrderNumber();
                    CenterContral.oPayWay.stockOutSerialNumber = CenterContral.oStockOutDTO.Base.serialNumber;
                    CenterContral.oPayWay.payStatus = CenterContral.PAY_STATE_SUCCESS;
                    CenterContral.oPayWay.cloudState = CenterContral.CLOUD_SATE_PAY_SUCESS;
             
                    return true;
                }
            }
            return false;
        }
        internal static PayType GetPayTyIndexByPayTypeId( int index)
        {
            return CenterContral.oPayTypeList.list[index];
        }
        internal static void GetGoodsStringWithoutMemberPrice()
        {
            string strTmp = "";
            for (var i = 0; i < CenterContral.oStockOutDTO.details.Count; ++i)
            {
                //设置会员价
                if (CenterContral.oStockOutDTO.details[i].unitPrice != CenterContral.oStockOutDTO.details[i].cloudProductPricing.retailPrice)
                {
                    //strTmp +="id:"+Main.oStockOutDTO.details[i].goodsId+" ";
                    strTmp += CenterContral.oStockOutDTO.details[i].goodsName;
                    strTmp += " 会员价:" + CommUiltl.CoverMoneyUnionToStrYuan(CenterContral.oStockOutDTO.details[i].cloudProductPricing.memberPrice);
                    strTmp += " 现价:" + CommUiltl.CoverMoneyUnionToStrYuan(CenterContral.oStockOutDTO.details[i].unitPrice);
                    strTmp += "\n";
                }

            }
            CenterContral.oMember.goodsStringWithoutMemberPrice = strTmp;

        }
        //***************************充值相关

        internal static void ShowWindows_RechargeMoneyForMember()
        {
            flagCallShowRecharge = PRODUCTlIST_WINDOW;
            Window_RechargeMoneyForMember.ShowByProductListWindow();
        }

        //充值后返回
        internal static void ControlWindowsAfterRecharge()
        {
            if (flagCallShowRecharge == PRODUCTlIST_WINDOW)
            {
                CenterContral.Window_ProductList.Show();
            }
            if (flagCallShowRecharge == MEMBER_RECIEVE_MONEY_WINDOWS)
            {
                CenterContral.Window_ProductList.Show();
            }
        }

        //当会员取消界面
        internal static void ShowWindowWhenMemberInfoCancel()
        {

            if (flagCallShowMember == MEMBER_RECIEVE_MONEY_WINDOWS)
            {
                CenterContral.Window_RecieveMoney.Show();
                return;
            }
            if (flagCallShowMember == MEMBER_RECHAREGE_WINDOWS)
            {
                CenterContral.Window_ProductList.Show();
                return;
            }
        }

        internal static bool GeneratePostId()
        {
            if (!HttpUtility.GeneratePostId(CenterContral.oStoreWhouse.storeWhouseId, CommUiltl.GetMacInfo(), ref CenterContral.iPostId))
            {
                MessageBox.Show("生成post机ID失败:" + HttpUtility.lastErrorMsg);
                return false;
            }
            return true;
        }

        public static void Clean()
        {
            CenterContral.oStockOutDTO = new StockOutDTO();//商品列表
            CenterContral.oStockOutDToRespond = new StockOutDTORespone();

            CenterContral.oStockOutDTO.Base.generateSeariseNumber();
            CenterContral.oStockOutDTO.Base.stockOutId = 0;
            CenterContral.oStockOutDTO.Base.RecieveFee = 0;
            CenterContral.oStockOutDTO.Base.orderAmount = 0;
            CenterContral.oStockOutDTO.Base.ChangeFee = 0;
            CenterContral.oStockOutDTO.Base.allGoodsMoneyAmount = 0;

            CenterContral.oStockOutDTO.Base.type = 1;
            CenterContral.oStockOutDTO.Base.storeId = CenterContral.oStoreWhouse.storeId;
            CenterContral.oStockOutDTO.Base.whouseId = CenterContral.oStoreWhouse.storeWhouseId;
            CenterContral.oStockOutDTO.Base.relatedOrder = 0;
            CenterContral.oStockOutDTO.Base.posId = CenterContral.iPostId;
            CenterContral.oStockOutDTO.Base.clientId = 1;
          //  CenterContral.oStockOutDTO.Base.cashierId = CenterContral.oLoginer.data.id;
       
          //  CenterContral.oStockOutDTO.Base.creator = CenterContral.oLoginer.data.userName;
            CenterContral.oStockOutDTO.Base.status = CenterContral.STOCK_BASE_STATUS_INIT;
            CenterContral.oStockOutDTO.Base.remark = "";

            CenterContral.oStockOutDTO.Base.cloudAddFlag = HttpUtility.CLOUD_SATE_HTTP_FAILD;
            CenterContral.oStockOutDTO.Base.cloudUpdateFlag = HttpUtility.CLOUD_SATE_HTTP_FAILD;
            CenterContral.oStockOutDTO.Base.baseDataJson = "";
            CenterContral.oStockOutDTO.Base.cloudCloseFlag = HttpUtility.CLOUD_SATE_HTTP_FAILD;
            CenterContral.oStockOutDTO.Base.cloudDeleteFlag = HttpUtility.CLOUD_SATE_HTTP_FAILD;

            CenterContral.oStockOutDTO.Base.localSaveFlag = Dao.STOCK_BASE_SAVE_FLAG_INIT;
            CenterContral.oStockOutDTO.Base.dbGenerateFlag = CenterContral.STOCK_BASE_DB_GENERATE_INIT;


            //收款折扣
            CenterContral.oStockOutDTO.Base.discountRate = 100;
            CenterContral.oStockOutDTO.Base.discountAmount =0;

            CenterContral.Window_ProductList.UpdateDiscount();
            //*收银台界面
            CenterContral.Window_ProductList.SetSerialNumber(CenterContral.oStockOutDTO.Base.serialNumber);
            CenterContral.Window_ProductList.SetStoreName(CenterContral.oStoreWhouse.name);
            
    }
       public static void updateOrderAmount(long orderPrice)
        {
            CommUiltl.Log(" updateOrderAmount:" + orderPrice);
            CenterContral.oStockOutDTO.Base.allGoodsMoneyAmount = orderPrice;
            CenterContral.UpdateDiscountRate(CenterContral.oStockOutDTO.Base.discountRate);
        }
        public static void UpdateDiscountRate(long discountRate)
        {
            CenterContral.oStockOutDTO.Base.orderAmount = GetMoneyAmountByDiscountRate(discountRate);
            CenterContral.oStockOutDTO.Base.discountRate = discountRate;
            CenterContral.oStockOutDTO.Base.discountAmount = CenterContral.oStockOutDTO.Base.allGoodsMoneyAmount - CenterContral.oStockOutDTO.Base.orderAmount;
            CenterContral.Window_ProductList.UpdateDiscount();
        }
        public static long GetMoneyAmountByDiscountRate(long discountRate)
        {
            //CenterContral.oStockOutDTO.Base.allGoodsMoneyAmount本来是4位长度
            return (long)( (double)(CenterContral.oStockOutDTO.Base.allGoodsMoneyAmount) /100 * discountRate);
        }
        public static void ControlWindowsAfterPay()
        {
            CommUiltl.Log("ControlWindowsAfterPay");
            if (CenterContral.oStockOutDTO.Base.RecieveFee < CenterContral.oStockOutDTO.Base.orderAmount)
            {
                CommUiltl.Log("Window_RecieveMoney Show");
                Window_RecieveMoney.ShowPaidMsg();
                return;
            }
            //Order.RecieveFee >= Order.orderAmount 说明已经收钱完毕
            if (!CloseOrderWhenPayAllFee())
            {
                return;
            }
            Window_ProductList.CloseOrderByControlWindow();
        }
        //***********************************关闭订单***************************
        internal static bool CloseOrderWhenPayAllFee()
        {
            CenterContral.oStockOutDTO.Base.status = STOCK_BASE_STATUS_OUT;
            SetSaveFlag();//挂单->关单
            CenterContral.oStockOutDTO.Base.cloudCloseFlag = HttpUtility.RetailSettlement(CenterContral.oStockOutDTO, ref CenterContral.oHttpRespone);
            CenterContral.oStockOutDTO.Base.baseDataJson = JsonConvert.SerializeObject(CenterContral.oStockOutDTO);

            if (!Dao.updateRetailStock(CenterContral.oStockOutDTO))
            {
                return false;
            }
            RemoveSaveStock();//挂单->关单
            return true;
        }
        internal static void RemoveSaveStock()
        {
            if (CenterContral.oStockOutDTO.Base.localSaveFlag == Dao.STOCK_BASE_SAVE_FLAG_CLOSE)
            {
                //挂单给关闭掉
                for (int i = 0; i < CenterContral.oLocalSaveStock.listStock.Count; ++i)
                {
                    if (oStockOutDTO.Base.serialNumber == CenterContral.oLocalSaveStock.listStock[i].Base.serialNumber)
                    {
                        CenterContral.oLocalSaveStock.listStock.RemoveAt(i);
                        break;
                    }
                }
                CenterContral.Window_ProductList.SetLocalSaveDataNumber();
            }
        }
        internal static void SetSaveFlag()
        {
            if (CenterContral.oStockOutDTO.Base.localSaveFlag == Dao.STOCK_BASE_SAVE_FLAG_SAVING)
            {
                CenterContral.oStockOutDTO.Base.localSaveFlag = Dao.STOCK_BASE_SAVE_FLAG_CLOSE;
            }
        }
        //***********************************生成订单***************************
        internal static bool IsCurrentOrderInit()
        {
            return CenterContral.oStockOutDTO.Base.dbGenerateFlag == CenterContral.STOCK_BASE_DB_GENERATE_INIT;
        }

        internal static bool GenerateOrder(string strProductList)
        {
            if (CenterContral.oStockOutDTO.details.Count == 0)
            {
                CommUiltl.Log("Main.oStockOutDTO.details.Count == 0]");
                return true;
            }
            CommUiltl.Log("CenterContral.oStockOutDTO.Base.dbGenerateFlag:"+CenterContral.oStockOutDTO.Base.dbGenerateFlag);
            if (IsCurrentOrderInit())
            {
                
                CenterContral.oStockOutDTO.Base.ProductList = strProductList;
                CommUiltl.Log("Order.OrderCode ==  empty GenerateOrder ");

                // CenterContral.oStockOutDTO.Base.cloudAddFlag = HttpUtility.GenerateOrder(CenterContral.oStockOutDTO, ref CenterContral.oStockOutDToRespond);
                CenterContral.oStockOutDTO.Base.cloudAddFlag = HttpUtility.CLOUD_SATE_HTTP_SUCESS;
                CenterContral.oStockOutDTO.Base.dbGenerateFlag = CenterContral.STOCK_BASE_DB_GENERATE_DONE;//新增

                CenterContral.oStockOutDTO.Base.baseDataJson = JsonConvert.SerializeObject(CenterContral.oStockOutDTO);
                //插入本地数据库表
                if (!Dao.GenerateOrder(CenterContral.oStockOutDTO))
                {
                    return false;
                }
                return true;
            }
            //更新订单
            if (strProductList != null && 0 != CenterContral.oStockOutDTO.Base.ProductList.CompareTo(strProductList))
            {
                CommUiltl.Log(" strProductList is modify [" + CenterContral.oStockOutDTO.Base.ProductList + "] -> [" + strProductList + "]");
                CenterContral.oStockOutDTO.Base.ProductList = strProductList;
                CenterContral.oStockOutDTO.Base.cloudUpdateFlag = HttpUtility.updateRetailStock(CenterContral.oStockOutDTO, ref CenterContral.oStockOutDToRespond);

                CenterContral.oStockOutDTO.Base.baseDataJson = JsonConvert.SerializeObject(CenterContral.oStockOutDTO);
                if (!Dao.updateRetailStock(CenterContral.oStockOutDTO))
                {
                    return false;
                }

                return true;
            }
            CommUiltl.Log(" not modify strProductList:" + strProductList);
            return true;
        }

        internal static ProductPricing GetGoodsByProductCode(string barcode)
        {
            ProductPricingInfoResp oStockOutDetailInfoResp = new ProductPricingInfoResp();
            ProductPricing productInfo =null;
            if (!HttpUtility.GetProductByBarcode(barcode, ref oStockOutDetailInfoResp))
            {
                //网络出现错误，要访问本地
                string strJson = "";
                if (! Dao.GetProductByBarcode(barcode, ref strJson))
                {
                    MessageBox.Show("本地未找到商品资料");
                    return productInfo;
                }
                CommUiltl.Log("local strJson:" + strJson);
                if (strJson == "" )
                {
                    MessageBox.Show("本地未找到商品资料");
                    return productInfo;
                }
                productInfo = JsonConvert.DeserializeObject<ProductPricing>(strJson);
                CommUiltl.Log("GetGoodsByProductCode get goods ok:"+ productInfo.barcode);
                MessageBox.Show("网络不稳定，使用本地商品信息");
                return productInfo;
            }
            if (oStockOutDetailInfoResp.errorCode != 0 )
            {
                MessageBox.Show("后台返回错误:"+HttpUtility.lastErrorMsg);
                return productInfo;
            }
            if (oStockOutDetailInfoResp.data.list.Count == 0)
            {
                MessageBox.Show("未找到商品资料");
                return productInfo;
            }
            productInfo = oStockOutDetailInfoResp.data.list[0];
            CommUiltl.Log("http GetGoodsByProductCode get goods ok:" + productInfo.barcode);
            return productInfo;
        }

        //************************挂单***********************
        internal static bool SaveStock(string strProductList)
        {
            //生成订单，状态为挂单
            CenterContral.oStockOutDTO.Base.ProductList = strProductList;
            CenterContral.oStockOutDTO.Base.localSaveFlag = Dao.STOCK_BASE_SAVE_FLAG_SAVING;
            if (!CenterContral.GenerateOrder(strProductList))
            {
                return false;
            }
            addStockToLocal(CenterContral.oStockOutDTO);
            
            return true;
        }
        internal static void addStockToLocal(StockOutDTO oStockOutDTO)
        {
            CommUiltl.Log("addStockToLocal CenterContral.oStockOutDTO.Base.dbGenerateFlag:" + oStockOutDTO.Base.dbGenerateFlag);
            CommUiltl.Log("addStockToLocal Main.oSaveSotckOut.listStock.Count:" + CenterContral.oLocalSaveStock.listStock.Count);
            for (int i = 0; i < CenterContral.oLocalSaveStock.listStock.Count; ++i)
            {
                if (oStockOutDTO.Base.serialNumber == CenterContral.oLocalSaveStock.listStock[i].Base.serialNumber)
                {
                    CommUiltl.Log("addStockToLocal found");
                    CenterContral.oLocalSaveStock.listStock[i] = oStockOutDTO;//如果是已经存在挂单中的订单，那么就替换下
                    return;
                }
            }
            CenterContral.oLocalSaveStock.listStock.Add(oStockOutDTO);
            CenterContral.Window_ProductList.SetLocalSaveDataNumber();
        }
        public static int CurrentStockIndex = -1;



        internal static void GetSaveOrderToCurrentMsg()
        {
            if (CenterContral.oLocalSaveStock.listStock.Count == 0)
            {
                return;
            }
            ++CurrentStockIndex;
            CurrentStockIndex = CurrentStockIndex % CenterContral.oLocalSaveStock.listStock.Count;
            CenterContral.oStockOutDTO = CenterContral.oLocalSaveStock.listStock[CurrentStockIndex];
        }

        internal static void SetStockDetailByHttpRespone(StockOutDTO http, ref StockOutDTO Db)
        {
            if (oStockOutDToRespond.data.details.Count != CenterContral.oStockOutDTO.details.Count)
            {
                //说明是有问题的
                CommUiltl.Log("oRespond.data.details.Count[" + oStockOutDToRespond.data.details.Count + "] != Main.oStockOutDTO.details.Count [" + CenterContral.oStockOutDTO.details.Count + "]");
                MessageBox.Show("下单异常，请联系后台同学检查下单返回[" + oStockOutDToRespond.data.details.Count + "] != Main.oStockOutDTO.details.Count [" + CenterContral.oStockOutDTO.details.Count + "]");
            }
            else
            {
                for (int i = 0; i < oStockOutDToRespond.data.details.Count; ++i)
                {
                    CenterContral.oStockOutDTO.details[i].id = oStockOutDToRespond.data.details[i].id;
                }
            }
        }
        internal static bool PayOrder(long recieveFee)
        {
            oPayWay.payAmount = recieveFee;
            if (!Dao.GeneratePay(CenterContral.oPayWay))
            {
                return false;
            }
            //修改环境变量，表示这笔单支付成功
            CenterContral.oStockOutDTO.addPayWay(CenterContral.oPayWay);
            CommUiltl.Log("PayOrderByCash end:" + recieveFee);
            MessageBox.Show("支付" + CommUiltl.CoverMoneyUnionToStrYuan(recieveFee) + "元现金成功");
            return true;
        }
        internal static bool PayOrderByMember(long recieveFee)
        {
            oPayWay.payAmount = recieveFee;

            WalletHistory oRecharge = new WalletHistory();
            oRecharge.memberId = CenterContral.oMember.memberId;
            oRecharge.changeValue = recieveFee;
            oRecharge.generatePaySerialNamber();
            oRecharge.tradeTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
            if ( HttpUtility.MemberPay(oRecharge) != HttpUtility.CLOUD_SATE_HTTP_SUCESS)
            {
                MessageBox.Show("支付失败:" + HttpUtility.lastErrorMsg);
                return false;
            }
            //本地记录支付信息
            oPayWay.reqMemberZfJson = JsonConvert.SerializeObject(oRecharge);
            if (!Dao.GeneratePay(oPayWay))
            {
                return false;
            }
            CenterContral.oStockOutDTO.addPayWay(CenterContral.oPayWay);

            //重新拉会员信息
            CenterContral.GetMemberByMemberAccount(CenterContral.oMember.memberAccount);

            return true;
        }
        //充值
        internal static bool RechargeMoneyByMember(long recieveFee)
        {
            //充值金
            WalletHistory oRecharge = new WalletHistory();
            oRecharge.memberId = CenterContral.oMember.memberId;
            oRecharge.changeValue = recieveFee;
            oRecharge.generateRechargeSerialNamber();
            oRecharge.tradeTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
            //请求后台充值
            oRecharge.cloudState = HttpUtility.memberRecharge(oRecharge);
            if (HttpUtility.CLOUD_SATE_HTTP_SUCESS != oRecharge.cloudState)
            {
                MessageBox.Show("充值失败" + HttpUtility.lastErrorMsg );
                return false;
            }
            oRecharge.reqRechargeJson = JsonConvert.SerializeObject(oRecharge);
            long beforeMberBalance = CenterContral.oMember.balance ;
            //重新拉会员信息
            CenterContral.GetMemberByMemberAccount(CenterContral.oMember.memberAccount);
            long afterMemberAccount = CenterContral.oMember.balance ;
            //记录流水
            Dao.memberRecharge(oRecharge, beforeMberBalance, afterMemberAccount, recieveFee, CenterContral.oMember);
           
            return true;
        }

        //************************会员信息***********************
        internal static bool GetMemberByMemberAccount(string strMemberAccount)
        {
            MemberHttpRespone oMember = new MemberHttpRespone();
            int iMemberRet = HttpUtility.GetMemberByMemberAccount(strMemberAccount, ref oMember);
            if (iMemberRet == HttpUtility.CLOUD_SATE_HTTP_SUCESS)
            {
                CenterContral.oMember = oMember.data.list[0];
                return true;
            }
            if (iMemberRet == HttpUtility.CLOUD_SATE_HTTP_FAILD)
            {
                MessageBox.Show(HttpUtility.lastErrorMsg);
                return false;
            }
            MessageBox.Show("业务错误：" + HttpUtility.lastErrorMsg);
            return false;
        }
        //************************支付类型***********************
       
        internal static void _InitDbLocalMsg()
        {
            int iCount = 0;
            if (!Dao.GetLocalMsgDefaultCount(out iCount))
            {
                MessageBox.Show("初始化默认数据失败");
            }

            if (0 == iCount)
            {
                MessageBox.Show("第一次注册，请记得登陆完成后进行系统设置");
                Dao.InsertLocalMsgDefault();
            }
        }
        internal static bool _GetPayTypeList()
        {
            string json = "";
            //if (!HttpUtility.GetPayType(ref CenterContral.oPayTypeList))
            //{
                //取网络失败，那么就取数据库里面的
                if (!Dao.GetPayTypeList(ref json))
                {
                    MessageBox.Show("获取支付类型失败：" + HttpUtility.lastErrorMsg);
                    return false;
                }
               
                CenterContral.oPayTypeList = JsonConvert.DeserializeObject<PayTypeData>(json);
                return true;
            //}
            //json = JsonConvert.SerializeObject(CenterContral.oPayTypeList);
            ////取网络成功，则更新本地数据库
            //if (!Dao.SetPayType(ref json))
            //{
            //    return false;
            //}

            //return false;
        }
        internal static bool _GetPayTypeListByDao()
        {
            string json = "";
            //取网络成功，则更新本地数据库
            if (!Dao.GetPayTypeList(ref json))
            {
                MessageBox.Show("获取支付类型失败：" + HttpUtility.lastErrorMsg);
                return false;
            }
            //
            CenterContral.oPayTypeList = JsonConvert.DeserializeObject<PayTypeData>(json);
            return false;
        }

        //post 机id
        internal static void GetPostIdFromDb()
        {
            string strMac = CommUiltl.GetMacInfo();
            if (!Dao.GetPostId(ref CenterContral.iPostId))
            {
                CommUiltl.Log("Dao.GetPostId err");
                iPostId = -1;
                return;
            }
        }
        internal static void SetPostIdFromDb()
        {
            if (!Dao.SetPostId( CenterContral.iPostId))
            {
                CommUiltl.Log("Dao.SetPostId err");
                return;
            }
        }



        internal static void UpdateStoreWhouseDefault(string strStoreWhouseDefult)
        {
            Dao.UpdateStoreWhouseDefault(strStoreWhouseDefult);
        }

    }

}
