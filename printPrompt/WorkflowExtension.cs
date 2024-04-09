using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Windows.Forms;
using Common;
using DAL;
using LSEXT;
using LSSERVICEPROVIDERLib;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace printPrompt
{

    [ComVisible(true)]
    [ProgId("printPrompt.printPromptCls")]
    public class printPrompt : IWorkflowExtension
    {



        private const string Type = "4";//TODO : מה הסוג האמיתי  
        INautilusServiceProvider sp;
        private int _port = 9100;
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern SafeFileHandle CreateFile(string lpFileName, FileAccess dwDesiredAccess,
        uint dwShareMode, IntPtr lpSecurityAttributes, FileMode dwCreationDisposition,
        uint dwFlagsAndAttributes, IntPtr hTemplateFile);
        private IDataLayer dal;
        public void Execute(ref LSExtensionParameters Parameters)
        {
            try
            {
                #region param
                var tableName = Parameters["TABLE_NAME"];
                sp = Parameters["SERVICE_PROVIDER"];
                var Id = Parameters["WORKFLOW_NODE_ID"];
                var rs = Parameters["RECORDS"];
                var ResultName = rs.Fields["NAME"].Value;
                var workstationId = Parameters["WORKSTATION_ID"];

                ////////////יוצר קונקשן//////////////////////////
                var ntlCon = Utils.GetNtlsCon(sp);
                Utils.CreateConstring(ntlCon);
                /////////////////////////////
                dal = new DataLayer();
                dal.Connect();
                #endregion
              

                Workstation ws = dal.getWorkStaitionById(workstationId);
                ReportStation reportStation = dal.getReportStationByWorksAndType(ws.NAME, Type);
                string printerName = "";
                //                string ip = GetIp(printerName);
                string goodIp = ""; //removeBadChar(ip);
                if (reportStation.Destination != null)
                {
                    //                    printerName = reportStation.DESTINATION.INFO_TEXT1;
                    goodIp = reportStation.Destination.ManualIP;
                }
                if (reportStation.Destination != null && reportStation.Destination.RawTcpipPort != null)
                {
                    _port = (int)reportStation.Destination.RawTcpipPort;
                }

                DialogForm dialogForm = new DialogForm();
                dialogForm.ShowDialog();
                int promptValue = dialogForm.NumOfCopies;
                for (int i = 0; i < promptValue; i++)
                {
                    Print(ResultName, ResultName, "", "", goodIp);
                }
            }
            catch (Exception ex)
            {
                    
               Common.Logger.WriteLogFile(ex);
            }
           
        }
        private string removeBadChar(string ip)
        {
            string ret = "";
            foreach (var c in ip)
            {
                int ascii = (int)c;
                if ((ascii >= 48 && ascii <= 57) || ascii == 44 || ascii == 46)
                    ret += c;
            }
            return ret;
        }
        public string GetIp(string printerName)
        {
            string query = string.Format("SELECT * from Win32_Printer WHERE Name LIKE '%{0}'", printerName);
            string ret = "";
            var searcher = new ManagementObjectSearcher(query);
            var coll = searcher.Get();
            foreach (ManagementObject printer in coll)
            {
                foreach (PropertyData property in printer.Properties)
                {
                    if (property.Name == "PortName")
                    {
                        ret = property.Value.ToString();
                    }
                }
            }
            return ret;
        }
        private void Print(string name, string ID, string testcode, string mihol, string ip)
        {
            string ipAddress = ip;
            // ZPL Command(s)
            string ntxt = name;
            string tctxt = testcode;
            string mtxt = mihol;
            string itxt = ID;
            string ZPLString =
                 "^XA" +
                 "^LH10,10" +
                 "^FO60,30" +
                 "^A@N20,20" +
                string.Format("^FD{0}^FS", ntxt) +
                 "^FO10,80" +
                 "^A@N20,20" +
                string.Format("^FD{0}^FS", mtxt) +
                "^FO100,80" +
                 "^A@N20,20" +
                string.Format("^FD{0}^FS", tctxt) +
                "^FO245,20" + "^BQN,4,3" +
                 //string.Format("^FD   {0}^FS", itxt) +
                  string.Format("^FDLA,{0}^FS", itxt) + //ברקוד
                "^XZ";
            try
            {
                // Open connection
                System.Net.Sockets.TcpClient client = new System.Net.Sockets.TcpClient();
                client.Connect(ipAddress, _port);
                // Write ZPL String to connection
                StreamWriter writer = new StreamWriter(client.GetStream());
                writer.Write(ZPLString);
                writer.Flush();
                // Close Connection
                writer.Close();
                client.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);   
            }
        }
    }
}
