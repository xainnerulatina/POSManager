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
    public class OffsetDetailsConnection
    {

        private static string LoadConnectionString(string id = "default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }

        public static List<DepositXOffsetModel> SelectAllOffsetDeposit()
        {
            try
            {
                using (IDbConnection cnn = new MySqlConnection(LoadConnectionString()))
                {
                    var output = cnn.Query<DepositXOffsetModel>("SELECT * FROM offsetdeposits");
                    return output.ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static DepositXOffsetModel SelectDepositByInvoice(DepositXOffsetModel depositXOffset)
        {
            using (IDbConnection cnn = new MySqlConnection(LoadConnectionString()))
            {
                var output = cnn.Query<DepositXOffsetModel>("SELECT * FROM offsetdeposits WHERE IdDetailOffsetInvoice = @IdDetailOffsetInvoice", depositXOffset);
                return output.Single();
            }
        }

        public static int SelectLastOffsetNumber()
        {
            try
            {
                using (IDbConnection cnn = new MySqlConnection(LoadConnectionString()))
                {
                    var output = cnn.Query<int>("SELECT COUNT(IdDetailOffsetInvoice) FROM detailoffsetinvoice ORDER BY IdDetailOffsetInvoice DESC LIMIT 1");
                    return output.Single();
                }
            }
            catch (Exception ex)
            {

                return 0;
            }
        }

        public static bool InsertOffsetDetails(OffSetDetailsModel offSetDetails, List<int> productsIds, List<int> quantities, decimal deposit)
        {
            try
            {
                if (offSetDetails.IdClient != 0)
                {
                    using (IDbConnection cnn = new MySqlConnection(LoadConnectionString()))
                    {
                        cnn.Execute("INSERT detailoffsetinvoice (IdClient, IdEmployee, IdBusiness, EndDate, CurrencyType, Discount, SubTotal, Total, CashDeposit, CardDeposit, Taxes) VALUES (@IdClient, @IdEmployee, @IdBusiness, @EndDate, @CurrencyType, @Discount, @SubTotal, @Total, @CashDeposit, @CardDeposit, @Taxes)", offSetDetails);
                        InsertProductsXOffset(productsIds, quantities);
                        InsertDepositOffset(deposit);
                        return true;
                    }
                }
                else
                {
                    using (IDbConnection cnn = new MySqlConnection(LoadConnectionString()))
                    {
                        cnn.Execute("INSERT detailoffsetinvoice (idEmployee, idBusiness, EndDate, CurrencyType, Discount, SubTotal, Total, CashDeposit, CardDeposit, Taxes) VALUES (@IdEmployee, @IdBusiness, @EndDate, @CurrencyType, @Discount, @SubTotal, @Total, @CashDeposit, @CardDeposit, @Taxes)", offSetDetails);
                        InsertProductsXOffset(productsIds, quantities);
                        InsertDepositOffset(deposit);
                        return true;
                    }
                }

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private static void InsertProductsXOffset(List<int> productsids, List<int> quantities)
        {
            try
            {
                ProductXOffsetModel productXOffset;
                for (int i = 0; i < productsids.Count; i++)
                {
                    productXOffset = new ProductXOffsetModel()
                    {
                        IdDetailOffsetInvoice = SelectLastOffsetNumber(),
                        IdProduct = productsids[i],
                        Quantity = quantities[i]
                    };

                    using (IDbConnection cnn = new MySqlConnection(LoadConnectionString()))
                    {
                        cnn.Execute("INSERT offsetinvoice (IdDetailOffsetInvoice, IdProduct, Quantity) VALUES (@IdDetailOffsetInvoice, @IdProduct, @Quantity)", productXOffset);
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public static void InsertDepositOffset(decimal deposit)
        {
            try
            {
                DepositXOffsetModel depositXOffsetModel;

                depositXOffsetModel = new DepositXOffsetModel()
                {
                    IdDetailOffsetInvoice = SelectLastOffsetNumber(),
                    Deposit = deposit
                };

                using (IDbConnection cnn = new MySqlConnection(LoadConnectionString()))
                {
                    cnn.Execute("INSERT offsetdeposits (IdDetailOffsetInvoice, Deposit) VALUES (@IdDetailOffsetInvoice, @Deposit)", depositXOffsetModel);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static bool InsertOffsetDeposit (DepositXOffsetModel depositXOffset)
        {
            try
            {
                using (IDbConnection cnn = new MySqlConnection(LoadConnectionString()))
                {
                    cnn.Execute("INSERT INTO offsetdeposits (IdDetailOffsetInvoice, Deposit) VALUES(@IdDetailOffsetInvoice, @Deposit)", depositXOffset);
                    return true;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}
