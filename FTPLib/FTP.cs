using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Xml;
using System.Xml.Linq;
using System.Data.SqlClient;
using System.Data;

namespace FTPLib
{
    public class FTP
    {
        private string ftpServerIP = "";

        public string FtpServerIP
        {
            get { return ftpServerIP; }
            set { ftpServerIP = value; }
        }

        private string ftpUserID = "";

        public string FtpUserID
        {
            get { return ftpUserID; }
            set { ftpUserID = value; }
        }

        private string ftpPassword = "";

        public string FtpPassword
        {
            get { return ftpPassword; }
            set { ftpPassword = value; }
        }
        //private static string ftpIP = "116.62.116.36:9024/";
        //private static string ftpIP_Order = ftpIP + "order";
        //private static string ftpRootURL = "ftp://" + "116.62.116.36:9024/";
        //private static string ftpRootURL_Order = ftpRootURL + "order/";
        //private static string ftpRootURL_Confirm = ftpRootURL + "confirm/";
        //private static string ftpRootURL_Shipping = ftpRootURL + "shipping/";
        //private static string ftpUserID = "hko";
        //private static string ftpUserPW = "Qwer1234";
        //D:\Peter\SourceCode\Jinsftp\Files
        //D:\Program\FTP\Files
        //private static string saveFilePath = System.Environment.CurrentDirectory + @"\xmlfiles";
        //private static int i = 0;

        //public static void GetFile()
        //{
        //    string[] fileList = FTPGetFileList(ftpIP_Order, ftpUserID, ftpUserPW);
        //    while (null != fileList)
        //    {
        //        FTPDownloadFile(ftpIP_Order, ftpUserID, ftpUserPW, saveFilePath, fileList[0], fileList[0]);
        //        FTPFileDelete(ftpRootURL_Order, fileList[0], ftpUserID, ftpUserPW);
        //        fileList = FTPGetFileList(ftpIP_Order, ftpUserID, ftpUserPW);
        //    }
        //}

        #region FTP获取文件列表

        /// <summary>
        /// FTP获取文件列表
        /// </summary>
        /// <param name="ftpServerIP">116.62.116.36:9024</param>
        /// <param name="ftpServerFolder">order</param>
        /// <param name="ftpUserID">hko</param>
        /// <param name="ftpPassword">123456</param>
        /// <returns></returns>
        public string[] GetFileList(string ftpServerIP, string ftpServerFolder, string ftpUserID, string ftpPassword)
        {
            //响应结果
            StringBuilder result = new StringBuilder();

            //FTP请求
            FtpWebRequest ftpRequest = null;

            //FTP响应
            WebResponse ftpResponse = null;

            //FTP响应流
            StreamReader ftpResponsStream = null;

            string ftpUrl = "ftp://" + ftpServerIP + "/" + ftpServerFolder + "/";
            try
            {
                //生成FTP请求
                ftpRequest = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpUrl));

                //设置文件传输类型
                ftpRequest.UseBinary = true;

                //FTP登录
                ftpRequest.Credentials = new NetworkCredential(ftpUserID, ftpPassword);

                //设置FTP方法
                ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory;

                //生成FTP响应
                ftpResponse = ftpRequest.GetResponse();

                //FTP响应流
                ftpResponsStream = new StreamReader(ftpResponse.GetResponseStream());

                string line = ftpResponsStream.ReadLine();

                while (line != null)
                {
                    result.Append(line);
                    result.Append("\n");
                    line = ftpResponsStream.ReadLine();
                }

                //去掉结果列表中最后一个换行
                result.Remove(result.ToString().LastIndexOf('\n'), 1);

