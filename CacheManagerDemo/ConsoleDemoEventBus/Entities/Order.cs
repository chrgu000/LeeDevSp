using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleDemoEventBus.Entities
{
    public class Order
    {
        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderId { get; set; }

        /// <summary>
        /// 下单日期
        /// </summary>
        public DateTime OrderDateTime { get; set; }

        /// <summary>
        /// 订单金额
        /// </summary>
        public decimal OrderAmount { get; set; }
    }
}
