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

            SellerInfo bestWatermelonSeller = GetBiggestSellerInfoFromProduct("Pastèques");
            Console.WriteLine($"C'est {bestWatermelonSeller.Seller} qui a le plus de pastèques (stand {bestWatermelonSeller.StandId}, {bestWatermelonSeller.Quantity} pièces)");

            Console.ReadLine();
        }

        static int CountProductSellers(string productName)
        {
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                IWorkbook workbook = new XSSFWorkbook(fileStream);
                ISheet sheet = workbook.GetSheet("Produits");

                IEnumerable<ProductInfo> products = Enumerable.Range(0, sheet.LastRowNum + 1)
                    .Select(i => sheet.GetRow(i))
                    .Where(row => row != null)
                    .Select(row => new ProductInfo
                    {
                        StandId = row.GetCell(0)?.NumericCellValue is double sid ? (int)sid : 0,
                        Seller = row.GetCell(1)?.ToString() ?? string.Empty,
                        Name = row.GetCell(2)?.ToString() ?? string.Empty,
                        Quantity = row.GetCell(3)?.NumericCellValue is double q ? (int)q : 0
                    });

                return products.Count(p => p.Name == productName);
            }
        }

        static SellerInfo GetBiggestSellerInfoFromProduct(string productName)
        {
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                IWorkbook workbook = new XSSFWorkbook(fileStream);
                ISheet sheet = workbook.GetSheet("Produits");

                IEnumerable<ProductInfo> products = Enumerable.Range(0, sheet.LastRowNum + 1)
                    .Select(i => sheet.GetRow(i))
                    .Where(row => row != null)
                    .Select(row => new ProductInfo
                    {
                        StandId = row.GetCell(0)?.NumericCellValue is double sid ? (int)sid : 0,
                        Seller = row.GetCell(1)?.ToString() ?? string.Empty,
                        Name = row.GetCell(2)?.ToString() ?? string.Empty,
                        Quantity = row.GetCell(3)?.NumericCellValue is double q ? (int)q : 0
                    });

                var best = products
                    .Where(p => p.Name == productName)
                    .OrderByDescending(p => p.Quantity)
                    .FirstOrDefault();

                if (string.IsNullOrEmpty(best.Seller))
                {
                    return new SellerInfo { Seller = "", StandId = 0, Quantity = 0 };
                }

                return new SellerInfo
                {
                    Seller = best.Seller,
                    StandId = best.StandId,
                    Quantity = best.Quantity
                };
            }
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
