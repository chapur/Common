﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Services.Payment.Interfaces
{
    public interface IProfileCreditCardGateway:ICreditCardGateway
    {
        IGatewayProfile GetOrCreateCustomerProfile(ICustomerData customerData);
        IGatewayProfile GetCustomerProfile(string id);
    }
}
