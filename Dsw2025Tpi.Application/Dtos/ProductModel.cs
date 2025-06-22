using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Application.Dtos
{
    
        public record ProductModel
        {
            public record Request(
                string Sku,
                string Name,
                decimal CurrentUnitPrice,
                string? InternalCode,
                string? Description,
                int StockQuantity
            );

            public record Response(
                Guid Id,
                string Sku,
                string Name,
                decimal CurrentUnitPrice,
                int StockQuantity,
                bool IsActive
            );
        }
    }

