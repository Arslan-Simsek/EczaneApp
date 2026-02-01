namespace EczaneApp.Data.Common
{
	public class BaseEntity
	{
		public int Id { get; set; }	
		public DateTime CreatedAt { get; set; }
		public DateTime? UpdatedAt { get; set; }

	}
	public class BaseLongEntity
	{
		public long Id { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime? UpdatedAt { get; set; }
	}
}
