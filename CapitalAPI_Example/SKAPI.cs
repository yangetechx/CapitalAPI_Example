using SKCOMLib;
using System;
using System.IO;
using System.Threading;
using static System.Net.Mime.MediaTypeNames;

namespace CapitalAPI_Example
{
    public class SKAPI
    {
        private SKCenterLib m_pSKCenter;
        private SKOrderLib m_pSKOrder;
        private SKReplyLib m_pSKReply;
        private SKQuoteLib m_pSKQuote;
        private SKOSQuoteLib m_pSKOSQuote;
        private SKOOQuoteLib m_pSKOOQuote;
        private bool IsConnection = false;
        public SKAPI(string ID, string PW)
        {
            int m_nCode;

            m_pSKCenter = new SKCenterLib();
            m_pSKOrder = new SKOrderLib();
            m_pSKReply = new SKReplyLib();
            m_pSKQuote = new SKQuoteLib();
            m_pSKOSQuote = new SKOSQuoteLib();
            m_pSKOOQuote = new SKOOQuoteLib();

            m_pSKReply.OnReplyMessage += new _ISKReplyLibEvents_OnReplyMessageEventHandler(this.OnAnnouncement);
            m_pSKQuote.OnNotifyQuoteLONG += new _ISKQuoteLibEvents_OnNotifyQuoteLONGEventHandler(m_SKQuoteLib_OnNotifyQuote);
            m_pSKQuote.OnConnection += new _ISKQuoteLibEvents_OnConnectionEventHandler(m_SKQuoteLib_OnConnection);
            m_nCode = m_pSKCenter.SKCenterLib_Login(ID, PW);

            m_nCode = m_pSKQuote.SKQuoteLib_EnterMonitorLONG();
            
            while(IsConnection == false)
            {
                Thread.Sleep(1);
            }
            

            string[] Stocks = new string[] { "TX00", "MTX00" };

            foreach (string s in Stocks)
            {
                SKSTOCKLONG pSKStockLONG = new SKSTOCKLONG();

                int nCode = m_pSKQuote.SKQuoteLib_GetStockByNoLONG(s.Trim(), ref pSKStockLONG);
            }

            short sPage = 0;

            m_nCode = m_pSKQuote.SKQuoteLib_RequestStocks(ref sPage, "TX00, MTX00");

        }

        private void m_SKQuoteLib_OnConnection(int nKind, int nCode)
        {
            if (nKind == 3003 || nKind == 3036)
            {
                IsConnection = true;
            }
           
        }

        private void m_SKQuoteLib_OnNotifyQuote(short sMarketNo, int nStockIdx)
        {
            SKSTOCKLONG pSKStockLONG = new SKSTOCKLONG();

            m_pSKQuote.SKQuoteLib_GetStockByIndexLONG(sMarketNo, nStockIdx, ref pSKStockLONG);
            string data = $"{DateTime.Now.ToString("HH:mm:ss.fff")},{pSKStockLONG.nBid},{pSKStockLONG.nBc},{pSKStockLONG.nAsk},{pSKStockLONG.nAc}";
            Console.WriteLine(data);
            File.WriteAllLines($"./{pSKStockLONG.bstrStockName}.txt",new string[] { data } );
        }

        private void OnAnnouncement(string strUserID, string bstrMessage, out short nConfirmCode)
        {
            Console.WriteLine(strUserID + "_" + bstrMessage);
            nConfirmCode = -1;
        }
    }
}