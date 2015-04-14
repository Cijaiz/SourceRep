using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoScriptExecuter
{
    public static class Constant
    {
        public const string LocalxmlFileName = "ExecutionFiles\\ScriptDetails.xml";
        public const string AzurexmlFileName = "ExecutionFiles\\AzureScriptDetails.xml";
        public const string ConnectionStringxmlFileName = "ExecutionFiles\\connectionStringDetails.xml";

        public const string ConnectionStringxmlFileNameAttribute = "string";
        public const string ConnectionStringAttribute ="value";

        public const string readXMLstart = "Reading XML fine starts. ";
        public const string readXMLend = "Reading XML fine finished. ";
        public const string readXMLfile = " Reading XML fine starts. ";
        public const string rootPath = "Rootpath";
        public const string rootPathAttribute = "path";
        public const string script = "Script";
        public const string scriptAttributename = "name";
        public const string scriptAttributefiletype = "filetype";

        public const string startExecution = "---Start Application Execution---";
        public const string endExecution = "---End Application Execution---";

        public const string startDataLoading = "Data Loading Started. ";
        public const string endDataLoading = "Data Loading Finished.";

        public const string createSQLConn = "SQL connection created.";
        public const string SQLConnSucceed = "SQL connection created successfully.";
        public const string SQLConnOpen = "Open SQL connection. ";
        public const string SQLConnClose = "Close SQL connection. ";

        public const string createServerConn = "Server connection created.";
        public const string ServerConnSucceed = "Server connection created successfully.";
        public const string ServerConnOpen = "Connect Server Connection. ";
        public const string ServerConnClose = "Disconnect Server Connection. ";

        public const string startScriptExecution = "Start Script Execution.";
        public const string endScriptExecution = "Script Execution completed successfully.";

        public const string strRootPath = "Root Path is :";

        public const string strDBconfig = "DBConfig";
        public const string strFileType = "sql";

        public const string startFileExecution = " - Started Executing";
        public const string endFileExecution = " - Finished Execution";

        public const string fileNotFound = " - File not found." ;
        public const string fileNotFound2 = "Do you want to continue?";
        public const string fileNotFound3 =" Press 'Yes' to Continue or 'No' to Exit.";

        public const string msgBoxErrorTitle = "Error";

        public const string fileNotFoundMessageResultYes = " - File not found.Continue to Execute remaining scripts.";
        public const string fileNotFoundMessageResultNo = " - File not found.Execution Aborted.";

        public const string msgBoxScriptExecutionSuccess = "Execution completed successfully";
        public const string msgBoxSuccessTitle = "Success";

        public const string exceptionMessage1 = "Exception Occured - ";
        public const string exceptionMessage2 = "Exception is ";
        public const string exceptionMessage3 = "Execution Aborted.Please try again after re-creating the database. ";

        public const string msgBoxScriptExeFinalmessage = "Execution completed.Please press the<Exit> button to exit the application.";
        public const string msgBoxInfoTitle = "Info";

        public const string listboxTotalFilesExecuted = "Total files executed is ";

        public const string pathcontainsSeparator = "\\";
        public const char pathsplitSeparator = '\\';

        public const string filecontainsSeparator = ".";
        public const char filesplitSeparator = '.';

        public const string testconnectionSuccess= "Test Connection Successful";
        public const string testConnectionExceptionMessage1 = "Some Error Occured while connecting.";
        public const string testConnectionExceptionMessage2 = "Please Check the connection String.";
    }
}
