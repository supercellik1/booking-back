namespace Backend.DTOs;

public record ReviewDto(
    int Id,
    int HotelId,
    string HotelName,
    int Rating,
    string Comment,
    DateTime CreatedAt
);

public record CreateReviewDto(
    int HotelId,
    int Rating,
    string Comment
);
