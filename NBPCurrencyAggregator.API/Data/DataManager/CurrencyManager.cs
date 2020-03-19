using NBPCurrencyAggregator.API.Contracts;
using NBPCurrencyAggregator.API.Data.Entity;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace NBPCurrencyAggregator.API.Data.DataManager
{
    public class CurrencyManager : DbFactoryBase, ICurrencyManager
    {
        private readonly ILogger<CurrencyManager> _logger;
        private readonly INbpApiClient _client;
        public CurrencyManager(IConfiguration config, ILogger<CurrencyManager> logger, INbpApiClient client) : base(config)
        {
            _logger = logger;
            _client = client;
        }

        public async Task<(IEnumerable<Currency> Currencies, Pagination Pagination)> GetCurrenciesAsync(UrlQueryParameters urlQueryParameters)
        {
            IEnumerable<Currency> currencies;
            int recordCount = default;

            ////For PosgreSql
            //var query = @"SELECT Id, Table, CurrencyName, Code FROM Currency
            //                ORDER BY ID DESC 
            //                Limit @Limit Offset @Offset";


            ////For SqlServer
            var query = @"SELECT Id, Table, CurrencyName, Code FROM Currency
                            ORDER BY ID DESC
                            OFFSET @Limit * (@Offset -1) ROWS
                            FETCH NEXT @Limit ROWS ONLY";

            var param = new DynamicParameters();
            param.Add("Limit", urlQueryParameters.PageSize);
            param.Add("Offset", urlQueryParameters.PageNumber);

            if (urlQueryParameters.IncludeCount)
            {
                query += " SELECT COUNT(Id) FROM Currency";
                var pagedRows = await DbQueryMultipleAsync<Currency, int>(query, param);

                currencies = pagedRows.Data;
                recordCount = pagedRows.RecordCount;
            }
            else
            {
                currencies = await DbQueryAsync<Currency>(query, param);
            }

            var metadata = new Pagination
            {
                PageNumber = urlQueryParameters.PageNumber,
                PageSize = urlQueryParameters.PageSize,
                TotalRecords = recordCount

            };

            return (currencies, metadata);

        }
        public async Task<IEnumerable<Currency>> GetAllAsync()
        {
            return await DbQueryAsync<Currency>("SELECT * FROM Currency");
        }

        public async Task<Currency> GetByIdAsync(object id)
        {
            return await DbQuerySingleAsync<Currency>("SELECT * FROM Currency WHERE Id = @ID", new { id });
        }

        public async Task<int> CreateAsync(Currency currency)
        {
            string sqlQuery = $@"INSERT INTO Currency (Table, CurrencyName, Code) 
                                     VALUES (@Table, @CurrencyName, @Code)
                                     SELECT CAST(SCOPE_IDENTITY() as bigint)";

            return await DbQuerySingleAsync<int>(sqlQuery, currency);
        }
        public async Task<bool> UpdateAsync(Currency currency)
        {
            string sqlQuery = $@"IF EXISTS (SELECT 1 FROM Currency WHERE Id = @ID) 
                                            UPDATE Currency SET Table = @Table, CurrencyName = @CurrencyName, Code = @Code
                                            WHERE Id = @ID";

            return await DbExecuteAsync<bool>(sqlQuery, currency);
        }
        public async Task<bool> DeleteAsync(object id)
        {
            string sqlQuery = $@"IF EXISTS (SELECT 1 FROM Currency WHERE Id = @ID)
                                        DELETE Currency WHERE Id = @ID";

            return await DbExecuteAsync<bool>(sqlQuery, new { id });
        }
        public async Task<bool> ExistAsync(object id)
        {
            return await DbExecuteScalarAsync("SELECT COUNT(1) FROM Currency WHERE Id = @ID", new { id });
        }

        public async Task<bool> ExecuteWithTransactionScope()
        {

            using (var dbCon = new SqlConnection(DbConnectionString))
            {
                await dbCon.OpenAsync();
                var transaction = await dbCon.BeginTransactionAsync();

                try
                {
                    //Do stuff here Insert, Update or Delete
                    Task q1 = dbCon.ExecuteAsync("<Your SQL Query here>");
                    Task q2 = dbCon.ExecuteAsync("<Your SQL Query here>");
                    Task q3 = dbCon.ExecuteAsync("<Your SQL Query here>");

                    await Task.WhenAll(q1, q2, q3);

                    //Commit the Transaction when all query are executed successfully

                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    //Rollback the Transaction when any query fails
                    transaction.Rollback();
                    _logger.Log(LogLevel.Error, ex, "Error when trying to execute database operations within a scope.");

                    return false;
                }
            }
            return true;
        }

        public async Task<Currency> GetCurrenciesByDate(string symbol, DateTime from, DateTime? to)
        {
            var result = await DbQueryCurrencyFromTo("SELECT c.Table, c.CurrencyName, c.Code, r.Bid, r.Ask FROM Currency c" +
                                                " INNER JOIN Rate r ON c.Id = r.CurrencyId" +
                                                " WHERE r.EffectiveDate >= @from" +
                                                " AND r.EffectiveDate < @to " +
                                                " AND c.Code = @symbol", new { from, to, symbol });

            return result ?? await _client.GetCurrencyFromTo(symbol, @from, to);
        }
    }
}
