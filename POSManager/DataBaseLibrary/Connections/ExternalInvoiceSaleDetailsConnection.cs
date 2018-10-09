﻿using BusinessLibrary.Models;
using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBaseLibrary.Connections
{
    public static class ExternalInvoiceSaleDetailsConnection
    {
        private static string LoadConnectionString(string id = "default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }

        public static int SelectLastInvoiceNumber()
        {
            try
            {
                using (IDbConnection cnn = new MySqlConnection(LoadConnectionString()))
                {
                    var output = cnn.Query<int>("SELECT COUNT(numberInvoice) FROM detailexternalinvoicesell ORDER BY numberInvoice DESC LIMIT 1");
                    return output.Single();
                }
            }
            catch (Exception ex)
            {

                return 0;
            }
        }

        public static List<ExternalInvoiceSaleDetailsModel> SelectInvoices()
        {
            try
            {
                using (IDbConnection cnn = new MySqlConnection(LoadConnectionString()))
                {
                    var output = cnn.Query<ExternalInvoiceSaleDetailsModel>("SELECT * FROM detailexternalinvoicesell");
                    return output.ToList();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static bool InsertExternalInvoiceSaleDetails(ExternalInvoiceSaleDetailsModel externalInvoiceSaleDetailsModel, List<int> productsIds)
        {
            try
            {
                externalInvoiceSaleDetailsModel.ActualDate = DateTime.Now;
                if (externalInvoiceSaleDetailsModel.IdClient != 0)
                {
                    using (IDbConnection cnn = new MySqlConnection(LoadConnectionString()))
                    {
                        cnn.Execute("INSERT detailexternalinvoicesell (ActualDate, IdClient, idEmployee, idBusiness, currencyType, iviAmount, cashDeposit, cardDeposit, totalDiscount, subTotal, TOtal) VALUES (@ActualDate, @IdClient, @idEmployee, @idBusiness, @currencyType, @iviAmount, @cashDeposit, @cardDeposit, @totalDiscount, @subTotal, @Total)", externalInvoiceSaleDetailsModel);
                        InsertProductsXInvoice(productsIds);
                        return true;
                    }
                } else
                {
                    using (IDbConnection cnn = new MySqlConnection(LoadConnectionString()))
                    {
                        cnn.Execute("INSERT detailexternalinvoicesell (ActualDate, idEmployee, idBusiness, currencyType, iviAmount, cashDeposit, cardDeposit, totalDiscount, subTotal, TOtal) VALUES (@ActualDate, @idEmployee, @idBusiness, @currencyType, @iviAmount, @cashDeposit, @cardDeposit, @totalDiscount, @subTotal, @Total)", externalInvoiceSaleDetailsModel);
                        InsertProductsXInvoice(productsIds);
                        return true;
                    }
                }
                
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public static List<ExternalInvoiceSaleDetailsModel> SelectInvoicesByDate(DateTime start, DateTime end)
        {
            try
            {
                TestDateModel testDateModel = new TestDateModel()
                {
                    end = end,
                    start = start
                };
                using (IDbConnection cnn = new MySqlConnection(LoadConnectionString()))
                {
                    var output = cnn.Query<ExternalInvoiceSaleDetailsModel>("SELECT * FROM detailexternalinvoicesell WHERE ActualDate BETWEEN " +
                        "@start AND @end", testDateModel);
                    return output.ToList();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<ExternalInvoiceSaleDetailsModel> SelectInvoicesByMonth(DateTime date)
        {
            try
            {
                using (IDbConnection cnn = new MySqlConnection(LoadConnectionString()))
                {
                    var output = cnn.Query<ExternalInvoiceSaleDetailsModel>("SELECT * FROM detailexternalinvoicesell WHERE ActualDate = @date", new { date });
                    return output.ToList();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<ExternalInvoiceSaleDetailsModel> SelectInvoicesByDay(DateTime date)
        {
            try
            {
                using (IDbConnection cnn = new MySqlConnection(LoadConnectionString()))
                {
                    var output = cnn.Query<ExternalInvoiceSaleDetailsModel>("SELECT * FROM detailexternalinvoicesell WHERE ActualDate = @date", new { date });
                    return output.ToList();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private static void InsertProductsXInvoice(List<int> productsids)
        {
            ProductsXInvoiceModel productsXInvoiceModel;
            foreach (int item in productsids)
            {
                productsXInvoiceModel = new ProductsXInvoiceModel()
                {
                    Product_idProduct = item,
                    IdDetailExternalInvoiceSell = SelectLastInvoiceNumber()
                };
            
                using (IDbConnection cnn = new MySqlConnection(LoadConnectionString()))
                {
                    cnn.Execute("INSERT externalinvoicesell (Product_idProduct, idDetailExternalInvoiceSell) VALUES (@Product_idProduct, @idDetailExternalInvoiceSell)", productsXInvoiceModel);
                }
            }
        }
    }
}