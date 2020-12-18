using System.ComponentModel.DataAnnotations;

namespace Ems.Assets.Dto
{
    public class MoveAssetPartToWarehouseInput
    {
        public int AssetPartId { get; set; }

        public int? NewWarehouseId { get; set; }
    }
}