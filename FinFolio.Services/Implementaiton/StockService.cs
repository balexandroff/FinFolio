using CsvHelper;
using CsvHelper.Configuration;
using FinFolio.Core.Interfaces;
using FinFolio.Core.Entities;
using FinFolio.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FinFolio.Services.Implementaiton
{
    public class StockService : IStockService
    {
        private readonly IConfiguration _configuration;
        private readonly IStockRepository _stockRepository;
        private readonly IDividendRepository _dividendRepository;
        public StockService(IConfiguration configuration, IStockRepository stockRepository, IDividendRepository dividendRepository)
        {
            this._configuration = configuration;
            this._stockRepository = stockRepository;
            this._dividendRepository = dividendRepository;
        }

        public async Task<string> LoadRemoteStockInfo(string ticker)
        {
            var path = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase).TrimStart("file:\\".ToCharArray());

            if (!string.IsNullOrEmpty(ticker))
            {
                Directory.CreateDirectory(Path.Combine(path, $"Extractor\\Stocks\\{ticker}"));
            }

            System.IO.DirectoryInfo di = new DirectoryInfo(Path.Combine(path, $"Extractor\\Stocks\\{ticker}"));

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }

            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }

            run_cmd(_configuration.GetSection("PythonPath").Value, "pip install yfinance --upgrade --no-cache-dir");

            StringBuilder pythonFile = new StringBuilder();
            string pythonPath = path;
            pythonPath = pythonPath.Replace("\\", "/");

            pythonFile.AppendLine($"import yfinance as yf");
            pythonFile.AppendLine($"import pandas as pd");
            pythonFile.AppendLine($"{ticker} = yf.Ticker(\"{ticker}\")");
            pythonFile.AppendLine($"pd.DataFrame({ticker}.info).to_csv(\"{pythonPath}/Extractor/stocks/{ticker}/info.csv\")");
            pythonFile.AppendLine($"#pd.DataFrame({ticker}.actions).to_csv(\"{pythonPath}/Extractor/stocks/{ticker}/actions.csv\")");
            pythonFile.AppendLine($"pd.DataFrame({ticker}.dividends).to_csv(\"{pythonPath}/Extractor/stocks/{ticker}/dividends.csv\")");
            pythonFile.AppendLine($"pd.DataFrame({ticker}.splits).to_csv(\"{pythonPath}/Extractor/stocks/{ticker}/splits.csv\")");
            pythonFile.AppendLine($"pd.DataFrame({ticker}.financials).to_csv(\"{pythonPath}/Extractor/stocks/{ticker}/financials.csv\")");
            pythonFile.AppendLine($"pd.DataFrame({ticker}.quarterly_financials).to_csv(\"{pythonPath}/Extractor/stocks/{ticker}/quarterly_financials.csv\")");
            pythonFile.AppendLine($"pd.DataFrame({ticker}.major_holders).to_csv(\"{pythonPath}/Extractor/stocks/{ticker}/major_holders.csv\")");
            pythonFile.AppendLine($"pd.DataFrame({ticker}.institutional_holders).to_csv(\"{pythonPath}/Extractor/stocks/{ticker}/institutional_holders.csv\")");
            pythonFile.AppendLine($"pd.DataFrame({ticker}.balance_sheet).to_csv(\"{pythonPath}/Extractor/stocks/{ticker}/balance_sheet.csv\")");
            pythonFile.AppendLine($"pd.DataFrame({ticker}.quarterly_balance_sheet).to_csv(\"{pythonPath}/Extractor/stocks/{ticker}/quarterly_balance_sheet.csv\")");
            pythonFile.AppendLine($"pd.DataFrame({ticker}.cashflow).to_csv(\"{pythonPath}/Extractor/stocks/{ticker}/cashflow.csv\")");
            pythonFile.AppendLine($"pd.DataFrame({ticker}.quarterly_cashflow).to_csv(\"{pythonPath}/Extractor/stocks/{ticker}/quarterly_cashflow.csv\")");
            pythonFile.AppendLine($"pd.DataFrame({ticker}.earnings).to_csv(\"{pythonPath}/Extractor/stocks/{ticker}/earnings.csv\")");
            pythonFile.AppendLine($"pd.DataFrame({ticker}.quarterly_earnings).to_csv(\"{pythonPath}/Extractor/stocks/{ticker}/quarterly_earnings.csv\")");
            pythonFile.AppendLine($"pd.DataFrame({ticker}.sustainability).to_csv(\"{pythonPath}/Extractor/stocks/{ticker}/sustainability.csv\")");
            pythonFile.AppendLine($"pd.DataFrame({ticker}.recommendations).to_csv(\"{pythonPath}/Extractor/stocks/{ticker}/recommendations.csv\")");
            pythonFile.AppendLine($"pd.DataFrame({ticker}.calendar).to_csv(\"{pythonPath}/Extractor/stocks/{ticker}/calendar.csv\")");
            pythonFile.AppendLine($"#pd.DataFrame({ticker}.isin).to_csv(\"{pythonPath}/Extractor/stocks/{ticker}/isin.csv\")");
            pythonFile.AppendLine($"#pd.DataFrame({ticker}.options).to_csv(\"{pythonPath}/Extractor/stocks/{ticker}/options.csv\")");
            pythonFile.AppendLine($"#pd.DataFrame({ticker}.news).to_csv(\"{pythonPath}/Extractor/stocks/{ticker}/news.csv\")");

            File.WriteAllText(Path.Combine(path, $"Extractor\\{ticker}.py"), pythonFile.ToString());

            run_cmd(_configuration.GetSection("PythonPath").Value, Path.Combine(path, $"Extractor\\{ticker}.py"));

            var dividends = ParseDividends(Path.Combine(path, $"Extractor\\stocks\\{ticker}\\dividends.csv"));

            var stock = await _stockRepository.CreateAsync(new Stock { Ticker = ticker, Name = ticker });
            await this._dividendRepository.CreateAsync(stock.Id, dividends);

            return await Task.FromResult(string.Empty);
        }

        private void run_cmd(string cmd, string args)
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = cmd;//cmd is full path to python.exe
            start.Arguments = args;//args is path to .py file and any cmd line args
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            start.RedirectStandardError = true;
            start.Verb = "runas";
            using (Process process = Process.Start(start))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                }

                using (StreamReader reader = process.StandardError)
                {
                    string error = reader.ReadToEnd();
                }
            }
        }

        private List<Dividend> ParseDividends(string fileName)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
            };
            using (var reader = new StreamReader(fileName))
            using (var csv = new CsvReader(reader, config))
            {
                csv.Context.RegisterClassMap<DividendMap>();
                var dividendRecords = new List<Dividend>();
                while (csv.Read())
                {
                    dividendRecords.Add(csv.GetRecord<Dividend>());
                }

                return dividendRecords;
            }

        }


        public sealed class DividendMap : ClassMap<Dividend>
        {
            public DividendMap()
            {
                Map(m => m.Date).Index(0);
                Map(m => m.Amount).Index(1);
            }
        }
    }
}
