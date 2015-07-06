using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
//using Microsoft.Office.Interop.Excel;

namespace TestApplication
{
    public class ExcelInterop
    {
        private const string _fileName = "C:\\00063001_16_42_50.xls";

        public static void ExcalAsPdf()
        {
            try
            {
                //var excelApp = new Application();
                //var currentWorkBook = excelApp.Workbooks.Open(_fileName);
                //currentWorkBook.ExportAsFixedFormat(XlFixedFormatType.xlTypePDF, "C:\\text.pdf");
                //currentWorkBook.Save();
                //currentWorkBook.Close(false, _fileName, null);
                //Marshal.ReleaseComObject(currentWorkBook);
                //currentWorkBook = null;
                //Marshal.ReleaseComObject(excelApp);
                //excelApp = null;
            }
            catch (Exception)
            {
            }
        }
    }
}
