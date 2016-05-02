using System;
using System.IO;

namespace DBUtility
{
	/// <summary>
	/// 错误处理函数，用于记录错误日志
	/// </summary>
	public class SystemError 
	{
		//记录错误日志位置
		private static string m_fileName = "c:\\Systemlog.txt";

		public static String  FileName
		{
			get
			{
				return(m_fileName);
			}
			set
			{
				if(value != null || value != "")
				{
					m_fileName = value;
				}
			}
		}

		/// <summary>
		/// 记录日志至文本文件
		/// </summary>
		/// <param name="message">记录的内容</param>
		public static void CreateErrorLog(string message) 
		{
//			if(File.Exists(m_fileName))
//			{
//				///如果日志文件已经存在，则直接写入日志文件
//				StreamWriter sr = File.AppendText(FileName);
//				sr.WriteLine ("\n");
//				sr.WriteLine (DateTime.Now.ToString()+message);
//				sr.Close();
//			}
//			else
//			{
//				///创建日志文件
//				StreamWriter sr = File.CreateText(FileName);
//				sr.Close();
//			}	
		}
	}
}
