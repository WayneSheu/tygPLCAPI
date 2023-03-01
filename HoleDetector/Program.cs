using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TYG.HoleDetect;

namespace HoleDetector
{
    class Program
    {
        static void Main(string[] args)
        {
            OnStart();
        }

        static void OnStart()
        {
            //if (m_svcHost != null) m_svcHost.Close();

            //string strAdrHTTP = "http://localhost:9001/CalcService";
            //string strAdrTCP = "net.tcp://localhost:9002/CalcService";

            //Uri[] adrbase = { new Uri(strAdrHTTP), new Uri(strAdrTCP) };
            //m_svcHost = new ServiceHost(typeof(WCFCalcLib.CalcService), adrbase);

            //ServiceMetadataBehavior mBehave = new ServiceMetadataBehavior();
            //m_svcHost.Description.Behaviors.Add(mBehave);

            //BasicHttpBinding httpb = new BasicHttpBinding();
            //m_svcHost.AddServiceEndpoint(typeof(WCFCalcLib.ICalcService), httpb, strAdrHTTP);
            //m_svcHost.AddServiceEndpoint(typeof(IMetadataExchange),
            //MetadataExchangeBindings.CreateMexHttpBinding(), "mex");

            //NetTcpBinding tcpb = new NetTcpBinding();
            //m_svcHost.AddServiceEndpoint(typeof(WCFCalcLib.ICalcService), tcpb, strAdrTCP);
            //m_svcHost.AddServiceEndpoint(typeof(IMetadataExchange),
            //MetadataExchangeBindings.CreateMexTcpBinding(), "mex");

            //m_svcHost.Open();

        }

    }
}
