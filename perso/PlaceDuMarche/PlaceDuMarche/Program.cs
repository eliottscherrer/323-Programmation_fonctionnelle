using NPOI.HSSF.UserModel;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace PlaceDuMarche
{
    public class Program
    {
        const string filePath = @"C:\Users\pg13qnw\Documents\GitHub\323-Programmation_fonctionnelle\exos\marché\Place du marché.xlsx";

        static void Main()
        {
            PrintSheet("Produits");

            Console.ReadLine();
        }

        static void PrintSheet(string sheetName)
        {
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                IWorkbook workbook = new XSSFWorkbook(fileStream);

                ISheet sheet = workbook.GetSheet(sheetName);

                for (int rowIndex = 0; rowIndex <= sheet.LastRowNum; rowIndex++)
                {
                    IRow row = sheet.GetRow(rowIndex);
                    if (row != null)
                    {
                        for (int cellIndex = 0; cellIndex < row.LastCellNum; cellIndex++)
                        {
                            ICell cell = row.GetCell(cellIndex);
                            if (cell != null)
                            {
                                Console.Write(cell.ToString() + "\t");
                            }
                        }

                        Console.WriteLine();
                    }
                }

                Console.ReadLine();
            }
        }
    }
}
