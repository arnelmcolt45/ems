using System.ComponentModel.DataAnnotations;

namespace Ems.Assets.Dto
{
    public class MoveAssetPartToAssetInput
    {
        public int AssetPartId { get; set; }

        public int? AssetPartParentId { get; set; }

        public int? NewAssetId { get; set; }

        public bool? ImportAssetPart { get; set; }

        public int? NewAssetPartParentId { get; set; }
    }
}