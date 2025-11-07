USE ESB;
GO

EXEC sp_MSforeachtable "ALTER TABLE ? NOCHECK CONSTRAINT ALL";
DELETE FROM LogsExecucao;
DELETE FROM PassosOrquestracao;
DELETE FROM Orquestracoes;
DELETE FROM Conectores;
DELETE FROM Integracoes;
EXEC sp_MSforeachtable "ALTER TABLE ? WITH CHECK CHECK CONSTRAINT ALL";
GO

DBCC CHECKIDENT ('Integracoes', RESEED, 0);
DBCC CHECKIDENT ('Conectores', RESEED, 0);
DBCC CHECKIDENT ('Orquestracoes', RESEED, 0);
DBCC CHECKIDENT ('PassosOrquestracao', RESEED, 0);
DBCC CHECKIDENT ('LogsExecucao', RESEED, 0);
GO

INSERT INTO Integracoes (Nome, Descricao, Ativo)
VALUES ('Integração HTTP Teste', 'Executa uma chamada GET de exemplo', 1);
GO

INSERT INTO Conectores (IntegracaoID, Tipo, Endpoint, Metodo)
VALUES (1, 'HTTP', 'https://httpbin.org/get', 'GET');
GO

INSERT INTO Orquestracoes (Nome, Descricao)
VALUES ('Orquestração Teste 1', 'Fluxo de teste com um passo único');
GO

INSERT INTO PassosOrquestracao (OrquestracaoID, Ordem, ConectorID)
VALUES (1, 1, 1);
GO

SELECT * FROM Conectores;
SELECT * FROM Integracoes;
SELECT * FROM Orquestracoes;
SELECT * FROM PassosOrquestracao;
SELECT * FROM LogsExecucao;


ALTER TABLE LogsExecucao ADD
	Detalhes NVARCHAR(MAX) NULL,
	Endpoint VARCHAR(255) NULL,
	CodigoHttp INT NULL,
	DuracaoMs FLOAT NULL;

	UPDATE Conectores SET Endpoint = 'https://httpbin.org/status/500' WHERE ConectorID = 1;
