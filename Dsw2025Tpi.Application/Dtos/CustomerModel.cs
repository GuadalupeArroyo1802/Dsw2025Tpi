namespace Dsw2025Tpi.Application.Dtos
{
    public record CustomerModel
    {
        public record Request(
            string Name,
            string Email,
            string PhoneNumber
        );

        public record Response(
            Guid Id,
            string Name,
            string Email,
            string PhoneNumber
        );
    }
}
