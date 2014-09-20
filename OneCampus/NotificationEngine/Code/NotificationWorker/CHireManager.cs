
using Renci.SshNet;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using C2C.BusinessEntities.NotificationEntities;

namespace Octane.NotificationWorker
{
    public static class CHireManager
    {
        public static string SftpServer { private get; set; }
        public static string SftpUserName { private get; set; }
        public static string SftpPassword { private get; set; }

        public static MemoryStream DownloadCHireFile(string fileName)
        {
            MemoryStream chireStream = new MemoryStream();
            using (var sftp = new SftpClient(SftpServer, SftpUserName, SftpPassword))
            {
                sftp.Connect();
                sftp.DownloadFile(fileName, chireStream);
                sftp.Disconnect();
            }
            return chireStream;
        }

        public static List<CHireData> ReadCHireFile(Stream chireStream)
        {
            List<CHireData> CHireDataList = new List<CHireData>();
            chireStream.Seek(0, SeekOrigin.Begin);

            StreamReader chireReader = new StreamReader(chireStream);
            string chireData = chireReader.ReadLine();

            while (!string.IsNullOrEmpty(chireData))
            {
                //Read again to skip the headers of the CHire Csv file.
                chireData = chireReader.ReadLine();

                if (!string.IsNullOrEmpty(chireData))
                {
                    string[] data;
                    data = chireData.Split(',');

                    CHireData chire = new CHireData
                    {
                        CandidateId = data[0],
                        FirstName = data[1],
                        MiddleInitial = data[2],
                        LastName = data[3],
                        EmailAddress = data[4],
                        DateOfBirth = data[5],
                        TenthYearOfPassing = data[7],
                        TwelvethYearOfPassing = data[9],
                        EducationLevel = data[11],
                        College = data[12],
                        Program = data[14],
                        GraduationDate = data[17],
                        OtherInstitution = data[18],
                        OtherProgram = data[19]
                    };

                    CHireDataList.Add(chire);
                }
            }
            return CHireDataList;
        }

        public static string WriteSummaryMessage(List<ProcessResult> processResultList)
        {
            IEnumerable<ProcessResult> failureList = new List<ProcessResult>();
            int successCount = 0;
            int failureCount = 0;

            successCount = GetSuccessCount(processResultList);
            failureList = from results in processResultList
                          where results.Status.ToUpper() != "SUCCESS"
                          select results;
           failureCount =  failureList.Count();           

            StringBuilder summaryMessage = new StringBuilder();
            summaryMessage.Append("<p>CHire Import Details</p>\n");
            summaryMessage.Append("<p>" + successCount + ' ' + "Success</p>\n");
            summaryMessage.Append("<p>" + failureCount + ' ' + "failure</p>\n");
            summaryMessage.Append("<TABLE>\n");

            foreach (var item in failureList)
            {
                summaryMessage.Append("<TR>\n");
                summaryMessage.Append("<TD>" + item.CandidateId + "</TD>\n<TD>" + item.Message + "</TD>\n");
                summaryMessage.Append("</TR>\n");
            }

            summaryMessage.Append("</TABLE>");
            return summaryMessage.ToString();
        }
      
        public static List<ProcessResult> GetFailureList(List<ProcessResult> processResultList)
        {
            int successCount = 0;
            int failureCount = 0;
            List<ProcessResult> failureList = new List<ProcessResult>();
            ProcessResult failure;
            foreach (var item in processResultList)
            {
                if (item.Status == "Success")
                {
                    successCount++;
                }
                else
                {
                    failureCount++;
                    failure = new ProcessResult();
                    failure.CandidateId = item.CandidateId;
                    failure.Message = item.Message;
                    failureList.Add(failure);
                }
            }
            return failureList;

        }
        public static int GetSuccessCount(List<ProcessResult> processResultList)
        {

          int  successCount = (from results in processResultList
                            where results.Status.ToUpper().Equals("SUCCESS")
                            select results).Count();
          return successCount;

        }
    }
}

