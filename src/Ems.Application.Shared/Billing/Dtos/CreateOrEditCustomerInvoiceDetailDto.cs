using Abp.Application.Services.Dto;

namespace Ems.Billing.Dtos
{
    public class CreateOrEditCustomerInvoiceDetailDto : EntityDto<int?>
    {

        public string Description { get; set; }


        public decimal Quantity { get; set; }


        public decimal UnitPrice { get; set; }


        public decimal? Gross { get; set; }


        public decimal? Tax { get; set; }


        public decimal Net { get; set; }


        public decimal? Discount { get; set; }


        public decimal? Charge { get; set; }


        public int? BillingRuleRefId { get; set; }


        public int CustomerInvoiceId { get; set; }


        public bool IsTaxable { get; set; }

        public int? ItemTypeId { get; set; }

        public int? UomId { get; set; }

        public int? WorkOrderActionId { get; set; }

        public int? LeaseItemId { get; set; }

        public int? AssetId { get; set; }

        public int? AssetOwnershipId { get; set; }

    }
}