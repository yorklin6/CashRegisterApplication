﻿using CashRegisterApplication.comm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CashRegisterApplication.window.Member
{
    public partial class RechargeMoneyForMember : Form
    {
        public RechargeMoneyForMember()
        {
            InitializeComponent();
        }

        internal void ShowWithMemberInfo()
        {
            this.textBox_memberAccount.Text = CenterContral.oMember.memberAccount;
            this.textBox_name.Text = CenterContral.oMember.name;
            this.textBox_memberBalance.Text = CommUiltl.CoverMoneyUnionToStrYuan((CenterContral.oMember.memberBalance));
            this.textBox_phone.Text = CenterContral.oMember.phone;

            this.textBox_ReceiveFee.Text = "100";//默认100元

            this.textBox_ReceiveFee.Focus();
            this.textBox_ReceiveFee.SelectionStart = 0;
            this.textBox_ReceiveFee.SelectionLength = this.textBox_ReceiveFee.Text.Length;
            this.Show();
        }



        private void RechargeMoneyForMember_Load(object sender, EventArgs e)
        {
            ShowRechargeMoneyWindow();
        }
        private void RechargeMoneyForMember_Shown(object sender, EventArgs e)
        {
            ShowRechargeMoneyWindow();
        }
        
        public void ShowByProductListWindow()
        {
            ShowRechargeMoneyWindow();
        }

        public void ShowRechargeMoneyWindow()
        {
            this.Show();
            if (CenterContral.oMember.memberAccount == null || CenterContral.oMember.memberAccount == "")
            {
                CenterContral.Show_MemberInfoWindow_By_RechargeMoeneyByMember();
                return;
            }

            this.textBox_ReceiveFee.Focus();
        }

        protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, System.Windows.Forms.Keys keyData)
        {
            CommUiltl.Log("Keys:" + keyData);
            switch (keyData)
            {
                case System.Windows.Forms.Keys.Enter:
                    {
                        enterEvent();
                        break;
                    }
                case System.Windows.Forms.Keys.Escape:
                    {
                        this.Hide();
                        CenterContral.Window_ProductList.Show();
                        break;
                    }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
         protected void enterEvent()
        {
            buttonConfirm_Click(null, null);
        }
         protected void buttonConfirm_Click(object sender, EventArgs e)
        {
            long recieveFee = 0;
            if (!CommUiltl.ConverStrYuanToUnion(this.textBox_ReceiveFee.Text, out recieveFee))
            {
                MessageBox.Show("收款错误:" + this.textBox_ReceiveFee.Text);
                return;
            }
            string showTips = "确认充值：" + this.textBox_ReceiveFee.Text + " 元";

            var confirmPayApartResult = MessageBox.Show(showTips,
                                  "充值确认",
                                  MessageBoxButtons.YesNo);

            if (confirmPayApartResult != DialogResult.Yes)
            {
                //确认支付
                CommUiltl.Log("DialogResult.No recieveFee:" + recieveFee);
                _SelectRecieve();
                return;
            }
            //下单支付
            CommUiltl.Log("DialogResult.Yes recieveFee:" + recieveFee);
            string strBeforeRecharge = CommUiltl.CoverMoneyUnionToStrYuan((CenterContral.oMember.memberBalance));
            if (!CenterContral.RechargeMoneyByMember(recieveFee))
            {
                 _SelectRecieve();
                return;
            }


            MessageBox.Show("充值成功!\n\n充值前:"+ strBeforeRecharge 
                +"\n充值:"+ this.textBox_ReceiveFee.Text
                + "\n充值后:"+ CommUiltl.CoverMoneyUnionToStrYuan((CenterContral.oMember.memberBalance)),"充值结果");
           CenterContral.ControlWindowsAfterRecharge();
            this.Hide();
           
          
        }
        private void _SelectRecieve()
        {
            this.textBox_ReceiveFee.Focus();
            this.textBox_ReceiveFee.SelectionStart = 0;
            this.textBox_ReceiveFee.SelectionLength = this.textBox_ReceiveFee.Text.Length;
        }

    }
}
