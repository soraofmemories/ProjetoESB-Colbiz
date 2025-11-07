-- Limpar tabelas (opcional)
DELETE FROM LogsExecucao;
DELETE FROM PassosOrquestracao;
DELETE FROM Conectores;
DELETE FROM Orquestracoes;
DELETE FROM Integracoes;

-- Integrações
INSERT INTO Integracoes (Nome, Descricao, Ativo)
VALUES 
('Integração Envio SMS', 'Envio de SMS para sistema legado A', 1),
('Integração Consulta CEP', 'Consulta de CEP via API externa', 1),
('Integração Pagamentos', 'Integração com gateway de pagamentos', 1);

-- Conectores
INSERT INTO Conectores (IntegracaoID, Tipo, Endpoint, Metodo)
VALUES
(1, 'HTTP', 'https://httpbin.org/get', 'GET'),
(1, 'HTTP', 'https://httpbin.org/post', 'POST'),
(2, 'HTTP', 'https://viacep.com.br/ws/01001000/json/', 'GET'),
(3, 'HTTP', 'https://httpbin.org/status/200', 'GET');

-- Orquestrações
INSERT INTO Orquestracoes (Nome, Descricao)
VALUES
('Orquestração SMS', 'Fluxo de envio de SMS'),
('Orquestração CEP', 'Fluxo de consulta de CEP e log'),
('Orquestração Pagamento', 'Fluxo de teste de gateway de pagamento');

-- Passos de Orquestração
INSERT INTO PassosOrquestracao (OrquestracaoID, Ordem, ConectorID)
VALUES
(1, 1, 1),
(1, 2, 2),
(2, 1, 3),
(3, 1, 4);


SELECT * FROM LogsExecucao;

SELECT * FROM PassosOrquestracao;

SELECT * FROM Orquestracoes;

SELECT * FROM Conectores;

SELECT * FROM Integracoes;


DROP TABLE LogsExecucao;

CREATE TABLE LogsExecucao (
    LogID INT IDENTITY(1,1) PRIMARY KEY,
    IntegracaoID INT,
    DataHora DATETIME,
    Status VARCHAR(20),
    Mensagem VARCHAR(MAX)
);



-- Teste 06/11/2025 (16:54)

USE NASSegurosTestes;
GO

-- ===========================================================
-- 1?? Limpeza das tabelas antigas (opcional, se for recriar)
-- ===========================================================
IF OBJECT_ID('dbo.LogsExecucao', 'U') IS NOT NULL DROP TABLE dbo.LogsExecucao;
IF OBJECT_ID('dbo.PassosOrquestracao', 'U') IS NOT NULL DROP TABLE dbo.PassosOrquestracao;
IF OBJECT_ID('dbo.Orquestracoes', 'U') IS NOT NULL DROP TABLE dbo.Orquestracoes;
IF OBJECT_ID('dbo.Conectores', 'U') IS NOT NULL DROP TABLE dbo.Conectores;
IF OBJECT_ID('dbo.Integracoes', 'U') IS NOT NULL DROP TABLE dbo.Integracoes;
GO


-- ===========================================================
-- 2?? TABELA: Integracoes
-- ===========================================================
CREATE TABLE dbo.Integracoes (
    IntegracaoID INT IDENTITY(1,1) PRIMARY KEY,
    Nome VARCHAR(100) NOT NULL,
    Descricao VARCHAR(255) NULL,
    Ativo BIT NOT NULL DEFAULT(1)
);
GO


-- ===========================================================
-- 3?? TABELA: Conectores
-- ===========================================================
CREATE TABLE dbo.Conectores (
    ConectorID INT IDENTITY(1,1) PRIMARY KEY,
    IntegracaoID INT NOT NULL,
    Tipo VARCHAR(50) NOT NULL,
    Endpoint VARCHAR(255) NOT NULL,
    Metodo VARCHAR(10) NOT NULL,
    CONSTRAINT FK_Conectores_Integracoes FOREIGN KEY (IntegracaoID)
        REFERENCES dbo.Integracoes (IntegracaoID)
);
GO


-- ===========================================================
-- 4?? TABELA: Orquestracoes
-- ===========================================================
CREATE TABLE dbo.Orquestracoes (
    OrquestracaoID INT IDENTITY(1,1) PRIMARY KEY,
    Nome VARCHAR(100) NOT NULL,
    Descricao VARCHAR(255) NULL
);
GO


-- ===========================================================
-- 5?? TABELA: PassosOrquestracao
-- ===========================================================
CREATE TABLE dbo.PassosOrquestracao (
    PassoID INT IDENTITY(1,1) PRIMARY KEY,
    OrquestracaoID INT NOT NULL,
    Ordem INT NOT NULL,
    ConectorID INT NOT NULL,
    CONSTRAINT FK_Passos_Orquestracoes FOREIGN KEY (OrquestracaoID)
        REFERENCES dbo.Orquestracoes (OrquestracaoID),
    CONSTRAINT FK_Passos_Conectores FOREIGN KEY (ConectorID)
        REFERENCES dbo.Conectores (ConectorID)
);
GO


-- ===========================================================
-- 6?? TABELA: LogsExecucao
-- ===========================================================
CREATE TABLE dbo.LogsExecucao (
    LogID INT IDENTITY(1,1) PRIMARY KEY,
    IntegracaoID INT NOT NULL,
    DataHora DATETIME NOT NULL DEFAULT(GETDATE()),
    Status VARCHAR(20) NOT NULL,
    Mensagem VARCHAR(MAX) NULL,
    CONSTRAINT FK_Logs_Integracoes FOREIGN KEY (IntegracaoID)
        REFERENCES dbo.Integracoes (IntegracaoID)
);
GO


-- ===========================================================
-- 7?? INSERÇÃO DE DADOS DE TESTE
-- ===========================================================
INSERT INTO dbo.Integracoes (Nome, Descricao, Ativo)
VALUES ('Integração Teste Orquestrador', 'Fluxo simples de teste', 1);

INSERT INTO dbo.Conectores (IntegracaoID, Tipo, Endpoint, Metodo)
VALUES 
(1, 'HTTP', 'https://httpbin.org/get', 'GET'),
(1, 'HTTP', 'https://httpbin.org/post', 'POST');

INSERT INTO dbo.Orquestracoes (Nome, Descricao)
VALUES ('Orquestração Teste', 'Executa dois conectores REST em sequência');

INSERT INTO dbo.PassosOrquestracao (OrquestracaoID, Ordem, ConectorID)
VALUES 
(1, 1, 1),
(1, 2, 2);
GO


SELECT * FROM Integracoes;
SELECT * FROM Conectores;
SELECT * FROM Orquestracoes;
SELECT * FROM PassosOrquestracao;
SELECT * FROM LogsExecucao;