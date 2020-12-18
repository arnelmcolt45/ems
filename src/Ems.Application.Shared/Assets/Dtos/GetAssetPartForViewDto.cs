namespace Ems.Assets.Dtos
{
    public class GetAssetPartForViewDto
    {
		public AssetPartDto AssetPart { get; set; }

		public string AssetPartTypeType { get; set;}

		public string AssetPartName { get; set;}

		public string AssetPartStatusStatus { get; set;}

		public string AssetReference { get; set;}

		public string ItemTypeType { get; set;}

		public string WarehouseName { get; set;}
	}
}