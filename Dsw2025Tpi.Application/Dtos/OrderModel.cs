﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Application.Dtos
{
    public record OrderModel
    {
        public record Request(
            Guid CustomerId,
            string ShippingAddress,
            string BillingAddress,
            List<OrderItemModel.Request> OrderItems
        );

        public record Response(
            Guid Id,
            string ShippingAddress,
            string BillingAddress,
            string Status,
            decimal TotalAmount,
            List<OrderItemModel.Response> OrderItems
        );
    }
}
