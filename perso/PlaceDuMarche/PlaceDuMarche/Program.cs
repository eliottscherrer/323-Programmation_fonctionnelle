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
            Console.WriteLine($"Il y a {CountProductSellers("Pêches")} vendeurs de pêches.");

            string[] bestWatermelonSellerInfos = GetBiggestSellerInfoFromProduct("Pastèques");
            Console.WriteLine($"C'est {bestWatermelonSellerInfos[0]} qui a le plus de pastèques (stand {bestWatermelonSellerInfos[1]}, {bestWatermelonSellerInfos[2]} pièces)");

            Console.ReadLine();
        }

        static int CountProductSellers(string productName)
        {
            int count = 0;

            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                IWorkbook workbook = new XSSFWorkbook(fileStream);

                ISheet sheet = workbook.GetSheet("Produits");

                for (int rowIndex = 0; rowIndex <= sheet.LastRowNum; rowIndex++)
                {
                    IRow row = sheet.GetRow(rowIndex);
                    if (row != null)
                    {
                        ICell cell = row.GetCell(2);
                        if (cell != null && cell.ToString() == productName)
                        {
                            count++;
                        }
                    }
                }
            }

            return count;
        }

        static string[] GetBiggestSellerInfoFromProduct(string productName)
        {
            int maxQuantity = 0;
            string bestSeller = "";
            int standId = 0;

            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                IWorkbook workbook = new XSSFWorkbook(fileStream);

                ISheet sheet = workbook.GetSheet("Produits");

                for (int rowIndex = 0; rowIndex <= sheet.LastRowNum; rowIndex++)
                {
                    IRow row = sheet.GetRow(rowIndex);
                    if (row != null)
                    {
                        ICell name = row.GetCell(2);
                        ICell quantity = row.GetCell(3);
                        if (name != null && name.ToString() == productName)
                        {
                            if (quantity != null && (int)quantity.NumericCellValue > maxQuantity)
                            {
                                bestSeller = row.GetCell(1).StringCellValue;
                                standId = (int)row.GetCell(0).NumericCellValue;
                                maxQuantity = (int)quantity.NumericCellValue;
                            }
                        }
                    }
                }
            }

            return new string[]
            {
                bestSeller,
                standId.ToString(),
                maxQuantity.ToString(),
            };
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
