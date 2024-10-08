﻿using Projeto.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Projeto.API.Service
{
    public class CdbService : ICdbService
    {
        //CDI e TB fixos conforme especificado
        public decimal CDI { get; private set; } = 0.009m; // 0.9%

        public decimal TB { get; private set; } = 1.08m;   // 108%

        private readonly List<(int MinMeses, int MaxMeses, decimal TaxaImposto)> faixasImposto = new List<(int, int, decimal)>
        {
            (1, 6, 0.225m),   // 22,5%
            (7, 12, 0.20m),   // 20%
            (13, 24, 0.175m), // 17,5%
            (25, 60, 0.15m)   // 15%
        };

        public void SetCDI(decimal porcentagem) => CDI = porcentagem;

        public void SetTB(decimal porcentagem) => TB = porcentagem;

        public CdbResultModel CalcularCDB(decimal valorInicial, int meses)
        {
            if (valorInicial <= 0 || meses < 1)
                throw new InvalidOperationException($"Valores para cálculo estão incorretos");

            var valorBruto = valorInicial;
            for (int i = 0; i < meses; i++)
            {
                valorBruto *= (1 + (CDI * TB));
            }

            var taxaImposto = faixasImposto.First(faixa =>
                    meses >= faixa.MinMeses &&
                    meses <= faixa.MaxMeses)
                    .TaxaImposto;

            var imposto = CalcularImposto();
            var valorLiquido = valorBruto - imposto;

            return new CdbResultModel
            {
                ValorBruto = valorBruto,
                ValorLiquido = valorLiquido,
                Imposto = taxaImposto * 100
            };

            decimal CalcularImposto()
            {
                var rendimento = valorBruto - valorInicial;
                return rendimento * taxaImposto;
            }
        }
    }
}
