
CREATE UNLOGGED TABLE cliente (
    id INT PRIMARY KEY,
    limite INT NOT NULL,
    saldo INT NOT NULL
);

CREATE UNLOGGED TABLE transacao (
    id SERIAL PRIMARY KEY,
    valor INT NOT NULL,
    tipo_transacao CHAR(1) NOT NULL,
    descricao VARCHAR(10) NOT NULL,
    realizada_em TIMESTAMP NOT NULL DEFAULT NOW(),
    id_cliente INT NOT NULL,
    FOREIGN KEY (id_cliente) REFERENCES cliente(id)
);

CREATE INDEX idx_cliente ON cliente (id) INCLUDE (saldo, limite);
CREATE INDEX idx_transacao_cliente ON transacao (id_cliente);

INSERT INTO cliente (id, limite, saldo) VALUES
(1, 100000, 0),
(2, 80000, 0),
(3, 1000000, 0),
(4, 10000000, 0),
(5, 500000, 0);
