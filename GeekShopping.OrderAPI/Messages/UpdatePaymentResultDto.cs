﻿using GeekShopping.MessageBus;

namespace GeekShopping.OrderAPI.Messages
{
    public class UpdatePaymentResultDto : BaseMessage
    {
        public long OrderId { get; set; }
        public bool Status { get; set; }
        public string? Email { get; set; }
    }
}
