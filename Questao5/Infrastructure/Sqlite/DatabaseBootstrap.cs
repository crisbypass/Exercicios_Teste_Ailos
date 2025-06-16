using Dapper;
using Microsoft.Data.Sqlite;

namespace Questao5.Infrastructure.Sqlite
{
    public class DatabaseBootstrap(IServiceProvider serviceProvider) : IDatabaseBootstrap
    {        
        private readonly IServiceProvider _serviceProvider = serviceProvider;
        public async Task SetupAsync()
        {
            await using var scope = _serviceProvider.CreateAsyncScope();

            var connection = scope.ServiceProvider.GetRequiredService<SqliteConnection>();

            var table = await connection.QueryAsync<string>("SELECT name FROM sqlite_master WHERE type='table' AND (name = 'contacorrente' or name = 'movimento' or name = 'idempotencia');");
            var tableName = table.FirstOrDefault();
            if (!string.IsNullOrEmpty(tableName) && (tableName == "contacorrente" ||
                tableName == "movimento" || tableName == "idempotencia"))
                return;            

            await connection.ExecuteAsync("CREATE TABLE contacorrente ( " +
                               "idcontacorrente TEXT(37) PRIMARY KEY," +
                               "numero INTEGER(10) NOT NULL UNIQUE," +
                               "nome TEXT(100) NOT NULL," +
                               "ativo INTEGER(1) NOT NULL default 0," +
                               "CHECK(ativo in (0, 1)) " +
                               ");");
            
            // Correção efetuada para references com a coluna 'numero', na chave estrangeira.
            // Originalmente era 'idcontacorrente'.

            await connection.ExecuteAsync("CREATE TABLE movimento ( " +
                "idmovimento TEXT(37) PRIMARY KEY," +
                "idcontacorrente INTEGER(10) NOT NULL," +
                "datamovimento TEXT(25) NOT NULL," +
                "tipomovimento TEXT(1) NOT NULL," +
                "valor REAL NOT NULL," +
                "CHECK(tipomovimento in ('C', 'D')), " +
                "FOREIGN KEY(idcontacorrente) REFERENCES contacorrente(numero) " +
                ");");

            await connection.ExecuteAsync("CREATE TABLE idempotencia (" +
                               "chave_idempotencia TEXT(37) PRIMARY KEY," +
                               "requisicao TEXT(1000)," +
                               "resultado TEXT(1000));");

            await connection.ExecuteAsync("INSERT INTO contacorrente(idcontacorrente, numero, nome, ativo) VALUES('B6BAFC09 -6967-ED11-A567-055DFA4A16C9', 123, 'Katherine Sanchez', 1);");
            await connection.ExecuteAsync("INSERT INTO contacorrente(idcontacorrente, numero, nome, ativo) VALUES('FA99D033-7067-ED11-96C6-7C5DFA4A16C9', 456, 'Eva Woodward', 1);");
            await connection.ExecuteAsync("INSERT INTO contacorrente(idcontacorrente, numero, nome, ativo) VALUES('382D323D-7067-ED11-8866-7D5DFA4A16C9', 789, 'Tevin Mcconnell', 1);");
            await connection.ExecuteAsync("INSERT INTO contacorrente(idcontacorrente, numero, nome, ativo) VALUES('F475F943-7067-ED11-A06B-7E5DFA4A16C9', 741, 'Ameena Lynn', 0);");
            await connection.ExecuteAsync("INSERT INTO contacorrente(idcontacorrente, numero, nome, ativo) VALUES('BCDACA4A-7067-ED11-AF81-825DFA4A16C9', 852, 'Jarrad Mckee', 0);");
            await connection.ExecuteAsync("INSERT INTO contacorrente(idcontacorrente, numero, nome, ativo) VALUES('D2E02051-7067-ED11-94C0-835DFA4A16C9', 963, 'Elisha Simons', 0);");


            await connection.ExecuteAsync("INSERT INTO movimento(idmovimento, idcontacorrente, datamovimento, tipomovimento, valor) VALUES('D2E02052-7067-ED11-94C0-835DFA4A16C9', 963, '2025-06-12', 'C', 553.21);");
            await connection.ExecuteAsync("INSERT INTO movimento(idmovimento, idcontacorrente, datamovimento, tipomovimento, valor) VALUES('D2E02053-7067-ED11-94C0-735DFA4A16C9', 963, '2025-05-13', 'D', 159.28);");
            await connection.ExecuteAsync("INSERT INTO movimento(idmovimento, idcontacorrente, datamovimento, tipomovimento, valor) VALUES('D2E02054-7067-ED11-94C0-635DFA4A16C9', 963, '2025-04-14', 'C', 858.27);");
            await connection.ExecuteAsync("INSERT INTO movimento(idmovimento, idcontacorrente, datamovimento, tipomovimento, valor) VALUES('D2E02055-7067-ED11-94C0-535DFA4A16C9', 963, '2025-03-15', 'D', 254.29);");
        }
    }
}
