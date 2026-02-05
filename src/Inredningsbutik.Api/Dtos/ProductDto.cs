namespace Inredningsbutik.Api.Dtos;

public record ProductDto(
    int Id,
    string Name,
    string Description,
    decimal Price,
    string? ImageUrl,
    int StockQuantity,
    int CategoryId,
    string CategoryName
);
