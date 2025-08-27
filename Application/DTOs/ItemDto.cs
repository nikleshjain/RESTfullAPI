namespace RESTfullAPI.Application.DTOs;

public class ItemDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
}

public class CreateItemDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}

public class UpdateItemDto
{
    public int Quantity { get; set; }
}