                //返回结果
                return result.ToString().Split('\n');
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return (null);
            }
            finally
            {
                if (ftpResponsStream != null)
                {
                    ftpResponsStream.Close();
                }

                if (ftpResponse != null)
                {
                    ftpResponse.Close();
                }
            }
        }

        #endregion

        #region FTP下载文件
        //C:\Peter\SourceCode\TEST\Temp
        /// <summary>
        /// FTP下载文件
        /// </summary>
        /// <param name="ftpServerIP">FTP服务器IP</param>
        /// <param name="ftpUserID">FTP登录帐号</param>
        /// <param name="ftpPassword">FTP登录密码</param>
        /// <param name="downloadFileName">下载文件名</param>
        /// <param name="saveFilePath">保存文件路径eg:System.Environment.CurrentDirectory + @"\xmlfiles"</param>
        /// <param name="saveFileName">保存文件名</param>

        public void DownloadFile(string ftpServerIP, string ftpServerFolder, string ftpUserID, string ftpPassword,
            string downloadFileName, string saveFilePath, string saveFileName)
        {
            //定义FTP请求对象
            FtpWebRequest ftpRequest = null;
            //定义FTP响应对象
            FtpWebResponse ftpResponse = null;

            //存储流
            FileStream saveStream = null;
            FileStream saveStream1 = null;
            //FTP数据流
            Stream ftpStream = null;
            string ftpUrl = "ftp://" + ftpServerIP + "/" + ftpServerFolder + "/";
            try
            {
                //生成下载文件
                saveStream = new FileStream(saveFilePath + "\\" + saveFileName, FileMode.Create);
                saveStream1 = new FileStream(saveFilePath + "1" + "\\" + saveFileName, FileMode.Create);
                //生成FTP请求对象
                ftpRequest = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + ftpUrl + downloadFileName));

                //设置下载文件方法
                ftpRequest.Method = WebRequestMethods.Ftp.DownloadFile;

                //设置文件传输类型
                ftpRequest.UseBinary = true;

                //设置登录FTP帐号和密码
                ftpRequest.Credentials = new NetworkCredential(ftpUserID, ftpPassword);

                //生成FTP响应对象
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();

                //获取FTP响应流对象
                ftpStream = ftpResponse.GetResponseStream();

                //响应数据长度
                long cl = ftpResponse.ContentLength;

                int bufferSize = 2048;

                int readCount;

                byte[] buffer = new byte[bufferSize];

                //接收FTP文件流
                readCount = ftpStream.Read(buffer, 0, bufferSize);

                while (readCount > 0)
                {
                    saveStream.Write(buffer, 0, readCount);
                    saveStream1.Write(buffer, 0, readCount);
                    readCount = ftpStream.Read(buffer, 0, bufferSize);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (ftpStream != null)
                {
                    ftpStream.Close();
                }

                if (saveStream != null)
                {
                    saveStream.Close();
                }

                if (saveStream1 != null)
                {
                    saveStream1.Close();
                }

                if (ftpResponse != null)
                {
                    ftpResponse.Close();
                }
            }
        }

        #endregion

        #region FTP删除文件
        public bool DeleteFile(string ftpServerIP, string ftpServerFolder, string ftpFileName, string ftpUser, string ftpPassword)
        {
            bool success = false;
            FtpWebRequest ftpWebRequest = null;
            FtpWebResponse ftpWebResponse = null;
            Stream ftpResponseStream = null;
            StreamReader streamReader = null;
            try
            {
                string uri = "ftp://" + ftpServerIP + "/" + ftpServerFolder + "/" + ftpFileName;
                ftpWebRequest = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));
                ftpWebRequest.Credentials = new NetworkCredential(ftpUser, ftpPassword);
                ftpWebRequest.KeepAlive = false;
                ftpWebRequest.Method = WebRequestMethods.Ftp.DeleteFile;
                ftpWebResponse = (FtpWebResponse)ftpWebRequest.GetResponse();
                long size = ftpWebResponse.ContentLength;
                ftpResponseStream = ftpWebResponse.GetResponseStream();
                streamReader = new StreamReader(ftpResponseStream);
                string result = String.Empty;
                result = streamReader.ReadToEnd();

                success = true;
            }
            catch (Exception)
            {
                success = false;
                throw;
            }
            finally
            {
                if (streamReader != null)
                {
                    streamReader.Close();
                }
                if (ftpResponseStream != null)
                {
                    ftpResponseStream.Close();
                }
                if (ftpWebResponse != null)
                {
                    ftpWebResponse.Close();
                }
            }
            return success;
        }


        #endregion


    }
}
